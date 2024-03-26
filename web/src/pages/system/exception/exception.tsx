import { faCircleNotch } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Button, Pagination, Table } from 'antd';
import { useEffect, useState } from 'react';
import { dateFormat } from '../../../common/time';
import { ExceptionLogService } from '../../../http/requests/system/exception';


export default function Exception() {

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
        { title: '异常类型', dataIndex: "type", align: 'center', width: '300px' },
        { title: '异常消息', dataIndex: "message", align: 'left' },
        { title: '异常源', dataIndex: "source", align: 'center', width: '200px' },
        {
            title: '发生时间', dataIndex: "happenedTime", align: 'center', width: '180px',
            render: (date: any) => dateFormat(date)
        },
        { title: '请求人', dataIndex: "username", align: 'center', width: '100px' },
        { title: '请求路径', dataIndex: "path", align: 'center', width: '90px' }
    ];

    useEffect(() => {
        initAsync(page, size);
    }, []);// eslint-disable-line react-hooks/exhaustive-deps

    async function initAsync(p: number, s: number) {
        let response = await ExceptionLogService.getExceptionLogs({
            page: p,
            size: s,
            sorts: [
                {
                    propertyName: 'HappenedTime',
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
            <div style={{ marginBottom: 10 }}>
                <Button style={{ marginRight: '10px' }} icon={<FontAwesomeIcon icon={faCircleNotch} fixedWidth />}
                    onClick={() => initAsync(page, size)}>刷新</Button>
            </div>
            <Table bordered={true} size="small" columns={tableColumns} dataSource={tableData} scroll={{ x: 2130 }}
                expandable={{
                    expandedRowRender: record =>
                        <div>
                            <p style={{ margin: 0 }}>堆栈跟踪：</p>
                            <p style={{ margin: 0 }}><pre>{record.stackTrace}</pre></p>
                        </div>
                }}
                pagination={false}  ></Table>
            {total > 0 &&
                <Pagination current={page} total={total} showSizeChanger={true} style={{ marginTop: '10px' }}
                    onChange={async (p, s) => { setPage(p); setSize(s); await initAsync(p, s); }}></Pagination>
            }
        </>
    );
}