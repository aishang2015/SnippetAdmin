import { CommonResult,CommonResultNoData } from "../../common-result";
import { Axios } from "../../request";

export class SettingService {

    static GetSettings(param: GetSettingsInputModel) {
        return Axios.instance.post<CommonResult<GetSettingsOutputModel>>('api/Setting/GetSettings', param);
    }
    static UpdateSetting(param: UpdateSettingInputModel) {
        return Axios.instance.post<CommonResultNoData>('api/Setting/UpdateSetting', param);
    }
}


/*
 * GetSettingsInputModel
 */
export interface GetSettingsInputModel {
    keyList?: null | Array<string>;
}



/*
 * Setting
 */
export interface Setting {
    key?: null | string;
    value?: null | string;
}

/*
 * GetSettingsOutputModel
 */
export interface GetSettingsOutputModel {
    settings?: null | Array<Setting>;
}


/*
 * Setting
 */
export interface Setting {
    key?: null | string;
    value?: null | string;
}

/*
 * UpdateSettingInputModel
 */
export interface UpdateSettingInputModel {
    settings?: null | Array<Setting>;
}