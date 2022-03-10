import { first } from "lodash";
import { ActionType } from "../../../dto/enums";
import { ActionDto, MapDto } from "../../../dto/interfaces";
import { CENTRAL_HEX_INDEX, Identifier, Nullable } from "../../../utils/miscellanea";
import { ActionWorkflow } from "../action-workflow.base";
import { InteractiveElementState, InteractiveElementType } from "../enums";
import { ActiveView, Command, CommonCommands, CommonWorkflowStates } from "../types";

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
	private readonly _initialRotations: Map<string, number>;

	constructor(private readonly _initialMap: MapDto) {
		super(null);
		this._centralHexesBySector = this._initialMap.sectors.map(s => ({ sectorId: s.id, hexId: s.hexes.find(h => h.index === CENTRAL_HEX_INDEX)!.id }));
		this._initialRotations = new Map(this._initialMap.sectors.map(s => [s.id, s.rotation]));
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
		this.currentState = first(this.states)!;
	}

	elementSelected(id: Identifier, type: InteractiveElementType): void {
		if (this.stateId !== WaitingForSector) {
			return;
		}
		if (type !== InteractiveElementType.Hex) {
			return;
		}
		const hexId = id as string;
		this._selectedSectorId = this._centralHexesBySector.find(o => o.hexId === hexId)!.sectorId;
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
				let adjustment = this._adjustments.find(o => o.SectorId === sectorId);
				if (!adjustment) {
					adjustment = { SectorId: sectorId, Rotation: rotationAdjustment };
					this._adjustments.push(adjustment);
				} else {
					adjustment.Rotation += rotationAdjustment;
				}
				const actualRotation = this.getActualRotation(sectorId, adjustment.Rotation);
				this.vm!.rotateSector(sectorId, actualRotation);
				return null;
			case CommonWorkflowStates.PERFORM_ACTION:
				const action: AdjustSectorsActionDto = {
					Type: ActionType.AdjustSectors,
					Adjustments: this._adjustments.map(adj => ({
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
						id: this._centralHexesBySector.find(s => s.sectorId === this._selectedSectorId)!.hexId,
						type: InteractiveElementType.Hex,
						state: InteractiveElementState.Selected,
					},
			  ]
			: this._centralHexesBySector.map(o => ({
					id: o.hexId,
					type: InteractiveElementType.Hex,
					state: InteractiveElementState.Enabled,
			  }));
	}

	private getActualRotation(sectorId: string, adjustment: number): number {
		const initialRotation = this._initialRotations.get(sectorId)!;
		const currentRotation = (initialRotation + adjustment) % ONE_FULL_ROTATION;
		const actualRotation = currentRotation >= 0 ? currentRotation : ONE_FULL_ROTATION + currentRotation;
		return actualRotation;
	}
}
