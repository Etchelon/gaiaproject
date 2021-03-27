using System;
using GaiaProject.Engine.Model.Players;

namespace GaiaProject.ViewModels.Players
{
    public class PowerPoolsViewModel
    {
        public int Bowl1 { get; set; }
        public int Bowl2 { get; set; }
        public int Bowl3 { get; set; }
        public int GaiaArea { get; set; }
        public PowerPools.BrainstoneLocation? Brainstone { get; set; }
        public string BrainstoneSummary
        {
            get
            {
                if (!Brainstone.HasValue)
                {
                    return null;
                }
                return Brainstone.Value switch
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