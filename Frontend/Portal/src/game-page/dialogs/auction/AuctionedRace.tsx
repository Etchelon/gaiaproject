import { useTheme } from "@mui/material";
import Avatar from "@mui/material/Avatar";
import Typography from "@mui/material/Typography";
import CheckBoxRounded from "@mui/icons-material/CheckBoxRounded";
import { Race } from "../../../dto/enums";
import { AuctionDto } from "../../../dto/interfaces";
import { useAssetUrl } from "../../../utils/hooks";
import { getRaceColor, getRaceImage, getRaceName } from "../../../utils/race-utils";
import useStyles from "./auctioned-race.styles";

interface AuctionedRaceProps {
	auction: AuctionDto;
	selected: boolean;
	onSelected(race: Race): void;
}

const AuctionedRace = ({ auction, selected, onSelected }: AuctionedRaceProps) => {
	const theme = useTheme();
	const classes = useStyles();
	const race = auction.race;
	const imgUrl = useAssetUrl(`Races/${getRaceImage(race)}`);
	const background = getRaceColor(race);
	const color = theme.palette.getContrastText(background);
	return (
		<div className={classes.root} onClick={() => onSelected(race)}>
			<Typography variant="h6" className="gaia-font">
				{getRaceName(auction.race)}
			</Typography>
			<div className={classes.raceImage} style={{ backgroundColor: background, color }}>
				<Avatar src={imgUrl} />
				{selected && (
					<div className="check">
						<CheckBoxRounded />
					</div>
				)}
			</div>
			{auction.playerUsername && (
				<div className={classes.playerInfo}>
					<Typography variant="caption" className="gaia-font">
						{auction.playerUsername}
					</Typography>
					<Typography variant="caption" className="gaia-font">
						{`${auction.points} VP`}
					</Typography>
				</div>
			)}
		</div>
	);
};

export default AuctionedRace;
