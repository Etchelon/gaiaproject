import Typography from "@mui/material/Typography";
import TextareaAutosize from "@mui/material/TextareaAutosize";
import CircularProgress from "@mui/material/CircularProgress";
import DoneIcon from "@mui/icons-material/Done";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";
import Grid from "@mui/material/Grid";
import { ChangeEvent, useEffect, useRef, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { fetchNotes, saveNotes, selectPlayerNotes, selectSaveNotesProgress } from "../store/notes-thunks";
import useStyles from "./player-config.styles";
import _ from "lodash";
import { saveNotesFeedbackDisplayed } from "../store/active-game.slice";

interface PlayerConfigProps {
	gameId: string;
}

const PlayerConfig = ({ gameId }: PlayerConfigProps) => {
	const classes = useStyles();
	const dispatch = useDispatch();
	const notes = useSelector(selectPlayerNotes);
	const [tempNotes, setTempNotes] = useState("");
	const saveNotesProgress = useSelector(selectSaveNotesProgress);
	const debouncedSave = useRef(_.debounce((notes_: string) => dispatch(saveNotes({ gameId, notes: notes_ })), 2000));
	const onNotesChanged = (evt: ChangeEvent<HTMLTextAreaElement>) => {
		const notes_ = evt.target.value;
		setTempNotes(notes_);
		debouncedSave.current(notes_);
	};

	// When notes are loaded or updated
	useEffect(() => {
		setTempNotes(notes);
	}, [notes]);

	useEffect(() => {
		switch (saveNotesProgress) {
			case "idle":
				return;
			default:
				_.delay(() => dispatch(saveNotesFeedbackDisplayed()), 5000);
				return;
		}
	}, [saveNotesProgress]);

	useEffect(() => {
		dispatch(fetchNotes(gameId));
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

export default PlayerConfig;
