﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using FilmForumModels.Entities.BaseEntities;
using FilmForumModels.Dtos.FilmDtos;

namespace FilmForumModels.Entities;

public class Film : BaseMongoDatabaseEntity
{
    public Film()
    {
    }

    public Film(CreateFilmDto createFilmDto)
    {
        Title = createFilmDto.Title;
        Description = createFilmDto.Description;
    }

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("isMovie")]
    public bool IsMovie { get; set; } = true;
}
