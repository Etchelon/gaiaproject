import { useAuth0 } from "@auth0/auth0-react";
import ListItem from "@mui/material/ListItem";
import ListItemIcon from "@mui/material/ListItemIcon";
import ListItemText from "@mui/material/ListItemText";
import InputIcon from "@mui/icons-material/Input";

const LoginButton = () => {
	const { loginWithRedirect } = useAuth0();

	return (
		<ListItem button onClick={loginWithRedirect}>
			<ListItemIcon>
				<InputIcon />
			</ListItemIcon>
			<ListItemText primary="Login" primaryTypographyProps={{ className: "gaia-font", style: { fontSize: "0.8rem" } }} />
		</ListItem>
	);
};

export default LoginButton;
