import { CommonResult, CommonResultNoData } from "../common-result";
import { Axios } from "../request";

export class JobRecordService {

    static GetJobRecords(param: {
        page: number,
        size: number,
        jobState?: number,
        jobName: string
    }) {
        type getJobsResult = {
            total: number,
            data: [
                {
                    id: number,
                    triggerMode: string,
                    jobState: string,
                    duration: string,
                    beginTime: Date,
                    endTime: Date,
                    infomation: string
                }
            ]
        };
        return Axios.instance.post<CommonResult<getJobsResult>>('api/JobRecord/GetJobRecords', param);
    }

    static RemoveJobRecords(param: {
        recordIds: Array<number>,
    }) {
        return Axios.instance.post<CommonResultNoData>('api/JobRecord/RemoveJobRecords', param);
    }
}