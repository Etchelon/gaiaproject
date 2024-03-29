import { initialize } from "@ionic/core/components";
import { IonAccordion } from "@ionic/core/components/ion-accordion";
import { IonAccordionGroup } from "@ionic/core/components/ion-accordion-group";
import { IonActionSheet } from "@ionic/core/components/ion-action-sheet";
import { IonAlert } from "@ionic/core/components/ion-alert";
import { IonApp } from "@ionic/core/components/ion-app";
import { IonAvatar } from "@ionic/core/components/ion-avatar";
import { IonBackdrop } from "@ionic/core/components/ion-backdrop";
import { IonBackButton } from "@ionic/core/components/ion-back-button";
import { IonBadge } from "@ionic/core/components/ion-badge";
import { IonBreadcrumb } from "@ionic/core/components/ion-breadcrumb";
import { IonBreadcrumbs } from "@ionic/core/components/ion-breadcrumbs";
import { IonButton } from "@ionic/core/components/ion-button";
import { IonButtons } from "@ionic/core/components/ion-buttons";
import { IonCard } from "@ionic/core/components/ion-card";
import { IonCardContent } from "@ionic/core/components/ion-card-content";
import { IonCardHeader } from "@ionic/core/components/ion-card-header";
import { IonCardSubtitle } from "@ionic/core/components/ion-card-subtitle";
import { IonCardTitle } from "@ionic/core/components/ion-card-title";
import { IonCheckbox } from "@ionic/core/components/ion-checkbox";
import { IonChip } from "@ionic/core/components/ion-chip";
import { IonCol } from "@ionic/core/components/ion-col";
import { IonContent } from "@ionic/core/components/ion-content";
import { IonDatetime } from "@ionic/core/components/ion-datetime";
import { IonFab } from "@ionic/core/components/ion-fab";
import { IonFabButton } from "@ionic/core/components/ion-fab-button";
import { IonFabList } from "@ionic/core/components/ion-fab-list";
import { IonFooter } from "@ionic/core/components/ion-footer";
import { IonGrid } from "@ionic/core/components/ion-grid";
import { IonHeader } from "@ionic/core/components/ion-header";
import { IonImg } from "@ionic/core/components/ion-img";
import { IonInfiniteScroll } from "@ionic/core/components/ion-infinite-scroll";
import { IonInfiniteScrollContent } from "@ionic/core/components/ion-infinite-scroll-content";
import { IonInput } from "@ionic/core/components/ion-input";
import { IonItem } from "@ionic/core/components/ion-item";
import { IonItemDivider } from "@ionic/core/components/ion-item-divider";
import { IonItemGroup } from "@ionic/core/components/ion-item-group";
import { IonItemOption } from "@ionic/core/components/ion-item-option";
import { IonItemOptions } from "@ionic/core/components/ion-item-options";
import { IonItemSliding } from "@ionic/core/components/ion-item-sliding";
import { IonLabel } from "@ionic/core/components/ion-label";
import { IonList } from "@ionic/core/components/ion-list";
import { IonListHeader } from "@ionic/core/components/ion-list-header";
import { IonLoading } from "@ionic/core/components/ion-loading";
import { IonMenu } from "@ionic/core/components/ion-menu";
import { IonMenuButton } from "@ionic/core/components/ion-menu-button";
import { IonMenuToggle } from "@ionic/core/components/ion-menu-toggle";
import { IonModal } from "@ionic/core/components/ion-modal";
import { IonNav } from "@ionic/core/components/ion-nav";
import { IonNavLink } from "@ionic/core/components/ion-nav-link";
import { IonPicker } from "@ionic/core/components/ion-picker";
import { IonPickerColumn } from "@ionic/core/components/ion-picker-column";
import { IonPopover } from "@ionic/core/components/ion-popover";
import { IonProgressBar } from "@ionic/core/components/ion-progress-bar";
import { IonRadio } from "@ionic/core/components/ion-radio";
import { IonRadioGroup } from "@ionic/core/components/ion-radio-group";
import { IonRange } from "@ionic/core/components/ion-range";
import { IonRefresher } from "@ionic/core/components/ion-refresher";
import { IonRefresherContent } from "@ionic/core/components/ion-refresher-content";
import { IonSearchbar } from "@ionic/core/components/ion-searchbar";
import { IonSegment } from "@ionic/core/components/ion-segment";
import { IonSegmentButton } from "@ionic/core/components/ion-segment-button";
import { IonSelect } from "@ionic/core/components/ion-select";
import { IonSelectOption } from "@ionic/core/components/ion-select-option";
import { IonSelectPopover } from "@ionic/core/components/ion-select-popover";
import { IonSpinner } from "@ionic/core/components/ion-spinner";
import { IonSplitPane } from "@ionic/core/components/ion-split-pane";
import { IonTab } from "@ionic/core/components/ion-tab";
import { IonTabBar } from "@ionic/core/components/ion-tab-bar";
import { IonTabButton } from "@ionic/core/components/ion-tab-button";
import { IonTabs } from "@ionic/core/components/ion-tabs";
import { IonText } from "@ionic/core/components/ion-text";
import { IonTextarea } from "@ionic/core/components/ion-textarea";
import { IonThumbnail } from "@ionic/core/components/ion-thumbnail";
import { IonToggle } from "@ionic/core/components/ion-toggle";
import { IonRow } from "@ionic/core/components/ion-row";
import { IonTitle } from "@ionic/core/components/ion-title";
import { IonToast } from "@ionic/core/components/ion-toast";
import { IonToolbar } from "@ionic/core/components/ion-toolbar";
import { IonIcon } from "ionicons/components/ion-icon";

