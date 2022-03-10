import ListItem from "@mui/material/ListItem";
import ListItemText from "@mui/material/ListItemText";
import { GameLogDto } from "../../dto/interfaces";
import styles from "./GameLog.module.scss";
import PlayerLog from "./PlayerLog";

interface SystemLogProps {
	message: string;
}

const SystemLog = ({ message }: SystemLogProps) => {
	return (
		<ListItem className={styles.systemLog}>
			<ListItemText primary={message} primaryTypographyProps={{ className: "gaia-font text-center" }} />
		</ListItem>
	);
};

interface GameLogProps {
	log: GameLogDto;
	canRollback: boolean;
	doRollback(actionId: number): void;
}

const GameLog = ({ log, canRollback, doRollback }: GameLogProps) =>
	log.isSystem ? <SystemLog message={log.message} /> : <PlayerLog log={log} canRollback={canRollback} doRollback={doRollback} />;

export default GameLog;
