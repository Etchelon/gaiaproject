using System.Collections.Generic;
using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class FormFederationActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.FormFederation;
		public List<string> SelectedBuildings { get; }
		public List<string> SelectedSatellites { get; }
		public FederationTokenType SelectedFederationToken { get; }

		public FormFederationActionViewModel(List<string> selectedBuildings, List<string> selectedSatellites, FederationTokenType selectedFederationToken)
		{
			SelectedBuildings = selectedBuildings;
			SelectedSatellites = selectedSatellites;
			SelectedFederationToken = selectedFederationToken;
		}
	}
}