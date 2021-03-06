
namespace GaiaProject.ViewModels.Players
{
	public class DeployedBuildingsViewModel
	{
		private int _mines;
		public int Mines
		{
			get => _mines;
			set
			{
				if (_mines == value) return;
				_mines = value;
			}
		}

		private int _tradingStations;
		public int TradingStations
		{
			get => _tradingStations;
			set
			{
				if (_tradingStations == value) return;
				_tradingStations = value;
			}
		}

		private int _researchLabs;
		public int ResearchLabs
		{
			get => _researchLabs;
			set
			{
				if (_researchLabs == value) return;
				_researchLabs = value;
			}
		}

		private bool _planetaryInstitute;
		public bool PlanetaryInstitute
		{
			get => _planetaryInstitute;
			set
			{
				if (_planetaryInstitute == value) return;
				_planetaryInstitute = value;
			}
		}

		private bool _academyLeft;
		public bool AcademyLeft
		{
			get => _academyLeft;
			set
			{
				if (_academyLeft == value) return;
				_academyLeft = value;
			}
		}

		private bool _academyRight;
		public bool AcademyRight
		{
			get => _academyRight;
			set
			{
				if (_academyRight == value) return;
				_academyRight = value;
			}
		}
	}
}