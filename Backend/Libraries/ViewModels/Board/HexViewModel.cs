using System.Collections.Generic;
using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Board
{
	public class HexViewModel
	{
		private string _id;
		public string Id
		{
			get => _id;
			set
			{
				if (_id == value) return;
				_id = value;
			}
		}

		private int _index;
		public int Index
		{
			get => _index;
			set
			{
				if (_index == value) return;
				_index = value;
			}
		}

		private int _row;
		public int Row
		{
			get => _row;
			set
			{
				if (_row == value) return;
				_row = value;
			}
		}

		private int _column;
		public int Column
		{
			get => _column;
			set
			{
				if (_column == value) return;
				_column = value;
			}
		}

		private PlanetType? _planetType;
		public PlanetType? PlanetType
		{
			get => _planetType;
			set
			{
				if (_planetType == value) return;
				_planetType = value;
			}
		}

		private bool _wasGaiaformed;
		public bool WasGaiaformed
		{
			get => _wasGaiaformed;
			set
			{
				if (_wasGaiaformed == value) return;
				_wasGaiaformed = value;
			}
		}

		private BuildingViewModel _building;
		public BuildingViewModel Building
		{
			get => _building;
			set
			{
				if (_building == value) return;
				_building = value;
			}
		}

		private BuildingViewModel _lantidsParasiteBuilding;
		public BuildingViewModel LantidsParasiteBuilding
		{
			get => _lantidsParasiteBuilding;
			set
			{
				if (_lantidsParasiteBuilding == value) return;
				_lantidsParasiteBuilding = value;
			}
		}

		private BuildingViewModel _ivitsSpaceStation;
		public BuildingViewModel IvitsSpaceStation
		{
			get => _ivitsSpaceStation;
			set
			{
				if (_ivitsSpaceStation == value) return;
				_ivitsSpaceStation = value;
			}
		}

		private List<BuildingViewModel> _satellites;
		public List<BuildingViewModel> Satellites
		{
			get => _satellites;
			set
			{
				if (_satellites == value) return;
				_satellites = value;
			}
		}
	}
}