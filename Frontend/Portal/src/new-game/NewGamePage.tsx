import DeleteIcon from "@mui/icons-material/Delete";
import Autocomplete from "@mui/material/Autocomplete";
import Avatar from "@mui/material/Avatar";
import Button from "@mui/material/Button";
import CircularProgress from "@mui/material/CircularProgress";
import FormControl from "@mui/material/FormControl";
import FormControlLabel from "@mui/material/FormControlLabel";
import FormLabel from "@mui/material/FormLabel";
import Grid from "@mui/material/Grid";
import IconButton from "@mui/material/IconButton";
import Paper from "@mui/material/Paper";
import Radio from "@mui/material/Radio";
import RadioGroup from "@mui/material/RadioGroup";
import TextField from "@mui/material/TextField";
import Typography from "@mui/material/Typography/Typography";
import { format } from "date-fns";
import { debounce, reject, size, some } from "lodash";
import { useSnackbar } from "notistack";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { MapShape, RaceSelectionMode, TurnOrderSelectionMode } from "../dto/enums";
import { CreateGameCommand, UserInfoDto } from "../dto/interfaces";
import httpClient from "../utils/http-client";
import { Nullable } from "../utils/miscellanea";
import useStyles from "./new-game-page.styles";

const MIN_OTHER_PLAYERS = 1;
const MAX_OTHER_PLAYERS = 3;
const DEFAULT_STARTING_VPS = 10;

type BalancingMethod = "auction" | "sector-rotation";

