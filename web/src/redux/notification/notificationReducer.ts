import { Reducer } from "redux";
import { NotificationAction, ReceiveMessageAction } from "./notificationAction";
import { NotificationState } from "./notificationState";


export const NotificationReducer: Reducer<NotificationState> = (state, action: NotificationAction) => {

    if (state === undefined) {
        return { notifications: [] };
    }

    switch (action.type) {
        case "RECEIVE_MESSAGE":
            let msg = (action as ReceiveMessageAction).message;
            return {
                notifications: msg ?
                    state.notifications.concat(msg) : state.notifications
            };
        case "CLEAR_MESSAGE":
            return { notifications: [] };
        default:
            return state;
    }
}