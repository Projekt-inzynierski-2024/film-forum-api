using FilmForumModels.Dtos.RequestDtos;
using FilmForumModels.Entities;
using FilmForumWebAPI.Database;
using FilmForumWebAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmForumWebAPI.Services;

public class RequestLogService : IRequestLogService
{
    private readonly UsersDatabaseContext _usersDatabaseContext;

    public RequestLogService(UsersDatabaseContext usersDatabaseContext) => _usersDatabaseContext = usersDatabaseContext;

    /// <summary>
    /// Saves new request log combined with user in database
    /// </summary>
    /// <param name="createRequestLogDto">Request's details to save</param>
    /// <returns>The number of entities written to the database</returns>
    public async Task<int> CreateAsync(CreateRequestLogDto createRequestLogDto)
    {
        RequestLog requestLog = new(createRequestLogDto);

        await _usersDatabaseContext.RequestLogs.AddAsync(requestLog);
        return await _usersDatabaseContext.SaveChangesAsync();
    }

    /// <summary>
    /// Gets all requests' logs from database
    /// </summary>
    /// <returns>List of requests' logs from database</returns>
    public async Task<List<GetRequestLogDto>> GetAllAsync()
        => await _usersDatabaseContext.RequestLogs.AsNoTracking().Select(x => new GetRequestLogDto(x)).ToListAsync();

    /// <summary>
    /// Gets request's log with given <paramref name="id"/> from database
    /// </summary>
    /// <param name="id">Request's log's id</param>
    /// <returns>Request's log's details</returns>
    public async Task<GetRequestLogDto?> GetAsync(int id)
        => await _usersDatabaseContext.RequestLogs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id) is RequestLog requestLog ? new GetRequestLogDto(requestLog) : null;

    /// <summary>
    /// Gets all requests' logs from database for user with given <paramref name="userId"/>
    /// </summary>
    /// <param name="userId">User's id</param>
    /// <returns>List of all requests' logs for specified user</returns>
    public async Task<List<GetRequestLogDto>> GetUserAllRequestsLogsAsync(int userId)
        => await _usersDatabaseContext.RequestLogs.AsNoTracking().Where(x => x.UserId == userId).Select(x => new GetRequestLogDto(x)).ToListAsync();

    /// <summary>
    /// Removes request's log with given <paramref name="id"/> from database
    /// </summary>
    /// <param name="id">Request's log's id</param>
    /// <returns>The number of rows deleted in the database</returns>
    public async Task<int> RemoveAsync(int id)
        => await _usersDatabaseContext.RequestLogs.Where(x => x.Id == id).ExecuteDeleteAsync();

    /// <summary>
    /// Removes all requests' logs from database for user with given <paramref name="userId"/>
    /// </summary>
    /// <param name="userId">User's id</param>
    /// <returns>The number of rows deleted in the database</returns>
    public async Task<int> RemoveUserRequestsLogsAsync(int userId)
        => await _usersDatabaseContext.RequestLogs.Where(x => x.UserId == userId).ExecuteDeleteAsync();
}