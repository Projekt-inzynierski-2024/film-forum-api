using AuthenticationManager.Interfaces;
using FilmForumModels.Dtos.UserDtos;
using FilmForumModels.Entities;
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
    public UserService(UsersDatabaseContext usersDatabaseContext,
                       IPasswordService passwordService,
                       IJwtService jwtService,
                       IOptions<JwtDetails> jwtDetails)
    {
        _usersDatabaseContext = usersDatabaseContext;
        _passwordService = passwordService;
        _jwtService = jwtService;
        _jwtDetails = jwtDetails;
    }

    public async Task<bool> UserWithIdExistsAsync(int id)
      => await _usersDatabaseContext.Users.AnyAsync(user => user.Id == id);

    public async Task<bool> UserWithUsernameExistsAsync(string username)
        => await _usersDatabaseContext.Users.AnyAsync(user => user.Username == username);

    public async Task<bool> UserWithEmailExistsAsync(string email)
        => await _usersDatabaseContext.Users.AnyAsync(user => user.Email == email);

    public async Task<List<GetUserDto>> GetAllAsync()
        => await _usersDatabaseContext.Users.ToListAsync() is IEnumerable<User> users ? users.Select(x => new GetUserDto(x.Id, x.Username, x.Email)).ToList() : new();

    public async Task<GetUserDto?> GetUserAsync(int id)
        => await _usersDatabaseContext.Users.FirstOrDefaultAsync(x => x.Id == id) is User user ? new GetUserDto(user.Id, user.Username, user.Email) : null;

    public async Task<int> ChangePasswordAsync(int id, ChangePasswordDto changePasswordDto)
         => await _usersDatabaseContext.Users.Where(x => x.Id == id)
                                       .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Password, _passwordService.HashPassword(changePasswordDto.Password)));

    public async Task<int> ChangeEmailAsync(int id, string email)
        => await _usersDatabaseContext.Users.Where(x => x.Id == id)
                                      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Email, email));

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

        return new UserCreatedDto(createUserDto.Username, createUserDto.Email, token);
    }

    public async Task<string> LogInAsync(LogInDto logInDto)
    {
        User? user = await _usersDatabaseContext.Users.FirstOrDefaultAsync(x => x.Email == logInDto.Email);
        if (user == null)
        {
            return string.Empty;
        }
        if (!_passwordService.VerifyPassword(logInDto.Password, user.Password))
        {
            return string.Empty;
        }

        //Roles will be moved to database
        return _jwtService.GenerateToken(user, new List<string>() { "Admin", "Moderator", "User" }, _jwtDetails.Value);
    }
}