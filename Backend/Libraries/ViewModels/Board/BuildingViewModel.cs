using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Board
{
	public class BuildingViewModel
	{
		private string _username;
		public string Username
		{
			get => _username;
			set
			{
				if (_username == value) return;
				_username = value;
			}
		}

		private Race _raceId;
		public Race RaceId
		{
			get => _raceId;
			set
			{
				if (_raceId == value) return;
				_raceId = value;
			}
		}

		private BuildingType _type;
		public BuildingType Type
		{
			get => _type;
			set
			{
				if (_type == value) return;
				_type = value;
			}
		}

		private int _powerValue;
		public int PowerValue
		{
			get => _powerValue;
			set
			{
				if (_powerValue == value) return;
				_powerValue = value;
			}
		}

		private int _powerValueInFederation;
		public int PowerValueInFederation
		{
			get => _powerValueInFederation;
			set
			{
				if (_powerValueInFederation == value) return;
				_powerValueInFederation = value;
			}
		}

		private string _federationId;
		public string FederationId
		{
			get => _federationId;
			set
			{
				if (_federationId == value) return;
				_federationId = value;
			}
		}
	}
}