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

    public async Task<int> CreateAsync(CreateRequestLogDto createRequestLogDto)
    {
        RequestLog requestLog = new(createRequestLogDto);

        await _usersDatabaseContext.RequestLogs.AddAsync(requestLog);
        return await _usersDatabaseContext.SaveChangesAsync();
    }

    public async Task<List<GetRequestLogDto>> GetAllAsync()
        => await _usersDatabaseContext.RequestLogs.AsNoTracking().Select(x => new GetRequestLogDto(x)).ToListAsync();

    public async Task<GetRequestLogDto?> GetAsync(int id)
        => await _usersDatabaseContext.RequestLogs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id) is RequestLog requestLog ? new GetRequestLogDto(requestLog) : null;

    public async Task<List<GetRequestLogDto>> GetUserAllRequestsLogsAsync(int userId)
        => await _usersDatabaseContext.RequestLogs.AsNoTracking().Where(x => x.UserId == userId).Select(x => new GetRequestLogDto(x)).ToListAsync();

    public async Task<int> RemoveAsync(int id)
        => await _usersDatabaseContext.RequestLogs.Where(x => x.Id == id).ExecuteDeleteAsync();

    public async Task<int> RemoveUserRequestsLogsAsync(int userId)
        => await _usersDatabaseContext.RequestLogs.Where(x => x.UserId == userId).ExecuteDeleteAsync();
}