using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MultiDBAcademy.Application.Dtos;
using MultiDBAcademy.Application.Interfaces;
using MultiDBAcademy.Application.Services;
using MultiDBAcademy.Domain.Entities;
using MultiDBAcademy.Domain.Interfaces;
using MultiDBAcademy.Infrastructure.Data;
using MultiDBAcademy.Infrastructure.Repositories;
using MultiDBAcademy.Infrastructure.Services.DbEngines;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
var builder = WebApplication.CreateBuilder(args);

// ========== SWAGGER ==========
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MultiDBAcademy API",
        Version = "v1",
        Description = "API para gestión de instancias de bases de datos"
    });

    // Configuración de JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();

// ========== REPOSITORIOS EXISTENTES ==========
builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<ICredentialsRepository, CredentialsRepository>();

// ========== NUEVOS REPOSITORIOS (Instancias) ==========
builder.Services.AddScoped<IInstanceRepository, InstanceRepository>();
builder.Services.AddScoped<ICredentialsRepository, CredentialsRepository>();

// ========== SERVICIOS DE APLICACIÓN EXISTENTES ==========
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICredentialsDbService, CredentialsDbService>();
builder.Services.AddScoped<EmailService>();

// ========== NUEVO SERVICIO (Instancias) ==========
builder.Services.AddScoped<IInstanceService, InstanceService>();

// Agregar en el contenedor DI
builder.Services.AddScoped<IQueryExecutionService, QueryExecutionService>();

// ========== SERVICIOS DE MOTORES DE BD (DB ENGINES) ==========
builder.Services.AddSingleton<IDbEngineService, MySqlEngineService>();
builder.Services.AddSingleton<IDbEngineService, PostgreSqlEngineService>();
builder.Services.AddSingleton<IDbEngineService, MongoDbEngineService>();
builder.Services.AddSingleton<IDbEngineService, RedisEngineService>();
builder.Services.AddSingleton<IDbEngineService, SqlServerEngineService>();

// ========== AUTOMAPPER ==========
builder.Services.AddAutoMapper(typeof(MapProfile));

// ========== DATABASE ==========
var connection = builder.Configuration.GetConnectionString("ConnectionDefault");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connection, MySqlServerVersion.AutoDetect(connection)));

// ========== JWT AUTHENTICATION ==========
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            RoleClaimType = ClaimTypes.Role,
        };
    });

// ========== AUTHORIZATION ==========
builder.Services.AddAuthorization();

// ========== CORS (Opcional - si necesitas para frontend) ==========
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ========== MIDDLEWARE ==========
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// Si activaste CORS, descomenta la siguiente línea:
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "API MultiDBAcademy Running...");

app.MapControllers();


app.Run();


