using System;
using GaiaProject.Engine.Model.Players;

namespace GaiaProject.ViewModels.Players
{
	public class PowerPoolsViewModel
	{
		private int _bowl1;
		public int Bowl1
		{
			get => _bowl1;
			set
			{
				if (_bowl1 == value) return;
				_bowl1 = value;
			}
		}

		private int _bowl2;
		public int Bowl2
		{
			get => _bowl2;
			set
			{
				if (_bowl2 == value) return;
				_bowl2 = value;
			}
		}

		private int _bowl3;
		public int Bowl3
		{
			get => _bowl3;
			set
			{
				if (_bowl3 == value) return;
				_bowl3 = value;
			}
		}

		private int _gaiaArea;
		public int GaiaArea
		{
			get => _gaiaArea;
			set
			{
				if (_gaiaArea == value) return;
				_gaiaArea = value;
			}
		}

		private PowerPools.BrainstoneLocation? _brainstone;
		public PowerPools.BrainstoneLocation? Brainstone
		{
			get => _brainstone;
			set
			{
				if (_brainstone == value) return;
				_brainstone = value;
			}
		}

		public string BrainstoneSummary
		{
			get
			{
				if (!_brainstone.HasValue)
				{
					return null;
				}
				return _brainstone.Value switch
				{
					PowerPools.BrainstoneLocation.Removed => "Removed",
					PowerPools.BrainstoneLocation.GaiaArea => "Gaia Area",
					PowerPools.BrainstoneLocation.Bowl1 => "Bowl 1",
					PowerPools.BrainstoneLocation.Bowl2 => "Bowl 2",
					PowerPools.BrainstoneLocation.Bowl3 => "Bowl 3",
					_ => throw new ArgumentOutOfRangeException("Brainstone location")
				};
			}
		}
	}
}