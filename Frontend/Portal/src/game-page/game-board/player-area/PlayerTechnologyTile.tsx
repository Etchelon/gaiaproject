import { createStyles, makeStyles } from "@material-ui/core/styles";
import _ from "lodash";
import { MouseEvent } from "react";
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
			"& .advanced-tile": {
				position: "absolute",
				width: "82.5%",
				top: "8%",
				left: "5%",
			},
			"&:not(.interactive):hover .advanced-tile": {
				transform: "rotateX(90deg)",
				transition: "transform 250ms",
			},
		},
		actionToken: {
			position: "absolute",
			width: "50%",
			top: "17%",
			left: "25%",
			"&.advanced": {
				width: "64%",
				top: "5%",
				left: "10%",
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
	const tileClicked = (evt: MouseEvent) => {
		evt.stopPropagation();
		if (!isClickable) {
			return;
		}
		activeWorkflow?.elementSelected(tileId, covered ? InteractiveElementType.OwnAdvancedTile : InteractiveElementType.OwnStandardTile);
	};

	return (
		<div className={classes.root + (isClickable || isSelected ? " interactive" : "")}>
			<StandardTechnologyTile type={tile.id} />
			{!covered && <div className={classes.actionToken}>{tile.used && <ActionToken />}</div>}
			{covered && (
				<div className="advanced-tile">
					<AdvancedTechnologyTile type={tile.coveredByAdvancedTile!} />
					<div className={classes.actionToken + " advanced"}>{tile.used && <ActionToken />}</div>
				</div>
			)}
			<div className={interactiveElementClass(isClickable, isSelected)} style={{ pointerEvents: "all" }} onClick={tileClicked}></div>
		</div>
	);
};

export default PlayerTechnologyTile;
