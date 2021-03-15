using System;
using GaiaProject.Common.Reflection;
using GaiaProject.Engine.Enums;
using GaiaProject.Engine.Logic;
using GaiaProject.Engine.Model.Actions;
using GaiaProject.ViewModels.Actions;
using Newtonsoft.Json.Linq;

namespace GaiaProject.Endpoint.WorkerServices
{
	internal static class ActionMapper
	{
		public static PlayerAction GetActualAction(JToken action)
		{
			var actionType = (ActionType)action.Value<int>(nameof(ActionViewModel.Type));
			var phase = actionType.GetAttributeOfType<AvailableInPhaseAttribute>().Phase;
			return phase == GamePhase.Setup
				? GetSetupAction(action)
				: GetRoundsAction(action);
		}

		private static PlayerAction GetSetupAction(JToken action)
		{
			var type = action.Value<int>(nameof(ActionViewModel.Type));
			var typeEnum = (ActionType)type;
			switch (typeEnum)
			{
				default:
					throw new NotImplementedException();
				case ActionType.SelectRace:
					{
						var actionVm = action.ToObject<SelectRaceActionViewModel>();
						return new SelectRaceAction
						{
							Race = actionVm.Race,
						};
					}
				case ActionType.BidForRace:
					{
						var actionVm = action.ToObject<BidForRaceActionViewModel>();
						return new BidForRaceAction
						{
							Race = actionVm.Race,
							Points = actionVm.Points,
						};
					}
				case ActionType.PlaceInitialStructure:
					{
						var actionVm = action.ToObject<PlaceInitialStructureActionViewModel>();
						return new PlaceInitialStructureAction
						{
							TargetHexId = actionVm.HexId,
						};
					}
				case ActionType.SelectStartingRoundBooster:
					{
						var actionVm = action.ToObject<SelectRoundBoosterActionViewModel>();
						return new SelectStartingRoundBoosterAction
						{
							Booster = actionVm.Booster,
						};
					}
			}
		}

