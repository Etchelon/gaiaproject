import CheckBoxRounded from "@mui/icons-material/CheckBoxRounded";
import { Theme, Typography, useTheme } from "@mui/material";
import Avatar from "@mui/material/Avatar";
import Button from "@mui/material/Button";
import { isNil, isNull } from "lodash";
import { Fragment, useState } from "react";
import { Race } from "../../../dto/enums";
import { useAssetUrl } from "../../../utils/hooks";
import { Nullable } from "../../../utils/miscellanea";
import { getRaceBoard, getRaceColor, getRaceImage } from "../../../utils/race-utils";
import { useGamePageContext } from "../../GamePage.context";
import { useWorkflow } from "../../WorkflowContext";
import { SelectRaceWorkflow } from "../../workflows/setup-phase/select-race.workflow";
import { CommonWorkflowStates } from "../../workflows/types";
import useStyles from "./select-race-dialog.styles";

const SelectableRaceAvatar = ({ race, selected, onSelected, theme }: { race: Race; selected: boolean; onSelected(race: Race): void; theme: Theme }) => {
	const imgUrl = useAssetUrl(`Races/${getRaceImage(race)}`);
	const background = getRaceColor(race);
	const color = theme.palette.getContrastText(background);
	return (
		<div onClick={() => onSelected(race)} style={{ position: "relative", padding: theme.spacing(1), backgroundColor: background, color, cursor: "pointer" }}>
			<Avatar src={imgUrl} />
			{selected && (
				<div style={{ position: "absolute", bottom: -3, right: 0 }}>
					<CheckBoxRounded />
				</div>
			)}
		</div>
	);
};

const boardVersion = "rework";

const RaceBoard = ({ race }: { race: Race }) => {
	const imgUrl = useAssetUrl(`Races/Boards_${boardVersion}/${getRaceBoard(race)}`);
	return <img style={{ width: "100%" }} src={imgUrl} alt="" />;
};

interface SelectRaceDialogProps {
	gameId: string;
}

const SelectRaceDialog = ({ gameId }: SelectRaceDialogProps) => {
	const theme = useTheme();
	const classes = useStyles();
	const { vm } = useGamePageContext();
	const [isSelecting, setIsSelecting] = useState(false);
	const [selectedRace, setSelectedRace] = useState<Nullable<Race>>(null);
	const { activeWorkflow } = useWorkflow();
	const availableRaces = (activeWorkflow as SelectRaceWorkflow)?.availableRaces ?? [];

	const closeDialog = () => {
		activeWorkflow!.handleCommand({ nextState: CommonWorkflowStates.CANCEL });
	};
	const confirmSelection = () => {
		const action = activeWorkflow!.handleCommand({ nextState: CommonWorkflowStates.PERFORM_ACTION, data: selectedRace })!;
		setIsSelecting(true);
		vm.executePlayerAction(gameId, action);
	};

	return (
		<div className={classes.root}>
			<Typography variant="h6" className={classes.header + " gaia-font text-center"}>
				Select a race
			</Typography>
			<div className={classes.raceList}>
				{availableRaces.map(race => (
					<Fragment key={race}>
						<SelectableRaceAvatar race={race} selected={race === selectedRace} onSelected={setSelectedRace} theme={theme} />
						<div className={classes.spacer}></div>
					</Fragment>
				))}
			</div>
			<div className={classes.raceBoard}>
				{isNull(selectedRace) ? <Typography variant="h5">Select a race to view its board</Typography> : <RaceBoard race={selectedRace} />}
			</div>
			<div className={classes.commands}>
				<Button variant="contained" className="command" onClick={closeDialog}>
					<span className="gaia-font">Close</span>
				</Button>
				<Button variant="contained" color="primary" className="command" disabled={isNil(selectedRace) || isSelecting} onClick={confirmSelection}>
					<span className="gaia-font">Confirm</span>
				</Button>
			</div>
		</div>
	);
};

export default SelectRaceDialog;
