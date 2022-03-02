import _ from "lodash";
import { Dispatch } from "redux";
import { BehaviorSubject, Subject } from "rxjs";
import { ActionType } from "../../dto/enums";
import { ActionDto, InteractionStateDto } from "../../dto/interfaces";
import { Identifier, Nullable } from "../../utils/miscellanea";
import { InteractiveElementState, InteractiveElementType } from "./enums";
import { Command, InteractiveElement, WorkflowState } from "./types";

export abstract class ActionWorkflow {
	states: WorkflowState[] = [];
	private readonly _switchToAction = new Subject<Nullable<ActionType>>();
	switchToAction$ = this._switchToAction.asObservable();
	private readonly _currentState = new BehaviorSubject<WorkflowState>({ id: 0, message: "" });
	currentState$ = this._currentState.asObservable();
	get currentState(): WorkflowState {
		return this._currentState.getValue();
	}
	set currentState(val: WorkflowState) {
		val.interactiveElements = this.getInteractiveElements();
		this._currentState.next(val);
	}
	protected get stateId(): number {
		return this.currentState.id;
	}
	protected dispatch: Nullable<Dispatch> = null;

	constructor(protected interactionState: Nullable<InteractionStateDto> = null, skipInit = true) {
		if (skipInit) {
			return;
		}
		this.init();
	}

	protected abstract init(): void;
	abstract handleCommand(command: Command): Nullable<ActionDto>;

	protected switchToAction(type: ActionType): void {
		this._switchToAction.next(type);
	}
	protected cancelAction(): void {
		this._switchToAction.next(null);
	}

	elementSelected(id: Identifier, type: InteractiveElementType): void {}

	protected updateState(state: WorkflowState): void {
		this.currentState = state;
	}

	advanceState(next: Nullable<number> = null, message: Nullable<string> = null, commands: Nullable<Command[]> = null): void {
		next = next ?? this.currentState.id + 1;
		const state = _.find(this.states, s => s.id === next);
		if (!state) {
			throw new Error(`Cannot advance to state with id ${next}`);
		}

		if (!_.isNil(message)) {
			state.message = message;
		}
		if (!_.isNil(commands)) {
			state.commands = commands;
		}
		this.currentState = state;
	}

	protected notifyInteractionStateChanged(): void {
		// tslint:disable-next-line:no-self-assign
		this.currentState = this.currentState;
	}

	protected getInteractiveElements(): InteractiveElement[] {
		const ret: InteractiveElement[] = [];

		const addElements = (propSelector: () => Identifier[] | undefined, type: InteractiveElementType) => {
			const mapper = mapSelectableElement(type);
			ret.push(..._.map(propSelector(), hexId => mapper(hexId)));
		};

		addElements(() => this.interactionState?.clickableAdvancedTiles, InteractiveElementType.AdvancedTile);
		addElements(() => this.interactionState?.clickableFederations, InteractiveElementType.FederationToken);

		ret.push(
			..._.map(this.interactionState?.clickableHexes, h => {
				const el: InteractiveElement = {
					id: h.id,
					type: InteractiveElementType.Hex,
					state: InteractiveElementState.Enabled,
				};
				h.requiredQics && (el.notes = `Spend ${h.requiredQics} QICs for range boost`);
				return el;
			})
		);
		addElements(() => this.interactionState?.clickableOwnAdvancedTiles, InteractiveElementType.OwnAdvancedTile);
		addElements(() => this.interactionState?.clickableOwnFederations, InteractiveElementType.OwnFederationToken);
		addElements(() => this.interactionState?.clickableOwnStandardTiles, InteractiveElementType.OwnStandardTile);

		ret.push(
			..._.map(this.interactionState?.clickablePowerActions, pa => {
				const el: InteractiveElement = {
					id: pa.type,
					type: InteractiveElementType.PowerAction,
					state: InteractiveElementState.Enabled,
				};
				pa.powerToBurn && (el.notes = `Burn ${pa.powerToBurn}`);
				return el;
			})
		);
		addElements(() => this.interactionState?.clickableQicActions, InteractiveElementType.QicAction);
		addElements(() => _.map(this.interactionState?.clickableResearchTracks, rt => rt.track), InteractiveElementType.ResearchStep);
		addElements(() => this.interactionState?.clickableRoundBoosters, InteractiveElementType.RoundBooster);
		addElements(() => this.interactionState?.clickableStandardTiles, InteractiveElementType.StandardTile);
		this.interactionState?.canUseOwnRoundBooster && ret.push({ type: InteractiveElementType.OwnRoundBooster, state: InteractiveElementState.Enabled });
		this.interactionState?.canUsePlanetaryInstitute && ret.push({ type: InteractiveElementType.PlanetaryInstitute, state: InteractiveElementState.Enabled });
		this.interactionState?.canUseRaceAction && ret.push({ type: InteractiveElementType.RaceAction, state: InteractiveElementState.Enabled });
		this.interactionState?.canUseRightAcademy && ret.push({ type: InteractiveElementType.RightAcademy, state: InteractiveElementState.Enabled });

		return ret;
	}

	setDispatch(dispatch: Dispatch): void {
		this.dispatch = dispatch;
	}
}

const mapSelectableElement = (type: InteractiveElementType) => (id: Identifier) => {
	const ret: InteractiveElement = {
		id,
		type,
		state: InteractiveElementState.Enabled,
	};
	return ret;
};
