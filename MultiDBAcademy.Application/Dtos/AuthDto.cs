using MultiDBAcademy.Domain.Entities;

namespace MultiDBAcademy.Application.Dtos;

public class AuthResponseDto
{
    public Guid Id { get; set; }
    
    public string UserName { get; set; }
    public string Email { get; set; }
    public int RoleId { get; set; }
    public string Token { get; set; }
    
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpire { get; set; }
    
    public DateTime CreateAt {get; set;}
    public DateTime UpdateAt { get; set; }
}


public class AuthReguisterResponseDTo
{
    public Guid Id { get; set; }
    
    public string UserName { get; set; }
    public string Email { get; set; }
    public int RoleId { get; set; }

    public DateTime CreateAt {get; set;}
    public DateTime UpdateAt { get; set; }
}

public class LoginDTo
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class RegisterDTo
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public int RoleId { get; set; }
    public string Password { get; set; }
}

public class RefreshTokenDTo
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}

public class RevokeDto
{
    public string Email { get; set; }
}