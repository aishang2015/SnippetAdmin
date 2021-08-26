import './home.less';

import React from "react";
import { getUserInfo } from '../../http/requests/account';

interface home {
    id?: number;
    userName?: string;
    email?: string;
    phoneNumber?: string;
}

export default class Home extends React.Component<any, home>{

    constructor(props: any) {
        super(props);
        this.state = {};
    }

    async componentDidMount() {
        try {
            let response = await getUserInfo();
            let data = response.data.data;
            this.setState({
                id: data.id,
                userName: data.userName,
                email: data.email,
                phoneNumber: data.phoneNumber
            });
        } catch (err) {
            return;
        }
    }

    render() {
        return (
            <div>
                <p>登录用户信息</p>
                <p>id:{this.state.id}</p>
                <p>登录名:{this.state.userName}</p>
                <p>邮箱:{this.state.email}</p>
                <p>电话:{this.state.phoneNumber}</p>
            </div>
        );
    }


}