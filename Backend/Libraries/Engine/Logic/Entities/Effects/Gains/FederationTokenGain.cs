using GaiaProject.Engine.Enums;

namespace GaiaProject.Engine.Logic.Entities.Effects.Gains
{
	public class FederationTokenGain : CompositeGain
	{
		public override GainType Type => GainType.FederationToken;
		public FederationTokenType Token { get; set; }

		public FederationTokenGain(FederationTokenType token)
		{
			Token = token;
		}

		public override string ToString()
		{
			return $"federation token {Token} which grants:\n" + base.ToString();
		}
	}
}
