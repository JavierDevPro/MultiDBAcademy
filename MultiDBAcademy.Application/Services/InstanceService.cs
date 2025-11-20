using MultiDBAcademy.Application.Dtos;
using MultiDBAcademy.Application.Helpers;
using MultiDBAcademy.Application.Interfaces;
using MultiDBAcademy.Domain.Entities;
using MultiDBAcademy.Domain.Interfaces;

namespace MultiDBAcademy.Application.Services;

public class InstanceService : IInstanceService
{
    private readonly IInstanceRepository _instanceRepository;
    private readonly ICredentialsRepository _credentialsRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IEnumerable<IDbEngineService> _dbEngineServices;

    public InstanceService(
        IInstanceRepository instanceRepository,
        ICredentialsRepository credentialsRepository,
        IRepository<User> userRepository,
        IEnumerable<IDbEngineService> dbEngineServices)
    {
        _instanceRepository = instanceRepository;
        _credentialsRepository = credentialsRepository;
        _userRepository = userRepository;
        _dbEngineServices = dbEngineServices;
    }

    public async Task<InstanceResponseDto> CreateInstanceAsync(CreateInstanceDto dto)
    {
        // 1. Validar que el usuario existe
        var user = await _userRepository.GetByIdAsync(dto.UserId);
        if (user == null)
            throw new ArgumentException("Usuario no encontrado");

        // 2. Obtener el servicio del motor específico
        var engineService = _dbEngineServices.FirstOrDefault(s => s.EngineType == dto.EngineType);
        if (engineService == null)
            throw new NotSupportedException($"Motor {dto.EngineType} no soportado");

        // 3. Generar nombre único de BD y credenciales
        var databaseName = $"{user.UserName}_{dto.EngineType.ToString().ToLower()}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        var username = $"user_{user.UserName}_{Guid.NewGuid().ToString("N").Substring(0, 6)}";
        var password = PasswordGenerator.Generate(16);

        // 4. Verificar que la BD no existe
        if (await _instanceRepository.ExistsAsync(databaseName, dto.EngineType))
            throw new InvalidOperationException("Ya existe una instancia con ese nombre");

        // 5. Crear la base de datos en el motor master
        var created = await engineService.CreateDatabaseAsync(databaseName, username, password);
        if (!created)
            throw new InvalidOperationException("Error al crear la base de datos en el motor");

        // 6. Crear registro de credenciales
        var credentials = new CredentialsDb
        {
            Username = username,
            PasswordHash = password, // En producción, considera hashear
            Database = databaseName,
            Host = "localhost",
            Port = engineService.DefaultPort,
            CreatedAt = DateTime.UtcNow
        };
        
        var savedCredentials = await _credentialsRepository.AddAsync(credentials);

        // 7. Crear registro de instancia
        var instance = new InstanceDB
        {
            Name = dto.Name,
            EngineType = dto.EngineType,
            Status = DbInstanceStatus.Active,
            DatabaseName = databaseName,
            Host = "localhost",
            Port = engineService.DefaultPort,
            UserId = dto.UserId,
            CredentialsDbId = savedCredentials.Id,
            CreateAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow
        };

        var savedInstance = await _instanceRepository.AddAsync(instance);

        // 8. Retornar DTO de respuesta
        return MapToResponseDto(savedInstance, user, savedCredentials);
    }

    public async Task<InstanceResponseDto> GetByIdAsync(int id, int requestingUserId, bool isAdmin)
    {
        var instance = await _instanceRepository.GetByIdWithCredentialsAsync(id);
        if (instance == null)
            throw new KeyNotFoundException("Instancia no encontrada");

        // Verificar permisos: solo el dueño o un admin pueden ver
        if (!isAdmin && instance.UserId != requestingUserId)
            throw new UnauthorizedAccessException("No tienes permisos para ver esta instancia");

        return MapToResponseDto(instance, instance.User, instance.Credentials);
    }

    public async Task<IEnumerable<InstanceResponseDto>> GetAllAsync()
    {
        var instances = await _instanceRepository.GetAllAsync();
        return instances.Select(i => MapToResponseDto(i, i.User, i.Credentials));
    }

