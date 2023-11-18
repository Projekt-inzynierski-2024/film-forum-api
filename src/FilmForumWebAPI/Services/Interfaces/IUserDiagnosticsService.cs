namespace FilmForumWebAPI.Services.Interfaces;

public interface IUserDiagnosticsService
{
    Task<int> CreateAsync(int userId);

    Task<int> UpdateLastFailedSignInAsync(string userEmail);

    Task<int> UpdateLastSuccessfullSignInAsync(int userId);

    Task<int> UpdateLastUsernameChangeAsync(int userId);

    Task<int> UpdateLastEmailChangeAsync(int userId);

    Task<int> UpdateLastPasswordChangeAsync(int userId);
}