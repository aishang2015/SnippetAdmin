import { faTrash } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Button, Pagination, Radio, Select, Space, Table, Tag, Tooltip } from 'antd';
import { useEffect, useState } from 'react';
import './taskRecord.less';

export default function TaskRecord(props: any) {

    const [page, setPage] = useState(1);
    const [total, setTotal] = useState(0);
    const [size, setSize] = useState(10);

    const [selectedRowKeys, setSelectedRowKeys] = useState(new Array<any>());

    const [taskTableData, setTaskTableData] = useState(new Array<any>());

    const taskTableColumns: any = [
        {
            title: '编号', dataIndex: "id", align: 'center', width: '100px',
        },
        { title: '任务名', dataIndex: "name", align: 'center', width: '300px' },
        { title: '任务描述', dataIndex: "describe", align: 'center' },
        { title: '执行时长', dataIndex: "duration", align: 'center', width: '180px' },
        { title: '执行时间', dataIndex: "runTime", align: 'center', width: '180px' },
        {
            title: '状态', dataIndex: "state", align: 'center', width: '120px',
            render: (text: any, record: any) => {
                if (text === 0) {
                    return (<Tag color="lime">就绪</Tag>);
                } else if (text === 1) {
                    return (<Tag color="green">运行中</Tag>);
                } else if (text === 2) {
                    return (<Tag color="green">成功</Tag>);
                } else if (text === 3) {
                    return (<Tag color="red">失败</Tag>);
                }
            },
        },
        {
            title: '操作', key: 'operate', align: 'center', width: '120px',
            render: (text: any, record: any) => (
                <Space size="middle">
                    <Tooltip title="删除记录">
                        <a onClick={() => { deleteRecord(record.id) }}><FontAwesomeIcon icon={faTrash} fixedWidth /></a>
                    </Tooltip>
                </Space>
            ),
        }
    ];

    useEffect(() => {
        initial();
    }, []);

    function initial() {
        setTaskTableData([
            {
                id: "4", name: 'SnippetAdmin.Data.Entity.Scheduler.SiNGeognoegeonJob', describe: "定时下达开关栓指令",
                duration: "1h2min3s", runTime: "2020/10/22 10:10:10", state: 1
            },
            {
                id: "3", name: 'SnippetAdmin.Data.Entity.Scheduler.SiNGeognoegeonJob', describe: "定时下达开关栓指令",
                duration: "1h2min3s", runTime: "2020/10/22 10:10:10", state: 2
            },
            {
                id: "2", name: 'SnippetAdmin.Data.Entity.Scheduler.SiNGeognoegeonJob', describe: "定时下达开关栓指令",
                duration: "1h2min3s", runTime: "2020/10/22 10:10:10", state: 3
            },
            {
                id: "1", name: 'SnippetAdmin.Data.Entity.Scheduler.SiNGeognoegeonJob', describe: "定时下达开关栓指令",
                duration: "1h2min3s", runTime: "2020/10/22 10:10:10", state: 0
            },
        ]);
    }


    function deleteRecord(id: number) {

    }

    function deleteRecords() {

    }

    function rowSelectChange(selectedRowKeys: any) {
        setSelectedRowKeys(selectedRowKeys);
    }

    return (
        <>
            <div style={{ marginBottom: '10px' }}>
                <Radio.Group defaultValue="" buttonStyle="solid">
                    <Radio.Button value="" style={{ width: "80px", textAlign: "center" }}>全部</Radio.Button>
                    <Radio.Button value="0" style={{ width: "80px", textAlign: "center" }}>就绪</Radio.Button>
                    <Radio.Button value="1" style={{ width: "80px", textAlign: "center" }}>运行中</Radio.Button>
                    <Radio.Button value="2" style={{ width: "80px", textAlign: "center" }}>成功</Radio.Button>
                    <Radio.Button value="3" style={{ width: "80px", textAlign: "center" }}>失败</Radio.Button>
                </Radio.Group>
                <Select style={{ width: 300, marginLeft: 20 }} placeholder="请选择job类型">
                </Select>
            </div>
            <div style={{ marginBottom: '10px' }}>
                <Button onClick={deleteRecords}><Space><FontAwesomeIcon icon={faTrash} />删除记录</Space></Button>
            </div>
            <Table style={{ marginBottom: '10px' }} columns={taskTableColumns} dataSource={taskTableData} pagination={false}
                bordered scroll={{ x: 1600 }} rowSelection={{ type: "checkbox", onChange: rowSelectChange }}></Table>
            <Pagination pageSize={size} total={total} current={page} showSizeChanger={true} />
        </>
    );
}