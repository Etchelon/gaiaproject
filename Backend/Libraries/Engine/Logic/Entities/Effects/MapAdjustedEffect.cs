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
			game.LogEffect(this, $"rotates sectors to adjust the map");
		}
	}
}
