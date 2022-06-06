<script lang="ts">
	import { getAppContext } from "../App.context";

	const {
		auth: { loggedUser },
	} = getAppContext();
</script>

<h1>Profile page for user {$loggedUser.username}</h1>