// Prevents exception when hot reloading.
function tryDefine(tag: string, impl: any) {
	try {
		customElements.define(tag, impl);
	} catch (error) {}
}

export const setupIonic = async () => {
	initialize();
	tryDefine("ion-accordion-group", IonAccordionGroup);
	tryDefine("ion-accordion", IonAccordion);
	tryDefine("ion-action-sheet", IonActionSheet);
	tryDefine("ion-alert", IonAlert);
	tryDefine("ion-app", IonApp);
	tryDefine("ion-avatar", IonAvatar);
	tryDefine("ion-back-button", IonBackButton);
	tryDefine("ion-backdrop", IonBackdrop);
	tryDefine("ion-badge", IonBadge);
	tryDefine("ion-breadcrumb", IonBreadcrumb);
	tryDefine("ion-breadcrumbs", IonBreadcrumbs);
	tryDefine("ion-button", IonButton);
	tryDefine("ion-buttons", IonButtons);
	tryDefine("ion-card-content", IonCardContent);
	tryDefine("ion-card-header", IonCardHeader);
	tryDefine("ion-card-subtitle", IonCardSubtitle);
	tryDefine("ion-card-title", IonCardTitle);
	tryDefine("ion-card", IonCard);
	tryDefine("ion-checkbox", IonCheckbox);
	tryDefine("ion-chip", IonChip);
	tryDefine("ion-col", IonCol);
	tryDefine("ion-content", IonContent);
	tryDefine("ion-datetime", IonDatetime);
	tryDefine("ion-fab-button", IonFabButton);
	tryDefine("ion-fab-list", IonFabList);
	tryDefine("ion-fab", IonFab);
	tryDefine("ion-footer", IonFooter);
	tryDefine("ion-grid", IonGrid);
	tryDefine("ion-header", IonHeader);
	tryDefine("ion-icon", IonIcon);
	tryDefine("ion-img", IonImg);
	tryDefine("ion-input", IonInput);
	tryDefine("ion-item-divider", IonItemDivider);
	tryDefine("ion-item-group", IonItemGroup);
	tryDefine("ion-item-option", IonItemOption);
	tryDefine("ion-item-options", IonItemOptions);
	tryDefine("ion-item-sliding", IonItemSliding);
	tryDefine("ion-item", IonItem);
	tryDefine("ion-infinite-scroll", IonInfiniteScroll);
	tryDefine("ion-infinite-scroll-content", IonInfiniteScrollContent);
	tryDefine("ion-label", IonLabel);
	tryDefine("ion-list-header", IonListHeader);
	tryDefine("ion-list", IonList);
	tryDefine("ion-loading", IonLoading);
	tryDefine("ion-menu-button", IonMenuButton);
	tryDefine("ion-menu-toggle", IonMenuToggle);
	tryDefine("ion-menu", IonMenu);
	tryDefine("ion-modal", IonModal);
	tryDefine("ion-nav-link", IonNavLink);
	tryDefine("ion-nav", IonNav);
	tryDefine("ion-picker-column", IonPickerColumn);
	tryDefine("ion-picker", IonPicker);
	tryDefine("ion-popover", IonPopover);
	tryDefine("ion-progress-bar", IonProgressBar);
	tryDefine("ion-radio-group", IonRadioGroup);
	tryDefine("ion-radio", IonRadio);
	tryDefine("ion-range", IonRange);
	tryDefine("ion-refresher-content", IonRefresherContent);
	tryDefine("ion-refresher", IonRefresher);
	tryDefine("ion-row", IonRow);
	tryDefine("ion-searchbar", IonSearchbar);
	tryDefine("ion-segment-button", IonSegmentButton);
	tryDefine("ion-segment", IonSegment);
	tryDefine("ion-select-option", IonSelectOption);
	tryDefine("ion-select-popover", IonSelectPopover);
	tryDefine("ion-select", IonSelect);
	tryDefine("ion-spinner", IonSpinner);
	tryDefine("ion-split-pane", IonSplitPane);
	tryDefine("ion-tab-bar", IonTabBar);
	tryDefine("ion-tab-button", IonTabButton);
	tryDefine("ion-tab", IonTab);
	tryDefine("ion-tabs", IonTabs);
	tryDefine("ion-text", IonText);
	tryDefine("ion-textarea", IonTextarea);
	tryDefine("ion-thumbnail", IonThumbnail);
	tryDefine("ion-title", IonTitle);
	tryDefine("ion-toast", IonToast);
	tryDefine("ion-toggle", IonToggle);
	tryDefine("ion-toolbar", IonToolbar);

	// Applies required global styles
	document.documentElement.classList.add("ion-ce");
};
