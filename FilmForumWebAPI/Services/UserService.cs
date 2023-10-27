using FilmForumWebAPI.Database;
using FilmForumWebAPI.Models.Dtos.UserDtos;
using FilmForumWebAPI.Models.Entities;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmForumWebAPI.Services;

public class UserService : IUserService
{
    private readonly UsersDatabaseContext _usersDatabaseContext;
    private readonly IPasswordService _passwordService;

    public UserService(UsersDatabaseContext usersDatabaseContext,
                       IPasswordService passwordService)
    {
        _usersDatabaseContext = usersDatabaseContext;
        _passwordService = passwordService;
    }

    public async Task<bool> UserWithIdExistsAsync(int id)
      => await _usersDatabaseContext.Users.AnyAsync(user => user.Id == id);

    public async Task<bool> UserWithUsernameExistsAsync(string username)
        => await _usersDatabaseContext.Users.AnyAsync(user => user.Username == username);

    public async Task<bool> UserWithEmailExistsAsync(string email)
        => await _usersDatabaseContext.Users.AnyAsync(user => user.Email == email);

    public async Task<GetUserDto?> GetUserAsync(int id)
    {
        User? user = await _usersDatabaseContext.Users.FirstOrDefaultAsync(x => x.Id == id);       
        return user is not null ? new GetUserDto(user.Id, user.Username, user.Email) : null;
    }

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

        return new UserCreatedDto(createUserDto.Username, createUserDto.Email);
    }

    public async Task<bool> LogInAsync(LogInDto logInDto)
    {
        User? user = await _usersDatabaseContext.Users.FirstOrDefaultAsync(x => x.Email == logInDto.Email);
        if (user == null)
        {
            return false;
        }
        if (_passwordService.VerifyPassword(logInDto.Password, user.Password))
        {
            return true;
        }
        throw new NotImplementedException();
    }
}