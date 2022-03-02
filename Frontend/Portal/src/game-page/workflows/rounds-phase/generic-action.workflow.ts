import _ from "lodash";
import { ActionType, Race } from "../../../dto/enums";
import { ActionDto } from "../../../dto/interfaces";
import { Nullable } from "../../../utils/miscellanea";
import { ActionWorkflow } from "../action-workflow.base";
import { Command, CommonCommands, CommonWorkflowStates } from "../types";

const WaitingForConfirmation = 0;

const getMessage = (type: ActionType, race: Race): string => {
	switch (type) {
		case ActionType.BescodsResearchProgress:
			return "Advance in one of the least developed technologies?";
		case ActionType.UseRightAcademy:
			return `Activate the academy to gain ${race === Race.BalTaks ? "4 Credits" : "1 Qic"}?`;
		default:
			throw new Error(`Action ${type} not supported.`);
	}
};

export class GenericActionWorkflow extends ActionWorkflow {
	constructor(private readonly _actionType: ActionType, private readonly _race: Race, private readonly _message: Nullable<string> = null) {
		super(null);
		this.init();
	}

	protected init(): void {
		this.states = [
			{
				id: WaitingForConfirmation,
				message: this._message ?? getMessage(this._actionType, this._race),
				commands: [CommonCommands.Cancel, CommonCommands.Confirm],
			},
		];
		this.currentState = _.first(this.states)!;
	}

	handleCommand(command: Command): ActionDto | null {
		switch (command.nextState) {
			case CommonWorkflowStates.CANCEL:
				this.cancelAction();
				return null;
			case CommonWorkflowStates.PERFORM_ACTION:
				return { Type: this._actionType };
			default:
				throw new Error(`State ${command.nextState} not handled.`);
		}
	}
}
