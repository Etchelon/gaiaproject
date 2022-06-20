<script lang="ts">
	import LoadingSpinner from "$components/LoadingSpinner.svelte";
	import Page from "$components/Page.svelte";
	import { isNil } from "lodash";
	import type { Subscription } from "rxjs";
	import { onMount } from "svelte";
	import { push } from "svelte-spa-router";
	import { get, writable } from "svelte/store";
	import { getAppContext } from "../app/App.context";
	import { IGamePageContext, setGamePageContext } from "./GamePage.context";
	import GamePage from "./GamePage.svelte";
	import { GamePageSignalRConnectionService } from "./GamePageSignalRConnection.service";
	import { GamePageStore } from "./store/GamePage.store";
	import type { ActionWorkflow } from "./workflows/action-workflow.base";
	import { fromAction } from "./workflows/utils";

	export let params: { id: string };

	const { id } = params;
	const {
		http,
		hub,
		auth: { user },
	} = getAppContext();

	const store = new GamePageStore(http);

	//#region Workflow management

	let activeWorkflowSub: Subscription | null = null;
	const activeWorkflow = writable<ActionWorkflow | null>(null);

	const startWorkflow = (workflow: ActionWorkflow) => {
		const sub = workflow.currentState$.subscribe(state => {
			store.setStatusFromWorkflow(state);
		});
		sub.add(
			workflow.switchToAction$.subscribe(actionType => {
				closeWorkflow();

				if (isNil(actionType)) {
					store.setWaitingForAction();
					return;
				}

				const { availableActions, currentPlayer, game } = store;
				const action = get(availableActions).find(act => act.type === actionType)!;
				const newWorkflow = fromAction(get(currentPlayer)!.id, get(game), action, store);
				startWorkflow(newWorkflow);
			})
		);

		activeWorkflow.set(workflow);
		activeWorkflowSub = sub;
	};

	const closeWorkflow = () => {
		activeWorkflowSub?.unsubscribe();
		activeWorkflowSub = null;
		activeWorkflow.set(null);
	};

	//#endregion

	const signalR = new GamePageSignalRConnectionService(hub, store, closeWorkflow);

	const ctx: IGamePageContext = {
		id,
		store,
		signalR,
		activeWorkflow,
		startWorkflow,
		closeWorkflow,
	};
	setGamePageContext(ctx);

	let gameName$: Promise<string> | null = null;
	let pageTitle = "Loading...";
	onMount(async () => {
		try {
			gameName$ = store.loadGame(id).then(name => {
				pageTitle = name;
				return name;
			});
		} catch {
			console.error(`Failed to load game ${id}.`);
			push("#/not-found");
		}
	});

	$: {
		$user && store.setCurrentUser($user.id);
	}
</script>

{#if gameName$}
	<Page title={pageTitle}>
		{#await gameName$}
			<LoadingSpinner />
		{:then}
			<GamePage />
		{/await}
	</Page>
{/if}
