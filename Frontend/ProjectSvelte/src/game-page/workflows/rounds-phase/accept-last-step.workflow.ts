import { first } from "lodash";
import { ActionType, PendingDecisionType, ResearchTrackType } from "../../../dto/enums";
import type { ActionDto, PendingDecisionDto } from "../../../dto/interfaces";
import { ActionWorkflow } from "../action-workflow.base";
import type { Command } from "../types";

const WaitingForDecision = 0;
const AcceptLastStep = 1;
const DeclineLastStep = 2;

export interface AcceptOrDeclineLastStepDecisionDto extends PendingDecisionDto {
	type: PendingDecisionType.AcceptOrDeclineLastStep;
	track: ResearchTrackType;
}

interface AcceptOrDeclineLastStepActionDto extends ActionDto {
	Type: ActionType.AcceptOrDeclineLastStep;
	Accepted: boolean;
	Track: ResearchTrackType;
}

export class AcceptOrDeclineLastStepWorkflow extends ActionWorkflow {
	private readonly decision: AcceptOrDeclineLastStepDecisionDto;

	constructor(decision: AcceptOrDeclineLastStepDecisionDto) {
		super(null);
		this.decision = decision;
		this.init();
	}

	protected init(): void {
		this.states = [
			{
				id: WaitingForDecision,
				message: this.decision.description,
				commands: [
					{
						nextState: DeclineLastStep,
						text: "Decline",
					},
					{
						nextState: AcceptLastStep,
						text: "Accept",
						isPrimary: true,
					},
				],
			},
		];
		this.currentState = first(this.states)!;
	}

	handleCommand(command: Command): ActionDto | null {
		switch (command.nextState) {
			case AcceptLastStep:
				const accept: AcceptOrDeclineLastStepActionDto = {
					Type: ActionType.AcceptOrDeclineLastStep,
					Accepted: true,
					Track: this.decision.track,
				};
				return accept;
			case DeclineLastStep:
				const decline: AcceptOrDeclineLastStepActionDto = {
					Type: ActionType.AcceptOrDeclineLastStep,
					Accepted: false,
					Track: this.decision.track,
				};
				return decline;
			default:
				throw new Error(`State ${command.nextState} not handled.`);
		}
	}

	getInteractiveElements() {
		return [];
	}
}
