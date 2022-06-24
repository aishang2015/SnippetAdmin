import { Pagination, Space, Switch, Table } from 'antd';
import { useEffect, useState } from 'react';
import { dateFormat } from '../../../common/time';
import { AccessLogService } from '../../../http/requests/access';

import './access.less';


export default function Access() {

    const [page, setPage] = useState(1);
    const [size, setSize] = useState(10);
    const [total, setTotal] = useState(0);

    const [tableData, setTableData] = useState(new Array<any>());

    const tableColumns: any = [
        {
            title: '序号', dataIndex: "num", align: 'center', width: '90px',
            render: (data: any, record: any, index: any) => (
                <span>{(page - 1) * size + 1 + index}</span>
            )
        },
        { title: '请求方法', dataIndex: "method", align: 'center', width: '100px' },
        { title: '请求路径', dataIndex: "path", align: 'center' },
        { title: '用户名', dataIndex: "username", align: 'center', width: '220px' },
        { title: '访问者ip', dataIndex: "remoteIp", align: 'center', width: '100px' },
        { title: '访问时间', dataIndex: "accessedTime", align: 'center', width: '180px',
            render: (date: any) => dateFormat(date) },
        { title: '响应时间', dataIndex: "elapsedTime", align: 'center', width: '100px' },
        { title: '状态码', dataIndex: "statusCode", align: 'center', width: '90px' }
    ];

    useEffect(() => {
        initAsync(page, size);
    }, []);// eslint-disable-line react-hooks/exhaustive-deps

    async function initAsync(p: number, s: number) {
        let response = await AccessLogService.getAccessLogs({
            page: p,
            size: s,
            sorts: [
                {
                    propertyName: 'AccessedTime',
                    isAsc: false
                }
            ]
        });
        setTotal(response.data.data.total);
        response.data.data.data.forEach((d: any) => d.key = d.id);
        setTableData(response.data.data.data);
    }

    return (
        <>
            <Table bordered={true} columns={tableColumns} dataSource={tableData} scroll={{ x: 1230 }}
                expandable={{
                    expandedRowRender: record =>
                        <div>
                            <p style={{ margin: 0 }}>请求体：</p>
                            <p style={{ margin: 0 }}>{record.requestBody}</p>
                            <p style={{ margin: 0 }}>响应体：</p>
                            <p style={{ margin: 0 }}>{record.responseBody}</p>
                        </div>
                }}
                pagination={false} size="small" ></Table>
            {total > 0 &&
                <Pagination current={page} total={total} showSizeChanger={true} style={{ marginTop: '10px' }}
                    onChange={async (p, s) => { setPage(p); setSize(s); await initAsync(p, s); }}></Pagination>
            }
        </>
    );
}