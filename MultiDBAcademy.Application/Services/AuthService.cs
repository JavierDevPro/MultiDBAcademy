using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MultiDBAcademy.Application.Dtos;
using MultiDBAcademy.Application.Interfaces;
using MultiDBAcademy.Domain.Entities;
using MultiDBAcademy.Domain.Interfaces;

namespace MultiDBAcademy.Application.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _repository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;

    public AuthService(IRepository<User> userRepository, IMapper mapper, IConfiguration config)
    {
        _mapper = mapper;
        _repository = userRepository;
        _config = config;
    }

    public async Task<AuthReguisterResponseDTo> RegisterAsync(RegisterDTo registerDTo)
    {
        if (registerDTo == null)
            throw new ArgumentNullException("El cuerpo de la peticion no puede estar vacio");

        if (string.IsNullOrEmpty(registerDTo.Email) || string.IsNullOrEmpty(registerDTo.Password) ||
            string.IsNullOrEmpty(registerDTo.UserName))
            throw new ArgumentException("Todos los campos son obligatorios");

        var exist = await _repository.FindByEmailAsync(registerDTo.Email);

        if (exist != null)
            throw new ArgumentException("Ya existe un usuario registrado con el email ingresado");

        var user = _mapper.Map<User>(registerDTo);
        user.PassHash = BCrypt.Net.BCrypt.HashPassword(registerDTo.Password);
        user.CreateAt = DateTime.UtcNow;
        user.UpdateAt = DateTime.UtcNow;

        await _repository.AddAsync(user);
        return _mapper.Map<AuthReguisterResponseDTo>(user);
    }


    public async Task<AuthResponseDto> LoginAsync(LoginDTo loginDTo)
    {
        if (loginDTo == null)
            throw new ArgumentNullException("El cuerpo de la peticion no puede estar vacio");

        if (string.IsNullOrEmpty(loginDTo.Email) || string.IsNullOrEmpty(loginDTo.Password))
            throw new ArgumentException("Todos los campos son obligatorios");

        var exist = await _repository.GetUserByEmailWithRoleAsync(loginDTo.Email);

        if (exist == null || !BCrypt.Net.BCrypt.Verify(loginDTo.Password, exist.PassHash))
            throw new SecurityException("Credenciales invalidas");

        return await GenerateTokens(exist);
    }

    public async Task<bool> RevokeAsync(RevokeDto revokeDto)
    {
        if (revokeDto == null)
            throw new ArgumentNullException("El cuerpo de la peticion no puede estar vacio");

        var exist = await _repository.FindByEmailAsync(revokeDto.Email);

        if (exist == null) return false;

        exist.RefreshToken = null;
        exist.RefreshTokenExpire = null;
        exist.UpdateAt = DateTime.UtcNow;

        await _repository.UpdateAsync(exist);
        return true;
    }

    public async Task<AuthResponseDto> RefreshAsync(RefreshTokenDTo refreshTokenDTo)
    {
        var principal = GetClaimsFromExpire(refreshTokenDTo.Token);
        var email = principal.FindFirst(ClaimTypes.Email);

        var exist = await _repository.FindByEmailAsync(email.Value);

        if (exist == null || exist.RefreshToken != refreshTokenDTo.RefreshToken ||
            exist.RefreshTokenExpire <= DateTime.UtcNow)
            throw new SecurityException("Token invalido o expiro");

        return await GenerateTokens(exist);
    }

    // Generate Jwt
    private JwtSecurityToken GenerateJwt(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };

        return new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            signingCredentials: credentials,
            expires: DateTime.UtcNow.AddMinutes(15)
        );
    }

    // Generate Refresh
    private string GenerateRefresh()
    {
        var array = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(array);
        return Convert.ToBase64String(array);
    }

    // Validamos que el token sea valido
    private ClaimsPrincipal GetClaimsFromExpire(string token)
    {
        var validParams = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
            ValidateLifetime = false
        };

        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(token, validParams, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityException("Token invalido");

        return principal;
    }

    // Generamos los tokens
    private async Task<AuthResponseDto> GenerateTokens(User user)
    {
        var token = GenerateJwt(user);
        var refresh = GenerateRefresh();

        user.RefreshToken = refresh;
        user.RefreshTokenExpire = DateTime.UtcNow.AddDays(7);
        user.UpdateAt = DateTime.UtcNow;

        await _repository.UpdateAsync(user);

        var handler = new JwtSecurityTokenHandler();
        var tokenString = handler.WriteToken(token);

        var response = _mapper.Map<AuthResponseDto>(user);
        response.Token = tokenString;

        return response;
    }

}