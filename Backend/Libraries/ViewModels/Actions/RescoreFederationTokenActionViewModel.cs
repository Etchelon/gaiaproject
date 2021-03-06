using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class RescoreFederationTokenActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.RescoreFederationToken;
		public FederationTokenType Token { get; }

		public RescoreFederationTokenActionViewModel(FederationTokenType token)
		{
			Token = token;
		}
	}
}