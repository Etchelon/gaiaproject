import _ from "lodash";
import { ActionType, PendingDecisionType } from "../../../dto/enums";
import { ActionDto, PendingDecisionDto } from "../../../dto/interfaces";
import { ActionWorkflow } from "../action-workflow.base";
import { Command } from "../types";

const WaitingForDecision = 0;
const ChargePower = 1;
const DeclinePower = 2;

export interface ChargePowerDecisionDto extends PendingDecisionDto {
	type: PendingDecisionType.ChargePower;
	description: string;
}

interface ChargePowerActionDto extends ActionDto {
	Type: ActionType.ChargePower;
	Accepted: boolean;
}

export class ChargePowerWorkflow extends ActionWorkflow {
	private readonly decision: ChargePowerDecisionDto;

	constructor(decision: ChargePowerDecisionDto) {
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
						nextState: DeclinePower,
						text: "Decline",
					},
					{
						nextState: ChargePower,
						text: "Charge",
						isPrimary: true,
					},
				],
			},
		];
		this.currentState = _.first(this.states)!;
	}

	handleCommand(command: Command): ActionDto | null {
		switch (command.nextState) {
			case ChargePower:
				const accept: ChargePowerActionDto = {
					Type: ActionType.ChargePower,
					Accepted: true,
				};
				return accept;
			case DeclinePower:
				const decline: ChargePowerActionDto = {
					Type: ActionType.ChargePower,
					Accepted: false,
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
