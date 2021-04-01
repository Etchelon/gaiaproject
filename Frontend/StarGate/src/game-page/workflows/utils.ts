import _ from "lodash";
import { Dispatch } from "redux";
import { ActionType, GamePhase, PendingDecisionType, Race } from "../../dto/enums";
import { AvailableActionDto, GameStateDto, PendingDecisionDto } from "../../dto/interfaces";
import { localizeRoundBooster } from "../../utils/localization";
import { ActionWorkflow } from "./action-workflow.base";
import { AcceptOrDeclineLastStepDecisionDto, AcceptOrDeclineLastStepWorkflow } from "./rounds-phase/accept-last-step.workflow";
import { ChargePowerDecisionDto, ChargePowerWorkflow } from "./rounds-phase/charge-power.workflow";
import { ChooseTechnologyTileWorkflow } from "./rounds-phase/choose-technology-tile.workflow";
import { ColonizePlanetWorkflow } from "./rounds-phase/colonize-planet.workflow";
import { ConversionsWorkflow } from "./rounds-phase/conversions.workflow";
import { FormFederationWorkflow } from "./rounds-phase/form-federation.workflow";
import { GenericActionWorkflow } from "./rounds-phase/generic-action.workflow";
import { PassWorkflow } from "./rounds-phase/pass.workflow";
import { PerformConversionsOrPassTurnWorkflow } from "./rounds-phase/perform-conversions-or-pass.workflow";
import { PlaceLostPlanetWorkflow } from "./rounds-phase/place-lost-planet.workflow";
import { PlanetaryInstituteActionWorkflow } from "./rounds-phase/planetary-institute-action.workflow";
import { PowerOrQicActionWorkflow } from "./rounds-phase/power-or-qic-action.workflow";
import { RescoreFederationTokenWorkflow } from "./rounds-phase/rescore-federation-token.workflow";
import { ResearchTechnologyWorkflow } from "./rounds-phase/research-technology.workflow";
import { SortIncomesDecisionDto, SortIncomesWorkflow } from "./rounds-phase/sort-incomes.workflow";
import { StartGaiaProjectWorkflow } from "./rounds-phase/start-gaia-project.workflow";
import { TaklonsLeechDecisionDto, TaklonsLeechWorkflow } from "./rounds-phase/taklons-leech.workflow";
import { TerransDecideIncomeDecisionDto, TerransDecideIncomeWorkflow } from "./rounds-phase/terrans-decide-income.workflow";
import { UpgradeExistingStructureWorkflow } from "./rounds-phase/upgrade-existing-structure.workflow";
import { UseTechnologyTileWorkflow } from "./rounds-phase/use-technology-tile.workflow";
import { AdjustSectorsWorkflow } from "./setup-phase/adjust-sectors.workflow";
import { BidForRaceWorkflow } from "./setup-phase/bid-for-race.workflow";
import { PlaceInitialStructureWorkflow } from "./setup-phase/place-initial-structure.workflow";
import { SelectRaceWorkflow } from "./setup-phase/select-race.workflow";
import { SelectStartingRoundBoosterWorkflow } from "./setup-phase/select-starting-round-booster.workflow";

const ACTIVATABLE_ACTIONS: ActionType[] = [
	ActionType.AmbasSwapPlanetaryInstituteAndMine,
	ActionType.BescodsResearchProgress,
	ActionType.FiraksDowngradeResearchLab,
	ActionType.IvitsPlaceSpaceStation,
	ActionType.StartGaiaProject,
	ActionType.UseRightAcademy,
	ActionType.UseRoundBooster,
	ActionType.UseTechnologyTile,
];

export function fromAction(playerId: string, game: GameStateDto, action: AvailableActionDto, dispatch: Dispatch): ActionWorkflow {
	const isSetup = game.currentPhase === GamePhase.Setup;
	const workflow = isSetup ? fromSetupAction(game, action) : fromRoundsAction(playerId, game, action);
	workflow.setDispatch(dispatch);
	return workflow;
}

