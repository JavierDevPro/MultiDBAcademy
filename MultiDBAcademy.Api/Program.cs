using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MultiDBAcademy.Application.Dtos;
using MultiDBAcademy.Application.Interfaces;
using MultiDBAcademy.Application.Services;
using MultiDBAcademy.Domain.Entities;
using MultiDBAcademy.Domain.Interfaces;
using MultiDBAcademy.Infrastructure.Data;
using MultiDBAcademy.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddAuthentication();

builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddAutoMapper(typeof(MapProfile));

//  Database
var connection = builder.Configuration.GetConnectionString("ConnectionDefault");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connection, MySqlServerVersion.AutoDetect(connection)));

// Jwt Config
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();

