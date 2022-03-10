import createStyles from "@mui/styles/createStyles";
import makeStyles from "@mui/styles/makeStyles";
import { noop } from "lodash";
import { observer } from "mobx-react";
import { RoundBoosterType } from "../../../dto/enums";
import { RoundBoosterDto, RoundBoosterTileDto } from "../../../dto/interfaces";
import { useAssetUrl } from "../../../utils/hooks";
import { fillParentAbs, interactiveBorder, interactiveElementClass, withAspectRatioW } from "../../../utils/miscellanea";
import { useGamePageContext } from "../../GamePage.context";
import { selectRoundBoosterInteractionState } from "../../store/selectors";
import { useWorkflow } from "../../WorkflowContext";
import { InteractiveElementType } from "../../workflows/enums";
import ActionToken from "../ActionToken";
import PlayerMarker from "../PlayerMarker";

const WIDTH_TO_HEIGHT_RATIO = 0.3423;
const roundBoosterNames = new Map<RoundBoosterType, string>([
	[RoundBoosterType.GainOreGainKnowledge, "BOOknw"],
	[RoundBoosterType.GainPowerTokensGainOre, "BOOpwt"],
	[RoundBoosterType.GainCreditsGainQic, "BOOqic"],
	[RoundBoosterType.TerraformActionGainCredits, "BOOter"],
	[RoundBoosterType.BoostRangeGainPower, "BOOnav"],
	[RoundBoosterType.PassPointsPerMineGainOre, "BOOmin"],
	[RoundBoosterType.PassPointsPerTradingStationsGainOre, "BOOtrs"],
	[RoundBoosterType.PassPointsPerResearchLabsGainKnowledge, "BOOlab"],
	[RoundBoosterType.PassPointsPerBigBuildingsGainPower, "BOOpia"],
	[RoundBoosterType.PassPointsPerGaiaPlanetsGainCredits, "BOOgai"],
]);

interface RoundBoosterProps {
	booster: RoundBoosterTileDto | RoundBoosterDto;
	withPlayerInfo: boolean;
	nonInteractive?: boolean;
}

const useStyles = makeStyles(() =>
	createStyles({
		root: {
			...withAspectRatioW(WIDTH_TO_HEIGHT_RATIO),
			...interactiveBorder,
		},
		image: {
			...fillParentAbs,
		},
		actionToken: {
			position: "absolute",
			width: "70%",
			top: "17%",
			left: "15%",
		},
		playerMarker: {
			position: "absolute",
			width: "40%",
			bottom: "3%",
			right: "10%",
		},
	})
);

const RoundBooster = ({ booster, withPlayerInfo, nonInteractive }: RoundBoosterProps) => {
	const classes = useStyles();
	const imgUrl = useAssetUrl(`Boards/RoundBoosters/${roundBoosterNames.get(booster.id)!}.png`);
	const { vm } = useGamePageContext();
	const availableBooster = booster as RoundBoosterTileDto;
	const actualType = withPlayerInfo ? InteractiveElementType.OwnRoundBooster : InteractiveElementType.RoundBooster;
	const { isClickable: isClickable_, isSelected: isSelected_ } = selectRoundBoosterInteractionState(actualType)(booster.id)(vm);
	const isClickable = isClickable_ && !nonInteractive;
	const isSelected = isSelected_ && !nonInteractive;
	const { activeWorkflow } = useWorkflow();
	const boosterClicked = isClickable
		? () => {
				activeWorkflow?.elementSelected(booster.id, actualType);
		  }
		: noop;

	return (
		<div className={classes.root}>
			<img className={classes.image} src={imgUrl} alt="" />
			{booster?.used && (
				<div className={classes.actionToken}>
					<ActionToken />
				</div>
			)}
			{availableBooster?.isTaken && (
				<div className={classes.playerMarker}>
					<PlayerMarker race={availableBooster.player.raceId!} />
				</div>
			)}
			<div className={interactiveElementClass(isClickable, isSelected)} onClick={boosterClicked}></div>
		</div>
	);
};

export default observer(RoundBooster);
