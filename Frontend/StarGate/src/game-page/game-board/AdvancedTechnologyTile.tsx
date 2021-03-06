import { createStyles, makeStyles } from "@material-ui/core/styles";
import _ from "lodash";
import { useSelector } from "react-redux";
import { AdvancedTechnologyTileType } from "../../dto/enums";
import { useAssetUrl } from "../../utils/hooks";
import { fillParentAbs, interactiveElementClass, withAspectRatioW } from "../../utils/miscellanea";
import { selectAdvancedTileInteractionState } from "../store/active-game.slice";
import { useWorkflow } from "../WorkflowContext";
import { InteractiveElementType } from "../workflows/enums";

const WIDTH_TO_HEIGHT_RATIO = 1.328;
const advancedTileImages = new Map<AdvancedTechnologyTileType, string>([
	[AdvancedTechnologyTileType.ActionGain1Qic5Credits, "ADVqic"],
	[AdvancedTechnologyTileType.ActionGain3Ores, "ADVore"],
	[AdvancedTechnologyTileType.ActionGain3Knowledge, "ADVknw"],
	[AdvancedTechnologyTileType.Immediate2PointsPerMine, "ADVminV"],
	[AdvancedTechnologyTileType.Immediate4PointsPerTradingStation, "ADVtrsV"],
	[AdvancedTechnologyTileType.Immediate5PointsPerFederation, "ADVfedV"],
	[AdvancedTechnologyTileType.Immediate2PointsPerSector, "ADVsecV"],
	[AdvancedTechnologyTileType.Immediate2PointsPerGaiaPlanet, "ADVgai"],
	[AdvancedTechnologyTileType.Immediate1OrePerSector, "ADVsecO"],
	[AdvancedTechnologyTileType.Pass3PointsPerFederation, "ADVfedP"],
	[AdvancedTechnologyTileType.Pass3PointsPerResearchLab, "ADVlab"],
	[AdvancedTechnologyTileType.Pass1PointsPerPlanetType, "ADVtyp"],
	[AdvancedTechnologyTileType.Passive2PointsPerResearchStep, "ADVstp"],
	[AdvancedTechnologyTileType.Passive3PointsPerMine, "ADVminB"],
	[AdvancedTechnologyTileType.Passive3PointsPerTradingStation, "ADVtrsB"],
]);

interface AdvancedTechnologyTileProps {
	type: AdvancedTechnologyTileType;
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

const AdvancedTechnologyTile = ({ type }: AdvancedTechnologyTileProps) => {
	const classes = useStyles();
	const imgUrl = useAssetUrl(`Boards/TechTiles/${advancedTileImages.get(type)}.png`);
	const { isClickable, isSelected } = useSelector(selectAdvancedTileInteractionState(type));
	const { activeWorkflow } = useWorkflow();
	const tileClicked = isClickable
		? () => {
				activeWorkflow?.elementSelected(type, InteractiveElementType.AdvancedTile);
		  }
		: _.noop;

	return (
		<div className={classes.root}>
			<img className={classes.tile} src={imgUrl} alt="" />
			<div className={interactiveElementClass(isClickable, isSelected)} onClick={tileClicked}></div>
		</div>
	);
};

export default AdvancedTechnologyTile;
