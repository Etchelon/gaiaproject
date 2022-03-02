import { withAuthenticationRequired } from "@auth0/auth0-react";
import { Route } from "react-router-dom";

const AuthenticatedRoute = ({ children: Component, ...rest }: any) => {
	return <Route {...rest}>{Component}</Route>;
};

export default withAuthenticationRequired(AuthenticatedRoute);
