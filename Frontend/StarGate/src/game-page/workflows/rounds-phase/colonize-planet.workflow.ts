import _ from "lodash";
import { ActionType } from "../../../dto/enums";
import { ActionDto, InteractionStateDto } from "../../../dto/interfaces";
import { Identifier, Nullable } from "../../../utils/miscellanea";
import { ActionWorkflow } from "../action-workflow.base";
import { InteractiveElementState, InteractiveElementType } from "../enums";
import { ActiveView, Command, CommonCommands, CommonWorkflowStates } from "../types";

const WaitingForHex = 0;
const WaitingForConfirmation = 1;

interface ColonizePlanetActionDto extends ActionDto {
	Type: ActionType.ColonizePlanet;
	HexId: string;
	AndPass: boolean;
}

export class ColonizePlanetWorkflow extends ActionWorkflow {
	private _selectedHexId: Nullable<string> = null;

	constructor(interaction: InteractionStateDto) {
		super(interaction);
		this.init();
	}

	protected init(): void {
		this.states = [
			{
				id: WaitingForHex,
				message: "Select a planet to colonize",
				commands: [CommonCommands.Abort],
				view: ActiveView.Map,
			},
			{
				id: WaitingForConfirmation,
				message: "Colonize the selected hex?",
				commands: [
					CommonCommands.Cancel,
					{
						text: "Build & Pass",
						nextState: CommonWorkflowStates.PERFORM_ACTION,
						isPrimary: false,
						data: true,
					},
					{
						text: "Build",
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
		this._selectedHexId = id as string;
		let message: Nullable<string> = null;
		const hexDto = _.find(this.interactionState?.clickableHexes, h => h.id === id)!;
		if (hexDto.requiredQics) {
			message = `Spend ${hexDto.requiredQics} QICs to boost range and colonize the selected hex?`;
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
				this._selectedHexId = null;
				this.advanceState(WaitingForHex);
				return null;
			case CommonWorkflowStates.PERFORM_ACTION:
				const action: ColonizePlanetActionDto = {
					Type: ActionType.ColonizePlanet,
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
		if (!_.isNil(this._selectedHexId)) {
			return [
				..._.reject(elements, el => el.type === InteractiveElementType.Hex && el.id === this._selectedHexId),
				{ id: this._selectedHexId, type: InteractiveElementType.Hex, state: InteractiveElementState.Selected },
			];
		}
		return elements;
	}
}
