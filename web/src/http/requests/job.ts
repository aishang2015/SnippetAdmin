import { CommonResult, CommonResultNoData } from "../common-result";
import { Axios } from "../request";

export class JobService {

    static GetJobs(param: {
        page: number,
        size: number
    }) {
        type getJobsResult = {
            total: number,
            data: [
                {
                    id: number,
                    name: string,
                    describe: string,
                    cron: string,
                    isActive: boolean,
                    nextTime: Date,
                    lastTime: Date,
                    createTime: Date
                }
            ]
        };
        return Axios.instance.post<CommonResult<getJobsResult>>('api/Job/GetJobs', param);
    }

    static ActiveJob(param: {
        id: number,
        isActive: boolean
    }) {
        return Axios.instance.post<CommonResultNoData>('api/Job/ActiveJob', param);
    }

    static GetJob(param: {
        id: number
    }) {
        type getElementResult = {
            id: number,
            type: string,
            name: string,
            describe: string,
            cron: string
        };
        return Axios.instance.post<CommonResult<getElementResult>>('api/Job/GetJob', param);
    }

    static UpdateJob(param: {
        id: number,
        type: string,
        name: string,
        describe: string,
        cron: string
    }) {
        return Axios.instance.post<CommonResultNoData>('api/Job/UpdateJob', param);
    }

    static DeleteJob(param: {
        id: number
    }) {
        return Axios.instance.post<CommonResultNoData>('api/Job/DeleteJob', param);
    }

    static AddJob(param: {
        type: string,
        name: string,
        describe: string,
        cron: string
    }) {
        return Axios.instance.post<CommonResultNoData>('api/Job/AddJob', param);
    }

    static RunJob(param: {
        id: number
    }) {
        return Axios.instance.post<CommonResultNoData>('api/Job/RunJob', param);
    }

    static GetJobNames() {
        return Axios.instance.post<CommonResult<Array<string>>>('api/Job/GetJobNames', null);
    }

    static GetJobTypeList() {
        return Axios.instance.post<CommonResult<Array<string>>>('api/Job/GetJobTypeList', null);
    }
}