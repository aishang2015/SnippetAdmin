import { CommonResult } from "../common-result";
import { Axios } from "../request";


export class ExceptionLogService {


    static getExceptionLogs(model: SysExceptionLogRequest) {
        return Axios.instance.post<CommonResult<SysExceptionLogResponse>>('api/sysExceptionLog/getMany2', model);
    }
}

// /\*.*\n.*\n.*\n.*\n.*/ 正则替换掉注解
export interface SysExceptionLogRequest {
    page?: number;
    size?: number;
    sorts?: Array<SortModel> | null;
    containedType?: string | null;
    equalType?: string | null;
    containedMessage?: string | null;
    equalMessage?: string | null;
    containedSource?: string | null;
    equalSource?: string | null;
    containedStackTrace?: string | null;
    equalStackTrace?: string | null;
    upperHappenedTime?: string | null;
    lowerHappenedTime?: string | null;
    equalHappenedTime?: string | null;
    containedPath?: string | null;
    equalPath?: string | null;
    containedUsername?: string | null;
    equalUsername?: string | null;
}

export interface SortModel {
    propertyName?: string | null;
    isAsc?: boolean;
}

export interface SysExceptionLogResponse {
    total: number,
    data: [
        {
            id?: number;
            type?: string | null;
            message?: string | null;
            source?: string | null;
            stackTrace?: string | null;
            happenedTime?: string;
            path?: string | null;
            username?: string | null;
        }
    ]
}