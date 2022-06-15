import { chain, cloneDeep, first, isEmpty, isNil, reject } from "lodash";
import { ActionType, FederationTokenType, Race } from "../../../dto/enums";
import type {
	ActionDto,
	AvailableActionDto,
	GameStateDto,
	InteractionStateDto,
	IvitsExpandFederationInfoDto,
	PlayerInGameDto,
} from "../../../dto/interfaces";
import { getHex, Identifier } from "../../../utils/miscellanea";
import { ActionWorkflow } from "../action-workflow.base";
import { InteractiveElementState, InteractiveElementType } from "../enums";
import { ActiveView, Command, CommonCommands, CommonWorkflowStates } from "../types";

const WaitingForBuilding = 0;
const WaitingForSatellite = 1;
const WaitingForFederationToken = 2;
const WaitingForConfirmation = 3;

interface FormFederationActionDto extends ActionDto {
	Type: ActionType.FormFederation;
	SelectedBuildings: string[];
	SelectedSatellites: string[];
	SelectedFederationToken: FederationTokenType;
}

export class FormFederationWorkflow extends ActionWorkflow {
	private readonly _initialInteractionState: InteractionStateDto;
	private readonly _requiredPower: number = 7;
	private _totalBuildingsPower = 0;
	private _selectedBuildings: string[] = [];
	private _selectedSatellites: string[] = [];
	private _selectedFederationToken: FederationTokenType | null = null;

	constructor(action: AvailableActionDto, private readonly _game: GameStateDto, private readonly _player: PlayerInGameDto) {
		super(action.interactionState);
		this._initialInteractionState = cloneDeep(this.interactionState!);
		if (_player.raceId === Race.Xenos && _player.state.buildings.planetaryInstitute) {
			this._requiredPower = 6;
		}
		this.init();

		// Check if it's Ivits expanding, and if they don't need to attach other buildings they canjust skip to selecting the token
		if (_player.raceId === Race.Ivits && !isEmpty(action.additionalData)) {
			const ivitsInfo = JSON.parse(action.additionalData) as IvitsExpandFederationInfoDto;
			this._requiredPower = ivitsInfo.CanTakeMoreTokens ? 0 : ivitsInfo.RequiredPower;
			if (this._requiredPower === 0) {
				this.gotoTokenSelection();
			}
		}
	}

	private readonly getSelectBuildingMessage = () =>
		`Select a building to include in the federation (current power = ${this._totalBuildingsPower})`;

	protected init(): void {
		this.states = [
			{
				id: WaitingForBuilding,
				message: this.getSelectBuildingMessage(),
				view: ActiveView.Map,
				commands: [CommonCommands.Abort],
			},
			{
				id: WaitingForSatellite,
				message: "Select a space hex where to place a satellite",
				commands: [{ nextState: WaitingForFederationToken, text: "Done" }],
			},
			{
				id: WaitingForFederationToken,
				message: "Select an available Federation token",
				view: ActiveView.ScoringBoard,
			},
			{
				id: WaitingForConfirmation,
				message: "Proceed with forming the Federation you selected?",
				commands: [CommonCommands.Reset, CommonCommands.Cancel, CommonCommands.Confirm],
			},
		];
		this.currentState = first(this.states)!;
	}

	elementSelected(id: Identifier, type: InteractiveElementType): void {
		switch (this.stateId) {
			case WaitingForBuilding:
				this.onBuildingSelected(id, type);
				return;
			case WaitingForSatellite:
				this.onSatelliteSelected(id, type);
				return;
			case WaitingForFederationToken:
				this.onFederationTokenSelected(id, type);
				return;
			default:
				return;
		}
	}

