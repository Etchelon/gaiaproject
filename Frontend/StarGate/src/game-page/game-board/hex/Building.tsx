import { createStyles, makeStyles } from "@material-ui/core/styles";
import { BuildingType, Race } from "../../../dto/enums";
import { useAssetUrl } from "../../../utils/hooks";

interface BuildingProps {
	type: BuildingType;
	raceId: Race;
	onMap?: boolean;
}

function getBuildingImagePrefix(type: BuildingType): string {
	switch (type) {
		default:
			throw new Error(`Building type ${type} not handled.`);
		case BuildingType.PlanetaryInstitute:
			return "PI";
		case BuildingType.AcademyLeft:
		case BuildingType.AcademyRight:
			return "AC";
		case BuildingType.ResearchLab:
			return "RL";
		case BuildingType.TradingStation:
			return "TS";
		case BuildingType.Gaiaformer:
			return "GF";
		case BuildingType.Mine:
			return "MI";
	}
}

function getBuildingScale(type: BuildingType, onMap = false): number {
	switch (type) {
		default:
			return 0;
		case BuildingType.PlanetaryInstitute:
			return onMap ? 0.9 : 1;
		case BuildingType.AcademyLeft:
		case BuildingType.AcademyRight:
			return onMap ? 0.75 : 0.9;
		case BuildingType.ResearchLab:
		case BuildingType.TradingStation:
			return onMap ? 1.15 : 1.5;
		case BuildingType.Gaiaformer:
		case BuildingType.Mine:
			return onMap ? 1.5 : 2;
	}
}

function getBuildingColor(raceId: Race): string {
	switch (raceId) {
		default:
			return "";
		case Race.Terrans:
		case Race.Lantids:
			return "blue";
		case Race.Taklons:
		case Race.Ambas:
			return "brown";
		case Race.Gleens:
		case Race.Xenos:
			return "yellow";
		case Race.Ivits:
		case Race.HadschHallas:
			return "red";
		case Race.Bescods:
		case Race.Firaks:
			return "grey";
		case Race.Geodens:
		case Race.BalTaks:
			return "orange";
		case Race.Nevlas:
		case Race.Itars:
			return "white";
	}
}

function getBuildingFolderSuffix(style: string, onPlayerBoard = false) {
	if (onPlayerBoard) {
		return "";
	}

	switch (style) {
		default:
		case "vanilla":
		case "vanilla_shadowed":
			return "";
		case "mono":
			return "_map_white";
		case "sandwich":
			return "_map";
	}
}

const useStyles = makeStyles(() =>
	createStyles({
		root: {
			display: "flex",
			alignItems: "center",
			justifyContent: "center",
			height: "100%",
			"& .building": {
				height: ({ type, onMap }: BuildingProps) => `${100 * getBuildingScale(type, onMap)}%`,
				objectFit: "cover",
				pointerEvents: "none",
			},
			"&.on-player-board:hover .building": {
				transform: "rotateY(90deg)",
				transformOrigin: "left -50%",
				transition: "transform 250ms",
			},
		},
	})
);

const Building = (props: BuildingProps) => {
	const { raceId, type } = props;
	const buildingStyle = props.onMap ? "sandwich" : "vanilla";
	const classes = useStyles(props);
	const imgUrl = useAssetUrl(`Races/Buildings${getBuildingFolderSuffix(buildingStyle)}/${getBuildingImagePrefix(type)}_${getBuildingColor(raceId)}.png`);
	return (
		<div className={classes.root + (props.onMap ? "" : " on-player-board")}>
			<img className="building" src={imgUrl} alt="" />
		</div>
	);
};

export default Building;
