using System;
using GaiaProject.Common.Database;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace GaiaProject.Common.Entities
{
	[CollectionName("GaiaProject.BackgroundOperation")]
	public class BackgroundOperation : MongoEntity
	{
		public string OperationAuthorUsername { get; set; }

		[BsonRequired]
		public DateTime ExpirationDate { get; set; }

		[BsonIgnoreIfNull]
		public DateTime? CompletionDate { get; set; }
	}
}
