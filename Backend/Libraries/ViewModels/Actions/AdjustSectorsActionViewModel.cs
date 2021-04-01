using System.Collections.Generic;
using GaiaProject.Engine.Enums;

namespace GaiaProject.ViewModels.Actions
{
	public class AdjustSectorsActionViewModel : ActionViewModel
	{
		public override ActionType Type => ActionType.AdjustSectors;
		public List<SectorAdjustmentViewModel> Adjustments { get; }

		public AdjustSectorsActionViewModel(List<SectorAdjustmentViewModel> adjustments)
		{
			Adjustments = adjustments;
		}

		public class SectorAdjustmentViewModel
		{
			public string SectorId { get; set; }
			public int Rotation { get; set; }
		}
	}
}
