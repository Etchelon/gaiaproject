using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Players
{
	public class SpecialActionSpaceViewModel : IActionSpaceViewModel<Race>
	{
		public string Kind { get; set; }

		private Race _type;
		public Race Type
		{
			get => _type;
			set
			{
				if (_type == value) return;
				_type = value;
			}
		}

		private bool _isAvailable;
		public bool IsAvailable
		{
			get => _isAvailable;
			set
			{
				if (_isAvailable == value) return;
				_isAvailable = value;
			}
		}
	}
}