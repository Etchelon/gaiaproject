import { useAuth0 } from "@auth0/auth0-react";
import LoginButton from "../login/Login";
import LogoutButton from "../logout/Logout";

const AuthButton = () => {
	const { isAuthenticated } = useAuth0();
	return isAuthenticated ? <LogoutButton /> : <LoginButton />;
};

export default AuthButton;
