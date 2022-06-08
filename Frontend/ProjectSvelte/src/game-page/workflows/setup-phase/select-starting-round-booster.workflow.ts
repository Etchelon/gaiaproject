import _ from "lodash";
import { ActionType, RoundBoosterType } from "../../../dto/enums";
import { ActionDto } from "../../../dto/interfaces";
import { localizeRoundBooster } from "../../../utils/localization";
import { Identifier, Nullable } from "../../../utils/miscellanea";
import { ActionWorkflow } from "../action-workflow.base";
import { InteractiveElementState, InteractiveElementType } from "../enums";
import { ActiveView, Command, CommonCommands, CommonWorkflowStates } from "../types";

const WaitingForBooster = 0;
const WaitingForConfirmation = 1;

interface SelectStartingRoundBoosterActionDto extends ActionDto {
	Type: ActionType.SelectStartingRoundBooster;
	Booster: RoundBoosterType;
}

// TODO: exact duplicate of the Colonize Planet workflow. Refactor!!
export class SelectStartingRoundBoosterWorkflow extends ActionWorkflow {
	private _selectedBooster: Nullable<RoundBoosterType> = null;

	protected init(): void {
		this.states = [
			{
				id: WaitingForBooster,
				message: "Select one of the available round boosters",
				view: ActiveView.ScoringBoard,
			},
			{
				id: WaitingForConfirmation,
				message: "Do you want to select this round booster?",
				commands: [CommonCommands.Cancel, CommonCommands.Confirm],
			},
		];
		this.currentState = _.first(this.states)!;
	}

	elementSelected(id: Identifier, type: InteractiveElementType): void {
		if (this.stateId !== WaitingForBooster) {
			return;
		}
		if (type !== InteractiveElementType.RoundBooster) {
			return;
		}
		this._selectedBooster = id as RoundBoosterType;
		this.advanceState(WaitingForConfirmation, `Select booster ${localizeRoundBooster(this._selectedBooster)}?`);
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
				this._selectedBooster = null;
				this.advanceState(WaitingForBooster);
				return null;
			case CommonWorkflowStates.PERFORM_ACTION:
				const action: SelectStartingRoundBoosterActionDto = {
					Type: ActionType.SelectStartingRoundBooster,
					Booster: this._selectedBooster!,
				};
				return action;
			default:
				throw new Error(`State ${command.nextState} not handled.`);
		}
	}

	protected getInteractiveElements() {
		const elements = super.getInteractiveElements();
		if (!_.isNil(this._selectedBooster)) {
			return [
				..._.reject(elements, el => el.type === InteractiveElementType.RoundBooster && el.id === this._selectedBooster),
				{ id: this._selectedBooster, type: InteractiveElementType.RoundBooster, state: InteractiveElementState.Selected },
			];
		}
		return elements;
	}
}
