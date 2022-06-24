import { first } from "lodash";
import { ActionType, Conversion, PendingDecisionType } from "../../../dto/enums";
import type { ActionDto, PendingDecisionDto } from "../../../dto/interfaces";
import { ActionWorkflow } from "../action-workflow.base";
import { ActiveView, Command, CommonWorkflowStates } from "../types";

const BoardExamination = 0;
const WaitingForConversions = 1;

export interface TerransDecideIncomeDecisionDto extends PendingDecisionDto {
	type: PendingDecisionType.TerransDecideIncome;
	power: number;
}

export interface TerransDecideIncomeActionDto extends ActionDto {
	Type: ActionType.TerransDecideIncome;
	Credits: number;
	Ores: number;
	Knowledge: number;
	Qic: number;
}

export class TerransDecideIncomeWorkflow extends ActionWorkflow {
	constructor(private readonly _powerToConvert: number) {
		super(null, false);
	}

	get powerToConvert(): number {
		return this._powerToConvert;
	}

	protected init(): void {
		this.states = [
			{
				id: BoardExamination,
				message: "You must decide how convert power from the Gaia area",
				commands: [
					{
						nextState: WaitingForConversions,
						text: "Select",
						isPrimary: true,
					},
				],
				view: ActiveView.Map,
			},
			{
				id: WaitingForConversions,
				message: "Waiting for conversion...",
				view: ActiveView.TerransConversionsDialog,
			},
		];
		this.currentState = first(this.states)!;
	}

	handleCommand(command: Command): ActionDto | null {
		switch (command.nextState) {
			case WaitingForConversions:
				this.advanceState(WaitingForConversions);
				return null;
			case CommonWorkflowStates.CANCEL:
				this.advanceState(BoardExamination);
				return null;
			case CommonWorkflowStates.PERFORM_CONVERSION:
				const conversions = command.data as Conversion[];
				const action: TerransDecideIncomeActionDto = {
					Type: ActionType.TerransDecideIncome,
					Credits: conversions.filter(c => c === Conversion.PowerToCredit).length,
					Ores: conversions.filter(c => c === Conversion.PowerToOre).length,
					Knowledge: conversions.filter(c => c === Conversion.PowerToKnowledge).length,
					Qic: conversions.filter(c => c === Conversion.PowerToQic).length,
				};
				return action;
			default:
				throw new Error(`State ${command.nextState} not handled.`);
		}
	}
}
