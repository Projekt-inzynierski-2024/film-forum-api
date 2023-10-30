using System.Text.Json;

namespace FilmForumModels.Models.Errors;

public record struct RequestError(int StatusCode, string ErrorMessage)
{
    public override readonly string ToString() => JsonSerializer.Serialize(this);
}
