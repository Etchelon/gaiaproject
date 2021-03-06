using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model.Players
{
	[BsonNoId]
	public class ResearchAdvancement
	{
		public ResearchTrackType Track { get; set; }
		public int Steps { get; set; }

		internal ResearchAdvancement Clone()
		{
			return new ResearchAdvancement
			{
				Track = Track,
				Steps = Steps
			};
		}
	}
}