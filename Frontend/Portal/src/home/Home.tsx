import { useAuth0 } from "@auth0/auth0-react";
import { useTheme } from "@material-ui/core/styles";
import AuthButton from "../frame/AuthButton";
import UserGames from "../games/UserGames";

const Home = () => {
	const theme = useTheme();
	const { isAuthenticated } = useAuth0();

	if (!isAuthenticated) {
		return (
			<div style={{ padding: theme.spacing(2) }}>
				<h2>Login to use the app!</h2>
				<AuthButton />
			</div>
		);
	}
	return <UserGames kind="waiting" />;
};

export default Home;
