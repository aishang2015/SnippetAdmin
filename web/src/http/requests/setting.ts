import { CommonResult, CommonResultNoData } from "../common-result";
import { Axios } from "../request";

export class SettingService {

    static getLoginPageSetting() {
        return Axios.instance.post<CommonResult<GetLoginPageSettingResponse>>('api/Setting/GetLoginPageSetting', null);
    }

    static saveLoginPageSetting(data: FormData) {
        return Axios.instance.post<CommonResultNoData>('api/Setting/SaveLoginPageSetting', data, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        });
    }
}

export interface GetLoginPageSettingResponse {
    title?: string | null;
    background?: string | null;
    icon?: string | null;
}