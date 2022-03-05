import _ from "lodash";
import { ActionType } from "../../../dto/enums";
import { ActionDto, MapDto } from "../../../dto/interfaces";
import { CENTRAL_HEX_INDEX, Identifier, Nullable } from "../../../utils/miscellanea";
import { resetSectors, rotateSector } from "../../store/active-game.slice";
import { ActionWorkflow } from "../action-workflow.base";
import { InteractiveElementState, InteractiveElementType } from "../enums";
import { ActiveView, Command, CommonCommands, CommonWorkflowStates, InteractiveElement } from "../types";

const WaitingForSector = 0;
const WaitingForRotation = 1;
const WaitingForConfirmation = 2;
const DoRotation = 3;
const ONE_FULL_ROTATION = 6;

interface SectorAdjustmentDto {
	SectorId: string;
	Rotation: number;
}

interface AdjustSectorsActionDto extends ActionDto {
	Type: ActionType.AdjustSectors;
	Adjustments: SectorAdjustmentDto[];
}

export class AdjustSectorsWorkflow extends ActionWorkflow {
	private readonly _centralHexesBySector: { sectorId: string; hexId: string }[];
	private _selectedSectorId: Nullable<string> = null;
	private _adjustments: SectorAdjustmentDto[] = [];

	constructor(private readonly _initialMap: MapDto) {
		super(null);
		this._centralHexesBySector = _.map(this._initialMap.sectors, s => ({ sectorId: s.id, hexId: _.find(s.hexes, h => h.index === CENTRAL_HEX_INDEX)!.id }));
		this.init();
	}

	protected init(): void {
		this.states = [
			{
				id: WaitingForSector,
				message: "Select a sector to rotate",
				view: ActiveView.Map,
				commands: [
					CommonCommands.Reset,
					{
						text: "Finish",
						nextState: WaitingForConfirmation,
						isPrimary: true,
					},
				],
			},
			{
				id: WaitingForRotation,
				message: "Select rotation",
				view: ActiveView.Map,
				commands: [
					{
						text: "<",
						nextState: DoRotation,
						data: 1, // 60° counter clockwise
					},
					{
						text: "Done",
						nextState: WaitingForSector,
						isPrimary: true,
					},
					{
						text: ">",
						nextState: DoRotation,
						data: -1, // 60° clockwise
					},
				],
			},
			{
				id: WaitingForConfirmation,
				message: "Confirm the adjustments you have chosen to do?",
				commands: [CommonCommands.Cancel, CommonCommands.Confirm],
			},
		];
		this.currentState = _.first(this.states)!;
	}

	elementSelected(id: Identifier, type: InteractiveElementType): void {
		if (this.stateId !== WaitingForSector) {
			return;
		}
		if (type !== InteractiveElementType.Hex) {
			return;
		}
		const hexId = id as string;
		this._selectedSectorId = _.find(this._centralHexesBySector, o => o.hexId === hexId)!.sectorId;
		this.advanceState();
	}

	handleCommand(command: Command): ActionDto | null {
		switch (command.nextState) {
			case CommonWorkflowStates.RESET:
				this._selectedSectorId = null;
				this._adjustments = [];
				this.advanceState(WaitingForSector);
				this.vm!.resetSectors(this._initialMap);
				return null;
			case CommonWorkflowStates.CANCEL:
				this._selectedSectorId = null;
				this.advanceState(WaitingForSector);
				return null;
			case WaitingForSector:
			case WaitingForConfirmation:
				this._selectedSectorId = null;
				this.advanceState(command.nextState);
				return null;
			case DoRotation:
				const sectorId = this._selectedSectorId!;
				const rotationAdjustment = command.data as number;
				let adjustment = _.find(this._adjustments, o => o.SectorId === sectorId);
				if (!adjustment) {
					adjustment = { SectorId: sectorId, Rotation: rotationAdjustment };
					this._adjustments.push(adjustment);
				} else {
					adjustment.Rotation += rotationAdjustment;
				}
				const actualRotation = this.getActualRotation(sectorId, adjustment.Rotation);
				this.vm!.rotateSector({ id: sectorId, rotation: actualRotation });
				return null;
			case CommonWorkflowStates.PERFORM_ACTION:
				const action: AdjustSectorsActionDto = {
					Type: ActionType.AdjustSectors,
					Adjustments: _.map(this._adjustments, adj => ({
						SectorId: adj.SectorId,
						Rotation: adj.Rotation % ONE_FULL_ROTATION,
					})),
				};
				return action;
			default:
				throw new Error(`State ${command.nextState} not handled.`);
		}
	}

	protected getInteractiveElements() {
		const isRotatingSector = this._selectedSectorId !== null;
		return isRotatingSector
			? [
					{
						id: _.find(this._centralHexesBySector, s => s.sectorId === this._selectedSectorId)!.hexId,
						type: InteractiveElementType.Hex,
						state: InteractiveElementState.Selected,
					},
			  ]
			: _.map(this._centralHexesBySector, o => ({
					id: o.hexId,
					type: InteractiveElementType.Hex,
					state: InteractiveElementState.Enabled,
			  }));
	}

	private getActualRotation(sectorId: string, adjustment: number): number {
		const initialRotation = _.find(this._initialMap.sectors, s => s.id === sectorId)!.rotation;
		const currentRotation = (initialRotation + adjustment) % ONE_FULL_ROTATION;
		const actualRotation = currentRotation >= 0 ? currentRotation : ONE_FULL_ROTATION + currentRotation;
		return actualRotation;
	}
}
