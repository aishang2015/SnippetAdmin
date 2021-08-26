import { Reducer } from "redux";
import { CounterAction } from "./counterAction";
import { CounterState } from "./counterState";


export const CounterReducer: Reducer<CounterState> = (state, action: CounterAction) => {

    if (state === undefined) {
        return { count: 0 };
    }

    switch (action.type) {
        case "INCREMENT_COUNT":
            return { count: ++state.count };
        case "DECREMENT_COUNT":
            return { count: --state.count };
        default:
            return state;
    }
}