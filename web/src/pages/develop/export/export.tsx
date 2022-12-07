import { faDownload } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Card, Col, Divider, Row, Typography } from 'antd';
import { AxiosResponse } from 'axios';
import { useEffect, useState } from 'react';
import { ExportService } from '../../../http/requests/export';


export default function Export() {

    const [csvDataTypeList, setCsvDataTypeList] = useState<Array<string>>([]);
    const [codeDataTypeList, setCodeDataTypeList] = useState<Array<string>>([]);

    useEffect(() => {
        init();
    }, []);

    async function init() {
        let codeTypes = await ExportService.getCodeDataType();
        let csvTypes = await ExportService.getCsvDataType();
        setCodeDataTypeList(codeTypes.data.data);
        setCsvDataTypeList(csvTypes.data.data);
    }

    async function downloadCsvData(type: string) {
        let response = await ExportService.exportCsvData(type);
        exportToFile(response);
    }

    async function downloadCodeData(type: string) {
        let response = await ExportService.exportCodeData(type);
        exportToFile(response);
    }

    function exportToFile(response: AxiosResponse<Blob>) {
        let blob = new Blob([response.data], { type: response.data.type });
        let downloadElement = document.createElement("a");
        let href = window.URL.createObjectURL(blob);
        let fileName = response.headers["content-disposition"]
            ? response.headers["content-disposition"].split(";")[1].split("=")[1]
            : new Date().getTime();
        downloadElement.href = href;
        downloadElement.download = decodeURIComponent(fileName as string); //解码
        document.body.appendChild(downloadElement);
        downloadElement.click();
        document.body.removeChild(downloadElement);
        window.URL.revokeObjectURL(href);
    }

    return (
        <>
            <Typography.Title level={4}>导出代码</Typography.Title>
            <Row gutter={[16, 16]}>
                {codeDataTypeList.map(type =>
                    <Col span={4} key={type}>
                        <Card title={null}
                            actions={[
                                <FontAwesomeIcon icon={faDownload} onClick={() => downloadCodeData(type)} />
                            ]}>
                            <Typography.Title level={5}>{type}</Typography.Title>
                        </Card>
                    </Col>
                )}
            </Row>
            <Divider />
            <Typography.Title level={4}>导出CSV</Typography.Title>
            <Row gutter={[16, 16]}>
                {csvDataTypeList.map(type =>
                    <Col span={4} key={type}>
                        <Card title={null}
                            actions={[
                                <div onClick={() => downloadCsvData(type)}>
                                    <FontAwesomeIcon icon={faDownload} />
                                </div>
                            ]}>
                            <Typography.Title level={5}>{type}</Typography.Title>
                        </Card>
                    </Col>
                )}
            </Row>
        </>
    );
}