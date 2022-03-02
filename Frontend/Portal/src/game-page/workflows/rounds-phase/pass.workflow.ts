import _ from "lodash";
import { ActionType, RoundBoosterType } from "../../../dto/enums";
import { ActionDto, InteractionStateDto } from "../../../dto/interfaces";
import { localizeRoundBooster } from "../../../utils/localization";
import { Identifier, Nullable } from "../../../utils/miscellanea";
import { ActionWorkflow } from "../action-workflow.base";
import { InteractiveElementState, InteractiveElementType } from "../enums";
import { ActiveView, Command, CommonCommands, CommonWorkflowStates } from "../types";

const WaitingForActivatablesConfirmation = 0;
const WaitingForBooster = 1;
const WaitingForConfirmation = 2;

interface PassActionDto extends ActionDto {
	Type: ActionType.Pass;
	SelectedRoundBooster: Nullable<RoundBoosterType>;
}

export class PassWorkflow extends ActionWorkflow {
	private _selectedBooster: Nullable<RoundBoosterType> = null;

	constructor(interaction: InteractionStateDto, private readonly _stillHasActivatables: boolean, private readonly _isLastRound: boolean) {
		super(interaction);
		this.init();
	}

	protected init(): void {
		this.states = [
			{
				id: WaitingForConfirmation,
				message: "Are you sure you want to pass?",
				commands: [CommonCommands.Cancel, CommonCommands.Confirm],
			},
		];
		if (!this._isLastRound) {
			this.states.unshift({
				id: WaitingForBooster,
				message: "Select a round booster for the next round",
				commands: [CommonCommands.Abort],
				view: ActiveView.ScoringBoard,
			});
		}
		if (this._stillHasActivatables) {
			this.states.unshift({
				id: WaitingForActivatablesConfirmation,
				message: "Are you sure you want to pass? YOU STILL HAVE ACTIVE ACTIONS TO PERFORM!!",
				commands: [
					CommonCommands.Abort,
					{
						nextState: this._isLastRound ? CommonWorkflowStates.PERFORM_ACTION : WaitingForBooster,
						text: "Confirm",
						isPrimary: true,
					},
				],
			});
		}

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
		this.advanceState(WaitingForConfirmation, `Take booster ${localizeRoundBooster(this._selectedBooster)} and pass the rest of the round?`);
	}

	handleCommand(command: Command): ActionDto | null {
		if (command.nextState === CommonWorkflowStates.ABORT) {
			this.cancelAction();
			return null;
		}
		if (this.stateId !== WaitingForActivatablesConfirmation && this.stateId !== WaitingForConfirmation) {
			return null;
		}

		switch (command.nextState) {
			case WaitingForBooster:
				this.advanceState(WaitingForBooster);
				return null;
			case CommonWorkflowStates.RESET:
			case CommonWorkflowStates.CANCEL:
				this._selectedBooster = null;
				this.advanceState(WaitingForBooster);
				return null;
			case CommonWorkflowStates.PERFORM_ACTION:
				const action: PassActionDto = {
					Type: ActionType.Pass,
					SelectedRoundBooster: this._selectedBooster,
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
