import { createStyles, makeStyles } from "@material-ui/core/styles";
import _ from "lodash";
import { useSelector } from "react-redux";
import { FederationTokenStackDto } from "../../../dto/interfaces";
import { fillParentAbs, interactiveBorder, interactiveElementClass, withAspectRatioW } from "../../../utils/miscellanea";
import { selectFederationTokenStackInteractionState } from "../../store/active-game.slice";
import { useWorkflow } from "../../WorkflowContext";
import { InteractiveElementType } from "../../workflows/enums";
import FederationToken from "./FederationToken";

const HEIGHT_TO_WIDTH_RATIO = 1.05;

interface FederationTokenStackProps {
	stack: FederationTokenStackDto;
}

const useStyles = makeStyles(() =>
	createStyles({
		root: {
			...withAspectRatioW(1 / HEIGHT_TO_WIDTH_RATIO),
			...interactiveBorder,
		},
		wrapper: {
			...fillParentAbs,
		},
		token: {
			position: "absolute",
			width: "100%",
		},
	})
);

const FederationTokenStack = ({ stack }: FederationTokenStackProps) => {
	const classes = useStyles();
	const { isClickable, isSelected } = useSelector(selectFederationTokenStackInteractionState(stack.type));
	const { activeWorkflow } = useWorkflow();
	const tokenClicked = isClickable
		? () => {
				activeWorkflow?.elementSelected(stack.type, InteractiveElementType.FederationToken);
		  }
		: _.noop;

	return (
		<div className={classes.root}>
			<div className={classes.wrapper}>
				{_.map(_.range(0, stack.remaining), n => (
					<div key={n} className={classes.token} style={{ width: "70%", top: `${n * 12}%`, left: `${n * 15}%` }}>
						<FederationToken type={stack.type} />
					</div>
				))}
			</div>
			<div className={interactiveElementClass(isClickable, isSelected)} onClick={tokenClicked}></div>
		</div>
	);
};

export default FederationTokenStack;
