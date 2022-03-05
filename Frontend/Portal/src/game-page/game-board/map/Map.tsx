import { isNil } from "lodash";
import { MapShape } from "../../../dto/enums";
import { MapDto } from "../../../dto/interfaces";
import { smartMemoize } from "../../../utils/miscellanea";
import Sector from "../sector/Sector";
import { hexHeight as hexHeightFromWidth, hexWidth as hexWidthFromHeight } from "../shape-utils";
import useStyles from "./map.styles";

const tileSpacings = {
	verticalSpacing: 5,
	horizontalSpacing: 5,
};

//#region Sizing

const getSectorXOffset = smartMemoize((sectorNumber: number, shape: MapShape): number => {
	switch (shape) {
		default:
			throw new Error(`Shape ${shape} not handled`);
		case 1:
		case 2:
			return (sectorNumber < 2 ? sectorNumber + 1 : sectorNumber < 5 ? (sectorNumber - 2) * 1.25 : sectorNumber - 5 + 1) * tileSpacings.horizontalSpacing;
		case 3:
			return (sectorNumber < 3 ? sectorNumber : sectorNumber < 5 ? sectorNumber - 3 + 0.5 : sectorNumber - 5) * tileSpacings.horizontalSpacing;
		case 4:
			return (sectorNumber < 3 ? sectorNumber + 1 : sectorNumber < 7 ? (sectorNumber - 3) * 1.25 : sectorNumber - 7 + 1) * tileSpacings.horizontalSpacing;
	}
});

const getSectorYOffset = smartMemoize((sectorNumber: number, shape: MapShape): number => {
	switch (shape) {
		default:
			throw new Error(`Shape ${shape} not handled`);
		case 1:
		case 2:
			return (sectorNumber < 2 ? 0 : sectorNumber < 5 ? 1 : 2) * tileSpacings.verticalSpacing;
		case 3:
			return (sectorNumber < 3 ? 0 : sectorNumber < 5 ? 1 : 2) * tileSpacings.verticalSpacing;
		case 4:
			return (sectorNumber < 3 ? 0 : sectorNumber < 7 ? 1 : 2) * tileSpacings.verticalSpacing;
	}
});

const getHexWidth = smartMemoize((mapWidth: number, shape: MapShape) => {
	switch (shape) {
		default:
			throw new Error(`Map shape ${shape} not yet supported.`);
		case 1:
		case 2:
			// 9 hw + 6 hs - 2hsp
			// 9 hw + 6 (hw * sin pi/6) - hw (1 - sin pi/6)
			// (9 - 1) hw + (6 + 1) * sin pi/6 hw
			// hw (8 + 7 sin pi/6)
			return mapWidth / (8 + 7 * Math.sin(Math.PI / 6));
		case 3:
			// 10 hw + 6 hs - 3hsp
			// 10 hw + 6 (hw * sin pi/6) - 3/2hw (1 - sin pi/6)
			// (10 - 3/2) hw + (6 + 3/2) * sin pi/6 hw
			// hw (8.5 + 7.5 sin pi/6)
			return mapWidth / (8.5 + 7.5 * Math.sin(Math.PI / 6));
		case 4:
			// 12 hw + 8 hs - 3hsp
			// 12 hw + 8 (hw * sin pi/6) - 3/2hw (1 - sin pi/6)
			// (12 - 3/2) hw + (8 + 3/2) * sin pi/6 hw
			// hw (10.5 + 9.5 sin pi/6)
			return mapWidth / (10.5 + 9.5 * Math.sin(Math.PI / 6));
	}
});

const getHexHeight = smartMemoize((mapHeight: number, shape: MapShape) => {
	switch (shape) {
		default:
			throw new Error(`Map shape ${shape} not yet supported.`);
		case 1:
		case 2:
			return mapHeight / 13;
		case 3:
		case 4:
			return mapHeight / 13.5;
	}
});

interface HexDimensions {
	hexWidth: number;
	hexHeight: number;
	mapWidth: number;
	mapHeight: number;
}

const calculateDimensionsFromHeight = smartMemoize((parentHeight: number, shape: MapShape): HexDimensions => {
	const totalYSpacing = 2 * tileSpacings.verticalSpacing;
	const mapHeight = parentHeight;
	const hexHeight = getHexHeight(mapHeight - totalYSpacing, shape);
	const hexWidth_ = hexWidthFromHeight(hexHeight);
	let mapWidth = 0;
	let totalXSpacing = 0;
	switch (shape) {
		default:
			throw new Error(`Map shape ${shape} not yet supported.`);
		case 1:
		case 2:
			totalXSpacing = 3 * tileSpacings.horizontalSpacing;
			mapWidth = hexWidth_ * (8 + 7 * Math.sin(Math.PI / 6)) + totalXSpacing;
			break;
		case 3:
			totalXSpacing = 3 * tileSpacings.horizontalSpacing;
			mapWidth = hexWidth_ * (8.5 + 7.5 * Math.sin(Math.PI / 6)) + totalXSpacing;
			break;
		case 4:
			totalXSpacing = 4 * tileSpacings.horizontalSpacing;
			mapWidth = hexWidth_ * (10.5 + 9.5 * Math.sin(Math.PI / 6)) + totalXSpacing;
			break;
	}
	return { hexWidth: hexWidth_, hexHeight, mapWidth, mapHeight };
});

const calculateDimensionsFromWidth = smartMemoize((parentWidth: number, shape: MapShape) => {
	const totalXSpacing = (shape < 4 ? 3 : 4) * tileSpacings.horizontalSpacing;
	const mapWidth = parentWidth;
	const hexWidth_ = getHexWidth(parentWidth - totalXSpacing, shape);
	const hexHeight_ = hexHeightFromWidth(hexWidth_);
	let n = 0;
	switch (shape) {
		default:
			throw new Error(`Map shape ${shape} not yet supported.`);
		case 1:
		case 2:
			n = 13;
			break;
		case 3:
		case 4:
			n = 13.5;
			break;
	}
	const mapHeight = n * hexHeight_ + 2 * tileSpacings.verticalSpacing;
	return { hexWidth: hexWidth_, hexHeight: hexHeight_, mapWidth, mapHeight };
});

//#endregion

interface MapProps {
	map: MapDto;
	width: number;
	height?: number;
}

const Map = ({ map, width, height }: MapProps) => {
	const shape = map.shape;
	// width = Math.max(width, 600);
	height = isNil(height) ? 450 : Math.max(height, 450);
	const dimensions = isNil(height) ? calculateDimensionsFromWidth(width, shape) : calculateDimensionsFromHeight(height - 4, shape);
	const actualWidth = dimensions.mapWidth;
	const actualHeight = dimensions.mapHeight;
	const classes = useStyles({ mapWidth: actualWidth, mapHeight: actualHeight });

	return (
		<div className={classes.map}>
			{map.sectors.map(sector => {
				const props = {
					sector,
					hexWidth: dimensions.hexWidth,
					hexHeight: dimensions.hexHeight,
					xOffset: getSectorXOffset(sector.number, shape),
					yOffset: getSectorYOffset(sector.number, shape),
				};
				return <Sector key={sector.id} {...props} />;
			})}
		</div>
	);
};

export default Map;
