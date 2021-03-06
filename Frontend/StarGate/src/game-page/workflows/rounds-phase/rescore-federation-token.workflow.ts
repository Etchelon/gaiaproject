import _ from "lodash";
import { ActionType, FederationTokenType } from "../../../dto/enums";
import { ActionDto } from "../../../dto/interfaces";
import { localizeEnum } from "../../../utils/localization";
import { Identifier, Nullable } from "../../../utils/miscellanea";
import { ActionWorkflow } from "../action-workflow.base";
import { InteractiveElementState, InteractiveElementType } from "../enums";
import { ActiveView, Command, CommonCommands, CommonWorkflowStates } from "../types";

const WaitingForToken = 0;
const WaitingForConfirmation = 1;

const localizeFederationToken = _.partialRight(localizeEnum, "FederationTokenType");

interface RescoreFederationTokenActionDto extends ActionDto {
	Type: ActionType.RescoreFederationToken;
	Token: FederationTokenType;
}

// TODO: exact duplicate of the Colonize Planet workflow. Refactor!!
export class RescoreFederationTokenWorkflow extends ActionWorkflow {
	private _selectedToken: Nullable<FederationTokenType> = null;

	protected init(): void {
		this.states = [
			{
				id: WaitingForToken,
				message: "Select which Federation token to score",
				view: ActiveView.PlayerAreas,
			},
			{
				id: WaitingForConfirmation,
				message: "Rescore the selected token?",
				commands: [CommonCommands.Cancel, CommonCommands.Confirm],
			},
		];
		this.currentState = _.first(this.states)!;
	}

	elementSelected(id: Identifier, type: InteractiveElementType): void {
		if (this.stateId !== WaitingForToken) {
			return;
		}
		if (type !== InteractiveElementType.OwnFederationToken) {
			return;
		}
		this._selectedToken = id as FederationTokenType;
		this.advanceState(WaitingForConfirmation, `Rescore token ${localizeFederationToken(this._selectedToken)}?`);
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
				this._selectedToken = null;
				this.advanceState(WaitingForToken);
				return null;
			case CommonWorkflowStates.PERFORM_ACTION:
				const action: RescoreFederationTokenActionDto = {
					Type: ActionType.RescoreFederationToken,
					Token: this._selectedToken!,
				};
				return action;
			default:
				throw new Error(`State ${command.nextState} not handled.`);
		}
	}

	protected getInteractiveElements() {
		const elements = super.getInteractiveElements();
		if (!_.isNil(this._selectedToken)) {
			return [
				..._.reject(elements, el => el.type === InteractiveElementType.OwnFederationToken && el.id === this._selectedToken),
				{ id: this._selectedToken, type: InteractiveElementType.OwnFederationToken, state: InteractiveElementState.Selected },
			];
		}
		return elements;
	}
}
