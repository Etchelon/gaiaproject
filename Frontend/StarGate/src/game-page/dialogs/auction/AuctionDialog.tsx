import Button from "@material-ui/core/Button";
import Typography from "@material-ui/core/Typography";
import _ from "lodash";
import { Fragment, useReducer, useState } from "react";
import { useDispatch } from "react-redux";
import { Race } from "../../../dto/enums";
import { PlayerInGameDto } from "../../../dto/interfaces";
import { useAssetUrl } from "../../../utils/hooks";
import { getRaceBoard, getRaceName } from "../../../utils/race-utils";
import { executePlayerAction } from "../../store/actions-thunks";
import { useWorkflow } from "../../WorkflowContext";
import { BidForRaceWorkflow, CurrentAuction } from "../../workflows/setup-phase/bid-for-race.workflow";
import { CommonWorkflowStates } from "../../workflows/types";
import useStyles from "./auction-dialog.styles";
import AuctionedRace from "./AuctionedRace";

const boardVersion = "rework";

const RaceBoard = ({ race }: { race: Race }) => {
	const imgUrl = useAssetUrl(`Races/Boards_${boardVersion}/${getRaceBoard(race)}`);
	return <img style={{ width: "100%" }} src={imgUrl} alt="" />;
};

const reducer = (
	state: CurrentAuction,
	action: { type: "select" | "increase" | "decrease"; selection?: CurrentAuction; variation?: number; minimumBid?: number }
): CurrentAuction => {
	switch (action.type) {
		case "select":
			return { ...action.selection! };
		case "increase":
		case "decrease":
			return {
				race: state.race!,
				bid: Math.max(action.minimumBid!, state.bid! + (action.type === "increase" ? 1 : -1) * action.variation!),
			};
		default:
			throw new Error(`Action of type ${action.type} not handled.`);
	}
};

interface AuctionDialogProps {
	gameId: string;
	currentPlayer: PlayerInGameDto;
}

const AuctionDialog = ({ gameId }: AuctionDialogProps) => {
	const classes = useStyles();
	const storeDispatch = useDispatch();
	const { activeWorkflow } = useWorkflow();
	const [isBidding, setIsBidding] = useState(false);
	const [currentAuction, dispatch] = useReducer(reducer, { race: null, bid: null });
	const auctionState = (activeWorkflow as BidForRaceWorkflow)?.auctionState;
	const auctions = auctionState?.auctionedRaces ?? [];
	const selectedRace = currentAuction.race;
	const ongoingAuctionForSelectedRace = _.find(auctions, o => o.race === selectedRace);
	const isOngoingAuction = ongoingAuctionForSelectedRace && ongoingAuctionForSelectedRace?.points !== null;
	const minimumBidForSelectedRace = ongoingAuctionForSelectedRace && ongoingAuctionForSelectedRace !== null ? ongoingAuctionForSelectedRace.points! + 1 : 0;
	const isLastRace =
		selectedRace !== null &&
		_.chain(auctions)
			.filter(o => o.race !== selectedRace)
			.every(o => o.points !== null)
			.value();
	const canBid0Points = isLastRace || !isOngoingAuction;

	const bid = (type: "increase" | "decrease", amount: number) => {
		dispatch({ type, variation: amount, minimumBid: minimumBidForSelectedRace });
	};
	const bidMore = (amount: number) => bid("increase", amount);
	const bidLess = (amount: number) => bid("decrease", amount);
	const select = (race: Race) => {
		const auctionForSelectedRace = _.find(auctions, o => o.race === race)!;
		const minimumBid = auctionForSelectedRace.points !== null ? auctionForSelectedRace.points + 1 : 0;
		dispatch({ type: "select", selection: { race, bid: minimumBid } });
	};

	const closeDialog = () => {
		activeWorkflow!.handleCommand({ nextState: CommonWorkflowStates.CANCEL });
	};
	const doBid = () => {
		const action = activeWorkflow!.handleCommand({ nextState: CommonWorkflowStates.PERFORM_ACTION, data: currentAuction })!;
		setIsBidding(true);
		storeDispatch(executePlayerAction({ gameId, action }));
	};

	return (
		<div className={classes.root}>
			<Typography variant="h6" className={classes.header + " gaia-font text-center"}>
				Bid for a race
			</Typography>
			<div className={classes.raceList}>
				{_.map(auctions, auction => (
					<Fragment key={auction.race}>
						<AuctionedRace auction={auction} selected={auction.race === selectedRace} onSelected={select} />
						<div className={classes.spacer}></div>
					</Fragment>
				))}
			</div>
			<div className={classes.raceBoard}>
				{_.isNull(selectedRace) ? <Typography variant="h5">Select a race to view its board</Typography> : <RaceBoard race={selectedRace} />}
			</div>
			{currentAuction && selectedRace && (
				<div className={classes.bidCommands}>
					{canBid0Points && (
						<Typography variant="h6" className="gaia-font">
							Play {getRaceName(selectedRace)} for 0 VP
						</Typography>
					)}
					{!canBid0Points && (
						<>
							<Button variant="contained" color="default" className="command" disabled={currentAuction.bid! === minimumBidForSelectedRace} onClick={() => bidLess(1)}>
								<span className="gaia-font">-1</span>
							</Button>
							<Typography variant="h6" className={classes.marginH + " gaia-font"}>
								Bid {currentAuction.bid} VP for {getRaceName(selectedRace)}
							</Typography>
							<Button variant="contained" color="default" className="command" onClick={() => bidMore(1)}>
								<span className="gaia-font">+1</span>
							</Button>
						</>
					)}
				</div>
			)}
			<div className={classes.commands}>
				<Button variant="contained" color="default" className="command" onClick={closeDialog}>
					<span className="gaia-font">Close</span>
				</Button>
				<Button variant="contained" color="primary" className="command" disabled={_.isNil(selectedRace) || isBidding} onClick={doBid}>
					<span className="gaia-font">Confirm</span>
				</Button>
			</div>
		</div>
	);
};

export default AuctionDialog;
