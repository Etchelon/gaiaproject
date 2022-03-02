import _ from "lodash";
import Sector1 from "../../../assets/Resources/Sectors/1.png";
import Sector10 from "../../../assets/Resources/Sectors/10.png";
import Sector2 from "../../../assets/Resources/Sectors/2.png";
import Sector3 from "../../../assets/Resources/Sectors/3.png";
import Sector4 from "../../../assets/Resources/Sectors/4.png";
import Sector5 from "../../../assets/Resources/Sectors/5.png";
import Sector5Outlined from "../../../assets/Resources/Sectors/5outlined.png";
import Sector6 from "../../../assets/Resources/Sectors/6.png";
import Sector6Outlined from "../../../assets/Resources/Sectors/6outlined.png";
import Sector7 from "../../../assets/Resources/Sectors/7.png";
import Sector7Outlined from "../../../assets/Resources/Sectors/7outlined.png";
import Sector8 from "../../../assets/Resources/Sectors/8.png";
import Sector9 from "../../../assets/Resources/Sectors/9.png";
import { SectorDto } from "../../../dto/interfaces";
import Hex from "../hex/Hex";
import useStyles from "./sector.styles";

const sectorBackgrounds = new Map<string, string>([
	["1", Sector1],
	["2", Sector2],
	["3", Sector3],
	["4", Sector4],
	["5", Sector5],
	["5outlined", Sector5Outlined],
	["6", Sector6],
	["6outlined", Sector6Outlined],
	["7", Sector7],
	["7outlined", Sector7Outlined],
	["8", Sector8],
	["9", Sector9],
	["10", Sector10],
]);

interface SectorProps {
	sector: SectorDto;
	hexWidth: number;
	hexHeight: number;
	xOffset: number;
	yOffset: number;
}

const Sector = ({ sector, hexWidth, hexHeight, xOffset, yOffset }: SectorProps) => {
	const classes = useStyles({
		hexWidth,
		hexHeight,
		row: sector.row,
		column: sector.column,
		xOffset,
		yOffset,
		rotation: sector.rotation,
	});

	return (
		<div className={classes.sector}>
			<img className={classes.image} src={sectorBackgrounds.get(sector.id)} alt="" />
			{_.map(sector.hexes, hex => (
				<Hex key={hex.id} hex={hex} width={hexWidth} />
			))}
		</div>
	);
};

export default Sector;
