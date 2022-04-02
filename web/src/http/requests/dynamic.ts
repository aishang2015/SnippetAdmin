import { CommonResult } from "../common-result";
import { Axios } from "../request";

export class DynamicService {

    static getApiInfo() {
        type getApiInfoResult = {
            group: string;
            dynamicInfoGroups: Array<dynamicInfoGroup>;
        };
        type dynamicInfoGroup = {
            name: string;
            entityName: string;
        };
        return Axios.instance.post<CommonResult<Array<getApiInfoResult>>>('api/Dynamic/GetDynamicInfo', {});
    }

    static getColumnInfo(entityName: string) {
        type getColumnInfoResult = {
            propertyName: string,
            propertyDescribe: string,
            propertyType: string
        };
        return Axios.instance.post<CommonResult<Array<getColumnInfoResult>>>('api/Dynamic/getColumns', { EntityName: entityName });
    }

    static findOne(entityName: string, id: number) {
        return Axios.instance.post<CommonResult<any>>(`api/${entityName}/FindOne`, { Id: id });
    }

    static getMany(entityName: string, searchObj: object) {
        return Axios.instance.post<CommonResult<any>>(`api/${entityName}/GetMany`, searchObj);
    }

    static addOne(entityName: string, addObj: object) {
        return Axios.instance.post<CommonResult<any>>(`api/${entityName}/AddOne`, addObj);
    }

    static addMany(entityName: string, addObj: Array<object>) {
        return Axios.instance.post<CommonResult<any>>(`api/${entityName}/AddMany`, addObj);
    }

    static updateOne(entityName: string, updateObj: object) {
        return Axios.instance.post<CommonResult<any>>(`api/${entityName}/UpdateOne`, updateObj);
    }

    static deleteOne(entityName: string, id: number) {
        return Axios.instance.post<CommonResult<any>>(`api/${entityName}/DeleteOne`, { Id: id });
    }

    static getDic(entityName: string) {
        type DicOutput = {
            key: number,
            value: string
        }
        return Axios.instance.post<CommonResult<Array<DicOutput>>>(`api/${entityName}/GetDic`, {});
    }
}