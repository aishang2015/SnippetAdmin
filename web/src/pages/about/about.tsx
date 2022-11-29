import React from "react";

interface aboutProp {
    count: number;
    add: () => void;
    minus: () => void;
}

export default class About extends React.Component<any>{

    render() {
        return (
            <div>
                <div>this is about page</div>
                <div>{this.props.count}</div>
                <button onClick={this.props.add}>+</button>
                <button onClick={this.props.minus}>-</button>
            </div>
        );
    }
}
