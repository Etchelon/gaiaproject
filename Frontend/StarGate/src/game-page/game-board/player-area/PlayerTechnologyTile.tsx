import { createStyles, makeStyles } from "@material-ui/core/styles";
import _ from "lodash";
import { useSelector } from "react-redux";
import { TechnologyTileDto } from "../../../dto/interfaces";
import { interactiveElementClass } from "../../../utils/miscellanea";
import { selectOwnAdvancedTileInteractionState, selectOwnStandardTileInteractionState } from "../../store/active-game.slice";
import { useWorkflow } from "../../WorkflowContext";
import { InteractiveElementType } from "../../workflows/enums";
import ActionToken from "../ActionToken";
import AdvancedTechnologyTile from "../AdvancedTechnologyTile";
import StandardTechnologyTile from "../StandardTechnologyTile";

interface PlayerTechnologyTileProps {
	tile: TechnologyTileDto;
	playerId: string;
}

const useStyles = makeStyles(() =>
	createStyles({
		root: {
			width: "100%",
			position: "relative",
		},
		advancedTile: {
			position: "absolute",
			width: "82.5%",
			top: "8%",
			left: "5%",
		},
		actionToken: {
			position: "absolute",
			width: "50%",
			top: "17%",
			left: "25%",
			"&.advanced": {
				top: "12%",
				left: "16%",
			},
		},
	})
);

const PlayerTechnologyTile = ({ tile, playerId }: PlayerTechnologyTileProps) => {
	const classes = useStyles();
	const covered = !_.isNil(tile.coveredByAdvancedTile);
	const tileId = covered ? tile.coveredByAdvancedTile! : tile.id;
	const selector = covered ? selectOwnAdvancedTileInteractionState(tile.coveredByAdvancedTile!) : selectOwnStandardTileInteractionState(tile.id);
	const { isClickable, isSelected } = useSelector(_.partialRight(selector, playerId));
	const { activeWorkflow } = useWorkflow();
	const tileClicked = isClickable
		? () => {
				activeWorkflow?.elementSelected(tileId, covered ? InteractiveElementType.OwnAdvancedTile : InteractiveElementType.OwnStandardTile);
		  }
		: _.noop;

	return (
		<div className={classes.root}>
			<StandardTechnologyTile type={tile.id} />
			{covered && (
				<div className={classes.advancedTile}>
					<AdvancedTechnologyTile type={tile.coveredByAdvancedTile!} />
				</div>
			)}
			<div className={classes.actionToken + (covered ? " advanced" : "")}>{tile.used && <ActionToken />}</div>
			<div className={interactiveElementClass(isClickable, isSelected)} onClick={tileClicked}></div>
		</div>
	);
};

export default PlayerTechnologyTile;
