import { type } from "os";
import { CommonResult, CommonResultNoData } from "../common-result";
import { Axios } from "../request";

export type getOrganizationResult = {
    id: number,
    upId: number,
    name: string,
    code: string,
    type: string,
    typeName: string,
    icon: string,
    iconId: string,
    phone: string,
    address: string,
    sorting: number
}

export type getOrganizationTreeResult = {
    title: string,
    icon: string,
    key: number,
    children: Array<string>
}

export type createOrganizationInput = {
    upId: number,
    name: string,
    code: string,
    type: string,
    icon: string,
    iconId: string,
    phone: string,
    address: string,
    sorting: number
}

export type updateOrganizationInput = {
    upId: number,
    id: number,
    name: string,
    code: string,
    type: string,
    icon: string,
    iconId: string,
    phone: string,
    address: string,
    sorting: number
}

export type AddOrUpdateOrganizationTypeInput = {
    id: number,
    name: string,
    code: string
}

export type GetOrganizationTypesInput = {
    page: number;
    size: number;
}

export type GetOrganizationTypesOutput = {
    id: number,
    name: string,
    code: string
}

export type RemoveOrganizationTypeInput = {
    id: number,
}

export class OrganizationService {

    static getOrganization(id: number) {
        return Axios.instance.post<CommonResult<getOrganizationResult>>('api/organization/getOrganization', { id: id });
    }

    static getOrganizationTree() {
        return Axios.instance.post<CommonResult<Array<getOrganizationTreeResult>>>('api/organization/getOrganizationTree', null);
    }

    static createOrganization(params: createOrganizationInput) {
        return Axios.instance.post<CommonResultNoData>('api/organization/createOrganization', params);
    }

    static deleteOrganization(id: number) {
        return Axios.instance.post<CommonResultNoData>('api/organization/deleteOrganization', { id: id });
    }

    static updateOrganization(params: updateOrganizationInput) {
        return Axios.instance.post<CommonResultNoData>('api/organization/updateOrganization', params);
    }

    static GetOrganizationTypes() {
        return Axios.instance.post<CommonResult<Array<GetOrganizationTypesOutput>>>('api/organization/getOrganizationTypes', null);
    }

    static AddOrUpdateOrganizationType(params: AddOrUpdateOrganizationTypeInput) {
        return Axios.instance.post<CommonResultNoData>('api/organization/addOrUpdateOrganizationType', params);
    }

    static RemoveOrganizationType(params: RemoveOrganizationTypeInput) {
        return Axios.instance.post<CommonResultNoData>('api/organization/removeOrganizationType', params);
    }
}