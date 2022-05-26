<script lang="ts">
	import ListItem from "$components/list/ListItem.svelte";
	import ListItemAvatar from "$components/list/ListItemAvatar.svelte";
	import type { GameLogDto } from "$dto/interfaces";
	import { assetUrl } from "$utils/miscellanea";
	import { getRaceImage } from "$utils/race-utils";

	export let log: GameLogDto;
	export let canRollback: boolean;
	export let doRollback: (actionId: number) => void;

	$: imgUrl = assetUrl(`Races/${getRaceImage(log.race)}`);
</script>

<ListItem>
	<ListItemAvatar src={imgUrl} />
	<!-- <ListItemText
		style={{ margin: theme.spacing(0) }}
		primary={log.message}
		primaryTypographyProps={{ class: `${styles.mainLog} ${"gaia-font"}` }}
		secondary={some(log.subLogs) && subLogs()}
		secondaryTypographyProps={{ component: "div" }}
	/>
	{#if canRollback}
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

<style lang="scss">
	.playerLog {
		padding: 4px 8px !important;
		position: relative;

		.mainLog {
			font-size: 0.75rem;
		}

		.rollbackButton {
			display: none;
			position: absolute;
			top: 5px;
			right: 5px;
			cursor: pointer;
		}

		&:hover .rollbackButton {
			display: block;
		}
	}
</style>
