using System.Collections.Generic;

namespace GaiaProject.ViewModels.Board
{
	public class ResearchBoardViewModel
	{
		private List<ResearchTrackViewModel> _tracks;
		public List<ResearchTrackViewModel> Tracks
		{
			get => _tracks;
			set
			{
				if (_tracks == value) return;
				_tracks = value;
			}
		}

		private List<StandardTechnologyTileStackViewModel> _freeStandardTiles;
		public List<StandardTechnologyTileStackViewModel> FreeStandardTiles
		{
			get => _freeStandardTiles;
			set
			{
				if (_freeStandardTiles == value) return;
				_freeStandardTiles = value;
			}
		}

		private List<PowerActionSpaceViewModel> _powerActions;
		public List<PowerActionSpaceViewModel> PowerActions
		{
			get => _powerActions;
			set
			{
				if (_powerActions == value) return;
				_powerActions = value;
			}
		}

		private List<QicActionSpaceViewModel> _qicActions;
		public List<QicActionSpaceViewModel> QicActions
		{
			get => _qicActions;
			set
			{
				if (_qicActions == value) return;
				_qicActions = value;
			}
		}
	}
}