using System;
using System.Collections.Generic;
using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;
using ScoreSheets.Common.Reflection;

namespace GaiaProject.Engine.Model
{
	[BsonNoId]
	public class GameLog
	{
		public DateTime Timestamp { get; set; }
		public string Message { get; set; }
		public bool Important { get; set; }

		[BsonIgnoreIfNull]
		public int? Turn { get; set; }

		[BsonIgnoreIfNull]
		public int? ActionId { get; set; }

		[BsonIgnoreIfNull]
		public string PlayerId { get; set; }

		[BsonIgnoreIfNull]
		public Race? Race { get; set; }

		[BsonIgnoreIfNull]
		public List<GameLog> SubLogs { get; set; }

		[BsonIgnore]
		public bool IsSystem => PlayerId == null;

		public virtual GameLog Clone()
		{
			return ReflectionUtils.Clone(this);
		}
	}
}