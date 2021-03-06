import _ from "lodash";
import { ActionType, PowerActionType, QicActionType } from "../../../dto/enums";
import { ActionDto, InteractionStateDto } from "../../../dto/interfaces";
import { localizeEnum } from "../../../utils/localization";
import { Identifier } from "../../../utils/miscellanea";
import { ActionWorkflow } from "../action-workflow.base";
import { InteractiveElementState, InteractiveElementType } from "../enums";
import { ActiveView, Command, CommonCommands, CommonWorkflowStates } from "../types";

const WaitingForAction = 0;
const WaitingForConfirmation = 1;

interface PowerOrQicActionDto extends ActionDto {
	Type: ActionType.Power | ActionType.Qic;
	Id: PowerActionType | QicActionType;
}

// TODO: exact duplicate of the Colonize Planet workflow. Refactor!!
export class PowerOrQicActionWorkflow extends ActionWorkflow {
	private _selectedAction: PowerActionType | QicActionType | null = null;
	private readonly _actualType: ActionType.Power | ActionType.Qic;
	private readonly _actualInteractiveElementType: InteractiveElementType.PowerAction | InteractiveElementType.QicAction;

	constructor(interaction: InteractionStateDto, private readonly _isPower: boolean) {
		super(interaction);
		this._actualType = _isPower ? ActionType.Power : ActionType.Qic;
		this._actualInteractiveElementType = _isPower ? InteractiveElementType.PowerAction : InteractiveElementType.QicAction;
		this.init();
	}

	protected init(): void {
		const type = this._isPower ? "Power" : "Qic";
		this.states = [
			{
				id: WaitingForAction,
				message: `Select an available ${type} action`,
				commands: [CommonCommands.Abort],
				view: ActiveView.ResearchBoard,
			},
			{
				id: WaitingForConfirmation,
				message: `Take the selected ${type} action?`,
				commands: [CommonCommands.Cancel, CommonCommands.Confirm],
			},
		];
		this.currentState = _.first(this.states)!;
	}

	elementSelected(id: Identifier, type: InteractiveElementType): void {
		if (this.stateId !== WaitingForAction) {
			return;
		}
		if ((this._isPower && type !== InteractiveElementType.PowerAction) || (!this._isPower && type !== InteractiveElementType.QicAction)) {
			return;
		}

		this._selectedAction = id as PowerActionType | QicActionType;
		const action = localizeEnum(this._selectedAction, this._isPower ? "PowerActionType" : "QicActionType");
		let message = `Take action ${action}?`;
		if (this._isPower) {
			const actionDto = _.find(this.interactionState?.clickablePowerActions, pa => pa.type === id)!;
			if (actionDto.powerToBurn) {
				message = `Burn ${actionDto.powerToBurn} and take action ${action}?`;
			}
		}
		this.advanceState(WaitingForConfirmation, message);
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
				this._selectedAction = null;
				this.advanceState(WaitingForAction);
				return null;
			case CommonWorkflowStates.PERFORM_ACTION:
				const action: PowerOrQicActionDto = {
					Type: this._actualType,
					Id: this._selectedAction!,
				};
				return action;
			default:
				throw new Error(`State ${command.nextState} not handled.`);
		}
	}

	protected getInteractiveElements() {
		const elements = super.getInteractiveElements();
		if (!_.isNil(this._selectedAction)) {
			return [
				..._.reject(elements, el => el.type === this._actualInteractiveElementType && el.id === this._selectedAction),
				{ id: this._selectedAction, type: this._actualInteractiveElementType, state: InteractiveElementState.Selected },
			];
		}
		return elements;
	}
}
