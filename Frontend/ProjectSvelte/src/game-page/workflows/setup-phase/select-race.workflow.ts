import { first } from "lodash";
import { ActionType, Race } from "../../../dto/enums";
import type { ActionDto } from "../../../dto/interfaces";
import { ActionWorkflow } from "../action-workflow.base";
import { ActiveView, Command, CommonWorkflowStates } from "../types";

const cardinalNumbers = new Map<number, string>([
	[1, "1st"],
	[2, "2nd"],
	[3, "3rd"],
	[4, "4th"],
]);
const BoardExamination = 0;
const WaitingForSelection = 1;

interface SelectRaceActionDto extends ActionDto {
	Type: ActionType.SelectRace;
	Race: Race;
}

export class SelectRaceWorkflow extends ActionWorkflow {
	constructor(public readonly availableRaces: Race[], private readonly turnOrder: number, private readonly isForBid: boolean) {
		super(null, true);
		this.init();
	}

	protected init(): void {
		this.states = [
			{
				id: BoardExamination,
				message: `Select ${this.isForBid ? "a" : "your"} race to play in ${cardinalNumbers.get(this.turnOrder)!} position`,
				commands: [
					{
						nextState: WaitingForSelection,
						text: "Select",
					},
				],
				view: ActiveView.Map,
			},
			{
				id: WaitingForSelection,
				message: "Waiting for selection...",
				view: ActiveView.RaceSelectionDialog,
			},
		];
		this.currentState = first(this.states)!;
	}

	handleCommand(command: Command): ActionDto | null {
		switch (command.nextState) {
			case WaitingForSelection:
				this.advanceState(WaitingForSelection);
				return null;
			case CommonWorkflowStates.CANCEL:
				this.advanceState(BoardExamination);
				return null;
			case CommonWorkflowStates.PERFORM_ACTION:
				const race = command.data as Race;
				const action: SelectRaceActionDto = {
					Type: ActionType.SelectRace,
					Race: race,
				};
				return action;
			default:
				throw new Error(`State ${command.nextState} not handled.`);
		}
	}
}
