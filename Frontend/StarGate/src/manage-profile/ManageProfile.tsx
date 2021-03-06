import Avatar from "@material-ui/core/Avatar";
import Button from "@material-ui/core/Button";
import Grid from "@material-ui/core/Grid";
import TextField from "@material-ui/core/TextField";
import _ from "lodash";
import { useEffect, useReducer } from "react";
import placeholder from "../assets/person.png";
import { UserInfoDto } from "../dto/interfaces";
import { useToasts } from "../toast/ToastManager";
import { useCurrentUser } from "../utils/hooks";
import httpClient from "../utils/http-client";
import userInfoService from "../utils/user-info.service";
import styles from "./ManageProfile.module.scss";

const reducer = (state: UserInfoDto, action: { property: keyof UserInfoDto | "set"; value: any }): UserInfoDto => {
	if (action.property === "set") {
		return _.cloneDeep(action.value as UserInfoDto);
	}
	const ret = _.cloneDeep(state);
	ret[action.property] = action.value;
	return ret;
};

const ManageProfile = () => {
	const { open: openToast } = useToasts();
	const user = useCurrentUser()!;
	const [userInfo, dispatch] = useReducer(reducer, _.cloneDeep(user));

	useEffect(() => {
		user && dispatch({ property: "set", value: user });
	}, [user]);

	if (_.isNil(userInfo)) {
		return <div></div>;
	}

	const updateProfile = async () => {
		await httpClient.put(`api/Users/UpdateProfile/${userInfo.id}`, userInfo, { readAsString: true });
		userInfoService.update(userInfo);
		openToast({ message: "Profile updated!", type: "success" });
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
