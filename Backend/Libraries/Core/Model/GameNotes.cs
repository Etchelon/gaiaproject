using GaiaProject.Common.Database;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace GaiaProject.Core.Model
{
    [CollectionName("GaiaProject.GameNotes")]
    public class GameNotes : MongoEntity
    {
        [BsonRequired]
        public string GameId { get; set; }

        [BsonRequired]
        public string UserId { get; set; }

        [BsonRequired]
        public string Notes { get; set; }
    }
}