
namespace GaiaProject.ViewModels.Players
{
	public class ResearchAdvancementsViewModel
	{
		private int _terraformation;
		public int Terraformation
		{
			get => _terraformation;
			set
			{
				if (_terraformation == value) return;
				_terraformation = value;
			}
		}

		private int _navigation;
		public int Navigation
		{
			get => _navigation;
			set
			{
				if (_navigation == value) return;
				_navigation = value;
			}
		}

		private int _artificialIntelligence;
		public int ArtificialIntelligence
		{
			get => _artificialIntelligence;
			set
			{
				if (_artificialIntelligence == value) return;
				_artificialIntelligence = value;
			}
		}

		private int _gaiaformation;
		public int Gaiaformation
		{
			get => _gaiaformation;
			set
			{
				if (_gaiaformation == value) return;
				_gaiaformation = value;
			}
		}

		private int _economy;
		public int Economy
		{
			get => _economy;
			set
			{
				if (_economy == value) return;
				_economy = value;
			}
		}

		private int _science;
		public int Science
		{
			get => _science;
			set
			{
				if (_science == value) return;
				_science = value;
			}
		}
	}
}