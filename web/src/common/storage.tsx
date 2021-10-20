import { join, split } from "lodash";

export class StorageService {

    //#region 第三方登录授权信息

    static clearOauthStore(): void {
        localStorage.removeItem("snippetadmin-third-username");
        localStorage.removeItem("snippetadmin-third-type");
        localStorage.removeItem("snippetadmin-third-cachekey");
    }

    static setOauthStore(thirdPartyUserName: string, thirdPartyType: string, thirdPartyInfoCacheKey: string): void {
        localStorage.setItem("snippetadmin-third-username", thirdPartyUserName);
        localStorage.setItem("snippetadmin-third-type", thirdPartyType);
        localStorage.setItem("snippetadmin-third-cachekey", thirdPartyInfoCacheKey);
    }

    static getThirdPartyUserName = () => localStorage.getItem("snippetadmin-third-username");
    static getThirdPartyType = () => localStorage.getItem("snippetadmin-third-type");
    static getThirdPartyInfoCacheKey = () => localStorage.getItem("snippetadmin-third-cachekey");

    //#endregion

    //#region 用户登录信息

    static clearLoginStore(): void {
        localStorage.removeItem("token");
        localStorage.removeItem("user-name");
        localStorage.removeItem("expire");
        localStorage.removeItem("right");
        localStorage.removeItem("refresh-token");

        // tab页相关信息
        localStorage.removeItem('activeKey');
        localStorage.removeItem('panes');
    }

    static setLoginStore(accessToken: string, userName: string, expire: string, rights: string[], refreshToken:string): void {
        localStorage.setItem('token', accessToken);
        localStorage.setItem('user-name', userName);
        localStorage.setItem("expire", expire);
        localStorage.setItem("right", join(rights, ','));
        localStorage.setItem("refresh-token", refreshToken);
    }

    static getAccessToken = () => localStorage.getItem("token");
    static getUserName = () => localStorage.getItem("user-name");
    static getExpire = () => localStorage.getItem("expire");
    static getRights = () => split(localStorage.getItem("right"), ",");

    //#endregion
}