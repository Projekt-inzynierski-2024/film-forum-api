using FilmForumWebAPI.Database;
using Microsoft.EntityFrameworkCore;

namespace FilmForumWebAPI.IntegrationTests.HelpersForTests;

public class DatabaseHelper
{
    private static readonly string _usersDatabaseConnectionString = "Server=localhost,2433;Database=UsersDb;Uid=SA;Pwd=zaq1@WSX;TrustServerCertificate=True";
    
    private static async Task ResetUsersDatabaseAsync(UsersDatabaseContext usersDatabaseContext)
    {
        await usersDatabaseContext.Database.EnsureDeletedAsync();
        await usersDatabaseContext.Database.EnsureCreatedAsync();
    }

    public static async Task<UsersDatabaseContext> CreateAndPrepareUsersDatabaseContextForTesting()
    {
        DbContextOptionsBuilder<UsersDatabaseContext> builder = new();
        builder.UseSqlServer(_usersDatabaseConnectionString);
        UsersDatabaseContext usersDatabaseContext = new(builder.Options);
        await ResetUsersDatabaseAsync(usersDatabaseContext);
        return usersDatabaseContext;
    }
}