using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class IncomeVariationEffect : Effect
	{
		public IncomeSource Source { get; set; }

		public IncomeVariationEffect(IncomeSource source)
		{
			Source = source;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			var player = game.GetPlayer(PlayerId);
			player.State.Incomes = IncomeUtils.UpdateIncomeFrom(Source, player);

			game.LogEffect(this, "incomes have been updated");
		}
	}
}
