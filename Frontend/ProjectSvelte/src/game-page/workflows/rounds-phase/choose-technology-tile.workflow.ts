import { first, isEmpty, isNil, reject } from "lodash";
import { ActionType, AdvancedTechnologyTileType, StandardTechnologyTileType } from "../../../dto/enums";
import type { ActionDto, InteractionStateDto, PlayerInGameDto } from "../../../dto/interfaces";
import { localizeEnum } from "../../../utils/localization";
import type { Identifier } from "../../../utils/miscellanea";
import { ActionWorkflow } from "../action-workflow.base";
import { InteractiveElementState, InteractiveElementType } from "../enums";
import { ActiveView, Command, CommonCommands, CommonWorkflowStates } from "../types";

const WaitingForTile = 0;
const WaitingForConfirmation = 1;
const ChooseStandardTileToCover = 2;

interface ChooseTechnologyTileActionDto extends ActionDto {
	Type: ActionType.ChooseTechnologyTile;
	TileId: number;
	Advanced: boolean;
	CoveredTileId?: number;
}

export class ChooseTechnologyTileWorkflow extends ActionWorkflow {
	private _selectedStandardTile: StandardTechnologyTileType | null = null;
	private _selectedAdvancedTile: AdvancedTechnologyTileType | null = null;
	private _selectedStandardTileToCover: StandardTechnologyTileType | null = null;

	constructor(interaction: InteractionStateDto, private readonly _player: PlayerInGameDto) {
		super(interaction);
		this.init();
	}

	protected init(): void {
		this.states = [
			{
				id: WaitingForTile,
				message: "Select technology tile",
				view: ActiveView.ResearchBoard,
			},
			{
				id: ChooseStandardTileToCover,
				message: "Select a standard tile to cover",
				view: ActiveView.PlayerArea,
			},
			{
				id: WaitingForConfirmation,
				message: "Take the selected tile?",
				commands: [CommonCommands.Cancel, CommonCommands.Confirm],
			},
		];
		this.currentState = first(this.states)!;
	}

	elementSelected(id: Identifier, type: InteractiveElementType): void {
		if (this.stateId !== WaitingForTile && this.stateId !== ChooseStandardTileToCover) {
			return;
		}
		if (this.stateId === ChooseStandardTileToCover) {
			if (type !== InteractiveElementType.OwnStandardTile) {
				return;
			}
		} else if (type !== InteractiveElementType.StandardTile && type !== InteractiveElementType.AdvancedTile) {
			return;
		}

		if (this.stateId === WaitingForTile) {
			if (type === InteractiveElementType.StandardTile) {
				this._selectedStandardTile = id as StandardTechnologyTileType;
				this.advanceState(
					WaitingForConfirmation,
					`Select tile ${localizeEnum(this._selectedStandardTile, "StandardTechnologyTileType")}?`
				);
				return;
			}

			this._selectedAdvancedTile = id as AdvancedTechnologyTileType;
			const coverableTiles = this._player.state.technologyTiles.filter(st => !st.coveredByAdvancedTile);
			if (isEmpty(coverableTiles)) {
				throw new Error("You have no tiles to cover");
			}

			this.interactionState = {
				clickableOwnStandardTiles: coverableTiles.map(st => st.id),
			};
			this.advanceState(ChooseStandardTileToCover);
		} else {
			this._selectedStandardTileToCover = id as StandardTechnologyTileType;
			const coveringTile = localizeEnum(this._selectedAdvancedTile!, "AdvancedTechnologyTileType");
			const coveredTile = localizeEnum(this._selectedStandardTileToCover, "StandardTechnologyTileType");
			this.advanceState(WaitingForConfirmation, `Select tile ${coveringTile} and cover tile ${coveredTile}?`);
		}
	}

	handleCommand(command: Command): ActionDto | null {
		if (this.stateId === WaitingForTile) {
			return null;
		}

		switch (command.nextState) {
			case CommonWorkflowStates.RESET:
			case CommonWorkflowStates.CANCEL:
				if (command.nextState === CommonWorkflowStates.CANCEL && !isNil(this._selectedStandardTileToCover)) {
					this._selectedStandardTileToCover = null;
					this.advanceState(ChooseStandardTileToCover);
					return null;
				}
				this._selectedStandardTile = null;
				this._selectedAdvancedTile = null;
				this.advanceState(WaitingForTile);
				return null;
			case CommonWorkflowStates.PERFORM_ACTION:
				const action: ChooseTechnologyTileActionDto = !isNil(this._selectedAdvancedTile)
					? {
							Type: ActionType.ChooseTechnologyTile,
							TileId: this._selectedAdvancedTile,
							Advanced: true,
							CoveredTileId: this._selectedStandardTileToCover!,
					  }
					: {
							Type: ActionType.ChooseTechnologyTile,
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
		if (!isNil(this._selectedStandardTile)) {
			elements = [
				...reject(elements, el => el.type === InteractiveElementType.StandardTile && el.id === this._selectedStandardTile),
				{ id: this._selectedStandardTile, type: InteractiveElementType.StandardTile, state: InteractiveElementState.Selected },
			];
		}
		if (!isNil(this._selectedAdvancedTile)) {
			elements = [
				...reject(elements, el => el.type === InteractiveElementType.AdvancedTile && el.id === this._selectedAdvancedTile),
				{ id: this._selectedAdvancedTile, type: InteractiveElementType.AdvancedTile, state: InteractiveElementState.Selected },
			];
		}
		if (!isNil(this._selectedStandardTileToCover)) {
			elements = [
				...reject(
					elements,
					el => el.type === InteractiveElementType.OwnStandardTile && el.id === this._selectedStandardTileToCover
				),
				{
					id: this._selectedStandardTileToCover,
					type: InteractiveElementType.OwnStandardTile,
					state: InteractiveElementState.Selected,
				},
			];
		}
		return elements;
	}
}
