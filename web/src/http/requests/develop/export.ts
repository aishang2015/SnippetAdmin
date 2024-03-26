import { CommonResult } from "../../common-result";
import { Axios } from "../../request";


export class ExportService {

    static getCsvDataType() {
        return Axios.instance.get<CommonResult<Array<string>>>('api/Data/GetCsvDataType');
    }

    static getCodeDataType() {
        return Axios.instance.get<CommonResult<Array<string>>>('api/Data/GetCodeDataType');
    }

    static exportCodeData(type: string) {
        return Axios.instance.post<Blob>('api/Data/ExportCodeData', { id: type }, { responseType: 'blob' });
    }

    static exportCsvData(type: string) {
        return Axios.instance.post<Blob>('api/Data/ExportCsvData', { id: type }, { responseType: 'blob' });
    }
}