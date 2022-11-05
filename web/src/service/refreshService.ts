import { StorageService } from "../common/storage";
import { refresh } from "../http/requests/account";


export class RefreshService {

    static async refreshTokenAsync() {

        let userName = localStorage.getItem("user-name");
        let token = localStorage.getItem("token");
        let expires = localStorage.getItem("expire");

        if (token && expires && userName) {

            try {
                let response = await refresh(userName, token);

                if (response.data.isSuccess) {
                    let result = response.data.data;

                    // 保存登录信息
                    StorageService.setLoginStore(result.accessToken, result.userName, result.expire.toString(),
                        result.identifies);
                }
            } catch (e: any) {

                if (e.status === 200 && !e.data.isSuccess) {
                    
                    // 清空登录信息
                    StorageService.clearLoginStore();
                    window.location.reload();
                }
            }
        }
    }

}