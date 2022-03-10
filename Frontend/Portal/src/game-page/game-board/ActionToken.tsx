import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';
import ActionTokenImg from "../../assets/Resources/Markers/ActionToken.png";
import { fillParentAbs, withAspectRatioW } from "../../utils/miscellanea";

const useStyles = makeStyles(() =>
	createStyles({
		root: {
			...withAspectRatioW(1),
		},
		token: {
			...fillParentAbs,
			objectFit: "cover",
		},
	})
);

const ActionToken = () => {
	const classes = useStyles();
	return (
		<div className={classes.root}>
			<img className={classes.token} src={ActionTokenImg} alt="" />
		</div>
	);
};

export default ActionToken;
