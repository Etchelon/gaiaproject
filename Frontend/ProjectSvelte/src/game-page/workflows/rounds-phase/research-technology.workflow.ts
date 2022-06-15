import { first, isNil, partialRight, reject } from "lodash";
import { ActionType, ResearchTrackType } from "../../../dto/enums";
import type { ActionDto, InteractionStateDto } from "../../../dto/interfaces";
import { localizeEnum } from "../../../utils/localization";
import type { Identifier } from "../../../utils/miscellanea";
import { ActionWorkflow } from "../action-workflow.base";
import { InteractiveElementState, InteractiveElementType } from "../enums";
import { ActiveView, Command, CommonCommands, CommonWorkflowStates } from "../types";

const WaitingForTrack = 0;
const WaitingForConfirmation = 1;

const localizeTrack = partialRight(localizeEnum, "ResearchTrackType");

interface ResearchTechnologyActionDto extends ActionDto {
	Type: ActionType.ResearchTechnology;
	Track: ResearchTrackType;
}

export class ResearchTechnologyWorkflow extends ActionWorkflow {
	private _selectedTrack: ResearchTrackType | null = null;

	constructor(interaction: InteractionStateDto, private readonly _isAction: boolean) {
		super(interaction);
		this.init();
	}

	protected init(): void {
		this.states = [
			{
				id: WaitingForTrack,
				message: "Select a technology to research",
				view: ActiveView.ResearchBoard,
			},
			{
				id: WaitingForConfirmation,
				message: "Advance in the selected track?",
				commands: [CommonCommands.Cancel, CommonCommands.Confirm],
			},
		];
		const firstState = first(this.states)!;
		if (this._isAction) {
			firstState.commands = [CommonCommands.Abort];
		}
		this.currentState = firstState;
	}

	elementSelected(id: Identifier, type: InteractiveElementType): void {
		if (this.stateId !== WaitingForTrack) {
			return;
		}
		if (type !== InteractiveElementType.ResearchStep) {
			return;
		}

		this._selectedTrack = id as ResearchTrackType;
		this.advanceState(WaitingForConfirmation, `Advance research in ${localizeTrack(this._selectedTrack)}?`);
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
				this._selectedTrack = null;
				this.advanceState(WaitingForTrack);
				return null;
			case CommonWorkflowStates.PERFORM_ACTION:
				const action: ResearchTechnologyActionDto = {
					Type: ActionType.ResearchTechnology,
					Track: this._selectedTrack!,
				};
				return action;
			default:
				throw new Error(`State ${command.nextState} not handled.`);
		}
	}

	protected getInteractiveElements() {
		const elements = super.getInteractiveElements();
		if (!isNil(this._selectedTrack)) {
			return [
				...reject(elements, el => el.type === InteractiveElementType.ResearchStep && el.id === this._selectedTrack),
				{ id: this._selectedTrack, type: InteractiveElementType.ResearchStep, state: InteractiveElementState.Selected },
			];
		}
		return elements;
	}
}
