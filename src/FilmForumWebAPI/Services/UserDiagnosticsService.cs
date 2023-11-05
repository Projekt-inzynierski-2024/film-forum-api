using FilmForumModels.Entities;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmForumWebAPI.Services;

public class UserDiagnosticsService : IUserDiagnosticsService
{
    private readonly UsersDatabaseContext _usersDatabaseContext;

    public UserDiagnosticsService(UsersDatabaseContext usersDatabaseContext)
    {
        _usersDatabaseContext = usersDatabaseContext;
    }

    public async Task CreateAsync(int userId)
    {
        await _usersDatabaseContext.UserDiagnostics.AddAsync(new UserDiagnostics(userId));
        await _usersDatabaseContext.SaveChangesAsync();
    }

    public async Task UpdateLastFailedSignInAsync(string userEmail)
        => await _usersDatabaseContext.Users.FirstOrDefaultAsync(user => user.Email == userEmail)
                                            .ContinueWith(async task =>
                                            {
                                                if(task?.Result?.Id is int userId)
                                                {
                                                    await _usersDatabaseContext.UserDiagnostics.Where(x => x.UserId == userId)
                                                                                               .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LastFailedSignIn, DateTime.UtcNow));
                                                }
                                            });

    public async Task UpdateLastSuccessfullSignInAsync(int userId)
        => await _usersDatabaseContext.UserDiagnostics.Where(x => x.UserId == userId)
                                                      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LastSuccessfullSignIn, DateTime.UtcNow));
}
