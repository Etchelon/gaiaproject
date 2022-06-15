import { first } from "lodash";
import { ActionType, PendingDecisionType } from "../../../dto/enums";
import type { ActionDto, PendingDecisionDto } from "../../../dto/interfaces";
import { ActionWorkflow } from "../action-workflow.base";
import type { Command } from "../types";

const WaitingForDecision = 0;
const ChargePowerThenToken = 1;
const ChargeTokenThenPower = 2;
const DeclinePower = 3;

export interface TaklonsLeechDecisionDto extends PendingDecisionDto {
	type: PendingDecisionType.TaklonsLeech;
	powerBeforeToken: number;
	powerAfterToken: number;
}

interface TaklonsLeechActionDto extends ActionDto {
	Type: ActionType.TaklonsLeech;
	Accepted: boolean;
	ChargeFirstThenToken?: boolean;
}

export class TaklonsLeechWorkflow extends ActionWorkflow {
	constructor(private readonly _decision: TaklonsLeechDecisionDto) {
		super(null);
		this.init();
	}

	protected init(): void {
		const commands = [
			{
				nextState: DeclinePower,
				text: "Decline",
			},
			{
				nextState: ChargeTokenThenPower,
				text: `+1 Token -> ${this._decision.powerAfterToken}x${this._decision.powerAfterToken - 1}VP`,
				isPrimary: true,
			},
		];
		if (this._decision.powerBeforeToken > 0) {
			commands.push({
				nextState: ChargePowerThenToken,
				text: `${this._decision.powerBeforeToken}x${this._decision.powerBeforeToken - 1}VP -> +1 Token`,
				isPrimary: true,
			});
		}

		this.states = [
			{
				id: WaitingForDecision,
				message: "Choose",
				commands,
			},
		];
		this.currentState = first(this.states)!;
	}

	handleCommand(command: Command): ActionDto | null {
		switch (command.nextState) {
			case DeclinePower:
				const decline: TaklonsLeechActionDto = {
					Type: ActionType.TaklonsLeech,
					Accepted: false,
				};
				return decline;
			case ChargeTokenThenPower:
				const tokenFirst: TaklonsLeechActionDto = {
					Type: ActionType.TaklonsLeech,
					Accepted: true,
					ChargeFirstThenToken: false,
				};
				return tokenFirst;
			case ChargePowerThenToken:
				const powerFirst: TaklonsLeechActionDto = {
					Type: ActionType.TaklonsLeech,
					Accepted: true,
					ChargeFirstThenToken: true,
				};
				return powerFirst;
			default:
				throw new Error(`State ${command.nextState} not handled.`);
		}
	}

	getInteractiveElements() {
		return [];
	}
}
