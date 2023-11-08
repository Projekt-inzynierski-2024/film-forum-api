namespace FilmForumModels.Dtos.UserDtos;

public record UserSignedInDto(int Id, string Username, string JwtToken);