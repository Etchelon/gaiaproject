using System;
using GaiaProject.Common.Database.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Common.Database
{
	public abstract class MongoEntity : IMongoEntity
	{
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public virtual DateTime? Timestamp => this.Id == null ? (DateTime?)null : MongoDB.Bson.ObjectId.Parse(this.Id).CreationTime;

		public virtual int Version => 1;
	}
}
