import { ClearMessageAction, ReceiveMessageAction } from './notificationAction';

export const onReceiveMessage = (msg: string): ReceiveMessageAction => ({ type: "RECEIVE_MESSAGE", message: msg });
export const onClearMessage = (): ClearMessageAction => ({ type: "CLEAR_MESSAGE" });