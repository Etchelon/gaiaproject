import { RefObject, useEffect, useLayoutEffect, useState } from "react";
import { UserInfoDto } from "../dto/interfaces";
import { Nullable } from "./miscellanea";
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

export function useAssetUrl(relativeUrl: string): string {
	const [imgUrl, setImgUrl] = useState("");

	useEffect(() => {
		import(`../assets/Resources/${relativeUrl}`).then(imgModule => setImgUrl(imgModule.default)).catch(err => setImgUrl(""));
	}, [relativeUrl]);

	return imgUrl;
}

export function useCurrentUser(): Nullable<UserInfoDto> {
	const [currentUser, setCurrentUser] = useState<Nullable<UserInfoDto>>(null);
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