		private static PlayerAction GetRoundsAction(JToken action)
		{
			var type = action.Value<int>(nameof(ActionViewModel.Type));
			var typeEnum = (ActionType)type;
			switch (typeEnum)
			{
				default:
					throw new NotImplementedException();
				case ActionType.PassTurn:
					{
						return new PassTurnAction();
					}
				case ActionType.ColonizePlanet:
					{
						var actionVm = action.ToObject<ColonizePlanetActionViewModel>();
						return new ColonizePlanetAction
						{
							TargetHexId = actionVm.HexId,
						};
					}
				case ActionType.UpgradeExistingStructure:
					{
						var actionVm = action.ToObject<UpgradeExistingStructureActionViewModel>();
						return new UpgradeExistingStructureAction
						{
							TargetHexId = actionVm.HexId,
							TargetBuildingType = actionVm.TargetBuilding
						};
					}
				case ActionType.ResearchTechnology:
					{
						var actionVm = action.ToObject<ResearchTechnologyActionViewModel>();
						return new ResearchTechnologyAction
						{
							TrackId = actionVm.Track
						};
					}
				case ActionType.ChooseTechnologyTile:
					{
						var actionVm = action.ToObject<ChooseTechnologyTileActionViewModel>();
						return new ChooseTechnologyTileAction
						{
							TileId = actionVm.TileId,
							Advanced = actionVm.Advanced,
							CoveredTileId = (StandardTechnologyTileType?)actionVm.CoveredTileId
						};
					}
				case ActionType.ChargePower:
					{
						var actionVm = action.ToObject<ChargeOrDeclinePowerActionViewModel>();
						return new ChargePowerAction
						{
							Accepted = actionVm.Accepted
						};
					}
				case ActionType.Pass:
					{
						var actionVm = action.ToObject<PassActionViewModel>();
						return new PassAction
						{
							SelectedRoundBooster = actionVm.SelectedRoundBooster,
						};
					}
				case ActionType.Conversions:
					{
						var actionVm = action.ToObject<ConversionsActionViewModel>();
						return new ConversionsAction
						{
							Conversions = actionVm.Conversions
						};
					}
				case ActionType.StartGaiaProject:
					{
						var actionVm = action.ToObject<StartGaiaProjectActionViewModel>();
						return new StartGaiaProjectAction
						{
							HexId = actionVm.HexId
						};
					}
				case ActionType.SortIncomes:
					{
						var actionVm = action.ToObject<SortIncomesActionViewModel>();
						return new SortIncomesAction
						{
							SortedIncomes = actionVm.SortedIncomes
						};
					}
				case ActionType.Power:
					{
						var actionVm = action.ToObject<PowerActionViewModel>();
						return new PowerAction
						{
							ActionId = actionVm.Id
						};
					}
				case ActionType.Qic:
					{
						var actionVm = action.ToObject<QicActionViewModel>();
						return new QicAction
						{
							ActionId = actionVm.Id
						};
					}
				case ActionType.UseTechnologyTile:
					{
						var actionVm = action.ToObject<UseTechnologyTileActionViewModel>();
						return new UseTechnologyTileAction
						{
							TileId = actionVm.TileId,
							Advanced = actionVm.Advanced
						};
					}
				case ActionType.UseRoundBooster:
					{
						return new UseRoundBoosterAction();
					}
				case ActionType.BescodsResearchProgress:
					{
						return new BescodsResearchProgressAction();
					}
				case ActionType.IvitsPlaceSpaceStation:
					{
						var actionVm = action.ToObject<IvitsPlaceSpaceStationActionViewModel>();
						return new IvitsPlaceSpaceStationAction
						{
							HexId = actionVm.HexId
						};
					}
				case ActionType.AmbasSwapPlanetaryInstituteAndMine:
					{
						var actionVm = action.ToObject<AmbasSwapPlanetaryInstituteAndMineActionViewModel>();
						return new AmbasSwapPlanetaryInstituteAndMineAction
						{
							HexId = actionVm.HexId
						};
					}
				case ActionType.FiraksDowngradeResearchLab:
					{
						var actionVm = action.ToObject<FiraksDowngradeResearchLabActionViewModel>();
						return new FiraksDowngradeResearchLabAction
						{
							HexId = actionVm.HexId
						};
					}
				case ActionType.ItarsBurnPowerForTechnologyTile:
					{
						return new ItarsBurnPowerForTechnologyTileAction();
					}
				case ActionType.TerransDecideIncome:
					{
						var actionVm = action.ToObject<TerransDecideIncomeActionViewModel>();
						return new TerransDecideIncomeAction
						{
							Credits = actionVm.Credits,
							Ores = actionVm.Ores,
							Knowledge = actionVm.Knowledge,
							Qic = actionVm.Qic
						};
					}
				case ActionType.UseRightAcademy:
					{
						return new UseRightAcademyAction();
					}
				case ActionType.FormFederation:
					{
						var actionVm = action.ToObject<FormFederationActionViewModel>();
						return new FormFederationAction
						{
							SelectedBuildings = actionVm.SelectedBuildings,
							SelectedSatellites = actionVm.SelectedSatellites,
							SelectedFederationToken = actionVm.SelectedFederationToken
						};
					}
				case ActionType.PlaceLostPlanet:
					{
						var actionVm = action.ToObject<PlaceLostPlanetActionViewModel>();
						return new PlaceLostPlanetAction
						{
							HexId = actionVm.HexId
						};
					}
				case ActionType.RescoreFederationToken:
					{
						var actionVm = action.ToObject<RescoreFederationTokenActionViewModel>();
						return new RescoreFederationTokenAction
						{
							Token = actionVm.Token
						};
					}
				case ActionType.TaklonsLeech:
					{
						var actionVm = action.ToObject<TaklonsLeechActionViewModel>();
						return new TaklonsLeechAction
						{
							Accepted = actionVm.Accepted,
							ChargeFirstThenToken = actionVm.ChargeFirstThenToken
						};
					}
				case ActionType.AcceptOrDeclineLastStep:
					{
						var actionVm = action.ToObject<AcceptOrDeclineLastStepActionViewModel>();
						return new AcceptOrDeclineLastStepAction
						{
							Accepted = actionVm.Accepted,
							Track = actionVm.Track
						};
					}
			}
		}
	}
}