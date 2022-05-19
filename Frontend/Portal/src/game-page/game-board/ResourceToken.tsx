import createStyles from "@mui/styles/createStyles";
import makeStyles from "@mui/styles/makeStyles";
import { useAssetUrl } from "../../utils/hooks";
import { centeredFlexRow, fillParent } from "../../utils/miscellanea";

interface ResourceTokenProps {
	type?: string;
	assetUrl?: string;
	scale?: number;
}

const useStyles = makeStyles(() =>
	createStyles({
		root: {
			...centeredFlexRow,
			...fillParent,
		},
		image: {
			...fillParent,
			objectFit: "cover",
		},
	})
);

const ResourceToken = ({ type, assetUrl, scale = 1 }: ResourceTokenProps) => {
	const classes = useStyles({ type });
	const imgUrl = useAssetUrl(assetUrl ?? `Markers/${type}.png`);
	return (
		<div className={classes.root}>
			<img className={classes.image} src={imgUrl} alt={type} style={{ transform: `scale(${scale})` }} />
		</div>
	);
};

export default ResourceToken;
