import createStyles from "@mui/styles/createStyles";
import makeStyles from "@mui/styles/makeStyles";
import { noop, range } from "lodash";
import { observer } from "mobx-react";
import { FederationTokenStackDto } from "../../../dto/interfaces";
import { fillParentAbs, interactiveBorder, interactiveElementClass, withAspectRatioW } from "../../../utils/miscellanea";
import { useGamePageContext } from "../../GamePage.context";
import { selectFederationTokenStackInteractionState } from "../../store/selectors";
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
	const { vm } = useGamePageContext();
	const { isClickable, isSelected } = selectFederationTokenStackInteractionState(stack.type)(vm);
	const { activeWorkflow } = useWorkflow();
	const tokenClicked = isClickable
		? () => {
				activeWorkflow?.elementSelected(stack.type, InteractiveElementType.FederationToken);
		  }
		: noop;

	return (
		<div className={classes.root}>
			<div className={classes.wrapper}>
				{range(0, stack.remaining).map(n => (
					<div key={n} className={classes.token} style={{ width: "70%", top: `${n * 12}%`, left: `${n * 15}%` }}>
						<FederationToken type={stack.type} />
					</div>
				))}
			</div>
			<div className={interactiveElementClass(isClickable, isSelected)} onClick={tokenClicked}></div>
		</div>
	);
};

export default observer(FederationTokenStack);
