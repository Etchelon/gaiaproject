import { createContext, useContext } from "react";
import { ActionWorkflow } from "./workflows/action-workflow.base";

export interface IWorkflowContext {
	activeWorkflow: ActionWorkflow | null;
	startWorkflow(workflow: ActionWorkflow): void;
	closeWorkflow(): void;
}

export const WorkflowContext = createContext<IWorkflowContext>({
	activeWorkflow: null,
	startWorkflow() {},
	closeWorkflow() {},
});

export function useWorkflow() {
	return useContext(WorkflowContext);
}
