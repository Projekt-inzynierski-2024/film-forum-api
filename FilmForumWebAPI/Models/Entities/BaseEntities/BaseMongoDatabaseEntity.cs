using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FilmForumWebAPI.Models.Entities.BaseEntities;

public abstract class BaseMongoDatabaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
}
