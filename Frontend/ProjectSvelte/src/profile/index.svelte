<script lang="ts">
	import { useAuth0 } from "src/auth";
	import { getAppContext } from "../App.context";

	const { user } = useAuth0;
</script>

<h1>Profile page for user {$user?.nickname}</h1>
