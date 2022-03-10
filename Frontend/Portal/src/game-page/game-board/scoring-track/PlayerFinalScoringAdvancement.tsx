import { Theme } from "@mui/material/styles";
import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import { useRef } from "react";
import { PlayerFinalScoringStatusDto } from "../../../dto/interfaces";
import { useContainerDimensions } from "../../../utils/hooks";
import { fillParent } from "../../../utils/miscellanea";
import { getRaceColor } from "../../../utils/race-utils";

const CUBE_HEIGHT_TO_WIDTH_RATIO = 0.077;
const getCubeX = (count: number): string => `${count === 0 ? 1 : 8.85 * (count - 1) + 11}%`;

interface PlayerFinalScoringAdvancementProps {
	playerStatus: PlayerFinalScoringStatusDto;
}

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: {
			...fillParent,
			position: "relative",
			"& > .cube": {
				position: "absolute",
				width: "calc(6% + 2px)", // 2px is twice the border
				paddingTop: "6%",
				top: 0,
				border: ({ playerStatus }: PlayerFinalScoringAdvancementProps) => {
					const color = getRaceColor(playerStatus.player.raceId!);
					return `1px solid ${theme.palette.getContrastText(color)}`;
				},
				borderRadius: 3,
				backgroundColor: ({ playerStatus }: PlayerFinalScoringAdvancementProps) => getRaceColor(playerStatus.player.raceId),
			},
		},
		firstCube: {
			left: ({ playerStatus: { count } }: PlayerFinalScoringAdvancementProps) => getCubeX(count > 10 ? 10 : count),
		},
		secondCube: {
			display: ({ playerStatus: { count } }: PlayerFinalScoringAdvancementProps) => (count > 10 ? "block" : "none"),
			left: ({ playerStatus: { count } }: PlayerFinalScoringAdvancementProps) => getCubeX(count - 10),
		},
	})
);

const PlayerFinalScoringAdvancement = ({ playerStatus }: PlayerFinalScoringAdvancementProps) => {
	const ref = useRef<HTMLDivElement>(null);
	const { width } = useContainerDimensions(ref);
	const height = width * CUBE_HEIGHT_TO_WIDTH_RATIO;
	const classes = useStyles({ playerStatus });

	return (
		<div ref={ref} className={classes.root}>
			<div className={classes.firstCube + " cube"} onClick={() => console.log(playerStatus, { width, height })}></div>
			<div className={classes.secondCube + " cube"}></div>
		</div>
	);
};

export default PlayerFinalScoringAdvancement;
