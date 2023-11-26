namespace FilmForumModels.Dtos.UserDtos;

public record ChangeMultifactorAuthenticationDto(string TotpCode, bool MultifactorAuthentication);
