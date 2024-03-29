import Avatar from "@mui/material/Avatar";
import Button from "@mui/material/Button";
import Grid from "@mui/material/Grid";
import TextField from "@mui/material/TextField";
import { cloneDeep, isNil } from "lodash";
import { useSnackbar } from "notistack";
import { useEffect, useReducer } from "react";
import placeholder from "../assets/person.png";
import { UserInfoDto } from "../dto/interfaces";
import { useCurrentUser } from "../utils/hooks";
import httpClient from "../utils/http-client";
import userInfoService from "../utils/user-info.service";
import styles from "./ManageProfile.module.scss";

const reducer = (state: UserInfoDto, action: { property: keyof UserInfoDto | "set"; value: any }): UserInfoDto => {
	if (action.property === "set") {
		return cloneDeep(action.value as UserInfoDto);
	}
	const ret = cloneDeep(state);
	ret[action.property] = action.value;
	return ret;
};

const ManageProfile = () => {
	const { enqueueSnackbar } = useSnackbar();
	const user = useCurrentUser()!;
	const [userInfo, dispatch] = useReducer(reducer, cloneDeep(user));

	useEffect(() => {
		user && dispatch({ property: "set", value: user });
	}, [user]);

	if (isNil(userInfo)) {
		return <div></div>;
	}

	const updateProfile = async () => {
		await httpClient.put(`api/Users/UpdateProfile/${userInfo.id}`, userInfo, { readAsString: true });
		userInfoService.update(userInfo);
		enqueueSnackbar("Profile updated!", { variant: "success" });
	};

	return (
		<div className={styles.profileManager}>
			<Grid container spacing={2}>
				<Grid item xs={12} md={6}>
					<div className={styles.info}>
						<Avatar className={styles.userImage} src={userInfo.avatar ?? placeholder} />
						<TextField
							className={styles.field}
							InputLabelProps={{ className: "gaia-font" }}
							value={userInfo.avatar ?? ""}
							label="Choose an avatar"
							onChange={evt => dispatch({ property: "avatar", value: evt.target.value })}
						/>
						<TextField
							className={styles.field}
							InputLabelProps={{ className: "gaia-font" }}
							InputProps={{ className: "gaia-font" }}
							value={userInfo.username}
							required
							label="Choose your username"
							onChange={evt => dispatch({ property: "username", value: evt.target.value })}
						/>
						<TextField
							className={styles.field}
							InputLabelProps={{ className: "gaia-font" }}
							InputProps={{ className: "gaia-font" }}
							value={userInfo.firstName ?? ""}
							label="Enter your first name"
							onChange={evt => dispatch({ property: "firstName", value: evt.target.value })}
						/>
						<TextField
							className={styles.field}
							InputLabelProps={{ className: "gaia-font" }}
							InputProps={{ className: "gaia-font" }}
							value={userInfo.lastName ?? ""}
							label="Enter your last name"
							onChange={evt => dispatch({ property: "lastName", value: evt.target.value })}
						/>
					</div>
					<div className={styles.actions}>
						<Button variant="contained" color="primary" onClick={updateProfile}>
							<span className="gaia-font">Save</span>
						</Button>
					</div>
				</Grid>
				<Grid item xs={12} md={6}></Grid>
			</Grid>
		</div>
	);
};

export default ManageProfile;
