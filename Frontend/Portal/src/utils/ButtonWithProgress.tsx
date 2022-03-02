import Button, { ButtonProps } from "@mui/material/Button";
import CircularProgress from "@mui/material/CircularProgress";
import { Theme, useTheme } from "@mui/material/styles";

import createStyles from '@mui/styles/createStyles';
import makeStyles from '@mui/styles/makeStyles';

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		wrapper: {
			position: "relative",
			"& > button": {
				width: "100%",
			},
		},
		loader: {
			position: "absolute",
			top: "50%",
			left: "50%",
			marginTop: -12,
			marginLeft: -12,
		},
	})
);

export interface ButtonWithProgressProps extends ButtonProps {
	label: string;
	loading: boolean;
}

const ButtonWithProgress = ({ label, loading, ...props }: ButtonWithProgressProps) => {
	const classes = useStyles();
	const theme = useTheme();
	loading && (props.disabled = true);

	return (
		<div className={classes.wrapper}>
			<Button {...props}>{label}</Button>
			{loading && <CircularProgress size={theme.spacing(3)} className={classes.loader}></CircularProgress>}
		</div>
	);
};

export default ButtonWithProgress;
