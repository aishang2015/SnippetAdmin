import { StorageService } from "../common/storage";
import { refresh } from "../http/requests/account";


export class RefreshService {

    static async refreshTokenAsync() {

        let userName = localStorage.getItem("user-name");
        let refreshToken = localStorage.getItem("refresh-token");
        let token = localStorage.getItem("token");
        let expires = localStorage.getItem("expire");

        if (token && expires && userName && refreshToken) {

            let response = await refresh(userName, token, refreshToken);

            if (response.data.isSuccess) {
                let result = response.data.data;

                // 保存登录信息
                StorageService.setLoginStore(result.accessToken, result.userName, result.expire.toString(),
                    result.identifies, result.refreshToken);
            }
        }
    }

}