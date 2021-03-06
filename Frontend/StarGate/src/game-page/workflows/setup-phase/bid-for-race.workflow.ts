import _ from "lodash";
import { ActionType, Race } from "../../../dto/enums";
import { ActionDto, AuctionStateDto } from "../../../dto/interfaces";
import { Nullable } from "../../../utils/miscellanea";
import { ActionWorkflow } from "../action-workflow.base";
import { ActiveView, Command, CommonWorkflowStates } from "../types";

const BoardExamination = 0;
const WaitingForBid = 1;

interface BidForRaceActionDto extends ActionDto {
	Type: ActionType.BidForRace;
	Race: Race;
	Points: number;
}

export interface CurrentAuction {
	race: Nullable<Race>;
	bid: Nullable<number>;
}

export class BidForRaceWorkflow extends ActionWorkflow {
	constructor(public readonly auctionState: AuctionStateDto) {
		super(null, false);
	}

	protected init(): void {
		this.states = [
			{
				id: BoardExamination,
				message: "Bid for a race",
				commands: [
					{
						nextState: WaitingForBid,
						text: "Bid",
					},
				],
				view: ActiveView.Map,
			},
			{
				id: WaitingForBid,
				message: "Waiting for selection...",
				view: ActiveView.AuctionDialog,
			},
		];
		this.currentState = _.first(this.states)!;
	}

	handleCommand(command: Command): ActionDto | null {
		switch (command.nextState) {
			case WaitingForBid:
				this.advanceState(WaitingForBid);
				return null;
			case CommonWorkflowStates.CANCEL:
				this.advanceState(BoardExamination);
				return null;
			case CommonWorkflowStates.PERFORM_ACTION:
				const data = command.data as CurrentAuction;
				const action: BidForRaceActionDto = {
					Type: ActionType.BidForRace,
					Race: data.race!,
					Points: data.bid!,
				};
				return action;
			default:
				throw new Error(`State ${command.nextState} not handled.`);
		}
	}
}
