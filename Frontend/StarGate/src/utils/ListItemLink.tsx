import { ListItem, ListItemIcon, ListItemProps, ListItemText } from "@material-ui/core";
import Tooltip from "@material-ui/core/Tooltip";
import { ElementType, forwardRef, useMemo } from "react";
import { Link } from "react-router-dom";

const ListItemLink = <T extends ElementType>(props: ListItemProps<T>) => {
	const { icon, primary, to } = props;

	const CustomLink = useMemo(() => forwardRef((linkProps, ref: any) => <Link ref={ref} to={to} {...linkProps} />), [to]);

	return (
		<li>
			<ListItem button component={CustomLink}>
				{icon && (
					<Tooltip title={primary}>
						<ListItemIcon>{icon}</ListItemIcon>
					</Tooltip>
				)}
				<ListItemText primary={primary} primaryTypographyProps={{ className: "gaia-font", style: { fontSize: "0.8rem" } }} />
			</ListItem>
		</li>
	);
};

export default ListItemLink;
