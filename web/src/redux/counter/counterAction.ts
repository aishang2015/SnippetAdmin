export interface IncrementAction {
    type: "INCREMENT_COUNT"
}

export interface DecrementAction {
    type: "DECREMENT_COUNT"
}

export type CounterAction = IncrementAction | DecrementAction;