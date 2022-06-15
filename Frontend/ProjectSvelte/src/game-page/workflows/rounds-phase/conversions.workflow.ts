import { first } from "lodash";
import { ActionType, Conversion } from "../../../dto/enums";
import type { ActionDto } from "../../../dto/interfaces";
import { ActionWorkflow } from "../action-workflow.base";
import { ActiveView, Command, CommonWorkflowStates } from "../types";

const WaitingForConversions = 0;

export interface ConversionsActionDto extends ActionDto {
	Type: ActionType.Conversions;
	Conversions: Conversion[];
}

export class ConversionsWorkflow extends ActionWorkflow {
	protected init(): void {
		this.states = [
			{
				id: WaitingForConversions,
				message: "Perform conversions",
				view: ActiveView.ConversionDialog,
			},
		];
		this.currentState = first(this.states)!;
	}

	handleCommand(command: Command): ActionDto | null {
		if (this.stateId !== WaitingForConversions) {
			return null;
		}

		switch (command.nextState) {
			case CommonWorkflowStates.CANCEL:
				this.cancelAction();
				return null;
			case CommonWorkflowStates.PERFORM_CONVERSION:
				const conversions = command.data as Conversion[];
				const action: ConversionsActionDto = {
					Type: ActionType.Conversions,
					Conversions: conversions,
				};
				return action;
			default:
				throw new Error(`State ${command.nextState} not handled.`);
		}
	}
}
