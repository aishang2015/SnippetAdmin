import './login.css';

import React from "react";
import { Button, Card, Form, Input } from 'antd';
import { LoginModel, login } from '../../http/requests/account';
import { OauthService } from '../../common/oauth';
import { StorageService } from '../../common/storage';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faKey, faUser } from '@fortawesome/free-solid-svg-icons';
import { faGithub } from '@fortawesome/free-brands-svg-icons';
import { SettingService } from '../../http/requests/setting';
import { Configuration } from '../../common/config';

export default class Login extends React.Component<any, any> {

    constructor(props: any) {
        super(props);
        this.state = {
            isLoading: false,
            background: '',
            icon: '',
            title: '',
        };
    }

    componentDidMount() {
        StorageService.clearOauthStore();
        this.init();
    }

    async init() {
        try {
            let response = await SettingService.getLoginPageSetting();
            let loginPageSetting = response.data.data;
            if (loginPageSetting.background !== null &&
                loginPageSetting.background !== undefined &&
                loginPageSetting.background !== '') {
                this.setState({
                    background: `url("${Configuration.BaseUrl + '/store/' + loginPageSetting.background}")`
                });
            } else {
                this.setState({
                    background: `url("images/backgroud.jpg")`
                });
            }

            if (loginPageSetting.icon !== null &&
                loginPageSetting.icon !== undefined &&
                loginPageSetting.icon !== '') {
                this.setState({
                    icon: `${Configuration.BaseUrl + '/store/' + loginPageSetting.icon}`
                });
            } else {
                this.setState({
                    icon: "logo192.png"
                });
            }

            if (loginPageSetting.title !== null &&
                loginPageSetting.title !== undefined &&
                loginPageSetting.title !== '') {
                this.setState({
                    title: loginPageSetting.title
                });
            } else {
                this.setState({
                    title: 'Snippet Admin'
                });
            }
        } catch (e) {
            this.setState({
                background: `url("images/backgroud.jpg")`,
                icon: "logo192.png",
                title: 'Snippet Admin',
            });
        }
    }

    render() {
        return (
            <div className="full-window" style={{
                //backgroundImage: `url("https://iph.href.lu/1920x1080")`
                backgroundImage: this.state.background,
                backgroundSize: '100% 100%'
            }}>
                <Card className="login-card">
                    <div className="logo-contaier">
                        <img className="logo" src={this.state.icon} />
                        <span className="logo-text">{this.state.title}</span>
                    </div>
                    <div className='form-container'>
                        <Form name="normal_login" onFinish={this.login.bind(this)}>
                            <Form.Item name="username"
                                rules={[{ required: true, message: '请输入你的用户名!' }]}>
                                <Input prefix={<FontAwesomeIcon fixedWidth icon={faUser} />}
                                    placeholder="用户名" autoComplete="off" />
                            </Form.Item>
                            <Form.Item name="password"
                                rules={[{ required: true, message: '请输入你的密码!' }]}>
                                <Input prefix={<FontAwesomeIcon fixedWidth icon={faKey} />}
                                    type="password" placeholder="密码" autoComplete="off" />
                            </Form.Item>

                            <Form.Item>
                                <Button block type="default" htmlType="submit" loading={this.state.isLoading}>登录</Button>
                            </Form.Item>
                        </Form>
                    </div>
                    <div className="thrid-login-bar">
                        <Button shape="circle" type="default" icon={<FontAwesomeIcon icon={faGithub} />} onClick={() => this.githubLogin()} />
                        <Button shape="circle" type="default" onClick={() => this.baiduLogin()} >Ba</Button>
                    </div>
                </Card>
                <div className='copy-right-bar'>CopyRight All Rights Reserved</div>
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
                result.identifies);
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
