using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Actions
{
	[BsonDiscriminator(nameof(ResearchTechnologyAction))]
	public class ResearchTechnologyAction : PlayerAction
	{
		public override ActionType Type => ActionType.ResearchTechnology;
		public ResearchTrackType TrackId { get; set; }

		public override string ToString()
		{
			return $"researches technology {TrackId.ToDescription()}";
		}
	}
}