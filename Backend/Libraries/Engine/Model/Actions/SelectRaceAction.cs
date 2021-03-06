using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(SelectRaceAction))]
	public class SelectRaceAction : PlayerAction
	{
		public override ActionType Type => ActionType.SelectRace;
		public Race Race { get; set; }

		public override string ToString()
		{
			return $"{PlayerUsername} chose {Race.ToDescription()}";
		}
	}
}