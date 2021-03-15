using System;
using System.Collections.Generic;
using System.Linq;
using GaiaProject.Common.Reflection;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic.Abstractions;
using GaiaProject.Engine.Logic.Entities.Effects;
using GaiaProject.Engine.Logic.Utils;
using GaiaProject.Engine.Model;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.Engine.Model.Players;

namespace GaiaProject.Engine.Logic.ActionHandlers.Rounds
{
	public class ConversionsActionHandler : ActionHandlerBase<ConversionsAction>
	{
		private const int BrainstonePowerValue = 3;

		protected override List<Effect> HandleImpl(GaiaProjectGame game, ConversionsAction action)
		{
			var effects = new List<Effect>();

			var isTaklons = Player.RaceId.Value == Race.Taklons;
			var isGleensWithoutQicAcademy = Player.RaceId.Value == Race.Gleens && !Player.State.Buildings.AcademyRight;
			var finalPlayerState = Player.State.Clone();
			var resources = finalPlayerState.Resources;
			var power = finalPlayerState.Resources.Power;
			var spentCredits = 0;
			var spentOres = 0;
			var spentPower = 0;
			var spentKnowledge = 0;
			var spentQic = 0;
			var earnedCredits = 0;
			var earnedOres = 0;
			var earnedKnowledge = 0;
			var earnedQic = 0;
			var earnedPowerTokens = 0;
			var rangeBoost = 0;
			var burnedPower = 0;
			var powerMovedToGaiaArea = 0;
			var gaiaformersMovedToGaiaArea = 0;
			var brainstoneLocation = power.Brainstone;

			int ActualPower2() => power.Bowl2 + (isTaklons && brainstoneLocation == PowerPools.BrainstoneLocation.Bowl2 ? 1 : 0);
			int ActualPower3() => power.Bowl3 + (isTaklons && brainstoneLocation == PowerPools.BrainstoneLocation.Bowl3 ? BrainstonePowerValue : 0);

			void SpendCredits(int amount)
			{
				spentCredits += amount;
				resources.Credits -= 1;
				if (resources.Credits < 0)
				{
					throw new InvalidActionException("Illegal conversion cannot decrease credits below 0.");
				}
			}
			void SpendOre()
			{
				spentOres += 1;
				resources.Ores -= 1;
				if (resources.Ores < 0)
				{
					throw new InvalidActionException("Illegal conversion cannot decrease ores below 0.");
				}
			}
			void SpendKnowledge()
			{
				spentKnowledge += 1;
				resources.Knowledge -= 1;
				if (resources.Knowledge < 0)
				{
					throw new InvalidActionException("Illegal conversion cannot decrease knowledge below 0.");
				}
			}
			void SpendQic()
			{
				spentQic += 1;
				resources.Qic -= 1;
				if (resources.Qic < 0)
				{
					throw new InvalidActionException("Illegal conversion cannot decrease Qic below 0.");
				}
			}
			void SpendPower(int amount, bool moveToGaiaArea = false)
			{
				var remainingPower = ActualPower3() - amount;
				if (remainingPower < 0)
				{
					throw new InvalidActionException("Illegal conversion cannot decrease power in bowl 3 below 0.");
				}

				if (moveToGaiaArea)
				{
					powerMovedToGaiaArea += amount;
				}
				else
				{
					if (isTaklons && brainstoneLocation == PowerPools.BrainstoneLocation.Bowl3 && amount >= BrainstonePowerValue)
					{
						var excess = amount - BrainstonePowerValue;
						power.Bowl3 -= excess;
						power.Bowl1 += excess;
						brainstoneLocation = PowerPools.BrainstoneLocation.Bowl1;
					}
					else
					{
						power.Bowl3 -= amount;
						power.Bowl1 += amount;
					}
					spentPower += amount;
				}
			}

			void EarnCredit()
			{
				earnedCredits += 1;
				resources.Credits += 1;
				if (resources.Credits > 30)
				{
					resources.Credits = 30;
				}
			}
			void EarnOre()
			{
				earnedOres += 1;
				resources.Ores += 1;
				if (resources.Ores > 15)
				{
					resources.Ores = 15;
				}
			}
			void EarnKnowledge()
			{
				earnedKnowledge += 1;
				resources.Knowledge += 1;
				if (resources.Knowledge > 15)
				{
					resources.Knowledge = 15;
				}
			}
			void EarnQic()
			{
				earnedQic += 1;
				resources.Qic += 1;
			}

			foreach (var conversion in action.Conversions)
			{
				switch (conversion)
				{
					case Conversion.BoostRange:
						SpendQic();
						rangeBoost += 2;
						break;
					case Conversion.BurnPower:
						if (ActualPower2() < 2)
						{
							throw new InvalidActionException("You cannot burn power when you have less than 2 tokens in bowl 2");
						}

						burnedPower += 1;
						if (isTaklons && brainstoneLocation == PowerPools.BrainstoneLocation.Bowl2)
						{
							brainstoneLocation = PowerPools.BrainstoneLocation.Bowl3;
						}
						else
						{
							power.Bowl3 += 1;
						}
						break;
					case Conversion.PowerToQic:
						if (isGleensWithoutQicAcademy)
						{
							throw new InvalidActionException("You cannot obtain QIC by conversion if you haven't build the corresponding academy");
						}

						SpendPower(4);
						EarnQic();
						break;
					case Conversion.PowerToOre:
						SpendPower(3);
						EarnOre();
						break;
					case Conversion.PowerToKnowledge:
						SpendPower(4);
						EarnKnowledge();
						break;
					case Conversion.PowerToCredit:
						SpendPower(1);
						EarnCredit();
						break;
					case Conversion.OreToCredit:
						SpendOre();
						EarnCredit();
						break;
					case Conversion.OreToPowerToken:
						SpendOre();
						if (isTaklons && brainstoneLocation == PowerPools.BrainstoneLocation.Removed)
						{
							brainstoneLocation = PowerPools.BrainstoneLocation.Bowl1;
						}
						earnedPowerTokens += 1;
						break;
					case Conversion.KnowledgeToCredit:
						SpendKnowledge();
						EarnCredit();
						break;
					case Conversion.QicToOre:
						SpendQic();
						EarnOre();
						break;
					case Conversion.NevlasPower3ToKnowledge:
						SpendPower(1, true);
						EarnKnowledge();
						break;
					case Conversion.Nevlas3PowerTo2Ores:
						SpendPower(3);
						EarnOre();
						EarnOre();
						break;
					case Conversion.Nevlas2PowerToQic:
						SpendPower(2);
						EarnQic();
						break;
					case Conversion.Nevlas2PowerToKnowledge:
						SpendPower(2);
						EarnKnowledge();
						break;
					case Conversion.Nevlas2PowerToOreAndCredit:
						SpendPower(2);
						EarnOre();
						EarnCredit();
						break;
					case Conversion.NevlasPowerTo2Credits:
						SpendPower(1);
						EarnCredit();
						EarnCredit();
						break;
					case Conversion.HadschHallas4CreditsToQic:
						SpendCredits(4);
						EarnQic();
						break;
					case Conversion.HadschHallas3CreditsToOre:
						SpendCredits(3);
						EarnOre();
						break;
					case Conversion.HadschHallas4CreditsToKnowledge:
						SpendCredits(4);
						EarnKnowledge();
						break;
					case Conversion.TaklonsBrainstoneToCredits:
						SpendPower(3);
						EarnCredit();
						EarnCredit();
						EarnCredit();
						break;
					case Conversion.BalTaksGaiaformerToQic:
						gaiaformersMovedToGaiaArea += 1;
						EarnQic();
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			var netCreditsVariation = earnedCredits - spentCredits;
			var netOresVariation = earnedOres - spentOres;
			var netKnowledgeVariation = earnedKnowledge - spentKnowledge;
			var netQicVariation = earnedQic - spentQic;
			var effect = new ConversionEffect(netCreditsVariation, netOresVariation, netKnowledgeVariation, netQicVariation,
				burnedPower, spentPower, brainstoneLocation, earnedPowerTokens, powerMovedToGaiaArea, gaiaformersMovedToGaiaArea, rangeBoost);
			effects.Add(effect);

			var passTurn = Player.Actions.PendingDecision?.Type == PendingDecisionType.PerformConversionOrPassTurn;
			if (passTurn)
			{
				var nextPlayer = TurnOrderUtils.GetNextPlayer(Player.Id, game, true);
				effects.Add(new PassTurnToPlayerEffect(nextPlayer));
			}
			return effects;
		}

		protected override (bool isValid, string errorMessage) Validate(GaiaProjectGame game, ConversionsAction action)
		{
			var playersRace = Player.RaceId!.Value;
			var illegalConversions = action.Conversions
				.Where(c => c.HasAttributeOfType<RaceActionAttribute>()
					&& c.GetAttributeOfType<RaceActionAttribute>().Race != playersRace)
				.ToArray();
			if (illegalConversions.Any())
			{
				return (false, $"You cannot perform the following conversions: {string.Join(", ", illegalConversions)} because they are reserved for another race");
			}

			return (true, null);
		}
	}
}
