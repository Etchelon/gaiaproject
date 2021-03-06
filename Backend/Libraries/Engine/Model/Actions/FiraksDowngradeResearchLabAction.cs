using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(FiraksDowngradeResearchLabAction))]
	public class FiraksDowngradeResearchLabAction : PlayerAction
	{
		public override ActionType Type => ActionType.FiraksDowngradeResearchLab;
		public string HexId { get; set; }

		public override string ToString()
		{
			return $"downgrades research lab on hex {HexId}";
		}
	}
}