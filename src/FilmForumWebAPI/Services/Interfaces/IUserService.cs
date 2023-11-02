using FilmForumModels.Dtos.UserDtos;

namespace FilmForumWebAPI.Services.Interfaces;

public interface IUserService
{
    Task<bool> UserWithIdExistsAsync(int id);

    Task<bool> UserWithUsernameExistsAsync(string username);

    Task<bool> UserWithEmailExistsAsync(string email);

    Task<List<GetUserDto>> GetAllAsync();

    Task<GetUserDto?> GetUserAsync(int id);

    Task<UserCreatedDto> CreateUserAsync(CreateUserDto createUserDto);

    Task<int> ChangePasswordAsync(int id, ChangePasswordDto changePasswordDto);

    Task<int> ChangeEmailAsync(int id, string email);

    Task<string?> LogInAsync(LogInDto logInDto);
}