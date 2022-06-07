import axios, { AxiosInstance, AxiosResponse } from 'axios';
import { message } from 'antd';
import { Configuration } from '../common/config';
import { LoginResult, refresh } from './requests/account';
import { CommonResult } from './common-result';
import { StorageService } from '../common/storage';

export class Axios {

    // http错误
    static codeMessage: { [key: string]: string } = {
        200: '服务器成功返回请求的数据。',
        201: '新建或修改数据成功。',
        202: '一个请求已经进入后台排队（异步任务）。',
        204: '删除数据成功。',
        400: '发出的请求有错误，服务器没有进行新建或修改数据的操作。',
        401: '用户没有权限（令牌、用户名、密码错误）。',
        403: '没有接口访问权限，访问被禁止。',
        404: '发出的请求针对的是不存在的记录，服务器没有进行操作。',
        406: '请求的格式不可得。',
        410: '请求的资源被永久删除，且不会再得到的。',
        422: '当创建一个对象时，发生一个验证错误。',
        500: '服务器发生错误，请检查服务器。',
        502: '网关错误。',
        503: '服务不可用，服务器暂时过载或维护。',
        504: '网关超时。',
    };

    // 创建实例
    static instance: AxiosInstance;

    static initAxiosInstance() {

        Axios.instance = axios.create({
            baseURL: Configuration.BaseUrl,
        });

       /*  // 添加检测token过期时间拦截器
        Axios.instance.interceptors.request.use(
            async config => {
                let userName = localStorage.getItem("user-name");
                let refreshToken = localStorage.getItem("refresh-token");
                let token = localStorage.getItem("token");
                let expires = localStorage.getItem("expire");
                if (token && expires && userName && refreshToken) {
                    if (new Date() > new Date(expires)) {
                        let response = await axios.create({
                            baseURL: Configuration.BaseUrl,
                        }).post<CommonResult<LoginResult>>('api/account/refresh', {
                            userName: userName,
                            jwtToken: token,
                            refreshToken: refreshToken
                        });

                        if (response.data.isSuccess) {
                            let result = response.data.data;

                            // 保存登录信息
                            StorageService.setLoginStore(result.accessToken, result.userName, result.expire.toString(),
                                result.identifies, result.refreshToken);
                        } else {

                            // 清空登录信息
                            message.error(`${response.data.message}(${response.data.code})`);
                            StorageService.clearLoginStore();
                            window.location.reload();
                        }

                    }
                }
                return config;
            },
            error => {
                return Promise.reject(error);
            }
        ); */


        // 添加认证头拦截器
        Axios.instance.interceptors.request.use(
            config => {
                config.headers["Authorization"] = "Bearer " + localStorage.getItem("token");
                return config;
            },
            error => {
                return Promise.reject(error);
            }
        );

        // 异常捕获拦截器
        Axios.instance.interceptors.response.use(
            (response: AxiosResponse<any>) => {
                if (response.config.responseType === "blob") {
                    return response;
                }

                if (response.data.isSuccess) {

                    // 如果有成功消息的话则显示
                    if (response.data.code && response.data.message) {
                        message.success(`${response.data.message}`);
                    }
                    return response;
                } else {
                    // 处理失败
                    message.error(`${response.data.message}(${response.data.code})`);
                    return Promise.reject(response);
                }
            },
            err => {
                console.error(err);
                if (!err.response) {
                    message.error(`请求失败！服务器已经下线`);
                } else {
                    let errorStatus = err.response.status;
                    if (!errorStatus) {
                        message.error(`发生未知错误！`);
                    } else {
                        if (errorStatus === 401) {
                            message.error(`认证已过期！`);
                            localStorage.clear();
                            setTimeout(() => {
                                window.location.reload();
                            }, 1000);

                        } else {
                            let msg = Axios.codeMessage[errorStatus];
                            message.error(`${errorStatus}:${msg}`);
                        }
                    }
                }
                return Promise.reject(err);
            }
        );
    }
}