
export class EventService {

    private static _eventEmitter: any;

    static EventEmitterInstance() {
        if (this._eventEmitter === undefined) {
            var EventEmitter = require('events');
            this._eventEmitter = new EventEmitter();
        }
        return this._eventEmitter;
    }

    static Subscribe(eventName: string, action: Function) {
        this.EventEmitterInstance().on(eventName, action);
    }

    static UnSubscribe(eventName: string, action: Function) {
        this.EventEmitterInstance().off(eventName, action);
    }

    static Emit(eventName: string, [...args]) {
        this.EventEmitterInstance().emit(eventName, [...args]);
    }


}