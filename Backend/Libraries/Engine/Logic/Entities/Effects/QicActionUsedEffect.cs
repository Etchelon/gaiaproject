using System.Linq;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Model;

namespace GaiaProject.Engine.Logic.Entities.Effects
{
	public class QicActionUsedEffect : Effect
	{
		public QicActionType Id { get; }

		public QicActionUsedEffect(QicActionType id)
		{
			Id = id;
		}

		public override void ApplyTo(GaiaProjectGame game)
		{
			var action = game.BoardState.ResearchBoard.QicActions.Single(pa => pa.Type == Id);
			action.IsAvailable = false;
		}
	}
}
