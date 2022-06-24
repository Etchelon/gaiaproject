import { first } from "lodash";
import { ActionType, Conversion } from "../../../dto/enums";
import type { ActionDto } from "../../../dto/interfaces";
import { ActionWorkflow } from "../action-workflow.base";
import { ActiveView, Command, CommonWorkflowStates } from "../types";
import type { ConversionsActionDto } from "./conversions.workflow";

const WaitingForDecision = 0;
const WaitingForConversions = 1;
export const PassTurn = 2;

interface PassTurnActionDto extends ActionDto {
	Type: ActionType.PassTurn;
}

export class PerformConversionsOrPassTurnWorkflow extends ActionWorkflow {
	protected init(): void {
		this.states = [
			{
				id: WaitingForDecision,
				message: "You can perform conversions before ending your turn",
				commands: [
					{ nextState: WaitingForConversions, text: "Conversions" },
					{ nextState: PassTurn, text: "End turn" },
				],
				view: ActiveView.Map,
			},
			{
				id: WaitingForConversions,
				message: "Conversions",
				view: ActiveView.ConversionDialog,
			},
		];
		this.currentState = first(this.states)!;
	}

	handleCommand(command: Command): ActionDto | null {
		switch (command.nextState) {
			case WaitingForConversions:
				this.advanceState(WaitingForConversions);
				return null;
			case CommonWorkflowStates.CANCEL:
				this.advanceState(WaitingForDecision);
				return null;
			case CommonWorkflowStates.PERFORM_CONVERSION:
				const conversions = command.data as Conversion[];
				const conversionAction: ConversionsActionDto = {
					Type: ActionType.Conversions,
					Conversions: conversions,
				};
				return conversionAction;
			case PassTurn:
				const action: PassTurnActionDto = {
					Type: ActionType.PassTurn,
				};
				return action;
			default:
				throw new Error(`State ${command.nextState} not handled.`);
		}
	}
}
