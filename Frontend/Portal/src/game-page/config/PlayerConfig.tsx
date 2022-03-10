import DoneIcon from "@mui/icons-material/Done";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import CircularProgress from "@mui/material/CircularProgress";
import Grid from "@mui/material/Grid";
import TextareaAutosize from "@mui/material/TextareaAutosize";
import Typography from "@mui/material/Typography";
import { debounce, delay } from "lodash";
import { observer } from "mobx-react";
import { ChangeEvent, useEffect, useRef, useState } from "react";
import { useGamePageContext } from "../GamePage.context";
import useStyles from "./player-config.styles";

interface PlayerConfigProps {
	gameId: string;
}

const PlayerConfig = ({ gameId }: PlayerConfigProps) => {
	const classes = useStyles();
	const { vm } = useGamePageContext();
	const [tempNotes, setTempNotes] = useState("");
	const notes = vm.playerNotes;
	const saveNotesProgress = vm.saveNotesProgress;
	const debouncedSave = useRef(debounce((notes_: string) => vm.saveNotes(gameId, notes_), 2000));
	const onNotesChanged = (evt: ChangeEvent<HTMLTextAreaElement>) => {
		const notes_ = evt.target.value;
		setTempNotes(notes_);
		debouncedSave.current(notes_);
	};

	// When notes are loaded or updated
	useEffect(() => {
		setTempNotes(notes ?? "");
	}, [notes]);

	useEffect(() => {
		switch (saveNotesProgress) {
			case "idle":
				return;
			default:
				delay(() => vm.saveNotesFeedbackDisplayed(), 5000);
				return;
		}
	}, [saveNotesProgress]);

	useEffect(() => {
		vm.fetchNotes(gameId);
	}, []);

	return (
		<div className={classes.root}>
			<Grid container>
				<Grid item xs={12} md={6}>
					<div className={classes.notesHeader}>
						<Typography variant="h5" className="gaia-font">
							Notes
						</Typography>
						<div className="spacer"></div>
						{saveNotesProgress === "loading" && <CircularProgress size={20} color="primary" />}
						{saveNotesProgress === "success" && <DoneIcon color="primary" />}
						{saveNotesProgress === "failure" && <ErrorOutlineIcon color="error" />}
					</div>
					<TextareaAutosize
						className={classes.notes + " gaia-font"}
						minRows={10}
						placeholder="Use this area to write down your notes about the game..."
						value={tempNotes}
						onChange={onNotesChanged}
					/>
				</Grid>
			</Grid>
		</div>
	);
};

export default observer(PlayerConfig);
