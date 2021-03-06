import _ from "lodash";
import { ActionType, AdvancedTechnologyTileType, StandardTechnologyTileType } from "../../../dto/enums";
import { ActionDto } from "../../../dto/interfaces";
import { localizeEnum } from "../../../utils/localization";
import { Identifier, Nullable } from "../../../utils/miscellanea";
import { ActionWorkflow } from "../action-workflow.base";
import { InteractiveElementState, InteractiveElementType } from "../enums";
import { ActiveView, Command, CommonCommands, CommonWorkflowStates } from "../types";

const WaitingForTile = 0;
const WaitingForConfirmation = 1;

interface UseTechnologyTileActionDto extends ActionDto {
	Type: ActionType.UseTechnologyTile;
	TileId: number;
	Advanced: boolean;
}

export class UseTechnologyTileWorkflow extends ActionWorkflow {
	private _selectedStandardTile: Nullable<StandardTechnologyTileType> = null;
	private _selectedAdvancedTile: Nullable<AdvancedTechnologyTileType> = null;

	protected init(): void {
		this.states = [
			{
				id: WaitingForTile,
				message: "Select one of your tiles",
				view: ActiveView.PlayerAreas,
				commands: [CommonCommands.Abort],
			},
			{
				id: WaitingForConfirmation,
				message: "Use the action of the selected tile?",
				commands: [CommonCommands.Cancel, CommonCommands.Confirm],
			},
		];
		this.currentState = _.first(this.states)!;
	}

	elementSelected(id: Identifier, type: InteractiveElementType): void {
		if (this.stateId !== WaitingForTile) {
			return;
		}
		if (type !== InteractiveElementType.OwnStandardTile && type !== InteractiveElementType.OwnAdvancedTile) {
			return;
		}

		let message: string;
		if (type === InteractiveElementType.OwnStandardTile) {
			this._selectedStandardTile = id as StandardTechnologyTileType;
			message = `Use action ${localizeEnum(this._selectedStandardTile, "StandardTechnologyTileType")}?`;
		} else {
			this._selectedAdvancedTile = id as AdvancedTechnologyTileType;
			message = `Use action ${localizeEnum(this._selectedAdvancedTile, "AdvancedTechnologyTileType")}?`;
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
				this._selectedStandardTile = null;
				this._selectedAdvancedTile = null;
				this.advanceState(WaitingForTile);
				return null;
			case CommonWorkflowStates.PERFORM_ACTION:
				const action: UseTechnologyTileActionDto = !_.isNil(this._selectedAdvancedTile)
					? {
							Type: ActionType.UseTechnologyTile,
							TileId: this._selectedAdvancedTile,
							Advanced: true,
					  }
					: {
							Type: ActionType.UseTechnologyTile,
							TileId: this._selectedStandardTile!,
							Advanced: false,
					  };
				return action;
			default:
				throw new Error(`State ${command.nextState} not handled.`);
		}
	}

	protected getInteractiveElements() {
		let elements = super.getInteractiveElements();
		if (!_.isNil(this._selectedStandardTile)) {
			elements = [
				..._.reject(elements, el => el.type === InteractiveElementType.OwnStandardTile && el.id === this._selectedStandardTile),
				{ id: this._selectedStandardTile, type: InteractiveElementType.OwnStandardTile, state: InteractiveElementState.Selected },
			];
		}
		if (!_.isNil(this._selectedAdvancedTile)) {
			elements = [
				..._.reject(elements, el => el.type === InteractiveElementType.OwnAdvancedTile && el.id === this._selectedAdvancedTile),
				{ id: this._selectedAdvancedTile, type: InteractiveElementType.OwnAdvancedTile, state: InteractiveElementState.Selected },
			];
		}
		return elements;
	}
}
