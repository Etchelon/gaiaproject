using System;
using GaiaProject.Common.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace GaiaProject.Core.Model
{
	[CollectionName("GaiaProject.Core.Notifications")]
	[BsonDiscriminator(RootClass = true)]
	[BsonKnownTypes(typeof(GameNotification))]
	public class Notification : MongoEntity
	{
		[BsonRequired]
		[BsonRepresentation(BsonType.String)]
		public NotificationType Type { get; protected set; }

		[BsonRequired]
		public DateTime DateCreated { get; set; }

		[BsonRequired]
		public string TargetUserId { get; set; }

		[BsonRequired]
		public string Text { get; set; }

		[BsonRequired]
		public bool IsRead { get; set; } = false;

		[BsonIgnoreIfNull]
		public DateTime? ReadTimestamp { get; set; }

		public Notification()
		{
			Type = NotificationType.Generic;
		}
	}

	[BsonDiscriminator(nameof(GameNotification))]
	public class GameNotification : Notification
	{
		[BsonRequired]
		public string GameId { get; set; }

		public GameNotification()
		{
			Type = NotificationType.Game;
		}
	}
}