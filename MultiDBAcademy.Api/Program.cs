using Microsoft.EntityFrameworkCore;
using MultiDBAcademy.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  Database
var connection = builder.Configuration.GetConnectionString("ConnectionDefault");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connection, MySqlServerVersion.AutoDetect(connection)));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();

