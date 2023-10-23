using FilmForumWebAPI.Models.Dtos;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IUserService
{
    public Task<bool> UsernameExists(string username);

    public Task<bool> EmailExists(string email);

    public Task<UserCreatedDto> CreateUserAsync(CreateUserDto createUserDto);

    public Task<int> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
}