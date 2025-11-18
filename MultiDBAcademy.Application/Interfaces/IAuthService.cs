using MultiDBAcademy.Application.Dtos;

namespace MultiDBAcademy.Application.Interfaces;

public interface IAuthService
{
    Task<AuthReguisterResponseDTo> RegisterAsync(RegisterDTo registerDTo);
    Task<AuthResponseDto> LoginAsync(LoginDTo loginDTo);
    Task<bool> RevokeAsync(RevokeDto revokeDto);
    Task<AuthResponseDto> RefreshAsync(RefreshTokenDTo refreshTokenDTo);
}