import { HubConnectionState } from "@microsoft/signalr";
import { includes, isNil } from "lodash";
import { derived } from "svelte/store";
import type {
	AdvancedTechnologyTileType,
	FederationTokenType,
	ResearchTrackType,
	RoundBoosterType,
	StandardTechnologyTileType,
} from "../../dto/enums";
import type { Identifier } from "../../utils/miscellanea";
import { InteractiveElementState, InteractiveElementType } from "../workflows/enums";
import type { GamePageStore } from "./GamePage.store";

const deriveInteractionState = (type: InteractiveElementType) => (id: Identifier) => (store: GamePageStore, playerId?: string) =>
	derived([store.currentUserId, store.interactiveElements], ([$currentUserId, $interactiveElements]) => {
		if (!isNil(playerId) && playerId !== $currentUserId) {
			return { isClickable: false, isSelected: false };
		}
		const interactiveElement = $interactiveElements.find(el => el.type === type && el.id === id);
		const interactionState = interactiveElement?.state ?? InteractiveElementState.Disabled;
		const isClickable = interactionState === InteractiveElementState.Enabled;
		const isSelected = interactionState === InteractiveElementState.Selected;
		return { isClickable, isSelected, notes: interactiveElement?.notes };
	});
export const deriveHexInteractionState = (hexId: string) => deriveInteractionState(InteractiveElementType.Hex)(hexId);
export const deriveAdvancedTileInteractionState = (type: AdvancedTechnologyTileType) =>
	deriveInteractionState(InteractiveElementType.AdvancedTile)(type);
export const deriveStandardTileInteractionState = (type: StandardTechnologyTileType) =>
	deriveInteractionState(InteractiveElementType.StandardTile)(type);
export const deriveOwnStandardTileInteractionState = (type: StandardTechnologyTileType) =>
	deriveInteractionState(InteractiveElementType.OwnStandardTile)(type);
export const deriveOwnAdvancedTileInteractionState = (type: AdvancedTechnologyTileType) =>
	deriveInteractionState(InteractiveElementType.OwnAdvancedTile)(type);
export const deriveResearchTrackInteractionState = (type: ResearchTrackType) =>
	deriveInteractionState(InteractiveElementType.ResearchStep)(type);
export const deriveActionSpaceInteractionState = (elementType: InteractiveElementType) => (type: number) =>
	deriveInteractionState(elementType)(type);
export const deriveRoundBoosterInteractionState = (elementType: InteractiveElementType) => (type: RoundBoosterType) =>
	deriveInteractionState(elementType)(type);
export const deriveFederationTokenStackInteractionState = (token: FederationTokenType) =>
	deriveInteractionState(InteractiveElementType.FederationToken)(token);
export const deriveOwnFederationTokenInteractionState = (type: FederationTokenType) =>
	deriveInteractionState(InteractiveElementType.OwnFederationToken)(type);
export const deriveIsOnline = (playerId: string) => (store: GamePageStore) =>
	derived([store.currentUserId, store.hubState], ([$currentUserId, $hubState]) => {
		const isCurrentPlayer = playerId === $currentUserId;
		return isCurrentPlayer ? $hubState.connectionState === HubConnectionState.Connected : includes($hubState.onlineUsers, playerId);
	});
