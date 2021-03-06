using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Rounds
{
	[BsonNoId]
	public class RoundsPhase
	{
		public RoundSubPhase SubPhase { get; set; } = RoundSubPhase.Income;
		public int CurrentRound { get; set; } = 1;

		public RoundsPhase Clone()
		{
			return new RoundsPhase
			{
				SubPhase = SubPhase,
				CurrentRound = CurrentRound
			};
		}
	}
}