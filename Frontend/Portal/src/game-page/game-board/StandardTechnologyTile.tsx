import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import _ from "lodash";
import { useSelector } from "react-redux";
import { StandardTechnologyTileType } from "../../dto/enums";
import { useAssetUrl } from "../../utils/hooks";
import { fillParentAbs, interactiveElementClass, withAspectRatioW } from "../../utils/miscellanea";
import { selectStandardTileInteractionState } from "../store/active-game.slice";
import { useWorkflow } from "../WorkflowContext";
import { InteractiveElementType } from "../workflows/enums";

const WIDTH_TO_HEIGHT_RATIO = 1.328;
const standardTileImages = new Map<StandardTechnologyTileType, string>([
	[StandardTechnologyTileType.ActionGain4Power, "TECpow"],
	[StandardTechnologyTileType.Immediate1Ore1Qic, "TECqic"],
	[StandardTechnologyTileType.Immediate1KnowledgePerPlanetType, "TECtyp"],
	[StandardTechnologyTileType.Immediate7Points, "TECvps"],
	[StandardTechnologyTileType.Income1Ore1Power, "TECore"],
	[StandardTechnologyTileType.Income1Knowledge1Coin, "TECknw"],
	[StandardTechnologyTileType.Income4Coins, "TECcre"],
	[StandardTechnologyTileType.PassiveBigBuildingsWorth4Power, "TECpia"],
	[StandardTechnologyTileType.Passive3PointsPerGaiaPlanet, "TECgai"],
]);

interface StandardTechnologyTileProps {
	type: StandardTechnologyTileType;
}

const useStyles = makeStyles(() =>
	createStyles({
		root: {
			...withAspectRatioW(WIDTH_TO_HEIGHT_RATIO),
		},
		tile: {
			...fillParentAbs,
		},
	})
);

const StandardTechnologyTile = ({ type }: StandardTechnologyTileProps) => {
	const classes = useStyles();
	const imgUrl = useAssetUrl(`Boards/TechTiles/${standardTileImages.get(type)}.png`);
	const { isClickable, isSelected } = useSelector(selectStandardTileInteractionState(type));
	const { activeWorkflow } = useWorkflow();
	const tileClicked = isClickable
		? () => {
				activeWorkflow?.elementSelected(type, InteractiveElementType.StandardTile);
		  }
		: _.noop;

	return (
		<div className={classes.root}>
			<img className={classes.tile} src={imgUrl} alt="" />
			<div className={interactiveElementClass(isClickable, isSelected)} onClick={tileClicked}></div>
		</div>
	);
};

export default StandardTechnologyTile;
