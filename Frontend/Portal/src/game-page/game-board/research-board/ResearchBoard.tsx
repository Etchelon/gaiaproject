import { isNil } from "lodash";
import ResearchBoardImg from "../../../assets/Resources/Boards/ResearchBoard.jpg";
import { ResearchBoardDto } from "../../../dto/interfaces";
import { ElementSize } from "../../../utils/hooks";
import ActionSpace from "../ActionSpace";
import ResearchTrack from "../research-track/ResearchTrack";
import TechnologyTileStack from "../TechnologyTileStack";
import useStyles, { HEIGHT_TO_WIDTH_RATIO } from "./research-board.styles";

interface ResearchBoardProps {
	board: ResearchBoardDto;
	width: number;
	height?: number;
}

function calculateDimensions(parentWidth: number, parentHeight?: number): ElementSize {
	if (!isNil(parentHeight)) {
		const widthFromHeight = parentHeight / HEIGHT_TO_WIDTH_RATIO;
		if (widthFromHeight <= parentWidth) {
			return { width: widthFromHeight, height: parentHeight };
		}
	}

	const heightFromWidth = parentWidth * HEIGHT_TO_WIDTH_RATIO;
	return { width: parentWidth, height: heightFromWidth };
}

const ResearchBoard = (props: ResearchBoardProps) => {
	const { width, height } = calculateDimensions(props.width, props.height);
	const classes = useStyles({ width, height });
	const board = props.board;
	const trackWidth = (width * 0.975) / 6;
	const trackHeight = height * 0.725;
	const freeTilesLeftMargins = ["7%", "17%", "15.5%"];

	return (
		<div className={classes.researchBoard}>
			<img className={classes.image} src={ResearchBoardImg} alt="" />
			<div className={classes.tracks}>
				{board.tracks.map(track => (
					<ResearchTrack key={track.id} track={track} width={trackWidth} height={trackHeight} />
				))}
			</div>
			<div className={classes.freeTiles}>
				{board.freeStandardTiles.map((stack, index) => (
					<div key={stack.type} className={classes.freeTile} style={{ marginLeft: freeTilesLeftMargins[index] }}>
						<TechnologyTileStack stack={stack} />
					</div>
				))}
			</div>
			<div className={classes.actions}>
				<div className={classes.powerActions}>
					{board.powerActions.map(pa => (
						<div key={`${pa.kind}_${pa.type}`} className={classes.powerAction}>
							<ActionSpace space={pa} />
						</div>
					))}
				</div>
				<div className={classes.qicActions}>
					{board.qicActions.map(qa => (
						<div key={`${qa.kind}_${qa.type}`} className={classes.qicAction}>
							<ActionSpace space={qa} />
						</div>
					))}
				</div>
			</div>
		</div>
	);
};

export default ResearchBoard;
