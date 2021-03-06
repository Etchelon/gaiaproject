using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Players
{
	public class FederationTokenViewModel
	{
		private FederationTokenType _id;
		public FederationTokenType Id
		{
			get => _id;
			set
			{
				if (_id == value) return;
				_id = value;
			}
		}

		private bool _usedForTechOrAdvancedTile;
		public bool UsedForTechOrAdvancedTile
		{
			get => _usedForTechOrAdvancedTile;
			set
			{
				if (_usedForTechOrAdvancedTile == value) return;
				_usedForTechOrAdvancedTile = value;
			}
		}
	}
}