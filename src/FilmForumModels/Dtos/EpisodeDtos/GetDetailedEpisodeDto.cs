﻿using FilmForumModels.Dtos.ActorDtos;
using FilmForumModels.Dtos.DirectorDtos;
using FilmForumModels.Dtos.FilmDtos;
using FilmForumModels.Dtos.ReviewDtos;
using FilmForumModels.Entities;

namespace FilmForumModels.Dtos.EpisodeDtos;

public class GetDetailedEpisodeDto
{
    public GetDetailedEpisodeDto(Episode episode)
    {
        Id = episode.Id;
        Title = episode.Title;
        Description = episode.Description;
        EpisodeNumber = episode.EpisodeNumber;
        SeasonNumber = episode.SeasonNumber;
        Length = episode.Length;
        Year = episode.Year;
        Film = new GetFilmDto(episode.Film ?? new());
        Directors = episode.Directors?.Select(d => new GetDirectorDto(d)).ToList() ?? new();
        Actors = episode.Actors?.Select(a => new GetActorDto(a)).ToList() ?? new();
        Reviews = episode.Reviews?.Select(r => new GetReviewDto(r)).ToList() ?? new();
    }

    public string Id { get; set; }

    public string? Title { get; set; } = null;

    public string? Description { get; set; } = null;

    public int? EpisodeNumber { get; set; } = null;

    public int? SeasonNumber { get; set; } = null;

    public int Length { get; set; } = 0;

    public int Year { get; set; } = 2023;

    public GetFilmDto Film { get; }

    public List<GetDirectorDto> Directors { get; }

    public List<GetActorDto> Actors { get; }

    public List<GetReviewDto> Reviews { get; set; } = new List<GetReviewDto>();
}