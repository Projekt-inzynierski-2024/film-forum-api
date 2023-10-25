using FilmForumWebAPI.Models.Dtos.User;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IUserService
{
    public Task<bool> UserWithUsernameExists(string username);

    public Task<bool> UserWithEmailExists(string email);

    public Task<UserCreatedDto> CreateUserAsync(CreateUserDto createUserDto);

    public Task<int> ChangePasswordAsync(int id, ChangePasswordDto changePasswordDto);

    public Task<bool> LogInAsync(LogInDto logInDto);

}