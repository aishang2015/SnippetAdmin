import { CommonResult } from "../common-result";
import { Axios } from "../request";



export class LoginLogService {

    static getLoginLogs(model: GetSysLoginLogRequest) {
        return Axios.instance.post<CommonResult<SysLoginLogResponse>>('api/SysLoginLog/GetMany2', model);
    }
}

export interface GetSysLoginLogRequest {
    page?: number;
    size?: number;
    takeCount?: number;
    skipCount?: number;
    sorts?: Array<SortModel> | null;
    containedUsername?: string | null;
    equalUsername?: string | null;
    containedRemoteIp?: string | null;
    equalRemoteIp?: string | null;
    upperAccessedTime?: string | null;
    lowerAccessedTime?: string | null;
    equalAccessedTime?: string | null;
    equalIsSucceed?: boolean | null;
}

export interface SortModel {
    propertyName?: string | null;
    isAsc?: boolean;
}

export interface SysLoginLogResponse {
    total: number,
    data: [
        {
            id?: number;
            username?: string | null;
            remoteIp?: string | null;
            accessedTime?: string;
            isSucceed?: boolean;
        }
    ]
}