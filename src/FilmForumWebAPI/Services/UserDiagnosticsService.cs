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
    public async Task<int> CreateAsync(int userId)
    {
        if (await _usersDatabaseContext.Users.AnyAsync(x => x.Id == userId))
        {
            await _usersDatabaseContext.UserDiagnostics.AddAsync(new UserDiagnostics(userId));
            return await _usersDatabaseContext.SaveChangesAsync();
        }
        return 0;
    }

    /// <summary>
    /// Updates user's last failed attempt to sign in date
    /// </summary>
    /// <param name="userEmail">User's email</param>
    /// <returns>The total number of rows updated in the database</returns>
    public async Task<int> UpdateLastFailedSignInAsync(string userEmail)
    {
        int? userId = (await _usersDatabaseContext.Users.FirstOrDefaultAsync(user => user.Email == userEmail))?.Id;
        if (userId.HasValue)
        {
            return await _usersDatabaseContext.UserDiagnostics.Where(x => x.UserId == userId.Value)
                                                              .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LastFailedSignIn, DateTime.UtcNow));
        }
        return 0;
    }

    /// <summary>
    /// Updates user's last successfull sign in date
    /// </summary>
    /// <param name="userId">User's id</param>
    /// <returns>The total number of rows updated in the database</returns>
    public async Task<int> UpdateLastSuccessfullSignInAsync(int userId)
        => await _usersDatabaseContext.UserDiagnostics.Where(x => x.UserId == userId)
                                                      .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LastSuccessfullSignIn, DateTime.UtcNow));

    /// <summary>
    /// Updates user's last username change date
    /// </summary>
    /// <param name="userId">User's id</param>
    /// <returns>The total number of rows updated in the database</returns>
    public async Task<int> UpdateLastUsernameChangeAsync(int userId)
       => await _usersDatabaseContext.UserDiagnostics.Where(x => x.UserId == userId)
                                                     .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LastUsernameChange, DateTime.UtcNow));

    /// <summary>
    /// Updates user's last email change date
    /// </summary>
    /// <param name="userId">User's id</param>
    /// <returns>The total number of rows updated in the database</returns>
    public async Task<int> UpdateLastEmailChangeAsync(int userId)
       => await _usersDatabaseContext.UserDiagnostics.Where(x => x.UserId == userId)
                                                     .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LastEmailChange, DateTime.UtcNow));

    /// <summary>
    /// Updates user's last password change date
    /// </summary>
    /// <param name="userId">User's id</param>
    /// <returns>The total number of rows updated in the database</returns>
    public async Task<int> UpdateLastPasswordChangeAsync(int userId)
       => await _usersDatabaseContext.UserDiagnostics.Where(x => x.UserId == userId)
                                                     .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LastPasswordChange, DateTime.UtcNow));
}