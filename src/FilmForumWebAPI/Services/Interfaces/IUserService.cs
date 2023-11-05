using FilmForumModels.Dtos.UserDtos;
using FilmForumModels.Models.Password;

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
    
    Task<int> ChangeUsernameAsync(int id, string username);

    Task<string?> LogInAsync(LogInDto logInDto);

    Task<int> UpdatePasswordResetTokenAsync(string email, PasswordResetTokenWithExpirationDate passwordResetTokenWithExpiration);

    Task<ValidateResetPasswordTokenResult> ValidateResetPasswordToken(string resetPasswordToken);

    Task<int> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
}