import styles from "./ClickableRectangle.module.scss";

interface ClickableRectangleProps {
	active: boolean;
	style: React.CSSProperties;
	onClick(...args: any): void;
}

const ClickableRectangle = ({ active, style, onClick }: ClickableRectangleProps) => {
	return <div className={`${styles.root}${active ? ` ${styles.active}` : ""}`} style={style} onClick={onClick}></div>;
};

export default ClickableRectangle;
