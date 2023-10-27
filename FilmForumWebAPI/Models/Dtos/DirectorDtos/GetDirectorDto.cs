using FilmForumWebAPI.Models.Entities;

namespace FilmForumWebAPI.Models.Dtos.DirectorDtos;

public class GetDirectorDto
{
    public GetDirectorDto(Director director)
    {
        Id = director.Id;
    }

    public string Id { get; set; }
}