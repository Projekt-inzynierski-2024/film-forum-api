using FilmForumModels.Entities;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.Services.Interfaces;

namespace FilmForumWebAPI.Services;

public class UserDiagnosticsService : IUserDiagnosticsService
{
    private readonly UsersDatabaseContext _usersDatabaseContext;

    public UserDiagnosticsService(UsersDatabaseContext usersDatabaseContext)
    {
        _usersDatabaseContext = usersDatabaseContext;
    }

    public async Task Create(int userId)
    {
        await _usersDatabaseContext.UserDiagnostics.AddAsync(new UserDiagnostics(userId));
        await _usersDatabaseContext.SaveChangesAsync();
    }
}
