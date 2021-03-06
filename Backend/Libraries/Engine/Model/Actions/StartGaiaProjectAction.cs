using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(StartGaiaProjectAction))]
	public class StartGaiaProjectAction : PlayerAction
	{
		public override ActionType Type => ActionType.StartGaiaProject;
		public string HexId { get; set; }

		public override string ToString()
		{
			return $"starts a Gaia project on hex {HexId}";
		}
	}
}