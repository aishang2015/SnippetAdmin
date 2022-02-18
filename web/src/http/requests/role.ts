import { CommonResult, CommonResultNoData } from "../common-result";
import { Axios } from "../request";

export type GetRoleResult = {
    id: number,
    name: string,
    code: string,
    remark: string,
    rights: Array<string>,
    isActive: boolean
}

export type GetRolesResult = {
    total: number,
    data: Array<GetRoleResult>
}

export type GetRoleDicResult = {
    key: number,
    value: string
}

export class RoleService {

    static activeRole(param: {
        id: number,
        isActive: boolean
    }) {
        return Axios.instance.post<CommonResultNoData>('api/role/activeRole', param);
    }

    static getRole(param: {
        id: number
    }) {
        return Axios.instance.post<CommonResult<GetRoleResult>>('api/role/getRole', param);
    }

    static getRoles(param: {
        page: number,
        size: number
    }) {
        return Axios.instance.post<CommonResult<GetRolesResult>>('api/role/getRoles', param);
    }

    static getRoleDic() {
        return Axios.instance.post<CommonResult<Array<GetRoleDicResult>>>('api/role/getRoleDic', null);
    }

    static addOrUpdateRole(param: {
        id: number,
        name: string,
        remark: string,
        rights: Array<number>,
    }) {
        return Axios.instance.post<CommonResultNoData>('api/role/addOrUpdateRole', param);
    }

    static removeRole(param: {
        id: number
    }) {
        return Axios.instance.post<CommonResultNoData>('api/role/removeRole', param);
    }
}