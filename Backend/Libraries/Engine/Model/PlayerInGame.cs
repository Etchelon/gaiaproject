using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model.Players;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model
{
	[BsonNoId]
	public class PlayerInGame
	{
		public string Id { get; set; }
		public string Username { get; set; }
		public int InitialOrder { get; set; }
		public Race? RaceId { get; set; }

		[BsonIgnoreIfNull]
		public bool? IsWinner { get; set; }

		[BsonIgnoreIfNull]
		public int? Placement { get; set; }

		[BsonIgnoreIfNull]
		public ActionState Actions { get; set; } = new ActionState();

		[BsonIgnoreIfNull]
		public PlayerState State { get; set; }

		[BsonIgnore]
		public bool HasPassed => Actions?.HasPassed ?? true;

		[BsonIgnore]
		public int TurnOrder => State?.CurrentRoundTurnOrder ?? InitialOrder;

		public void ClearTempEffects()
		{
			if (State == null)
			{
				return;
			}
			State.TempTerraformationSteps = null;
			State.RangeBoost = null;
		}

		public PlayerInGame Clone()
		{
			return new PlayerInGame
			{
				Id = Id,
				Username = Username,
				InitialOrder = InitialOrder,
				RaceId = RaceId,
				IsWinner = IsWinner,
				Actions = Actions?.Clone(),
				State = State?.Clone()
			};
		}
	}
}