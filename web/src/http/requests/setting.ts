import { CommonResult,CommonResultNoData } from "../common-result";
import { Axios } from "../request";

export class SettingService {

    static GetSettingGroups() {
        return Axios.instance.post<CommonResult<Array<GetSettingGroupsOutputModel>>>('api/Setting/GetSettingGroups',{});
    }
    static GetSettings(param:GetSettingsInputModel) {
        return Axios.instance.post<CommonResult<Array<GetSettingsOutputModel>>>('api/Setting/GetSettings',param);
    }
    static SaveSetting(param:SaveSettingInputModel) {
        return Axios.instance.post<CommonResultNoData>('api/Setting/SaveSetting',param);
    }
}


/*
 * GetSettingGroupsOutputModel
 */
export interface GetSettingGroupsOutputModel {
    icon?: null | string;
    name?: null | string;
    code?: null | string;
}

/*
 * GetSettingsInputModel
 */
export interface GetSettingsInputModel {
    groupCode?: null | string;
}

/*
 * SettingModel
 */
export interface SettingModel {
    icon?: null | string;
    name?: null | string;
    describe?: null | string;
    code?: null | string;
    value?: null | string;
    inputType?: null | number;
    options?: null | string;
    index?: null | number;
    min?: null | number;
    max?: null | number;
    regex?: null | string;
}

/*
 * GetSettingsOutputModel
 */
export interface GetSettingsOutputModel {
    icon?: null | string;
    name?: null | string;
    describe?: null | string;
    code?: null | string;
    settings?: null | SettingModel[];
}

/*
 * SaveSettingInputModel
 */
export interface SaveSettingInputModel {
    code?: null | string;
    value?: null | string;
}
