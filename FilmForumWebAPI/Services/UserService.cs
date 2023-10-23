using FilmForumWebAPI.Models.Dtos;
using FilmForumWebAPI.Services.Interfaces;

namespace FilmForumWebAPI.Services;

public class UserService : IUserService
{
    public async Task<bool> UsernameExists(string username)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> EmailExists(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<int> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
    {
        throw new NotImplementedException();
    }

    public async Task<UserCreatedDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        throw new NotImplementedException();
        return new UserCreatedDto(createUserDto.Username, createUserDto.Email);
    }
}