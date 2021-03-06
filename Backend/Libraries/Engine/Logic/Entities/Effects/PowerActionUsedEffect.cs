using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class PowerActionUsedEffect : Effect
	{
		public PowerActionType Id { get; }

		public PowerActionUsedEffect(PowerActionType id)
		{
			Id = id;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			var action = game.BoardState.ResearchBoard.PowerActions.Single(pa => pa.Type == Id);
			action.IsAvailable = false;
		}
	}
}
