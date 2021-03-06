using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Players;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class AcquireTechnologyTileEffect : Effect
	{
		public int TileId { get; }
		public bool Advanced { get; }
		public StandardTechnologyTileType? CoveredStandardTile { get; set; }

		public AcquireTechnologyTileEffect(int tileId, bool advanced, StandardTechnologyTileType? coveredStandardTile = null)
		{
			TileId = tileId;
			Advanced = advanced;
			CoveredStandardTile = coveredStandardTile;
		}

		/// <summary>
		/// Applies this effect to the supplied game, modifying it
		/// </summary>
		/// <param name="game">The game state to modify</param>
		public override void ApplyTo(GaiaProjectGame game)
		{
			string log;
			var player = game.GetPlayer(PlayerId);
			if (Advanced)
			{
				var newTile = new AdvancedTechnologyTile
				{
					Id = (AdvancedTechnologyTileType)TileId,
					CoveredTile = CoveredStandardTile!.Value,
					Used = false
				};
				player.State.AdvancedTechnologyTiles.Add(newTile);
				var coveredTile = player.State.StandardTechnologyTiles.Single(st => st.Id == CoveredStandardTile);
				coveredTile.CoveredByAdvancedTile = true;
				coveredTile.Used = false;
				var tilesTrack = game.BoardState.ResearchBoard.Tracks.Single(t => t.AdvancedTileType == newTile.Id);
				tilesTrack.IsAdvancedTileAvailable = false;
				log = $"acquires advanced tile {newTile.Id.ToDescription()}, covering standard tile {CoveredStandardTile!.Value.ToDescription()}";
			}
			else
			{
				var newTile = new StandardTechnologyTile
				{
					Id = (StandardTechnologyTileType)TileId,
					CoveredByAdvancedTile = false,
					Used = false
				};
				player.State.StandardTechnologyTiles.Add(newTile);
				var trackWithTile = game.BoardState.ResearchBoard
					.Tracks.SingleOrDefault(t => t.StandardTiles.Type == newTile.Id);
				if (trackWithTile != null)
				{
					--trackWithTile.StandardTiles.Remaining;
				}
				else
				{
					var freeTile = game.BoardState.ResearchBoard.FreeStandardTiles.First(t => t.Type == newTile.Id);
					--freeTile.Remaining;
				}
				log = $"acquires standard tile {newTile.Id.ToDescription()}";
			}
		}
	}
}