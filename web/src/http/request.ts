import axios, { AxiosInstance, AxiosResponse } from 'axios';
import { message } from 'antd';
import { Configuration } from '../common/config';

export class Axios {

    // http错误
    static codeMessage: { [key: string]: string } = {
        200: '服务器成功返回请求的数据。',
        201: '新建或修改数据成功。',
        202: '一个请求已经进入后台排队（异步任务）。',
        204: '删除数据成功。',
        400: '发出的请求有错误，服务器没有进行新建或修改数据的操作。',
        401: '用户没有权限（令牌、用户名、密码错误）。',
        403: '用户得到授权，但是访问是被禁止的。',
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
                    message.error(`请求失败！可能原因：服务器已经下线，跨域访问被禁止等`);
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