	private onBuildingSelected(id: Identifier, type: InteractiveElementType): void {
		if (this.stateId !== WaitingForBuilding) {
			return;
		}
		if (type !== InteractiveElementType.Hex) {
			return;
		}

		const hexId = id as string;
		const hex = getHex(hexId, this._game);
		this._selectedBuildings.push(hexId);
		const buildingsPowerValue =
			this._player.raceId === Race.Ivits && hex.ivitsSpaceStation !== null
				? 1
				: (this._player.raceId === Race.Lantids ? hex.lantidsParasiteBuilding ?? hex.building : hex.building)
						.powerValueInFederation;
		this._totalBuildingsPower += buildingsPowerValue;
		if (this._totalBuildingsPower < this._requiredPower) {
			const newState = cloneDeep(this.currentState);
			newState.message = this.getSelectBuildingMessage();
			this.updateState(newState);
			return;
		}

		const spaceHexes = chain(this._game.boardState.map.sectors)
			.flatMap(s => s.hexes)
			.filter(h => isNil(h.planetType))
			.value();
		this.interactionState = {
			clickableHexes: spaceHexes.map(h => ({ id: h.id, requiredQics: null })),
		};
		this.advanceState(WaitingForSatellite);
	}

	private onSatelliteSelected(id: Identifier, type: InteractiveElementType): void {
		if (this.stateId !== WaitingForSatellite) {
			return;
		}
		if (type !== InteractiveElementType.Hex) {
			return;
		}

		this._selectedSatellites.push(id as string);
		this.notifyInteractionStateChanged();
	}

	private onFederationTokenSelected(id: Identifier, type: InteractiveElementType): void {
		if (this.stateId !== WaitingForFederationToken) {
			return;
		}
		if (type !== InteractiveElementType.FederationToken) {
			return;
		}

		this._selectedFederationToken = id as FederationTokenType;
		this.advanceState(WaitingForConfirmation);
	}

	private gotoTokenSelection(): void {
		this._selectedFederationToken = null;
		this.interactionState = {
			clickableFederations: chain(this._game.boardState.availableFederations)
				.filter(fed => fed.remaining > 0)
				.map(fed => fed.type)
				.value(),
		};
		this.advanceState(WaitingForFederationToken);
	}

	handleCommand(command: Command): ActionDto | null {
		if (command.nextState === CommonWorkflowStates.ABORT) {
			this.cancelAction();
			return null;
		}

		if (this.stateId === WaitingForSatellite && command.nextState === WaitingForFederationToken) {
			this.gotoTokenSelection();
			return null;
		}

		if (this.stateId !== WaitingForConfirmation) {
			return null;
		}

		switch (command.nextState) {
			case CommonWorkflowStates.RESET:
				this._totalBuildingsPower = 0;
				this._selectedBuildings = [];
				this._selectedSatellites = [];
				this._selectedFederationToken = null;
				this.interactionState = cloneDeep(this._initialInteractionState);
				this.advanceState(WaitingForBuilding, this.getSelectBuildingMessage());
				return null;
			case CommonWorkflowStates.CANCEL:
				this._selectedFederationToken = null;
				this.advanceState(WaitingForFederationToken);
				return null;
			case CommonWorkflowStates.PERFORM_ACTION:
				const action: FormFederationActionDto = {
					Type: ActionType.FormFederation,
					SelectedBuildings: this._selectedBuildings,
					SelectedSatellites: this._selectedSatellites,
					SelectedFederationToken: this._selectedFederationToken!,
				};
				return action;
			default:
				throw new Error(`State ${command.nextState} not handled.`);
		}
	}

	protected getInteractiveElements() {
		const isSelected = (hexId: string) => this._selectedBuildings.includes(hexId) || this._selectedSatellites.includes(hexId);

		let elements = [
			...reject(super.getInteractiveElements(), el => el.type === InteractiveElementType.Hex && isSelected(el.id! as string)),
			...this._selectedBuildings.map(hexId => ({
				id: hexId,
				type: InteractiveElementType.Hex,
				state: InteractiveElementState.Selected,
			})),
			...this._selectedSatellites.map(hexId => ({
				id: hexId,
				type: InteractiveElementType.Hex,
				state: InteractiveElementState.Selected,
			})),
		];
		if (!isNil(this._selectedFederationToken)) {
			elements = [
				...reject(elements, el => el.type === InteractiveElementType.FederationToken && el.id === this._selectedFederationToken),
				{
					id: this._selectedFederationToken,
					type: InteractiveElementType.FederationToken,
					state: InteractiveElementState.Selected,
				},
			];
		}
		return elements;
	}
}
