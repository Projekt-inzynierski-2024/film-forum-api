using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FilmForumModels.Entities.BaseEntities;

public abstract class BaseMongoDatabaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
}
