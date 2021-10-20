import './login.less';

import React from "react";
import { Button, Card, Form, Input } from 'antd';
import { UserOutlined, LockOutlined, GithubOutlined } from '@ant-design/icons';
import { LoginModel, login } from '../../http/requests/account';
import { withRouter } from 'react-router-dom';
import { OauthService } from '../../common/oauth';
import { StorageService } from '../../common/storage';

class Login extends React.Component<any, any> {

    constructor(props: any) {
        super(props);
        this.state = {
            isLoading: false
        };
    }

    componentDidMount() {
        StorageService.clearOauthStore();
    }

    render() {
        return (
            <div className="full-window">
                <Card className="login-card">
                    <div className="logo-contaier">
                        <img className="logo" src="logo192.png" alt="logo" />
                    </div>
                    <Form name="normal_login" onFinish={this.login.bind(this)}>
                        <Form.Item name="username"
                            rules={[{ required: true, message: '请输入你的用户名!' }]}>
                            <Input prefix={<UserOutlined className="site-form-item-icon" />}
                                placeholder="用户名" autoComplete="off" />
                        </Form.Item>
                        <Form.Item name="password"
                            rules={[{ required: true, message: '请输入你的密码!' }]}>
                            <Input prefix={<LockOutlined className="site-form-item-icon" />}
                                type="password" placeholder="密码" autoComplete="off" />
                        </Form.Item>

                        <Form.Item>
                            <Button type="primary" block htmlType="submit" loading={this.state.isLoading}>登录</Button>
                        </Form.Item>
                    </Form>
                    <div className="thrid-login-bar">
                        <Button shape="circle" type="default" icon={<GithubOutlined />} onClick={() => this.githubLogin()} />
                        <Button shape="circle" type="default" onClick={() => this.baiduLogin()} >Ba</Button>
                    </div>
                </Card>
            </div>
        )
    }

    async login(values: any) {
        let model: LoginModel = {
            userName: values.username,
            password: values.password
        };

        try {
            this.setState({ isLoading: true });

            let response = await login(model);

            let result = response.data.data;

            // 保存登录信息
            StorageService.setLoginStore(result.accessToken, result.userName, result.expire.toString(), 
                result.identifies,result.refreshToken);
            window.location.reload();

        } catch (err) {
            this.setState({ isLoading: false });
            return;
        }
    }

    githubLogin() {
        OauthService.githubLogin();
    }

    baiduLogin() {
        OauthService.baiduLogin();
    }
}

export default withRouter(Login);