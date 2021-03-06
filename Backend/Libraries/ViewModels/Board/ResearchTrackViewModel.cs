using System.Collections.Generic;
using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Board
{
	public class ResearchTrackViewModel
	{
		private ResearchTrackType _id;
		public ResearchTrackType Id
		{
			get => _id;
			set
			{
				if (_id == value) return;
				_id = value;
			}
		}

		private List<PlayerAdvancementViewModel> _players;
		public List<PlayerAdvancementViewModel> Players
		{
			get => _players;
			set
			{
				if (_players == value) return;
				_players = value;
			}
		}

		private StandardTechnologyTileStackViewModel _standardTiles;
		public StandardTechnologyTileStackViewModel StandardTiles
		{
			get => _standardTiles;
			set
			{
				if (_standardTiles == value) return;
				_standardTiles = value;
			}
		}

		private AdvancedTechnologyTileType? _advancedTileType;
		public AdvancedTechnologyTileType? AdvancedTileType
		{
			get => _advancedTileType;
			set
			{
				if (_advancedTileType == value) return;
				_advancedTileType = value;
			}
		}

		private FederationTokenType? _federation;
		public FederationTokenType? Federation
		{
			get => _federation;
			set
			{
				if (_federation == value) return;
				_federation = value;
			}
		}

		private bool _lostPlanet;
		public bool LostPlanet
		{
			get => _lostPlanet;
			set
			{
				if (_lostPlanet == value) return;
				_lostPlanet = value;
			}
		}
	}
}