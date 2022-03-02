import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import _ from "lodash";
import { useSelector } from "react-redux";
import { FederationTokenType } from "../../../dto/enums";
import { useAssetUrl } from "../../../utils/hooks";
import { fillParentAbs, interactiveElementClass, withAspectRatioW } from "../../../utils/miscellanea";
import { selectOwnFederationTokenInteractionState } from "../../store/active-game.slice";
import { useWorkflow } from "../../WorkflowContext";
import { InteractiveElementType } from "../../workflows/enums";

const WIDTH_TO_HEIGHT_RATIO = 0.816;
const federationTokenImages = new Map<FederationTokenType, string>([
	[FederationTokenType.Knowledge, "FEDknw"],
	[FederationTokenType.Credits, "FEDcre"],
	[FederationTokenType.Ores, "FEDore"],
	[FederationTokenType.PowerTokens, "FEDpwt"],
	[FederationTokenType.Qic, "FEDqic"],
	[FederationTokenType.Points, "FEDvps"],
	[FederationTokenType.Gleens, "FEDgle"],
]);

interface FederationTokenProps {
	type: FederationTokenType;
	playerId?: string;
	used?: boolean;
}

const useStyles = makeStyles(() =>
	createStyles({
		root: {
			...withAspectRatioW(WIDTH_TO_HEIGHT_RATIO),
			"&.used": {
				opacity: 0.3,
			},
		},
		token: {
			...fillParentAbs,
			objectFit: "cover",
		},
	})
);

const FederationToken = ({ type, used, playerId }: FederationTokenProps) => {
	const classes = useStyles();
	const imgUrl = useAssetUrl(`Boards/Federations/${federationTokenImages.get(type)}.png`);
	const inPlayerArea = !_.isUndefined(playerId);
	const { isClickable, isSelected } = useSelector(_.partialRight(selectOwnFederationTokenInteractionState(type), playerId));
	const { activeWorkflow } = useWorkflow();
	const tokenClicked = isClickable
		? () => {
				activeWorkflow?.elementSelected(type, InteractiveElementType.OwnFederationToken);
		  }
		: _.noop;

	return (
		<div className={classes.root + (used ? " used" : "")}>
			<img className={classes.token} src={imgUrl} alt="" />
			{inPlayerArea && <div className={interactiveElementClass(isClickable, isSelected)} onClick={tokenClicked}></div>}
		</div>
	);
};

export default FederationToken;
