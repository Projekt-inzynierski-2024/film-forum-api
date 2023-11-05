using AuthenticationManager.Interfaces;
using FilmForumModels.Dtos.UserDtos;
using FilmForumModels.Entities;
using FilmForumModels.Models.Password;
using FilmForumModels.Models.Settings;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PasswordManager.Interfaces;

namespace FilmForumWebAPI.Services;

public class UserService : IUserService
{
    private readonly UsersDatabaseContext _usersDatabaseContext;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly IOptions<JwtDetails> _jwtDetails;
    private readonly IRoleService _rolesService;

    public UserService(UsersDatabaseContext usersDatabaseContext,
                       IPasswordService passwordService,
                       IJwtService jwtService,
                       IOptions<JwtDetails> jwtDetails,
                       IRoleService rolesService)
    {
        _usersDatabaseContext = usersDatabaseContext;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _jwtDetails = jwtDetails;
        _rolesService = rolesService;
    }

    public async Task<bool> UserWithIdExistsAsync(int id)
        => await _usersDatabaseContext.Users.AsNoTracking().AnyAsync(user => user.Id == id);

    public async Task<bool> UserWithUsernameExistsAsync(string username)
        => await _usersDatabaseContext.Users.AsNoTracking().AnyAsync(user => user.Username == username);

    public async Task<bool> UserWithEmailExistsAsync(string email)
        => await _usersDatabaseContext.Users.AsNoTracking().AnyAsync(user => user.Email == email);

    public async Task<List<GetUserDto>> GetAllAsync()
        => await _usersDatabaseContext.Users.AsNoTracking().ToListAsync() is IEnumerable<User> users ? users.Select(x => new GetUserDto(x.Id, x.Username, x.Email)).ToList() : new();

    public async Task<GetUserDto?> GetUserAsync(int id)
        => await _usersDatabaseContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id) is User user ? new GetUserDto(user.Id, user.Username, user.Email) : null;

    public async Task<int> ChangePasswordAsync(int id, ChangePasswordDto changePasswordDto)
         => await _usersDatabaseContext.Users.Where(x => x.Id == id)
                                       .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Password, _passwordService.HashPassword(changePasswordDto.Password)));

    public async Task<int> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
         => await _usersDatabaseContext.Users.Where(x => x.Email == resetPasswordDto.Email)
                                       .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Password, _passwordService.HashPassword(resetPasswordDto.Password)));

    public async Task<int> ChangeEmailAsync(int id, string email)
        => await _usersDatabaseContext.Users.Where(x => x.Id == id)
                                      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Email, email));

    public async Task<int> ChangeUsernameAsync(int id, string username)
        => await _usersDatabaseContext.Users.Where(x => x.Id == id)
                                      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Username, username));

    public async Task<int> UpdatePasswordResetTokenAsync(string email, PasswordResetTokenWithExpirationDate passwordResetTokenWithExpiration)
        => await _usersDatabaseContext.Users.Where(x => x.Email == email)
                                      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.RecoverPasswordToken, passwordResetTokenWithExpiration.Token)
                                                                            .SetProperty(x => x.RecoverPasswordTokenExpiration, passwordResetTokenWithExpiration.ExpirationDate));

    public async Task<UserCreatedDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        User user = new()
        {
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            Password = _passwordService.HashPassword(createUserDto.Password)
        };

        await _usersDatabaseContext.Users.AddAsync(user);
        await _usersDatabaseContext.SaveChangesAsync();

        User? createdUser = _usersDatabaseContext.Users.FirstOrDefault(x => x.Username == createUserDto.Username);
        string token = _jwtService.GenerateToken(createdUser!, new List<string>() { "Admin", "Moderator", "User" }, _jwtDetails.Value);

        return new UserCreatedDto(user.Id, createUserDto.Username, createUserDto.Email, token);
    }

    public async Task<string?> LogInAsync(LogInDto logInDto)
    {
        User? user = await _usersDatabaseContext.Users.FirstOrDefaultAsync(x => x.Email == logInDto.Email);
        if (user == null)
        {
            return null;
        }
        if (!_passwordService.VerifyPassword(logInDto.Password, user.Password))
        {
            return null;
        }

        List<string> roles = await _rolesService.GetUserRolesNamesAsync(user.Id);

        return _jwtService.GenerateToken(user, roles, _jwtDetails.Value);
    }

    public async Task<ValidateResetPasswordTokenResult> ValidateResetPasswordTokenAsync(string resetPasswordToken)
    {
        User? user = await _usersDatabaseContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.RecoverPasswordToken == resetPasswordToken);
        if (user is null)
        {
            return new ValidateResetPasswordTokenResult(false, "User not found");
        }
        if (string.IsNullOrWhiteSpace(user.RecoverPasswordToken) || user.RecoverPasswordToken != resetPasswordToken)
        {
            return new ValidateResetPasswordTokenResult(false, "Invalid token");
        }
        if (!user.RecoverPasswordTokenExpiration.HasValue || user.RecoverPasswordTokenExpiration.Value.ToUniversalTime() < DateTime.UtcNow)
        {
            return new ValidateResetPasswordTokenResult(false, "Rest password token is empty or expired");
        }
        return new ValidateResetPasswordTokenResult(true, "Valid token");
    }
}