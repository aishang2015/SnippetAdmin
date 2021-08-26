export interface ReceiveMessageAction {
    type: "RECEIVE_MESSAGE",
    message?: string,
}

export interface ClearMessageAction {
    type: "CLEAR_MESSAGE"
}

export type NotificationAction = ReceiveMessageAction | ClearMessageAction;