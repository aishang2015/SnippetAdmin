import { type } from "os";
import { CommonResult, CommonResultNoData } from "../common-result";
import { Axios } from "../request";

export type getOrganizationResult = {
    id: number,
    upId: number,
    name: string,
    code: string,
    icon: string,
    iconId: string,
    phone: string,
    address: string,
    upPositions: Array<string>,
    positions: {
        visibleToChild: boolean,
        name: string,
        code: string
    }[]
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
    icon: string,
    iconId: string,
    phone: string,
    address: string
}

export type updateOrganizationInput = {
    upId: number,
    id: number,
    name: string,
    code: string,
    icon: string,
    iconId: string,
    phone: string,
    address: string
}

export type setPositionInput = {
    organizationId: number,
    positions: {
        visibleToChild: boolean,
        name: string,
        code: string
    }[]
}

export type getPositionResult = {
    key: number,
    value: string
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

    static setPosition(params: setPositionInput) {
        return Axios.instance.post<CommonResultNoData>('api/organization/setPosition', params);
    }

    static GetPositionDic(params: { id: number }) {
        return Axios.instance.post<CommonResult<Array<getPositionResult>>>('api/organization/getPositionDic', params);
    }
}