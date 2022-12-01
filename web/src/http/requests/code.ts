import { CommonResult } from "../common-result";
import { Axios } from "../request";

export class CodeService {

    static GetControllers() {
        return Axios.instance.post<CommonResult<Array<string>>>('api/Code/GetControllers', {});
    }

    static GetTsRequestCode(param:GetTsRequestCodeInputModel) {
        return Axios.instance.post<CommonResult<GetTsRequestCodeOutputModel>>('api/Code/GetTsRequestCode', param);
    }
}

/**
 * GetTsRequestCodeInputModel
 */
export interface GetTsRequestCodeInputModel {
    controllerName?: null | string;
}

/**
 * GetTsRequestCodeOutputModel
 */
 export interface GetTsRequestCodeOutputModel {
    requestCode?: null | string;
}
