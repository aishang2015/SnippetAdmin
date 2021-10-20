import { Button, Card, Form, Input } from "antd";
import React from "react";
import { GithubOutlined, UserOutlined, LockOutlined } from '@ant-design/icons';

import './bind.less';
import { BindingModel, bindingThirdPartyAccount } from "../../http/requests/account";
import { StorageService } from "../../common/storage";

interface BindState {
    thirdUserName?: string;
    thirdType?: string;
    thirdCachekey?: string;
}

export class Bind extends React.Component<any, BindState> {


    constructor(props: any) {
        super(props);
        this.state = {};

        // 禁止后退到回调页面
        window.history.pushState(null, '', document.URL);
        window.addEventListener('popstate', function () {
            window.history.pushState(null, '', document.URL);
        });
    }

    componentDidMount() {
        this.setState({
            thirdUserName: StorageService.getThirdPartyUserName()!,
            thirdType: StorageService.getThirdPartyType()!,
            thirdCachekey: StorageService.getThirdPartyInfoCacheKey()!
        });
    }

    render() {
        return (
            <div className="bind-window">
                <Card className="bind-card">
                    <div className="logo-contaier">
                        {this.state.thirdType === "github" &&
                            <GithubOutlined />
                        }
                        {this.state.thirdType === "baidu" &&
                            <span>Baidu</span>
                        }
                    </div>
                    <div>{this.state.thirdUserName}</div>

                    <Form name="normal_login" className="login-form" onFinish={this.bindAccount.bind(this)}>
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
                            <Button type="primary" block htmlType="submit">绑定</Button>
                        </Form.Item>
                    </Form>
                </Card>
            </div>

        );
    }

    // 绑定账号并登录
    async bindAccount(values: any) {
        let model: BindingModel = {
            userName: values.username,
            password: values.password,
            thirdPartyType: this.state.thirdType!,
            thirdPartyInfoCacheKey: this.state.thirdCachekey!
        };
        try {
            let response = await bindingThirdPartyAccount(model);
            let result = response.data.data;

            // 清理第三方账号信息
            StorageService.clearOauthStore();

            // 保存登录信息
            StorageService.setLoginStore(result.accessToken, result.userName, result.expire.toString(), result.identifies);
            window.location.reload();
        } catch (err) {
            return;
        }
    }
}