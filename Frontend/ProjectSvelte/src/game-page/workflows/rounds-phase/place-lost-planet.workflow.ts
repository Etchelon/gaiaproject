import { find, first, isNil, reject } from "lodash";
import { ActionType } from "../../../dto/enums";
import type { ActionDto, InteractionStateDto } from "../../../dto/interfaces";
import type { Identifier, Nullable } from "../../../utils/miscellanea";
import { ActionWorkflow } from "../action-workflow.base";
import { InteractiveElementState, InteractiveElementType } from "../enums";
import { ActiveView, Command, CommonCommands, CommonWorkflowStates } from "../types";

const WaitingForHex = 0;
const WaitingForConfirmation = 1;

interface PlaceLostPlanetActionDto extends ActionDto {
	Type: ActionType.PlaceLostPlanet;
	HexId: string;
	AndPass: boolean;
}

// TODO: exact duplicate of the Colonize Planet workflow. Refactor!!
export class PlaceLostPlanetWorkflow extends ActionWorkflow {
	private _selectedHexId: Nullable<string> = null;

	constructor(interaction: InteractionStateDto) {
		super(interaction);
		this.init();
	}

	protected init(): void {
		this.states = [
			{
				id: WaitingForHex,
				message: "Select a space hex within your range",
				view: ActiveView.Map,
			},
			{
				id: WaitingForConfirmation,
				message: "Place the Lost Planet in the selected hex?",
				commands: [
					CommonCommands.Cancel,
					{
						text: "Place & Pass",
						nextState: CommonWorkflowStates.PERFORM_ACTION,
						isPrimary: false,
						data: true,
					},
					{
						text: "Place",
						nextState: CommonWorkflowStates.PERFORM_ACTION,
						isPrimary: true,
						data: false,
					},
				],
			},
		];
		this.currentState = first(this.states)!;
	}

	elementSelected(id: Identifier, type: InteractiveElementType): void {
		if (this.stateId !== WaitingForHex) {
			return;
		}
		if (type !== InteractiveElementType.Hex) {
			return;
		}
		this._selectedHexId = id as string;
		let message: Nullable<string> = null;
		const hexDto = find(this.interactionState?.clickableHexes, h => h.id === id)!;
		if (hexDto.requiredQics) {
			message = `Spend ${hexDto.requiredQics} QICs to boost range and place the Lost Planet in the selected hex?`;
		}

		this.advanceState(null, message);
	}

	handleCommand(command: Command): ActionDto | null {
		if (this.stateId !== WaitingForConfirmation) {
			return null;
		}

		switch (command.nextState) {
			case CommonWorkflowStates.RESET:
			case CommonWorkflowStates.CANCEL:
				this._selectedHexId = null;
				this.advanceState(WaitingForHex);
				return null;
			case CommonWorkflowStates.PERFORM_ACTION:
				const action: PlaceLostPlanetActionDto = {
					Type: ActionType.PlaceLostPlanet,
					HexId: this._selectedHexId!,
					AndPass: command.data as boolean,
				};
				return action;
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
}