const NewGamePage = () => {
	const classes = useStyles();
	const navigate = useNavigate();
	const { enqueueSnackbar } = useSnackbar();
	const [open, setOpen] = useState(false);
	const [isLoading, setIsLoading] = useState(false);
	const [searchedUsers, setSearchedUsers] = useState<UserInfoDto[]>([]);

	const [selectedUsers, setSelectedUsers] = useState<UserInfoDto[]>([]);
	const [raceSelectionMode, setRaceSelectionMode] = useState(RaceSelectionMode.TurnOrder);
	const [balancingMethod, setBalancingMethod] = useState<Nullable<BalancingMethod>>(null);
	const [startingVPs, setStartingVPs] = useState(DEFAULT_STARTING_VPS);
	const [gameName, setGameName] = useState<Nullable<string>>(null);
	const [isCreating, setIsCreating] = useState(false);

	const searchUsersImpl = debounce(async (filter: string) => {
		setIsLoading(true);
		try {
			const users_ = await httpClient.get<UserInfoDto[]>(`api/Users/Search/${filter}`);
			const selectableUsers = reject(users_, u => some(selectedUsers, su => su.id === u.id));
			setSearchedUsers(selectableUsers);
			setIsLoading(false);
		} catch (err) {
			enqueueSnackbar("Error trying to search for users. Try again later", { variant: "error" });
			setSearchedUsers([]);
		}
	}, 500);
	const searchUsers = (filter: string) => {
		if (size(filter) < 2) {
			return;
		}
		searchUsersImpl(filter);
	};

	const selectUser = (user: Nullable<UserInfoDto>, reason: any) => {
		console.log({ reason });
		if (!user) {
			return;
		}
		setSelectedUsers([...selectedUsers, user]);
	};
	const unselectUser = (user: UserInfoDto) => {
		const remainingUsers = reject(selectedUsers, u => u.id === user.id);
		setSelectedUsers(remainingUsers);
	};

	const onRaceSelectionModeChanged = (mode: RaceSelectionMode) => {
		setRaceSelectionMode(mode);
		mode === RaceSelectionMode.Random && balancingMethod === "sector-rotation" && setBalancingMethod(null);
	};

	const reset = () => {
		setSelectedUsers([]);
		setRaceSelectionMode(RaceSelectionMode.TurnOrder);
		setBalancingMethod(null);
		setGameName(null);
	};
	const startGame = async () => {
		setIsCreating(true);

		const command: CreateGameCommand = {
			playerIds: selectedUsers.map(u => u.id),
			options: {
				factionSelectionMode: raceSelectionMode,
				turnOrderSelectionMode: TurnOrderSelectionMode.Random,
				auction: balancingMethod === "auction",
				rotateSectorsInSetup: balancingMethod === "sector-rotation",
				mapShape: selectedUsers.length as MapShape,
				startingVPs,
				gameName: gameName ?? `Gaia Project - ${format(new Date(), "eee d, MMMM yyyy (HH:mm)")}`,
			},
		};

		const gameId = await httpClient.post<string>("api/GaiaProject/CreateGame", command, { readAsString: true });
		navigate(`/game/${gameId}`);
	};

	return (
		<div className={classes.wrapper}>
			<div className={classes.header}>
				<Typography variant="h5" className="gaia-font">
					Create new game
				</Typography>
			</div>
			<Grid container spacing={2}>
				<Grid item xs={12} md={6} xl={3}>
					<Typography variant="h6" className="gaia-font">
						Player Selection
					</Typography>
					<Autocomplete
						key={selectedUsers.length}
						freeSolo
						inputMode="search"
						open={open}
						onOpen={() => {
							setOpen(true);
						}}
						onClose={() => {
							setOpen(false);
						}}
						onChange={(__, val, reason) => selectUser(val as Nullable<UserInfoDto>, reason)}
						isOptionEqualToValue={(option, value) => option.id === value.id}
						getOptionLabel={option => option.username}
						options={searchedUsers}
						loading={isLoading}
						disabled={selectedUsers.length === MAX_OTHER_PLAYERS}
						renderInput={params => (
							<TextField
								{...params}
								label="Add players..."
								InputLabelProps={{ className: "gaia-font" }}
								variant="standard"
								onChange={evt => searchUsers(evt.target.value)}
								autoFocus
								InputProps={{
									...params.InputProps,
									className: "gaia-font",
									endAdornment: (
										<>
											{isLoading ? <CircularProgress color="inherit" size={20} /> : null}
											{params.InputProps.endAdornment}
										</>
									),
								}}
							/>
						)}
					/>
					<div className={classes.marginTop}>
						{selectedUsers.map(user => (
							<Paper key={user.id} className={classes.selectedUser}>
								<Avatar src={user.avatar} alt={user.username} />
								<Typography variant="body1" className="username gaia-font">
									{user.username}
								</Typography>
								<IconButton aria-label="remove" color="secondary" className="removeBtn" onClick={() => unselectUser(user)} size="large">
									<DeleteIcon />
								</IconButton>
							</Paper>
						))}
					</div>
				</Grid>
				<Grid item xs={12} md={6} xl={9}>
					<Typography variant="h6" className="gaia-font">
						Game Options
					</Typography>
					<div className={classes.marginTop}>
						<FormControl component="fieldset">
							<FormLabel component="legend" className="gaia-font">
								Race selection mode
							</FormLabel>
							<RadioGroup
								aria-label="Race selection mode"
								name="Race selection mode"
								value={raceSelectionMode}
								onChange={evt => onRaceSelectionModeChanged(+evt.target.value)}
							>
								<FormControlLabel classes={{ label: "gaia-font" }} value={RaceSelectionMode.TurnOrder} control={<Radio />} label="Free Choice" />
								<FormControlLabel classes={{ label: "gaia-font" }} value={RaceSelectionMode.Random} control={<Radio />} label="Random" />
							</RadioGroup>
						</FormControl>
					</div>
					<div className={classes.marginTop}>
						<FormControl component="fieldset">
							<FormLabel component="legend" className="gaia-font">
								Balancing method
							</FormLabel>
							<RadioGroup
								aria-label="Balancing method"
								name="Balancing method"
								value={balancingMethod}
								onChange={evt => setBalancingMethod(evt.target.value as BalancingMethod)}
							>
								<FormControlLabel classes={{ label: "gaia-font" }} value={"auction"} control={<Radio />} label="Auction" />
								<FormControlLabel
									classes={{ label: "gaia-font" }}
									value={"sector-rotation"}
									control={<Radio />}
									label="Last player adjusts sector rotation"
									disabled={raceSelectionMode === RaceSelectionMode.Random}
								/>
							</RadioGroup>
						</FormControl>
					</div>
					<div className={classes.marginTop}>
						<TextField
							className="w-100"
							InputLabelProps={{ className: "gaia-font" }}
							InputProps={{ className: "gaia-font", type: "number" }}
							value={startingVPs}
							label="Starting VPs"
							onChange={evt => setStartingVPs(Math.max(+evt.target.value, 0))}
						/>
					</div>
					<div className={classes.marginTop}>
						<TextField
							className="w-100"
							InputLabelProps={{ className: "gaia-font" }}
							InputProps={{ className: "gaia-font" }}
							value={gameName}
							label="Game name (required)"
							required
							onChange={evt => setGameName(evt.target.value)}
						/>
					</div>
				</Grid>
				<Grid item xs={12}>
					<div className={classes.actions}>
						<Button variant="contained" disabled={isCreating} onClick={reset}>
							<span className="gaia-font">Cancel</span>
						</Button>
						<Button variant="contained" color="primary" disabled={isCreating || selectedUsers.length < MIN_OTHER_PLAYERS || !gameName} onClick={startGame}>
							<span className="gaia-font">Start</span>
						</Button>
					</div>
				</Grid>
			</Grid>
		</div>
	);
};

export default NewGamePage;
