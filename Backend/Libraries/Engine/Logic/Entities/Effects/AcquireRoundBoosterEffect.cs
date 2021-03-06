using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class AcquireRoundBoosterEffect : Effect
	{
		public RoundBoosterType Booster { get; }
		public RoundBoosterType? PreviousBooster { get; }

		public AcquireRoundBoosterEffect(RoundBoosterType booster, RoundBoosterType? previousBooster)
		{
			Booster = booster;
			PreviousBooster = previousBooster;
		}

		/// <summary>
		/// Applies this effect to the supplied game, modifying it
		/// </summary>
		/// <param name="game">The game state to modify</param>
		public override void ApplyTo(GaiaProjectGame game)
		{
			var player = game.GetPlayer(PlayerId);
			if (PreviousBooster.HasValue)
			{
				var previous = game.BoardState.RoundBoosters.AvailableRoundBooster.Single(b => b.Id == PreviousBooster);
				previous.PlayerId = null;
			}
			var booster = game.BoardState.RoundBoosters.AvailableRoundBooster.Single(b => b.Id == Booster);
			booster.PlayerId = player.Id;
			player.State.RoundBooster = new Model.Players.RoundBooster
			{
				Id = Booster,
				Used = false
			};
		}
	}
}
