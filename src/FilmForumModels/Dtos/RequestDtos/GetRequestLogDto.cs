using FilmForumModels.Entities;

namespace FilmForumModels.Dtos.RequestDtos;

public class GetRequestLogDto
{
    public GetRequestLogDto(RequestLog requestLog)
    {
        Id = requestLog.Id;
        UserId = requestLog.UserId;
        RequestPath = requestLog.RequestPath;
        IpAddress = requestLog.IpAddress;
        HttpMethod = requestLog.HttpMethod;
        StatusCode = requestLog.StatusCode;
        SentAt = requestLog.SentAt;
    }

    public int Id { get; set; }

    public int UserId { get; set; }

    public string RequestPath { get; set; }

    public string IpAddress { get; set; }

    public string HttpMethod { get; set; }

    public int StatusCode { get; set; }

    public DateTime SentAt { get; set; }
}