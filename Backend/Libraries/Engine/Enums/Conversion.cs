using System.ComponentModel;
using GaiaProject.Common.Reflection;
using GaiaProject.Engine.Logic;

namespace GaiaProject.Engine.Enums
{
	public enum Conversion
	{
		PowerToQic,
		PowerToOre,
		PowerToKnowledge,
		PowerToCredit,
		OreToCredit,
		OreToPowerToken,
		KnowledgeToCredit,
		QicToOre,
		BoostRange,
		BurnPower,

		[RaceAction(Race.Nevlas)]
		NevlasPower3ToKnowledge,

		[RaceAction(Race.Nevlas)]
		Nevlas3PowerTo2Ores,

		[RaceAction(Race.Nevlas)]
		Nevlas2PowerToOreAndCredit,

		[RaceAction(Race.Nevlas)]
		Nevlas2PowerToQic,

		[RaceAction(Race.Nevlas)]
		Nevlas2PowerToKnowledge,

		[RaceAction(Race.Nevlas)]
		NevlasPowerTo2Credits,

		[RaceAction(Race.BalTaks)]
		BalTaksGaiaformerToQic,

		[RaceAction(Race.HadschHallas)]
		HadschHallas4CreditsToQic,

		[RaceAction(Race.HadschHallas)]
		HadschHallas3CreditsToOre,

		[RaceAction(Race.HadschHallas)]
		HadschHallas4CreditsToKnowledge,

		[RaceAction(Race.Taklons)]
		TaklonsBrainstoneToCredits
	}

	public static partial class EnumFormatters
	{
		public static string ToDescription(this Conversion o)
		{
			return o.GetAttributeOfType<DescriptionAttribute>()?.Description ?? o.ToString();
		}
	}
}