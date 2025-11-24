using AutoMapper;
using Moq;
using MultiDBAcademy.Application.Dtos;
using MultiDBAcademy.Application.Services;
using MultiDBAcademy.Domain.Entities;
using MultiDBAcademy.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MultiDBAcademy.Test.Unit;

public class AuthServiceTest
{
    private readonly Mock<IRepository<User>> _repository;
    private readonly Mock<IMapper> _mapper;
    private readonly AuthService _service;
    private readonly IConfiguration _config;

    public AuthServiceTest()
    {
        _repository = new Mock<IRepository<User>>();
        _mapper = new Mock<IMapper>();
        _service = new AuthService(_repository.Object, _mapper.Object, _config);
    }

    [Fact]
    public async Task ReturnDtoWhenUserRegister()
    {
        var registerDto = new RegisterDTo
        {
            UserName = "prueba",
            Email = "prueba@gmail.com",
            Password = "12345",
            RoleId = 1
        };

        var user = new User
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email,
            PassHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            RoleId = registerDto.RoleId,
        };
        
        var user2 = new User
        {
            UserName = registerDto.UserName,
            Email = registerDto.Email,
            PassHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            RoleId = registerDto.RoleId,
        };

        var responseDto = new AuthReguisterResponseDTo
        {
            UserName = user.UserName,
            Email = user.Email,
            RoleId = user.RoleId
        };

        _mapper.Setup(mapper => mapper.Map<User>(registerDto))
            .Returns(user);

        _mapper.Setup(mapper => mapper.Map<AuthReguisterResponseDTo>(user))
            .Returns(responseDto);

        _repository.Setup(repository => repository.AddAsync(user))
            .Returns(Task.FromResult(user));

        var response = await _service.RegisterAsync(registerDto);

        Assert.NotNull(response);
        Assert.Equal(response.Email, responseDto.Email);
        Assert.Equal(response.UserName, responseDto.UserName);
    }
}