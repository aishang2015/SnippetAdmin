import './login.css';

import React, { useEffect, useState } from "react";
import { Button, Card, Form, Input, Space } from 'antd';
import { LoginModel, getCaptcha, login } from '../../../http/requests/basic/account';
import { OauthService } from '../../../common/oauth';
import { StorageService } from '../../../common/storage';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCode, faKey, faSignIn, faUser } from '@fortawesome/free-solid-svg-icons';
import { faGithub } from '@fortawesome/free-brands-svg-icons';
import { SettingService } from '../../../http/requests/system/setting';
import { Configuration } from '../../../common/config';
import ParticlesBg from 'particles-bg';
import { random } from 'lodash';

export default function Login() {

    const [isLoading, setIsLoading] = useState(false);
    const [background, setBackground] = useState('');
    const [icon, setIcon] = useState('logo192.png');
    const [title, setTitle] = useState('SnippetAdmin');
    const [captcha, setCaptcha] = useState('');
    const [captchaKey, setCaptchaKey] = useState('');

    useEffect(() => {

        localStorage.clear();
        StorageService.clearOauthStore();

        loadCaptcha();
    }, []);


    async function loadCaptcha() {

        let response = await getCaptcha();

        let captchaKey = response.headers["x-captcha-key"]!;
        setCaptchaKey(captchaKey)
        let blob = new Blob([response.data], { type: response.data.type });

        setCaptcha(URL.createObjectURL(blob));
    }


    return (
        <>
            <div className="full-window">
                <Card className="login-card">
                    <div className="logo-contaier">
                        {/* <img className="logo" src={icon} /> */}
                        <span className="logo-text">{title}</span>
                    </div>
                    <div className='form-container'>
                        <Form name="normal_login" onFinish={loginClick}>
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

                            <div style={{ display: 'flex' }}>
                                <Form.Item name="captcha"
                                    rules={[{ required: true, message: '请输入验证码!' }]}>
                                    <Input prefix={<FontAwesomeIcon fixedWidth icon={faCode} />} style={{ width: "90%" }}
                                        type="captcha" placeholder="验证码" autoComplete="off" />
                                </Form.Item>
                                <div style={{ flex: 1 }}></div>
                                {captcha &&
                                    <img style={{ height: "32px" }} src={captcha} alt="验证码" onClick={loadCaptcha} />
                                }
                            </div>


                            <Form.Item>
                                <Button block type="default" htmlType="submit"
                                    loading={isLoading} ><FontAwesomeIcon icon={faSignIn} />&nbsp;&nbsp;&nbsp;登录</Button>
                            </Form.Item>
                        </Form>
                    </div>
                </Card>
                <div className='copy-right-bar'>CopyRight All Rights Reserved</div>
            </div>
        </>
    )

    async function loginClick(values: any) {
        let model: LoginModel = {
            userName: values.username,
            password: values.password,
            captchaKey: captchaKey,
            captchaCode: values.captcha
        };

        try {
            setIsLoading(true);

            let response = await login(model);

            let result = response.data.data;

            // 保存登录信息
            StorageService.setLoginStore(result.accessToken, result.userName, result.expire.toString(),
                result.identifies);
            window.location.replace('/');

        } catch (err) {
            setIsLoading(false);
            return;
        }
    }
}
