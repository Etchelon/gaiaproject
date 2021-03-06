import { useAuth0 } from "@auth0/auth0-react";
import ListItem from "@material-ui/core/ListItem";
import ListItemIcon from "@material-ui/core/ListItemIcon";
import ListItemText from "@material-ui/core/ListItemText";
import InputIcon from "@material-ui/icons/Input";

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
