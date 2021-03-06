using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;
using ScoreSheets.Common.Database;

namespace ScoreSheets.Common.Entities
{
	[CollectionName("ScoreSheets.BackgroundOperation")]
	public class BackgroundOperation : MongoEntity
	{
		public string OperationAuthorUsername { get; set; }

		[BsonRequired]
		public DateTime ExpirationDate { get; set; }

		[BsonIgnoreIfNull]
		public DateTime? CompletionDate { get; set; }
	}
}
