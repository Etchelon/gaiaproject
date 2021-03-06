import Tooltip from "@material-ui/core/Tooltip";
import _ from "lodash";
import { useSelector } from "react-redux";
import GaiaMarker from "../../../assets/Resources/GaiaMarker.png";
import IvitsSpaceStation from "../../../assets/Resources/Markers/SpaceStation.png";
import { BuildingType, PlanetType, Race } from "../../../dto/enums";
import { HexDto } from "../../../dto/interfaces";
import { smartMemoize } from "../../../utils/miscellanea";
import { selectHexInteractionState } from "../../store/active-game.slice";
import { useWorkflow } from "../../WorkflowContext";
import { InteractiveElementType } from "../../workflows/enums";
import { hexHeight, hexSide } from "../shape-utils";
import Building from "./Building";
import useStyles from "./hex.styles";
import LostPlanet from "./LostPlanet";
import Satellite from "./Satellite";

interface HexProps {
	hex: HexDto;
	width: number;
}

const getHexDimensions = smartMemoize((width: number) => {
	const height = hexHeight(width);
	const side = hexSide(width);
	return { height, side };
});

const hexCorners = smartMemoize((width: number, height: number, side: number) => {
	const points = [
		{ x: (width - side) / 2, y: 0 },
		{ x: (width + side) / 2, y: 0 },
		{ x: width, y: height / 2 },
		{ x: (width + side) / 2, y: height },
		{ x: (width - side) / 2, y: height },
		{ x: 0, y: height / 2 },
	];
	return _.chain(points)
		.map(p => `${p.x},${p.y}`)
		.value()
		.join(" ");
});

const Hex = ({ hex, width }: HexProps) => {
	const { height, side } = getHexDimensions(width);
	const classes = useStyles({ index: hex.index, width, height, side });
	const { isClickable, isSelected, notes } = useSelector(selectHexInteractionState(hex.id));
	const { activeWorkflow } = useWorkflow();
	const hexClicked = isClickable
		? () => {
				activeWorkflow?.elementSelected(hex.id, InteractiveElementType.Hex);
		  }
		: _.noop;

	const hasGaiaMarker = hex.planetType === PlanetType.Transdim && hex.wasGaiaformed;
	const hasLostPlanet = hex.planetType === PlanetType.LostPlanet;
	const hasIvitsSpaceStation = !!hex.ivitsSpaceStation;
	const building = hex.building;
	const hasBuilding = !_.isNil(building);
	const lantidsMine = hex.lantidsParasiteBuilding;
	const hasLantidsMine = !_.isNil(lantidsMine);
	const hasSatellites = !_.isEmpty(hex.satellites);

	const ret = (
		<div className={classes.hex}>
			{hasGaiaMarker && <img className={classes.gaiaMarker} src={GaiaMarker} alt="" />}
			{hasIvitsSpaceStation && <img className={classes.gaiaMarker} src={IvitsSpaceStation} alt="" />}
			{hasLostPlanet && (
				<div className={classes.building} style={{ width, height }}>
					<LostPlanet width={width} height={height} raceId={building.raceId} />
				</div>
			)}
			{hasBuilding && building.type !== BuildingType.LostPlanet && (
				<div className={classes.building} style={{ width, height }}>
					<Building raceId={building.raceId} type={building.type} onMap={true} />
				</div>
			)}
			{hasLantidsMine && (
				<div className={classes.lantidsMine} style={{ width, height }}>
					<Building raceId={Race.Lantids} type={BuildingType.Mine} onMap={true} />
				</div>
			)}
			{hasSatellites && (
				<div className={classes.satellites}>
					{_.map(hex.satellites, satellite => (
						<Satellite key={satellite.raceId} raceId={satellite.raceId} width={width / 4} />
					))}
				</div>
			)}
			{(isClickable || isSelected) && <div className={classes.selector + (isSelected ? " selected" : notes ? " with-qic" : "")}></div>}
			<svg className={classes.clicker}>
				<polygon className={isClickable ? "clickable" : ""} points={hexCorners(width, height, side)} onClick={hexClicked} />
			</svg>
		</div>
	);
	return notes ? <Tooltip title={notes}>{ret}</Tooltip> : ret;
};

export default Hex;
