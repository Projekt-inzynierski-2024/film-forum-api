using FilmForumModels.Dtos.RequestDtos;
using FilmForumModels.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmForumModels.Entities;

[Table("request_logs")]
public class RequestLog : BaseMsSqlDatabaseEntity
{
    public RequestLog()
    { }

    public RequestLog(CreateRequestLogDto createRequestLogDto)
    {
        UserId = createRequestLogDto.UserId;
        RequestPath = createRequestLogDto.RequestPath;
        IpAddress = createRequestLogDto.IpAddress;
        HttpMethod = createRequestLogDto.HttpMethod;
        StatusCode = createRequestLogDto.StatusCode;
    }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("request_path")]
    public string RequestPath { get; set; } = string.Empty;

    [Column("ip_address")]
    public string IpAddress { get; set; } = string.Empty;

    [Column("http_method")]
    public string HttpMethod { get; set; } = string.Empty;

    [Column("status_code")]
    public int StatusCode { get; set; }

    [Column("sent_at")]
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}