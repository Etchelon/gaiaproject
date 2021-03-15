using GaiaProject.Common.Reflection;
using GaiaProject.Engine.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace GaiaProject.Engine.Model
{
	[BsonNoId]
	public class GameOptions
	{
		public string GameName { get; set; }
		public MapShape MapShape { get; set; }
		public TurnOrderSelectionMode TurnOrderSelectionMode { get; set; } = TurnOrderSelectionMode.Random;
		public RaceSelectionMode FactionSelectionMode { get; set; } = RaceSelectionMode.Random;
		public bool Auction { get; set; }
		public bool PreventSandwiching { get; set; }
		public int MinPlayers { get; set; } = 2;
		public int MaxPlayers { get; set; } = 4;
		public int StartingVPs { get; set; } = 10;
		public int MinutesPerMove { get; set; } = 240;

		internal GameOptions Clone()
		{
			return ReflectionUtils.Clone(this);
		}
	}
}
