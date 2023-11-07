using FilmForumModels.Models.Enums;

namespace FilmForumModels.Dtos.RoleDtos;

public record CreateUserToRole(RoleEnum Role, int UserId);