    public async Task<IEnumerable<InstanceResponseDto>> GetByUserIdAsync(int userId)
    {
        var instances = await _instanceRepository.GetByUserIdAsync(userId);
        return instances.Select(i => MapToResponseDto(i, i.User, i.Credentials));
    }

    public async Task<IEnumerable<InstanceResponseDto>> GetByEngineTypeAsync(DbEngineType engineType)
    {
        var instances = await _instanceRepository.GetByEngineTypeAsync(engineType);
        return instances.Select(i => MapToResponseDto(i, i.User, i.Credentials));
    }

    public async Task<bool> DeleteInstanceAsync(int id)
    {
        var instance = await _instanceRepository.GetByIdAsync(id);
        if (instance == null)
            return false;

        // Obtener servicio del motor y eliminar BD
        var engineService = _dbEngineServices.FirstOrDefault(s => s.EngineType == instance.EngineType);
        if (engineService != null)
        {
            await engineService.DropDatabaseAsync(instance.DatabaseName);
        }

        return await _instanceRepository.DeleteAsync(id);
    }

    public async Task<bool> UpdateStatusAsync(UpdateInstanceStatusDto dto)
    {
        var instance = await _instanceRepository.GetByIdAsync(dto.InstanceId);
        if (instance == null)
            return false;

        instance.Status = dto.NewStatus;
        instance.UpdateAt = DateTime.UtcNow;

        await _instanceRepository.UpdateAsync(instance);
        return true;
    }

    public async Task<bool> AssignToStudentAsync(AssignInstanceDto dto)
    {
        var instance = await _instanceRepository.GetByIdAsync(dto.InstanceId);
        if (instance == null)
            return false;

        var student = await _userRepository.GetByIdAsync(dto.StudentId);
        if (student == null)
            return false;

        instance.UserId = dto.StudentId;
        instance.UpdateAt = DateTime.UtcNow;

        await _instanceRepository.UpdateAsync(instance);
        return true;
    }

    public async Task<CredentialsResponseDto> GetCredentialsAsync(int instanceId, int requestingUserId)
    {
        var instance = await _instanceRepository.GetByIdWithCredentialsAsync(instanceId);
        if (instance == null)
            throw new KeyNotFoundException("Instancia no encontrada");

        // Solo el dueño puede ver las credenciales
        if (instance.UserId != requestingUserId)
            throw new UnauthorizedAccessException("No tienes permisos para ver estas credenciales");

        if (instance.Credentials == null)
            throw new InvalidOperationException("No hay credenciales disponibles");

        return new CredentialsResponseDto
        {
            Username = instance.Credentials.Username,
            Password = instance.Credentials.PasswordHash,
            Database = instance.Credentials.Database,
            Host = instance.Credentials.Host,
            Port = instance.Credentials.Port,
            ConnectionString = ConnectionStringBuilder.Build(
                instance.EngineType,
                instance.Credentials.Host,
                instance.Credentials.Port,
                instance.Credentials.Database,
                instance.Credentials.Username,
                instance.Credentials.PasswordHash
            )
        };
    }

    // Método auxiliar para mapear
    private InstanceResponseDto MapToResponseDto(InstanceDB instance, User? user, CredentialsDb? credentials)
    {
        return new InstanceResponseDto
        {
            Id = instance.Id,
            Name = instance.Name,
            EngineType = instance.EngineType,
            EngineTypeName = instance.EngineType.ToString(),
            Status = instance.Status,
            StatusName = instance.Status.ToString(),
            DatabaseName = instance.DatabaseName,
            Host = instance.Host,
            Port = instance.Port,
            CreateAt = instance.CreateAt,
            LastAccessedAt = instance.LastAccessedAt,
            UserId = instance.UserId,
            UserName = user?.UserName ?? "",
            UserEmail = user?.Email ?? "",
            Credentials = credentials != null ? new CredentialsResponseDto
            {
                Username = credentials.Username,
                Password = credentials.PasswordHash,
                Database = credentials.Database,
                Host = credentials.Host,
                Port = credentials.Port,
                ConnectionString = ConnectionStringBuilder.Build(
                    instance.EngineType,
                    credentials.Host,
                    credentials.Port,
                    credentials.Database,
                    credentials.Username,
                    credentials.PasswordHash
                )
            } : null
        };
    }
}