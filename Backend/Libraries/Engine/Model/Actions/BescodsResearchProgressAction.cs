using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(BescodsResearchProgressAction))]
	public class BescodsResearchProgressAction : PlayerAction
	{
		public override ActionType Type => ActionType.BescodsResearchProgress;
		public override string ToString()
		{
			return "researches one of the least researched technologies";
		}
	}
}