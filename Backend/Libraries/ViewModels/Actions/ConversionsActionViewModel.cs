using System.Collections.Generic;
using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class ConversionsActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.Conversions;
		public List<Conversion> Conversions { get; }

		public ConversionsActionViewModel(List<Conversion> conversions)
		{
			Conversions = conversions;
		}
	}
}