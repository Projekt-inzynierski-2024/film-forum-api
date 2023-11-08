namespace FilmForumModels.Dtos.RequestDtos;

public record CreateRequestLogDto(int UserId, string RequestPath, string IpAddress, string HttpMethod, int StatusCode);