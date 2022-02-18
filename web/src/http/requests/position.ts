import { CommonResult, CommonResultNoData } from "../common-result";
import { Axios } from "../request";

export class PositionService {

    // 添加或更新职位
    static async addOrUpdatePosition(input: addOrUpdatePositionInput): Promise<void> {
        await Axios.instance.post<CommonResultNoData>('api/position/addOrUpdatePosition', input);
    }

    // 删除职位
    static async deletePosition(input: deletePositionInput): Promise<void> {
        await Axios.instance.post<CommonResultNoData>('api/position/deletePosition', input);
    }

    // 取得职位详情
    static async getPosition(input: getPositionInput) {
        return await Axios.instance.post<CommonResult<getPositionOutput>>('api/position/getPosition', input);
    }

    // 取得职位列表
    static async getPositions(input: getPositionsInput) {
        return await Axios.instance.post<CommonResult<getPositionsOutput>>('api/position/getPositions', input);
    }

    // 取得职位字典
    static async getPositionDic() {
        return await Axios.instance.post<CommonResult<Array<getPositionDicOutput>>>('api/position/getPositionDic', null);
    }
}

export type addOrUpdatePositionInput = {
    id: number | null,
    name: string
}

export type deletePositionInput = {
    id: number
}

export type getPositionsInput = {
    page: number,
    size: number
}

export type getPositionInput = {
    id: number
}

export type getPositionOutput = {
    id: number,
    name: string,
}

export type getPositionsOutput = {
    total: number,
    data: Array<positionResult>
}

export type positionResult = {
    id: number,
    name: string,
    code: string,
}

export type getPositionDicOutput = {
    key: number,
    value: string
}