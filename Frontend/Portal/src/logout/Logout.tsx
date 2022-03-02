import { useAuth0 } from "@auth0/auth0-react";
import { Button, Dialog, DialogActions, DialogTitle } from "@mui/material";
import ListItem from "@mui/material/ListItem";
import ListItemIcon from "@mui/material/ListItemIcon";
import ListItemText from "@mui/material/ListItemText";
import ExitToAppIcon from "@mui/icons-material/ExitToApp";
import { useState } from "react";

const LogoutButton = () => {
	const { logout } = useAuth0();
	const [open, setIsOpen] = useState(false);
	const openModal = () => setIsOpen(true);
	const closeModal = () => setIsOpen(false);

	return (
		<>
			<ListItem button onClick={openModal}>
				<ListItemIcon>
					<ExitToAppIcon />
				</ListItemIcon>
				<ListItemText primary="Logout" primaryTypographyProps={{ className: "gaia-font", style: { fontSize: "0.8rem" } }} />
			</ListItem>
			<Dialog open={open} onClose={closeModal} aria-labelledby="alert-dialog-title" aria-describedby="alert-dialog-description">
				<DialogTitle id="alert-dialog-title">Are you sure you want to logout?</DialogTitle>
				<DialogActions>
					<Button onClick={closeModal}>Cancel</Button>
					<Button onClick={() => logout({ returnTo: window.location.origin })}>Yes</Button>
				</DialogActions>
			</Dialog>
		</>
	);
};

export default LogoutButton;
