namespace GaiaProject.Engine.Logic.Entities.Effects.Costs
{
	public class UseFederationTokenCost : Cost
	{
		public override CostType Type => CostType.UseFederation;

		public override string ToString()
		{
			return "a federation token, which is now used";
		}
	}
}
