import React from "react";
import { connect } from "react-redux";
import { Dispatch } from "redux";
import { onDecrement, onIncrement } from "../../redux/counter/counterCreator";

interface aboutProp {
    count: number;
    add: () => void;
    minus: () => void;
}

class About extends React.Component<aboutProp>{

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

// 通过withRouter能把一些路由信息放入到当前页面的props内
export default connect(
    (state: any) => ({
        count: state.CounterReducer.count
    }),
    (dispatch: Dispatch) => ({
        add: () => dispatch(onIncrement()),
        minus: () => dispatch(onDecrement())
    })
)(About);