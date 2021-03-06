namespace GaiaProject.Engine.Logic.Entities.Effects.Costs
{
	public class StartGaiaProjectCost : Cost
	{
		public override CostType Type => CostType.StartGaiaProject;
		public string HexId { get; set; }

		public StartGaiaProjectCost(string hexId)
		{
			HexId = hexId;
		}
	}
}
