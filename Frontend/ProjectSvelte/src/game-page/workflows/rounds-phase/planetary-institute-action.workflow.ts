import { find, first, isNil, reject } from "lodash";
import { ActionType, Race } from "../../../dto/enums";
import type { ActionDto, InteractionStateDto } from "../../../dto/interfaces";
import type { Identifier, Nullable } from "../../../utils/miscellanea";
import { ActionWorkflow } from "../action-workflow.base";
import { InteractiveElementState, InteractiveElementType } from "../enums";
import { ActiveView, Command, CommonCommands, CommonWorkflowStates } from "../types";

const WaitingForSelection = 0;
const WaitingForConfirmation = 1;

type GenericPlanetaryActionType =
	| ActionType.AmbasSwapPlanetaryInstituteAndMine
	| ActionType.FiraksDowngradeResearchLab
	| ActionType.IvitsPlaceSpaceStation;
interface GenericPlanetaryInstituteActionDto extends ActionDto {
	Type: GenericPlanetaryActionType;
	HexId: string;
}

const piActionTypes = new Map<Race, GenericPlanetaryActionType>([
	[Race.Ambas, ActionType.AmbasSwapPlanetaryInstituteAndMine],
	[Race.Firaks, ActionType.FiraksDowngradeResearchLab],
	[Race.Ivits, ActionType.IvitsPlaceSpaceStation],
]);

const getPlanetaryInstituteAction = (race: Race, hexId: string): GenericPlanetaryInstituteActionDto => {
	const type = piActionTypes.get(race)!;
	return {
		Type: type,
		HexId: hexId,
	};
};

export class PlanetaryInstituteActionWorkflow extends ActionWorkflow {
	private _selectedHexId: Nullable<string> = null;

	constructor(
		interaction: InteractionStateDto,
		private readonly _race: Race,
		private readonly _selectionMessage: string,
		private readonly _confirmationMessage: string
	) {
		super(interaction);
		this.init();
	}

	protected init(): void {
		this.states = [
			{
				id: WaitingForSelection,
				message: this._selectionMessage,
				view: ActiveView.Map,
				commands: [CommonCommands.Abort],
			},
			{
				id: WaitingForConfirmation,
				message: this._confirmationMessage,
				commands: [CommonCommands.Cancel, CommonCommands.Confirm],
			},
		];
		this.currentState = first(this.states)!;
	}

	elementSelected(id: Identifier, type: InteractiveElementType): void {
		if (this.stateId !== WaitingForSelection) {
			return;
		}
		if (type !== InteractiveElementType.Hex) {
			return;
		}

		this._selectedHexId = id as string;
		let message: Nullable<string> = null;
		const hexDto = find(this.interactionState?.clickableHexes, h => h.id === id)!;
		if (hexDto.requiredQics) {
			message = `Spend ${hexDto.requiredQics} QICs to boost range and ${this._confirmationMessage}`;
		}

		this.advanceState(null, message);
	}

	handleCommand(command: Command): ActionDto | null {
		if (command.nextState === CommonWorkflowStates.ABORT) {
			this.cancelAction();
			return null;
		}
		if (this.stateId !== WaitingForConfirmation) {
			return null;
		}

		switch (command.nextState) {
			case CommonWorkflowStates.RESET:
			case CommonWorkflowStates.CANCEL:
				this.advanceState(WaitingForSelection);
				return null;
			case CommonWorkflowStates.PERFORM_ACTION:
				return getPlanetaryInstituteAction(this._race, this._selectedHexId!);
			default:
				throw new Error(`State ${command.nextState} not handled.`);
		}
	}

	protected getInteractiveElements() {
		const elements = super.getInteractiveElements();
		if (!isNil(this._selectedHexId)) {
			return [
				...reject(elements, el => el.type === InteractiveElementType.Hex && el.id === this._selectedHexId),
				{ id: this._selectedHexId, type: InteractiveElementType.Hex, state: InteractiveElementState.Selected },
			];
		}
		return elements;
	}

	static forRace(race: Race, interaction: InteractionStateDto): PlanetaryInstituteActionWorkflow {
		switch (race) {
			case Race.Ambas:
				return new PlanetaryInstituteActionWorkflow(
					interaction,
					race,
					"Select one of your mines to swap with the Planetary Institute",
					"Swap the Planetary institute with the selected mine?"
				);
			case Race.Firaks:
				return new PlanetaryInstituteActionWorkflow(
					interaction,
					race,
					"Select a Research Lab to downgrade to Trading Station",
					"Downgrade the selected Research Lab?"
				);
			case Race.Ivits:
				return new PlanetaryInstituteActionWorkflow(
					interaction,
					race,
					"Select a Space hex where to place your space station",
					"Place the space station in the selected hex?"
				);
			default:
				throw new Error("This race doesn't have a Planetary Institute action");
		}
	}
}
