import { Reducer } from "redux";
import { NavCollapsedAction } from "./navCollapsedAction";
import { NavCollapsedState } from "./navCollapsedState";


export const NavCollapsedReducer: Reducer<NavCollapsedState> = (state, action: NavCollapsedAction) => {

    if (state === undefined) {
        return { collapsed: false };
    }
    return action.type === "TOGGLE_COLLAPSED" ? {
        collapsed: !state.collapsed
    } : state;
}