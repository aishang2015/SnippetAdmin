import { faCircleNotch } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Button, Pagination, Table, Tag } from "antd";
import { useEffect, useState } from "react";
import { dateFormat } from "../../../common/time";
import { LoginLogService } from "../../../http/requests/login-log";



export default function Login() {

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
        { title: '登录名', dataIndex: "username", align: 'center', width: '300px' },
        { title: '登录ip', dataIndex: "remoteIp", align: 'center' },
        {
            title: '登录时间', dataIndex: "accessedTime", align: 'center', width: '180px',
            render: (date: any) => dateFormat(date)
        },
        {
            title: '是否成功', dataIndex: "isSucceed", align: 'center', width: '100px',
            render: (date: any) => date ? <Tag color="success">是</Tag> : <Tag color="error">否</Tag>
        },
    ];

    useEffect(() => {
        initAsync();
    }, [page, size]);// eslint-disable-line react-hooks/exhaustive-deps

    async function initAsync() {
        let response = await LoginLogService.getLoginLogs({
            page: page,
            size: size,
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
            <div style={{ marginBottom: 10 }}>
                <Button style={{ marginRight: '10px' }} icon={<FontAwesomeIcon icon={faCircleNotch} fixedWidth />}
                    onClick={initAsync}>刷新</Button>
            </div>
            <Table bordered={true} size="small" columns={tableColumns} dataSource={tableData} scroll={{ x: 930 }}
                pagination={false}  ></Table>
            {total > 0 &&
                <Pagination current={page} total={total} showSizeChanger={true} style={{ marginTop: '10px' }}
                    onChange={async (p, s) => { setPage(p); setSize(s); await initAsync(); }}></Pagination>
            }
        </>
    );
}