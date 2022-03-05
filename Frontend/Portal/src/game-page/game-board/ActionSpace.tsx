import WarningIcon from "@mui/icons-material/Warning";
import Tooltip from "@mui/material/Tooltip";
import createStyles from "@mui/styles/createStyles";
import makeStyles from "@mui/styles/makeStyles";
import { noop } from "lodash";
import { useRef } from "react";
import { ActionSpaceDto } from "../../dto/interfaces";
import { useContainerDimensions } from "../../utils/hooks";
import { interactiveElementClass } from "../../utils/miscellanea";
import { useGamePageContext } from "../GamePage.context";
import { selectActionSpaceInteractionState } from "../store/selectors";
import { useWorkflow } from "../WorkflowContext";
import { InteractiveElementType } from "../workflows/enums";
import ActionToken from "./ActionToken";

interface ActionSpaceProps {
	space: ActionSpaceDto;
}

const useStyles = makeStyles(() =>
	createStyles({
		root: {
			display: "flex",
			alignItems: "center",
			justifyContent: "center",
			height: "100%",
			position: "relative",
		},
		token: {
			width: "83%",
			position: "absolute",
			bottom: "6%",
			left: "8%",
		},
		warning: {
			position: "absolute",
			bottom: 5,
			right: 5,
		},
	})
);

const ActionSpace = ({ space }: ActionSpaceProps) => {
	const ref = useRef<HTMLDivElement>(null);
	const { vm } = useGamePageContext();
	const { width } = useContainerDimensions(ref);
	const classes = useStyles({ width, height: 0 });
	const elementType =
		space.kind === "power"
			? InteractiveElementType.PowerAction
			: space.kind === "qic"
			? InteractiveElementType.QicAction
			: space.kind === "planetary-institute"
			? InteractiveElementType.PlanetaryInstitute
			: space.kind === "right-academy"
			? InteractiveElementType.RightAcademy
			: InteractiveElementType.RaceAction;
	const { isClickable, isSelected, notes } = selectActionSpaceInteractionState(elementType)(space.type)(vm);
	const { activeWorkflow } = useWorkflow();
	const actionSpaceClicked = isClickable
		? () => {
				activeWorkflow?.elementSelected(space.type, elementType);
		  }
		: noop;

	return (
		<div ref={ref} className={classes.root}>
			{!space.isAvailable && (
				<div className={classes.token}>
					<ActionToken />
				</div>
			)}
			<div className={interactiveElementClass(isClickable, isSelected)} onClick={actionSpaceClicked}></div>
			{notes && (
				<Tooltip title={notes}>
					<div className={classes.warning}>
						<WarningIcon />
					</div>
				</Tooltip>
			)}
		</div>
	);
};

export default ActionSpace;
