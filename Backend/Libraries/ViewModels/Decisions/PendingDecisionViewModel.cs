using GaiaProject.Engine.Enums;
using GaiaProject.ViewModels.AvailableActions;

namespace GaiaProject.ViewModels.Decisions
{
	public abstract class PendingDecisionViewModel
	{
		public abstract PendingDecisionType Type { get; }
		public virtual string Description => "";
		public InteractionStateViewModel InteractionState { get; set; }
	}
}