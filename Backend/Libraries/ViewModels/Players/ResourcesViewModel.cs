﻿
namespace GaiaProject.ViewModels.Players
{
	public class ResourcesViewModel
	{
		private int _credits;
		public int Credits
		{
			get => _credits;
			set
			{
				if (_credits == value) return;
				_credits = value;
			}
		}

		private int _ores;
		public int Ores
		{
			get => _ores;
			set
			{
				if (_ores == value) return;
				_ores = value;
			}
		}

		private int _knowledge;
		public int Knowledge
		{
			get => _knowledge;
			set
			{
				if (_knowledge == value) return;
				_knowledge = value;
			}
		}

		private int _qic;
		public int Qic
		{
			get => _qic;
			set
			{
				if (_qic == value) return;
				_qic = value;
			}
		}

		private PowerPoolsViewModel _power;
		public PowerPoolsViewModel Power
		{
			get => _power;
			set
			{
				if (_power == value) return;
				_power = value;
			}
		}
	}
}