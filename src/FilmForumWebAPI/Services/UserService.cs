using AuthenticationManager.Interfaces;
using FilmForumModels.Dtos.UserDtos;
using FilmForumModels.Entities;
using FilmForumModels.Models.Enums;
using FilmForumModels.Models.Password;
using FilmForumModels.Models.Settings;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PasswordManager.Interfaces;
using FilmForumModels.Models.Exceptions;

namespace FilmForumWebAPI.Services;

public class UserService : IUserService
{
    private readonly UsersDatabaseContext _usersDatabaseContext;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;
    private readonly JwtDetails _jwtDetails;
    private readonly IRoleService _rolesService;
    private readonly IMultifactorAuthenticationService _multifactorAuthenticationService;

    public UserService(UsersDatabaseContext usersDatabaseContext,
                       IPasswordService passwordService,
                       IJwtService jwtService,
                       IOptions<JwtDetails> jwtDetails,
                       IRoleService rolesService,
                       IMultifactorAuthenticationService multifactorAuthenticationService)
    {
        _usersDatabaseContext = usersDatabaseContext;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _jwtDetails = jwtDetails.Value;
        _rolesService = rolesService;
        _multifactorAuthenticationService = multifactorAuthenticationService;
    }

    public async Task<int> ChangeMultifactorAuthAsync(int id, bool multifactorAuth)
        => await _usersDatabaseContext.Users.Where(x => x.Id == id)
                                      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.MultifactorAuth, multifactorAuth));

    /// <summary>
    /// Checks if user with given id exists in database
    /// </summary>
    /// <param name="id">User's id</param>
    /// <returns>True if user exists in database, otherwise false</returns>
    public async Task<bool> UserWithIdExistsAsync(int id)
        => await _usersDatabaseContext.Users.AsNoTracking().AnyAsync(user => user.Id == id);

    /// <summary>
    /// Checks if user with given username exists in database
    /// </summary>
    /// <param name="username">User's username</param>
    /// <returns>True if user exists in database, otherwise false</returns>
    public async Task<bool> UserWithUsernameExistsAsync(string username)
        => await _usersDatabaseContext.Users.AsNoTracking().AnyAsync(user => user.Username == username);

    /// <summary>
    /// Checks if user with given email exists in database
    /// </summary>
    /// <param name="email">User's email</param>
    /// <returns>True if user exists in database, otherwise false</returns>
    public async Task<bool> UserWithEmailExistsAsync(string email)
        => await _usersDatabaseContext.Users.AsNoTracking().AnyAsync(user => user.Email == email);

    /// <summary>
    /// Gets all users from database
    /// </summary>
    /// <returns>List of users existing in database or new empty list if there aren't any users in database</returns>
    public async Task<List<GetUserDto>> GetAllAsync()
        => await _usersDatabaseContext.Users.AsNoTracking().Select(x => new GetUserDto(x)).ToListAsync();

    /// <summary>
    /// Gets user with given id from database
    /// </summary>
    /// <param name="id">User's id</param>
    /// <returns>User instance if user was found, otherwise null</returns>
    public async Task<GetUserDto?> GetAsync(int id)
        => await _usersDatabaseContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id) is User user ? new GetUserDto(user) : null;

    /// <summary>
    /// Changes user's password
    /// </summary>
    /// <param name="id">User's id</param>
    /// <param name="changePasswordDto">Details to change password: new password and confirm password</param>
    /// <returns>The total number of rows updated in the database</returns>
    public async Task<int> ChangePasswordAsync(int id, ChangePasswordDto changePasswordDto)
         => await _usersDatabaseContext.Users.Where(x => x.Id == id)
                                       .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Password, _passwordService.HashPassword(changePasswordDto.Password)));

    /// <summary>
    /// Sets new password for user who has lost their password
    /// </summary>
    /// <param name="resetPasswordDto">Details to set new password: email, new password, confirm password and reset password token</param>
    /// <returns>The total number of rows updated in the database</returns>
    public async Task<int> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
         => await _usersDatabaseContext.Users.Where(x => x.Email == resetPasswordDto.Email)
                                       .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Password, _passwordService.HashPassword(resetPasswordDto.Password)));

    /// <summary>
    /// Changes user's email
    /// </summary>
    /// <param name="id">User's id</param>
    /// <param name="email">User's new email</param>
    /// <returns>The total number of rows updated in the database</returns>
    public async Task<int> ChangeEmailAsync(int id, string email)
        => await _usersDatabaseContext.Users.Where(x => x.Id == id)
                                      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Email, email));

    /// <summary>
    /// Changes user's username
    /// </summary>
    /// <param name="id">User's id</param>
    /// <param name="username">User's new username</param>
    /// <returns>The total number of rows updated in the database</returns>
    public async Task<int> ChangeUsernameAsync(int id, string username)
        => await _usersDatabaseContext.Users.Where(x => x.Id == id)
                                      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Username, username));

    /// <summary>
    /// Updates password reset token which is necessary when user wants to change their password and its expiration date for user with given email.
    /// </summary>
    /// <param name="email">User's email</param>
    /// <param name="passwordResetTokenWithExpiration">Details to update token: token's new value and token's new expiration date</param>
    /// <returns>The total number of rows updated in the database</returns>
    public async Task<int> UpdatePasswordResetTokenAsync(string email, PasswordResetTokenWithExpirationDate passwordResetTokenWithExpiration)
        => await _usersDatabaseContext.Users.Where(x => x.Email == email)
                                      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.RecoverPasswordToken, passwordResetTokenWithExpiration.Token)
                                                                            .SetProperty(x => x.RecoverPasswordTokenExpiration, passwordResetTokenWithExpiration.ExpirationDate));

    /// <summary>
    /// Adds new user to database
    /// </summary>
    /// <param name="createUserDto">Details to create new user: email, username, password</param>
    /// <returns><see cref="UserCreatedDto"/> instance with details about created user</returns>
    /// <exception cref="InvalidRoleNameException">When user's main role is not valid enum</exception>
    public async Task<UserCreatedDto> CreateAsync(CreateUserDto createUserDto, UserRole userMainRole)
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

        List<UserRole> roles = _rolesService.PrepareUserRolesToSaveInDatabase(userMainRole);
        await _rolesService.ChangeUserRolesAsync(createdUser!.Id, roles);

        string token = _jwtService.GenerateToken(createdUser!, roles.Select(x => x.ToString()), _jwtDetails);

        return new UserCreatedDto(user.Id, createUserDto.Username, createUserDto.Email, token);
    }

    /// <summary>
    /// Deletes user with given id from database
    /// </summary>
    /// <param name="id">User's id</param>
    /// <returns>The total number of rows deleted in the database</returns>
    public async Task<int> RemoveAsync(int id)
        => await _usersDatabaseContext.Users.Where(x => x.Id == id).ExecuteDeleteAsync();

    /// <summary>
    /// Logs in user with given email and password
    /// </summary>
    /// <param name="logInDto">Details to log in: email and password</param>
    /// <returns><see cref="UserSignedInDto"/> istance with details about logged in user</returns>
    public async Task<UserSignedInDto?> LogInAsync(LogInDto logInDto)
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
        if (user.MultifactorAuth && (logInDto.TotpCode is null || ! await _multifactorAuthenticationService.VerifyCodeAsync(user.Email, logInDto.TotpCode)))
        {
            return null;
        }

        List<string> roles = await _rolesService.GetUserRolesNamesAsync(user.Id);
        string token = _jwtService.GenerateToken(user, roles, _jwtDetails);

        return new UserSignedInDto(user.Id, user.Username, token);
    }

    /// <summary>
    /// Validates if reset password token and it expiration date are valid
    /// </summary>
    /// <param name="resetPasswordToken">Token necessary when user wants to reset their password</param>
    /// <returns><see cref="ValidateResetPasswordTokenResult"/> instance informing if token and its expiration date are valid</returns>
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
        if (!user.RecoverPasswordTokenExpiration.HasValue || user.RecoverPasswordTokenExpiration.Value < DateTime.UtcNow)
        {
            return new ValidateResetPasswordTokenResult(false, "Reset password token is empty or expired");
        }
        return new ValidateResetPasswordTokenResult(true, "Valid token");
    }
}
