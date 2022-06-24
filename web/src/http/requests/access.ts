import { CommonResult } from "../common-result";
import { Axios } from "../request";

export class AccessLogService {
    static getAccessLogs(model: SysAccessLogRequest) {
        return Axios.instance.post<CommonResult<SysAccessLogResponse>>('api/sysAccessLog/getMany2', model);
    }
}

export interface SysAccessLogRequest {
    page?: number;
    size?: number;
    sorts?: Array<SortModel> | null;
    containedMethod?: string | null;
    equalMethod?: string | null;
    containedPath?: string | null;
    equalPath?: string | null;
    containedUsername?: string | null;
    equalUsername?: string | null;
    containedRemoteIp?: string | null;
    equalRemoteIp?: string | null;
    upperAccessedTime?: string | null;
    lowerAccessedTime?: string | null;
    equalAccessedTime?: string | null;
    upperElapsedTime?: number | null;
    lowerElapsedTime?: number | null;
    equalElapsedTime?: number | null;
    containedRequestBody?: string | null;
    equalRequestBody?: string | null;
    upperStatusCode?: number | null;
    lowerStatusCode?: number | null;
    equalStatusCode?: number | null;
    containedResponseBody?: string | null;
    equalResponseBody?: string | null;
}

export interface SortModel {

    propertyName?: string | null;
    isAsc?: boolean;
}

export interface SysAccessLogResponse {
    total: number,
    data: [
        {
            id?: number;
            method?: string | null;
            path?: string | null;
            username?: string | null;
            remoteIp?: string | null;
            accessedTime?: string;
            elapsedTime?: number;
            requestBody?: string | null;
            statusCode?: number;
            responseBody?: string | null;
        }
    ]
}