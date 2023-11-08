using FilmForumModels.Models.Enums;

namespace FilmForumModels.Dtos.RoleDtos;

public record CreateUserToRole(UserRole Role, int UserId);