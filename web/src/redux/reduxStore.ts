import { combineReducers, createStore } from "redux";
import { CounterReducer } from "./counter/counterReducer";
import { NavCollapsedReducer } from "./navCollapsed/navCollapsedReducer";
import { NotificationReducer } from "./notification/notificationReducer";

export class ReduxStore {

    static initReduxStore() {
        const reducer = combineReducers({
            CounterReducer,
            NavCollapsedReducer,
            NotificationReducer
        });

        return createStore(reducer);
    }
}