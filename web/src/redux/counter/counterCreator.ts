import { IncrementAction, DecrementAction } from './counterAction';

export const onIncrement = (): IncrementAction => ({ type: "INCREMENT_COUNT" });
export const onDecrement = (): DecrementAction => ({ type: "DECREMENT_COUNT" });