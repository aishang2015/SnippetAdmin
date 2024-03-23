import { CommonResult, CommonResultNoData } from "../common-result";
import { Axios } from "../request";


export type SearchUserResult = {
    total: number,
    data: [
        {
            id: number,
            avatar: string,
            userName: string,
            realName: string,
            gender: number,
            phoneNumber: string,
            isActive: boolean,
            roles: Array<{
                roleName: string,
                isActive: boolean
            }>,
            organizations: Array<string>,
            positions: Array<string>
        }
    ]
}

export type GetUserResult = {
    id: number,
    userName: string,
    realName: string,
    gender: number,
    phoneNumber: string,
    roles: Array<number>,
    organizations: Array<number>
    positions: Array<number>
}


export class UserService {


    static activeUser(params: {
        id: number,
        isActive: boolean
    }) {
        return Axios.instance.post<CommonResultNoData>('api/user/activeUser', params);
    }

    static getUser(params: {
        id: number
    }) {
        return Axios.instance.post<CommonResult<GetUserResult>>('api/user/getUser', params);
    }

    static searchUser(params: {
        page: number,
        size: number,
        userName?: string,
        realName?: string,
        phone?: string,
        role?: number,
        org?: number,
        position?: number
    }) {
        return Axios.instance.post<CommonResult<SearchUserResult>>('api/user/searchUser', params);
    }

    static addOrUpdateUser(params: {
        id: number,
        userName: string,
        realName: string,
        gender: number,
        phoneNumber: string,
        roles: Array<number>,
        organizations: Array<number>,
        positions: Array<number>
    }) {
        return Axios.instance.post<CommonResultNoData>('api/user/addOrUpdateUser', params);
    }

    static removeUser(params: {
        id: number
    }) {
        return Axios.instance.post<CommonResultNoData>('api/user/removeUser', params);
    }

    static setUserPassword(params: {
        id: number,
        password: string,
        confirmPassword: string
    }) {
        return Axios.instance.post<CommonResultNoData>('api/user/setUserPassword', params);
    }

    static addOrgMember(params: {
        orgId: number,
        userIds: Array<number>,
        positions: Array<number>
    }) {
        return Axios.instance.post<CommonResultNoData>('api/user/addOrgMember', params);
    }

    static removeOrgMember(params: {
        orgId: number,
        userId: number
    }) {
        return Axios.instance.post<CommonResultNoData>('api/user/removeOrgMember', params);
    }

    
    static getUserDic() {
        return Axios.instance.post<CommonResult<Array<GetUserDicOutputModel>>>
            ('api/User/GetUserDic', null);
    }
}

/**
 * GetUserDicOutputModel
 */
export interface GetUserDicOutputModel {
    realName?: null | string;
    userId?: number;
}