import _ from "lodash";
import { ActionType, BuildingType, Race } from "../../../dto/enums";
import { ActionDto, GameStateDto, InteractionStateDto, PlayerInGameDto } from "../../../dto/interfaces";
import { localizeEnum } from "../../../utils/localization";
import { getHex, Identifier, Nullable } from "../../../utils/miscellanea";
import { ActionWorkflow } from "../action-workflow.base";
import { InteractiveElementState, InteractiveElementType } from "../enums";
import { ActiveView, Command, CommonCommands, CommonWorkflowStates } from "../types";

const WaitingForHex = 0;
const WaitingForChoice = 1;
const WaitingForConfirmation = 2;

const localizeBuilding = _.partialRight(localizeEnum, "BuildingType");

interface UpgradeExistingStructureActionDto extends ActionDto {
	Type: ActionType.UpgradeExistingStructure;
	HexId: string;
	TargetBuilding: BuildingType;
	AndPass: boolean;
}

export class UpgradeExistingStructureWorkflow extends ActionWorkflow {
	private _selectedHexId: Nullable<string> = null;
	private _sourceBuilding: BuildingType | null = null;
	private _targetBuilding: BuildingType | null = null;

	constructor(interaction: InteractionStateDto, private readonly _game: GameStateDto, private readonly _player: PlayerInGameDto) {
		super(interaction);
		this.init();
	}

	protected init(): void {
		this.states = [
			{
				id: WaitingForHex,
				message: "Select a building to upgrade",
				view: ActiveView.Map,
				commands: [CommonCommands.Abort],
			},
			{
				id: WaitingForChoice,
				message: "",
				view: ActiveView.Map,
			},
			{
				id: WaitingForConfirmation,
				message: "Upgrade the selected building?",
				commands: [
					CommonCommands.Cancel,
					{
						text: "Upgrade & Pass",
						nextState: CommonWorkflowStates.PERFORM_ACTION,
						isPrimary: false,
						data: true,
					},
					{
						text: "Upgrade",
						nextState: CommonWorkflowStates.PERFORM_ACTION,
						isPrimary: true,
						data: false,
					},
				],
			},
		];
		this.currentState = _.first(this.states)!;
	}

	elementSelected(id: Identifier, type: InteractiveElementType): void {
		if (this.stateId !== WaitingForHex) {
			return;
		}
		if (type !== InteractiveElementType.Hex) {
			return;
		}

		const hexId = id as string;
		if (!hexId) {
			throw new Error("Hex id is empty");
		}
		if (!_.some(this.interactionState?.clickableHexes, h => h.id === hexId)) {
			throw new Error("The selected hex is not clickable!");
		}

		const hex = getHex(hexId, this._game);
		this._selectedHexId = hexId;
		this._sourceBuilding = hex.building.type;
		const playerBuildings = this._player.state.buildings;
		const isBescods = this._player.raceId === Race.Bescods;
		const possibleTargets: BuildingType[] = [];
		switch (this._sourceBuilding) {
			case BuildingType.Mine:
				if (playerBuildings.tradingStations < 4) {
					possibleTargets.push(BuildingType.TradingStation);
				}
				break;
			case BuildingType.TradingStation:
				if (isBescods) {
					if (!playerBuildings.academyLeft) {
						possibleTargets.push(BuildingType.AcademyLeft);
					}
					if (!playerBuildings.academyRight) {
						possibleTargets.push(BuildingType.AcademyRight);
					}
				} else {
					if (!playerBuildings.planetaryInstitute) {
						possibleTargets.push(BuildingType.PlanetaryInstitute);
					}
				}

				if (playerBuildings.researchLabs < 3) {
					possibleTargets.push(BuildingType.ResearchLab);
				}
				break;
			case BuildingType.ResearchLab:
				if (isBescods) {
					if (!playerBuildings.planetaryInstitute) {
						possibleTargets.push(BuildingType.PlanetaryInstitute);
					}
					break;
				}

				if (!playerBuildings.academyLeft) {
					possibleTargets.push(BuildingType.AcademyLeft);
				}
				if (!playerBuildings.academyRight) {
					possibleTargets.push(BuildingType.AcademyRight);
				}
				break;
			default:
				throw new Error(`Building of type ${localizeBuilding(this._sourceBuilding)} cannot be upgraded.`);
		}

		if (_.isEmpty(possibleTargets)) {
			this._selectedHexId = null;
			_.remove(this.interactionState!.clickableHexes!, hexId);
			this.advanceState(WaitingForHex, "Choose another building, this cannot be upgraded any further.");
			return;
		}

		if (_.size(possibleTargets) === 1) {
			this._targetBuilding = _.first(possibleTargets)!;
			const sourceBuilding = localizeBuilding(this._sourceBuilding);
			const targetBuilding = localizeBuilding(this._targetBuilding);
			this.advanceState(WaitingForConfirmation, `Upgrade ${sourceBuilding} to ${targetBuilding}?`);
			return;
		}

		const message = "Upgrade to";
		const commands = _.map(possibleTargets, type_ => ({
			nextState: WaitingForConfirmation,
			text: localizeBuilding(type_),
			data: type_,
		}));
		this.advanceState(WaitingForChoice, message, commands);
	}

	advanceState(next: Nullable<number> = null, message: Nullable<string> = null, commands: Nullable<Command[]> = null): void {
		if (next !== WaitingForConfirmation || _.isNil(this._targetBuilding)) {
			super.advanceState(next, message, commands);
			return;
		}

		const commands_ = [..._.find(this.states, s => s.id === WaitingForConfirmation)!.commands!];
		if ([BuildingType.ResearchLab, BuildingType.AcademyLeft, BuildingType.AcademyRight].includes(this._targetBuilding!)) {
			_.remove(commands_, c => c.nextState === CommonWorkflowStates.PERFORM_ACTION && c.data === true);
		}
		super.advanceState(next, message, commands_);
	}

	handleCommand(command: Command): ActionDto | null {
		if (command.nextState === CommonWorkflowStates.ABORT) {
			this.cancelAction();
			return null;
		}

		switch (command.nextState) {
			case WaitingForConfirmation:
				this._targetBuilding = command.data as BuildingType;
				this.advanceState(WaitingForConfirmation, `Upgrade ${localizeBuilding(this._sourceBuilding!)} to ${localizeBuilding(this._targetBuilding)}?`);
				return null;
			case CommonWorkflowStates.RESET:
				this._selectedHexId = null;
				this._sourceBuilding = null;
				this._targetBuilding = null;
				this.advanceState(WaitingForHex);
				return null;
			case CommonWorkflowStates.CANCEL:
				this._targetBuilding = null;
				this.advanceState(WaitingForHex);
				return null;
			case CommonWorkflowStates.PERFORM_ACTION:
				const action: UpgradeExistingStructureActionDto = {
					Type: ActionType.UpgradeExistingStructure,
					HexId: this._selectedHexId!,
					TargetBuilding: this._targetBuilding!,
					AndPass: command.data as boolean,
				};
				return action;
			default:
				throw new Error(`State ${command.nextState} not handled.`);
		}
	}

	protected getInteractiveElements() {
		const elements = super.getInteractiveElements();
		if (_.isNil(this._selectedHexId)) {
			return elements;
		}

		return [
			..._.reject(elements, el => el.type === InteractiveElementType.Hex && el.id === this._selectedHexId),
			{ id: this._selectedHexId, type: InteractiveElementType.Hex, state: InteractiveElementState.Selected },
		];
	}
}
