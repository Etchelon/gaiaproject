using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Board
{
	public class PowerActionSpaceViewModel : IActionSpaceViewModel<PowerActionType>
	{
		public string Kind => "power";

		private PowerActionType _type;
		public PowerActionType Type
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