namespace FilmForumWebAPI.Services.Interfaces;

public interface IUserDiagnosticsService
{
    public Task CreateAsync(int userId);

    Task UpdateLastFailedSignInAsync(string userEmail);

    Task UpdateLastSuccessfullSignInAsync(int userId);

    Task UpdateLastUsernameChangeAsync(int userId);

    Task UpdateLastEmailChangeAsync(int userId);

    Task UpdateLastPasswordChangeAsync(int userId);
}