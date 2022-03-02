import _ from "lodash";
import { ActionType, PendingDecisionType } from "../../../dto/enums";
import { ActionDto, PendingDecisionDto } from "../../../dto/interfaces";
import { ActionWorkflow } from "../action-workflow.base";
import { ActiveView, Command, CommonCommands, CommonWorkflowStates } from "../types";

const WaitingForDecision = 0;

export interface ItarsBurnPowerForTechnologyTileDecisionDto extends PendingDecisionDto {
	type: PendingDecisionType.ItarsBurnPowerForTechnologyTile;
}

export interface ItarsBurnPowerForTechnologyTileActionDto extends ActionDto {
	Type: ActionType.ItarsBurnPowerForTechnologyTile;
	Accepted: boolean;
}

export class ItarsBurnPowerForTechnologyTileWorkflow extends ActionWorkflow {
	constructor() {
		super(null, false);
	}

	protected init(): void {
		this.states = [
			{
				id: WaitingForDecision,
				message: "Burn 4 power from the Gaia area for a technology tile?",
				commands: [CommonCommands.Cancel, CommonCommands.Confirm],
				view: ActiveView.Map,
			},
		];
		this.currentState = _.first(this.states)!;
	}

	handleCommand(command: Command): ActionDto | null {
		switch (command.nextState) {
			case CommonWorkflowStates.CANCEL:
			case CommonWorkflowStates.PERFORM_ACTION:
				const accepted = command.nextState === CommonWorkflowStates.PERFORM_ACTION;
				const action: ItarsBurnPowerForTechnologyTileActionDto = {
					Type: ActionType.ItarsBurnPowerForTechnologyTile,
					Accepted: accepted,
				};
				return action;
			default:
				throw new Error(`State ${command.nextState} not handled.`);
		}
	}
}
