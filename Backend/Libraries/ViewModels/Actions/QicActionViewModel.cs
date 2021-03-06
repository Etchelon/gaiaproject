using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class QicActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.Qic;
		public QicActionType Id { get; }

		public QicActionViewModel(QicActionType id)
		{
			Id = id;
		}
	}
}