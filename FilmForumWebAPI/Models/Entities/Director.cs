using FilmForumWebAPI.Models.Dtos.DirectorDtos;
using FilmForumWebAPI.Models.Entities.BaseEntities;

namespace FilmForumWebAPI.Models.Entities;

public class Director : BaseMongoDatabaseEntity
{
    public Director()
    {
    }

    public Director(CreateDirectorDto createDirectorDto)
    {
        Name = createDirectorDto.Name;
        Surname = createDirectorDto.Surname;
        Description = createDirectorDto.Description;
    }
    public string Name { get; set; } = string.Empty;

    public string Surname { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}