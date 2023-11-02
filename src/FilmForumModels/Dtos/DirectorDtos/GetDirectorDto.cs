using FilmForumModels.Entities;

namespace FilmForumModels.Dtos.DirectorDtos;

public class GetDirectorDto
{
    public GetDirectorDto(Director director)
    {
        Id = director.Id;
        Name = director.Name;
        Surname = director.Surname;
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }
}
