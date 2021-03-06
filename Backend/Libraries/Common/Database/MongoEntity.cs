using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ScoreSheets.Common.Database.Abstractions;

namespace ScoreSheets.Common.Database
{
	public abstract class MongoEntity : IMongoEntity
	{
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public virtual DateTime? Timestamp => this.Id == null ? (DateTime?)null : MongoDB.Bson.ObjectId.Parse(this.Id).CreationTime;

		public virtual int Version => 1;
	}
}
