using System.Collections.Generic;
using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(ConversionsAction))]
	public class ConversionsAction : PlayerAction
	{
		public override ActionType Type => ActionType.Conversions;
		public List<Conversion> Conversions { get; set; }

		public override string ToString()
		{
			return $"performs conversions";
		}
	}
}