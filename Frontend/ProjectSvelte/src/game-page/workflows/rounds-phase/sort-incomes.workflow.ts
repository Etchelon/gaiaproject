import { first } from "lodash";
import { ActionType, PendingDecisionType } from "../../../dto/enums";
import type { ActionDto, PendingDecisionDto, SortableIncomeDto } from "../../../dto/interfaces";
import { ActionWorkflow } from "../action-workflow.base";
import { ActiveView, Command, CommonWorkflowStates } from "../types";

const WaitingForSorting = 0;

export interface SortIncomesDecisionDto extends PendingDecisionDto {
	type: PendingDecisionType.SortIncomes;
	description: string;
	powerIncomes: SortableIncomeDto[];
	powerTokenIncomes: SortableIncomeDto[];
}

export interface SortIncomesActionDto extends ActionDto {
	Type: ActionType.SortIncomes;
	SortedIncomes: number[];
}

export class SortIncomesWorkflow extends ActionWorkflow {
	constructor(private readonly _powerIncomes: SortableIncomeDto[], private readonly _powerTokenIncomes: SortableIncomeDto[]) {
		super(null, true);
		this.init();
	}

	get unsortedIncomes(): SortableIncomeDto[] {
		return [...this._powerIncomes, ...this._powerTokenIncomes];
	}

	protected init(): void {
		this.states = [
			{
				id: WaitingForSorting,
				message: "You must decide how to sort power and power token incomes",
				view: ActiveView.SortIncomesDialog,
				data: {
					powers: this._powerIncomes,
					powerTokens: this._powerTokenIncomes,
				},
			},
		];
		this.currentState = first(this.states)!;
	}

	handleCommand(command: Command): ActionDto | null {
		if (this.stateId !== WaitingForSorting) {
			return null;
		}

		switch (command.nextState) {
			case CommonWorkflowStates.PERFORM_ACTION:
				const sortedIncomes = command.data as number[];
				const action: SortIncomesActionDto = {
					Type: ActionType.SortIncomes,
					SortedIncomes: sortedIncomes,
				};
				return action;
			default:
				throw new Error(`State ${command.nextState} not handled.`);
		}
	}
}
