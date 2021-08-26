import { message } from "antd";
import { Configuration } from "./config";

export class OauthService {

    // 初始化userManager
    static initUserManager() {
    }

    // 开始登录
    static githubLogin(): void {
        const authorizeUrl = "https://github.com/login/oauth/authorize";
        let clientId = Configuration.OauthSetting.github.client_id;
        if (clientId) {
            let uri = encodeURIComponent(Configuration.OauthSetting.github.redirect_uri);
            window.location.assign(`${authorizeUrl}?client_id=${clientId}&redirect_uri=${uri}`);
        } else {
            message.warning("没有配置github的client-id！");
        }
    }

    static baiduLogin(): void {
        const authorizeUrl = "http://openapi.baidu.com/oauth/2.0/authorize";
        let clientId = Configuration.OauthSetting.baidu.client_id;
        if (clientId) {
            let uri = encodeURIComponent(Configuration.OauthSetting.baidu.redirect_uri);
            window.location.assign(`${authorizeUrl}?client_id=${clientId}&redirect_uri=${uri}&response_type=code`);
        } else {
            message.warning("没有配置baidu的client-id！");
        }

    }
}