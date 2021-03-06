namespace GaiaProject.Engine.Logic.Entities.Effects.Gains
{
	public class ResourcesGain : Gain
	{
		public override GainType Type => GainType.Resources;
		public Resources Resources { get; }
		public string Source { get; }
		public int Credits => Resources?.Credits ?? 0;
		public int Ores => Resources?.Ores ?? 0;
		public int Knowledge => Resources?.Knowledge ?? 0;
		public int Qic => Resources?.Qic ?? 0;
		public int PowerTokens => Resources?.PowerTokens ?? 0;

		public ResourcesGain(Resources resources, string source = null)
		{
			Resources = resources;
			Source = source;
		}

		public override string ToString()
		{
			var ret = "";
			if (Credits > 0)
			{
				ret += $"{Credits} credits";
			}
			if (Ores > 0)
			{
				ret += $"{(string.IsNullOrEmpty(ret) ? "" : ", ")}{Ores} ores";
			}
			if (Knowledge > 0)
			{
				ret += $"{(string.IsNullOrEmpty(ret) ? "" : ", ")}{Knowledge} knowledge";
			}
			if (Qic > 0)
			{
				ret += $"{(string.IsNullOrEmpty(ret) ? "" : ", ")}{Qic} qic";
			}
			if (PowerTokens > 0)
			{
				ret += $"{(string.IsNullOrEmpty(ret) ? "" : ", ")}{PowerTokens} power tokens";
			}
			if (Source != null)
			{
				ret += $" ({Source})";
			}
			return string.IsNullOrEmpty(ret) ? null : ret;
		}
	}
}
