using MultiDBAcademy.Application.Dtos;
using MultiDBAcademy.Domain.Entities;

namespace MultiDBAcademy.Application.Interfaces;

public interface IUserService
{
   Task<IEnumerable<UserDto>> GetAllAsync();
   Task<UserDto> GetByIdAsync(int id);
   Task<UserDto> UpdateAsync(int id, UserDto userDto);
   Task<bool>  DeleteAsync(int id);
}