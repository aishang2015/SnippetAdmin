import { CommonResult, CommonResultNoData } from "../../common-result";
import { Axios } from "../../request";

export class DictionaryService {

    static getDicTypeList() {
        return Axios.instance.post<CommonResult<Array<GetDicTypeListResponse>>>('api/Dic/GetDicTypeList', null);
    }

    static addDicType(model: AddDicTypeRequest) {
        return Axios.instance.post<CommonResultNoData>('api/Dic/AddDicType', model);
    }

    static updateDicType(model: UpdateDicTypeRequest) {
        return Axios.instance.post<CommonResultNoData>('api/Dic/UpdateDicType', model);
    }

    static deleteDicType(model: DeleteDicTypeRequest) {
        return Axios.instance.post<CommonResultNoData>('api/Dic/DeleteDicType', model);
    }

    static getDicValueList(model: GetDicValueListRequest) {
        return Axios.instance.post<CommonResult<Array<GetDicValueListResponse>>>('api/Dic/GetDicValueList', model);
    }

    static addDicValue(model: AddDicValueRequest) {
        return Axios.instance.post<CommonResultNoData>('api/Dic/AddDicValue', model);
    }

    static updateDicValue(model: UpdateDicValueRequest) {
        return Axios.instance.post<CommonResultNoData>('api/Dic/UpdateDicValue', model);
    }

    static deleteDicValue(model: DeleteDicValueRequest) {
        return Axios.instance.post<CommonResultNoData>('api/Dic/DeleteDicValue', model);
    }
    static EnableDicValue(param:EnableDicValueInputModel) {
        return Axios.instance.post<CommonResultNoData>('api/Dic/EnableDicValue',param);
    }
}

/*
 * EnableDicValueInputModel
 */
export interface EnableDicValueInputModel {
    id?: null | number;
    isEnabled?: null | boolean;
}


export interface GetDicTypeListResponse {
    id?: number;
    name?: string | null;
    code?: string | null;
}

export interface AddDicTypeRequest {
    name?: string | null;
    code?: string | null;
}

export interface UpdateDicTypeRequest {
    id?: number;
    name?: string | null;
    code?: string | null;
}

export interface DeleteDicTypeRequest {
    id?: number;
}

export interface GetDicValueListRequest {
    id?: number;
}

export interface GetDicValueListResponse {
    id?: number;
    name?: string | null;
    code?: string | null;
    isEnabled: boolean | null;
    sorting?: number;
}

export interface AddDicValueRequest {
    typeId?: number;
    name?: string | null;
    code?: string | null;
    sorting?: number;
}

export interface UpdateDicValueRequest {
    id?: number;
    typeId?: number;
    name?: string | null;
    code?: string | null;
    sorting?: number;
}

export interface DeleteDicValueRequest {
    id?: number;
}