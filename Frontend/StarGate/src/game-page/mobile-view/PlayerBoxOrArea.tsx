import { MouseEvent, useState } from "react";
import { PlayerInGameDto } from "../../dto/interfaces";
import PlayerArea from "../game-board/player-area/PlayerArea";
import PlayerBox from "../game-board/player-box/PlayerBox";

interface PlayerBoxOrAreaProps {
	player: PlayerInGameDto;
	index: number;
}

const PlayerBoxOrArea = ({ player, index }: PlayerBoxOrAreaProps) => {
	const [showArea, setShowArea] = useState(false);
	const toggleView = (evt: MouseEvent<HTMLDivElement>) => {
		evt.stopPropagation();
		setShowArea(!showArea);
	};

	return <div onClick={toggleView}>{showArea ? <PlayerArea player={player} framed={true} /> : <PlayerBox player={player} index={index} />}</div>;
};

export default PlayerBoxOrArea;
