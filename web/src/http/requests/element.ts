import { CommonResult, CommonResultNoData } from "../common-result";
import { Axios } from "../request";

export class ElementService {

    static getElement(id: number) {
        type getElementResult = {
            id: number;
            name: string;
            type: number;
            identity: string;
            accessApi: string;
        };
        return Axios.instance.post<CommonResult<getElementResult>>('api/element/getElement', { id: id });
    }

    static getElementTree() {
        type getElementTreeResult = {
            title: string;
            type: number;
            key: number;
            children: Array<getElementTreeResult>;
        };
        return Axios.instance.post<CommonResult<Array<getElementTreeResult>>>('api/element/getElementTree', {});
    }

    static createElement(param: {
        upId: number,
        name: string,
        type: number,
        identity: string,
        accessApi: string
    }) {
        return Axios.instance.post<CommonResultNoData>('api/element/createElement', param);
    }

    static deleteElement(id: number) {
        return Axios.instance.post<CommonResultNoData>('api/element/deleteElement', { id: id });
    }

    static updateElement(param: {
        id: number,
        name: string,
        type: number,
        identity: string,
        accessApi: string
    }) {
        return Axios.instance.post<CommonResultNoData>('api/element/updateElement', param);
    }

    static exportElementData() {
        return Axios.instance.post('api/Element/ExportElementData', {}, { responseType: "blob" });
    }


}