export function fromDecision(playerId: string, game: GameStateDto, decision: PendingDecisionDto): ActionWorkflow {
	const player = _.find(game.players, p => p.id === playerId)!;
	switch (decision.type) {
		case PendingDecisionType.ChargePower:
			return new ChargePowerWorkflow(decision as ChargePowerDecisionDto);
		case PendingDecisionType.ChooseTechnologyTile:
			return new ChooseTechnologyTileWorkflow(decision.interactionState!, player);
		case PendingDecisionType.FreeTechnologyStep:
			return new ResearchTechnologyWorkflow(decision.interactionState!, false);
		case PendingDecisionType.ItarsBurnPowerForTechnologyTile:
			return new GenericActionWorkflow(ActionType.ItarsBurnPowerForTechnologyTile, Race.Itars);
		case PendingDecisionType.PerformConversionOrPassTurn:
			return new PerformConversionsOrPassTurnWorkflow(null, false);
		case PendingDecisionType.PlaceLostPlanet:
			return new PlaceLostPlanetWorkflow(decision.interactionState!);
		case PendingDecisionType.SelectFederationTokenToScore:
			return new RescoreFederationTokenWorkflow(decision.interactionState!, false);
		case PendingDecisionType.SortIncomes:
			const sortIncomes = decision as SortIncomesDecisionDto;
			return new SortIncomesWorkflow(sortIncomes.powerIncomes, sortIncomes.powerTokenIncomes);
		case PendingDecisionType.TaklonsLeech:
			return new TaklonsLeechWorkflow(decision as TaklonsLeechDecisionDto);
		case PendingDecisionType.TerransDecideIncome:
			const terranConversions = decision as TerransDecideIncomeDecisionDto;
			return new TerransDecideIncomeWorkflow(terranConversions.power);
		case PendingDecisionType.AcceptOrDeclineLastStep:
			return new AcceptOrDeclineLastStepWorkflow(decision as AcceptOrDeclineLastStepDecisionDto);
		default:
			throw new Error(`Decision of type ${decision.type} not handled`);
	}
}

function fromSetupAction(game: GameStateDto, action: AvailableActionDto): ActionWorkflow {
	switch (action.type) {
		case ActionType.AdjustSectors:
			return new AdjustSectorsWorkflow(game.boardState.map);
		case ActionType.SelectRace:
			return new SelectRaceWorkflow(action.interactionState.availableRaces!);
		case ActionType.BidForRace:
			return new BidForRaceWorkflow(game.auctionState!);
		case ActionType.PlaceInitialStructure:
			return new PlaceInitialStructureWorkflow(action.interactionState);
		case ActionType.SelectStartingRoundBooster:
			return new SelectStartingRoundBoosterWorkflow(action.interactionState, false);
		default:
			throw new Error(`Workflow of type ${action.type} not yet implemented.`);
	}
}

function fromRoundsAction(playerId: string, game: GameStateDto, action: AvailableActionDto): ActionWorkflow {
	const player = _.find(game.players, p => p.id === playerId)!;
	switch (action.type) {
		case ActionType.ColonizePlanet:
			return new ColonizePlanetWorkflow(action.interactionState);
		case ActionType.UpgradeExistingStructure:
			return new UpgradeExistingStructureWorkflow(action.interactionState, game, player);
		case ActionType.ResearchTechnology:
			return new ResearchTechnologyWorkflow(action.interactionState, true);
		case ActionType.Pass:
			const stillHasActivatables = _.some(game.activePlayer.availableActions, aa => ACTIVATABLE_ACTIONS.includes(aa.type));
			return new PassWorkflow(action.interactionState, stillHasActivatables, game.currentRound === 6);
		case ActionType.Conversions:
			return new ConversionsWorkflow(null, false);
		case ActionType.StartGaiaProject:
			return new StartGaiaProjectWorkflow(action.interactionState);
		case ActionType.Power:
			return new PowerOrQicActionWorkflow(action.interactionState, true);
		case ActionType.Qic:
			return new PowerOrQicActionWorkflow(action.interactionState, false);
		case ActionType.UseTechnologyTile:
			return new UseTechnologyTileWorkflow(action.interactionState, false);
		case ActionType.UseRoundBooster:
			const message = `Activate the action of booster ${localizeRoundBooster(player.state.roundBooster.id)}?`;
			return new GenericActionWorkflow(action.type, player.raceId!, message);
		case ActionType.AmbasSwapPlanetaryInstituteAndMine:
		case ActionType.FiraksDowngradeResearchLab:
		case ActionType.IvitsPlaceSpaceStation:
			return PlanetaryInstituteActionWorkflow.forRace(player.raceId!, action.interactionState);
		case ActionType.UseRightAcademy:
		case ActionType.BescodsResearchProgress:
			return new GenericActionWorkflow(action.type, player.raceId!);
		case ActionType.FormFederation:
			return new FormFederationWorkflow(action, game, player);
		default:
			throw new Error(`Workflow of type ${action.type} not yet implemented.`);
	}
}
