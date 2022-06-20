<script lang="ts">
	import ListItem from "$components/list/ListItem.svelte";
	import ListItemAvatar from "$components/list/ListItemAvatar.svelte";
	import ListItemText from "$components/list/ListItemText.svelte";
	import type { GameLogDto } from "$dto/interfaces";
	import { assetUrl } from "$utils/miscellanea";
	import { getRaceColors, getRaceImage } from "$utils/race-utils";
	import PlayerSubLog from "./PlayerSubLog.svelte";

	export let log: GameLogDto;
	export let canRollback: boolean;
	export let doRollback: (actionId: number) => void;

	$: imgUrl = assetUrl(`Races/${getRaceImage(log.race)}`);
	$: [playerColor, textColor] = getRaceColors(log.race);
</script>

<ListItem --background-color={playerColor} --color={textColor}>
	<ListItemAvatar src={imgUrl} />
	<div class="flex-auto min-w-0">
		<ListItemText text={log.message} size="sm" />
		{#each log.subLogs as subLog}
			<PlayerSubLog {subLog} isAnotherPlayer={log.player !== subLog.player} />
		{/each}
	</div>
	<!-- {#if canRollback}
		<div class={styles.rollbackButton}>
			<IconButton aria-label="Rollback to this action" onClick={() => setIsPromptingForRollback(true)} size="large">
				<HistoryIcon style={{ color }} />
			</IconButton>
			<Dialog
				open={isPromptingForRollback}
				onClose={() => setIsPromptingForRollback(false)}
				aria-labelledby="alert-dialog-title"
				aria-describedby="alert-dialog-description"
			>
				<DialogTitle id="alert-dialog-title">Rollback game state?</DialogTitle>
				<DialogContent>
					<DialogContentText id="alert-dialog-description">The game state will be rolled back to just after the selected action was performed</DialogContentText>
				</DialogContent>
				<DialogActions>
					<Button onClick={() => setIsPromptingForRollback(false)} disabled={rollbackProgress === "loading"}>
						Cancel
					</Button>
					<ButtonWithProgress
						label={"Yes"}
						loading={rollbackProgress === "loading"}
						onClick={() => doRollback(log.actionId!)}
						disabled={rollbackProgress === "loading"}
						autoFocus
					/>
				</DialogActions>
			</Dialog>
		</div>
	{/if} -->
</ListItem>
