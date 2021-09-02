import { CommonResult } from "../common-result";
import { Axios } from "../request";

export class ApiInfoService {

    static getApiInfo() {
        return Axios.instance.post<CommonResult<Array<string>>>('api/ApiInfo/GetApiPaths', {});
    }
}