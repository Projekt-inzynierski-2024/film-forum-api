using FilmForumWebAPI.Database;
using FilmForumWebAPI.Models.Dtos.User;
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

    public async Task<bool> UserWithUsernameExists(string username) 
        => await _usersDatabaseContext.Users.AnyAsync(user => user.Username == username);

    public async Task<bool> UserWithEmailExists(string email)
        => await _usersDatabaseContext.Users.AnyAsync(user => user.Email == email);    
    
    public async Task<int> ChangePasswordAsync(int id, ChangePasswordDto changePasswordDto)
         => await _usersDatabaseContext.Users.Where(x => x.Id == id)
                                       .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.Password, _passwordService.HashPassword(changePasswordDto.NewPassword)));

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
        if(_passwordService.VerifyPassword(logInDto.Password, user.Password))
        {
            return true;
        }
        throw new NotImplementedException();
    }
}