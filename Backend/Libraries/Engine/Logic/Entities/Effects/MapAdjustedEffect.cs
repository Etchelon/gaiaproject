using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Board;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class MapAdjustedEffect : Effect
	{
		public Map NewMapState { get; }

		public MapAdjustedEffect(Map newMapState)
		{
			NewMapState = newMapState;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			game.BoardState.Map = NewMapState;
			var player = game.GetPlayer(PlayerId);
			game.LogEffect(this, $"{player.Username} rotates sectors to adjust the map");
		}
	}
}
