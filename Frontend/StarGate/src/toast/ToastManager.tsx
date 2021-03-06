import Snackbar from "@material-ui/core/Snackbar";
import { createStyles, makeStyles, Theme } from "@material-ui/core/styles";
import MuiAlert, { AlertProps } from "@material-ui/lab/Alert";
import _ from "lodash";
import { createContext, useContext } from "react";

const DEFAULT_TOAST_DURATION = 6000;

export interface Toast {
	id?: string;
	type: "success" | "error" | "warning" | "info";
	message: string;
	duration?: number;
}

interface ToastContextProps {
	toasts: Toast[];
	open(toast: Toast): void;
	close(toastId: string): void;
}

export const ToastContext = createContext<ToastContextProps>({ toasts: [], open: _.noop, close: _.noop });

export function useToasts() {
	return useContext(ToastContext);
}

function Alert(props: AlertProps) {
	return <MuiAlert elevation={6} variant="filled" {...props} />;
}

const useStyles = makeStyles((theme: Theme) =>
	createStyles({
		root: {
			position: "fixed",
			bottom: 0,
			width: "100vw",
			display: "flex",
			flexDirection: "column-reverse",
			zIndex: theme.zIndex.snackbar,
		},
		toast: {
			position: "static",
			transform: "unset",
			marginBottom: theme.spacing(1),
		},
	})
);

const ToastManager = () => {
	const classes = useStyles();
	const { toasts, close } = useToasts();

	const handleClose = (toastId: string) => (evt?: React.SyntheticEvent, reason?: string) => {
		if (reason === "clickaway") {
			return;
		}
		close(toastId);
	};

	return (
		<div className={classes.root}>
			{_.map(toasts, t => (
				<Snackbar
					className={classes.toast}
					key={t.id}
					anchorOrigin={{ vertical: "bottom", horizontal: "center" }}
					open={true}
					autoHideDuration={t.duration ?? DEFAULT_TOAST_DURATION}
					onClose={handleClose(t.id!)}
				>
					<Alert onClose={handleClose(t.id!)} severity={t.type}>
						{t.message}
					</Alert>
				</Snackbar>
			))}
		</div>
	);
};

export default ToastManager;
