import { noop, isNil } from "lodash";
import { observer } from "mobx-react";
import LostPlanetToken from "../../../assets/Resources/Markers/LostPlanet.png";
import { ResearchTrackDto } from "../../../dto/interfaces";
import { ElementSize } from "../../../utils/hooks";
import { interactiveElementClass } from "../../../utils/miscellanea";
import { useGamePageContext } from "../../GamePage.context";
import { selectResearchTrackInteractionState } from "../../store/selectors";
import { useWorkflow } from "../../WorkflowContext";
import { InteractiveElementType } from "../../workflows/enums";
import AdvancedTechnologyTile from "../AdvancedTechnologyTile";
import FederationToken from "../federation-token/FederationToken";
import TechnologyTileStack from "../TechnologyTileStack";
import PlayerAdvancement from "./PlayerAdvancement";
import useStyles from "./research-track.styles";

interface ResearchTrackProps extends ElementSize {
	track: ResearchTrackDto;
}

const ResearchTrack = ({ track, width, height }: ResearchTrackProps) => {
	const classes = useStyles({ width, height });
	const { vm } = useGamePageContext();
	const { isClickable, isSelected } = selectResearchTrackInteractionState(track.id)(vm);
	const { activeWorkflow } = useWorkflow();
	const trackClicked = isClickable
		? () => {
				activeWorkflow?.elementSelected(track.id, InteractiveElementType.ResearchStep);
		  }
		: noop;
	const playerAdvancementWidth = width * 0.25;

	return (
		<div className={classes.researchTrack + " research-track"}>
			{!isNil(track.federation) && (
				<div className={classes.federationToken}>
					<FederationToken type={track.federation} />
				</div>
			)}
			{track.lostPlanet && <img className={classes.lostPlanetToken} src={LostPlanetToken} alt="" />}
			{!isNil(track.advancedTileType) && (
				<div className={classes.advancedTile}>
					<AdvancedTechnologyTile type={track.advancedTileType} />
				</div>
			)}
			<div className={classes.playerMarkers}>
				{track.players.map(playerAdvancement => (
					<PlayerAdvancement key={playerAdvancement.raceId} width={playerAdvancementWidth} raceId={playerAdvancement.raceId} steps={playerAdvancement.steps} />
				))}
			</div>
			<div className={classes.standardTiles}>
				<TechnologyTileStack stack={track.standardTiles} />
			</div>
			<div className={interactiveElementClass(isClickable, isSelected)} onClick={trackClicked}></div>
		</div>
	);
};

export default observer(ResearchTrack);
