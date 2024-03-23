import { Dictionary } from "lodash";
import { CommonResult } from "../common-result";
import { Axios } from "../request";

export class AccessLogService {
    static getAccessLogs(model: GetSysAccessLogInputModel) {
        return Axios.instance.post<CommonResult<SysAccessLogResponse>>('api/sysAccessLog/getMany2', model);
    }

    static GetModuleMethodDic() {
        return Axios.instance.post<CommonResult<any>>('api/SysAccessLog/GetModuleMethodDic', null);
    }

    static GetDataDetailLogs(param: GetDataDetailLogsInputModel) {
        return Axios.instance.post<CommonResult<Array<GetDataDetailLogsOutputModel>>>('api/SysAccessLog/GetDataDetailLogs', param);
    }
}

/*
 * GetDataDetailLogsInputModel
 */
export interface GetDataDetailLogsInputModel {
    traceIdentifier?: null | string;
}

/*
 * GetDataDetailLogsOutputModel
 */
export interface GetDataDetailLogsOutputModel {
    id?: null | number;
    entityName?: null | string;
    operation?: null | string;
    operateTime?: null | Date;
    dataDetailList?: null | Array<DataDetailModel>;
}

/*
 * DataDetailModel
 */
export interface DataDetailModel {
    propertyName?: null | string;
    oldValue?: null | string;
    newValue?: null | string;
}

/**
 * GetSysAccessLogInputModel
 */
export interface GetSysAccessLogInputModel {
    containedMethod?: null | string;
    containedModule?: null | string;
    containedPath?: null | string;
    containedRemoteIp?: null | string;
    containedRequestBody?: null | string;
    containedResponseBody?: null | string;
    equalAccessedTime?: Date | null;
    equalElapsedTime?: number | null;
    equalMethod?: null | string;
    equalModule?: null | string;
    equalPath?: null | string;
    equalRemoteIp?: null | string;
    equalRequestBody?: null | string;
    equalResponseBody?: null | string;
    equalStatusCode?: number | null;
    equalUserId?: number | null;
    lowerAccessedTime?: Date | null;
    lowerElapsedTime?: number | null;
    lowerStatusCode?: number | null;
    lowerUserId?: number | null;
    month?: null | string;
    page?: number;
    size?: number;
    skipCount?: number;
    sorts?: SortModel[] | null;
    takeCount?: number;
    upperAccessedTime?: Date | null;
    upperElapsedTime?: number | null;
    upperStatusCode?: number | null;
    upperUserId?: number | null;
}

/**
 * SortModel
 */
export interface SortModel {
    isAsc?: boolean;
    propertyName?: null | string;
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
            traceIdentifier?: string | null;
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