import { useLocation } from "react-router-dom";

const Unauthorized = () => {
	const location = useLocation();
	const { from } = (location.state as any) ?? { from: { pathname: "/" } };
	return <div>You are not authorized to navigate to {from.pathname}</div>;
};

export default Unauthorized;
