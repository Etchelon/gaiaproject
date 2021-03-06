using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;
using ScoreSheets.Common.Reflection;

namespace GaiaProject.Engine.Model.Rounds
{
	[BsonNoId]
	public class RoundsPhase
	{
		public RoundSubPhase SubPhase { get; set; } = RoundSubPhase.Income;
		public int CurrentRound { get; set; } = 1;

		public RoundsPhase Clone()
		{
			return ReflectionUtils.Clone(this);
		}
	}
}