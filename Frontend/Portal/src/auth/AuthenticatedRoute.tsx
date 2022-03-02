import { useAuth0 } from "@auth0/auth0-react";
import { FC } from "react";

const AuthenticatedRoute: FC = ({ children }) => {
	const { isAuthenticated, isLoading } = useAuth0();

	if (isLoading) {
		return <p>Loading...</p>;
	}

	return isAuthenticated ? <>{children}</> : null;
};

export default AuthenticatedRoute;
