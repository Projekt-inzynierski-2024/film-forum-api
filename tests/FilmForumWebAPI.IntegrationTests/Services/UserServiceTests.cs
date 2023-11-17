using AuthenticationManager.Services;
using FilmForumModels.Dtos.UserDtos;
using FilmForumModels.Entities;
using FilmForumModels.Models.Settings;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.IntegrationTests.HelpersForTests;
using FilmForumWebAPI.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PasswordManager.Services;

namespace FilmForumWebAPI.IntegrationTests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task GetAllAsync()
    {
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext));

        await usersDatabaseContext.Users.AddAsync(new User() { Password = "pdsade23#!@!DA2", Email = "ememail@da.pl", Username = "nickanem1232" });
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "dadada2#!@#@DAas", Email = "e2m@email.pl", Username = "name" });
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "da@#!@#a", Email = "em1@emailtest.pl", Username = "name12" });
        await usersDatabaseContext.SaveChangesAsync();

        List<GetUserDto> users = await userService.GetAllAsync();
        
        users.Count.Should().Be(3);
    }

    [Fact]
    public async Task GetAsync()
    {
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext));

        await usersDatabaseContext.Users.AddAsync(new User() { Password = "pdsade23#!@!DA2", Email = "ememail@da.pl", Username = "nickanem1232" });
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "dadada2#!@#@DAas", Email = "e2m@email.pl", Username = "name" });
        await usersDatabaseContext.Users.AddAsync(new User() { Password = "da@#!@#a", Email = "em1@emailtest.pl", Username = "name12" });
        await usersDatabaseContext.SaveChangesAsync();

        GetUserDto? user = await userService.GetAsync(1);

        user.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAsy2nc()
    {
        UsersDatabaseContext usersDatabaseContext = await DatabaseHelper.CreateAndPrepareUsersDatabaseContextForTesting();
        UserService userService = new(usersDatabaseContext, new PasswordService(), new JwtService(), Options.Create(new JwtDetails()), new RoleService(usersDatabaseContext));

        await usersDatabaseContext.Users.AddAsync(new User() { Password = "dasda", Email = "em@da.pl", Username = "da" });
        await usersDatabaseContext.SaveChangesAsync();

        int result = await userService.RemoveAsync(1);

        result.Should().Be(1);
    }
}