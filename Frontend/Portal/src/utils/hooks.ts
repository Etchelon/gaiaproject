import _ from "lodash";
import { createRef, RefObject, useCallback, useEffect, useLayoutEffect, useState } from "react";
import { UserInfoDto } from "../dto/interfaces";
import { Nullable, UniversalFn } from "./miscellanea";
import userInfoService from "./user-info.service";

export interface ElementSize {
	width: number;
	height: number;
}

export const useWindowSize = () => {
	const [windowSize, setWindowSize] = useState<ElementSize | null>(null);

	useEffect(() => {
		// Handler to call on window resize
		function handleResize() {
			// Set window width/height to state
			setWindowSize({
				width: window.innerWidth,
				height: window.innerHeight,
			});
		}

		// Add event listener
		window.addEventListener("resize", handleResize);

		// Call handler right away so state gets updated with initial window size
		handleResize();

		// Remove event listener on cleanup
		return () => window.removeEventListener("resize", handleResize);
	}, []); // Empty array ensures that effect is only run on mount

	return windowSize;
};

const getDimensions = (element: HTMLElement | null) =>
	element
		? {
				width: element.clientWidth,
				height: element.clientHeight,
		  }
		: { width: 0, height: 0 };

export const useContainerDimensions = (containerRef: RefObject<HTMLElement>, watch = false) => {
	const [dimensions, setDimensions] = useState<ElementSize>({ width: 0, height: 0 });

	useLayoutEffect(() => {
		if (containerRef.current === null) {
			return;
		}
		const dimensions_ = getDimensions(containerRef.current);
		setDimensions(dimensions_);
		if (!watch) {
			return;
		}

		const handleResize = () => {
			const dimensions_ = getDimensions(containerRef.current);
			setDimensions(dimensions_);
		};

		window.addEventListener("resize", handleResize);
		return () => {
			window.removeEventListener("resize", handleResize);
		};
	}, [containerRef, watch]);

	return dimensions;
};

const ALL_RESOURCES = import.meta.glob("../assets/Resources/**/*.(png|jpg|wav)");

export function useAssetUrl(relativeUrl: string): string {
	const [assetUrl, setAssetUrl] = useState("");

	useEffect(() => {
		const url = `../assets/Resources/${relativeUrl}`;
		ALL_RESOURCES[url]?.()
			.then(m => setAssetUrl(m.default))
			.catch(() => setAssetUrl(""));
	}, [relativeUrl]);

	return assetUrl;
}

export function useCurrentUser(): Nullable<UserInfoDto> {
	const [currentUser, setCurrentUser] = useState<Nullable<UserInfoDto>>(userInfoService.getCurrentUser());
	useEffect(() => {
		const sub = userInfoService.userInfo$.subscribe(user => {
			setCurrentUser(user);
		});
		return () => {
			sub.unsubscribe();
		};
	}, []);
	return currentUser;
}

export interface ScrollableNode {
	clientHeight: number;
	addEventListener(eventName: string, handlerFn: (...args: any[]) => any): void;
	removeEventListener(eventName: string, handlerFn: (...args: any[]) => any): void;
}

/**
 * Check if an element is in viewport
 * @param offset - Number of pixels up to the observable element from the top
 * @param throttleMilliseconds - Throttle observable listener, in ms
 */
export function useVisibility<Element extends HTMLElement>(
	parentScrollable: Nullable<ScrollableNode | (Window & typeof globalThis)>,
	offset = 0,
	throttleMilliseconds = 100
): [boolean, RefObject<Element>] {
	const [isVisible, setIsVisible] = useState(false);
	const currentElement = createRef<Element>();

	const onScroll = _.throttle(() => {
		if (!currentElement.current) {
			setIsVisible(false);
			return;
		}
		const scrollableHeight = parentScrollable === window ? window.innerHeight : (parentScrollable as ScrollableNode)?.clientHeight;
		const top = currentElement.current.getBoundingClientRect().top;
		const inViewport = top + offset >= 0 && top - offset <= scrollableHeight;
		setIsVisible(inViewport);
	}, throttleMilliseconds);

	useEffect(() => {
		if (parentScrollable === null) {
			return;
		}
		parentScrollable.addEventListener("scroll", onScroll);
		return () => parentScrollable.removeEventListener("scroll", onScroll);
	});

	useEffect(() => {
		if (parentScrollable === null) {
			return;
		}
		onScroll();
	}, [parentScrollable]);

	return [isVisible, currentElement];
}

export function usePageActivation(onActive: UniversalFn, onInactive: UniversalFn, deps: any[] = []) {
	const handleTabActivationChange = useCallback((isActive: boolean) => {
		if (isActive) {
			onActive();
		} else {
			onInactive();
		}
	}, []);

	useEffect(() => {
		// Hp: the call to this hook is initially done when the page is active
		handleTabActivationChange(true);

		const onVisibilityChange = () => {
			const isVisible = document.visibilityState === "visible";
			handleTabActivationChange(isVisible);
		};
		document.addEventListener("visibilitychange", onVisibilityChange);
		const onPageShow = () => {
			handleTabActivationChange(true);
		};
		window.addEventListener("pageshow", onPageShow);
		const onPageHide = () => {
			handleTabActivationChange(false);
		};
		window.addEventListener("pagehide", onPageHide);

		return () => {
			document.removeEventListener("visibilitychange", onVisibilityChange);
			window.removeEventListener("pageshow", onPageShow);
			window.removeEventListener("pagehide", onPageHide);
		};
	}, deps);
}
