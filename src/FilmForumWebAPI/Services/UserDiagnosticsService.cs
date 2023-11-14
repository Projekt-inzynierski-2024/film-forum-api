using FilmForumModels.Entities;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmForumWebAPI.Services;

public class UserDiagnosticsService : IUserDiagnosticsService
{
    private readonly UsersDatabaseContext _usersDatabaseContext;

    public UserDiagnosticsService(UsersDatabaseContext usersDatabaseContext) => _usersDatabaseContext = usersDatabaseContext;

    /// <summary>
    /// Adds new user diagnostics to database
    /// </summary>
    /// <param name="userId">User's id</param>
    /// <returns>The number of entities written to the database</returns>
    public async Task CreateAsync(int userId)
    {
        await _usersDatabaseContext.UserDiagnostics.AddAsync(new UserDiagnostics(userId));
        await _usersDatabaseContext.SaveChangesAsync();
    }

    /// <summary>
    /// Updates user's last failed attempt to sign in date
    /// </summary>
    /// <param name="userEmail">User's email</param>
    /// <returns>The total number of rows updated in the database</returns>
    public async Task UpdateLastFailedSignInAsync(string userEmail)
        => await _usersDatabaseContext.Users.FirstOrDefaultAsync(user => user.Email == userEmail)
                                            .ContinueWith(async task =>
                                            {
                                                if (task?.Result?.Id is int userId)
                                                {
                                                    await _usersDatabaseContext.UserDiagnostics.Where(x => x.UserId == userId)
                                                                                               .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LastFailedSignIn, DateTime.UtcNow));
                                                }
                                            });

    /// <summary>
    /// Updates user's last successfull sign in date
    /// </summary>
    /// <param name="userId">User's id</param>
    /// <returns>The total number of rows updated in the database</returns>
    public async Task UpdateLastSuccessfullSignInAsync(int userId)
        => await _usersDatabaseContext.UserDiagnostics.Where(x => x.UserId == userId)
                                                      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LastSuccessfullSignIn, DateTime.UtcNow));

    /// <summary>
    /// Updates user's last username change date
    /// </summary>
    /// <param name="userId">User's id</param>
    /// <returns>The total number of rows updated in the database</returns>
    public async Task UpdateLastUsernameChangeAsync(int userId)
       => await _usersDatabaseContext.UserDiagnostics.Where(x => x.UserId == userId)
                                                     .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LastUsernameChange, DateTime.UtcNow));

    /// <summary>
    /// Updates user's last email change date
    /// </summary>
    /// <param name="userId">User's id</param>
    /// <returns>The total number of rows updated in the database</returns>
    public async Task UpdateLastEmailChangeAsync(int userId)
       => await _usersDatabaseContext.UserDiagnostics.Where(x => x.UserId == userId)
                                                     .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LastEmailChange, DateTime.UtcNow));

    /// <summary>
    /// Updates user's last password change date
    /// </summary>
    /// <param name="userId">User's id</param>
    /// <returns>The total number of rows updated in the database</returns>
    public async Task UpdateLastPasswordChangeAsync(int userId)
       => await _usersDatabaseContext.UserDiagnostics.Where(x => x.UserId == userId)
                                                     .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LastPasswordChange, DateTime.UtcNow));
}