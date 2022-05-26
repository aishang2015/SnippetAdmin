import { faTrash } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Button, Modal, Pagination, Radio, Select, Space, Table, Tag, Tooltip } from 'antd';
import { useEffect, useState } from 'react';
import { dateFormat } from '../../../common/time';
import { JobService } from '../../../http/requests/job';
import { JobRecordService } from '../../../http/requests/job-record';
import './taskRecord.less';

export default function TaskRecord(props: any) {

    const [page, setPage] = useState(1);
    const [total, setTotal] = useState(0);
    const [size, setSize] = useState(10);

    const [jobNames, setJobNames] = useState(new Array<string>());
    const [selectedJobName, setSelectedJobName] = useState<any>(undefined);
    const [selectedJobState, setSelectedJobState] = useState<any>(undefined);

    const [selectedRowKeys, setSelectedRowKeys] = useState(new Array<any>());

    const [taskTableData, setTaskTableData] = useState(new Array<any>());

    const taskTableColumns: any = [
        {
            title: '编号', dataIndex: "id", align: 'center', width: '100px',
        },
        {
            title: '状态', dataIndex: "jobState", align: 'center', width: '120px',
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
        { title: '任务名', dataIndex: "name", align: 'center', width: '300px' },
        { title: '任务描述', dataIndex: "describe", align: 'center' },
        { title: '执行时长', dataIndex: "duration", align: 'center', width: '180px' },
        {
            title: '执行时间', dataIndex: "beginTime", align: 'center', width: '180px',
            render: (date: any) => dateFormat(date)
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
    }, []); // eslint-disable-line react-hooks/exhaustive-deps

    // 初始化
    async function initial() {
        let jobNamesResponse = await JobService.GetJobNames();
        setJobNames(jobNamesResponse.data.data);

        await initTableAsync(page, size, selectedJobState, selectedJobName);
    }

    // 初始化表格数据
    async function initTableAsync(page: number, size: number, jobState: number | undefined, jobName: string) {
        let tableDataResponse = await JobRecordService.GetJobRecords({
            page: page,
            size: size,
            jobState: jobState,
            jobName: jobName
        });
        tableDataResponse.data.data.data.forEach((d: any) => d.key = d.id);
        setTaskTableData(tableDataResponse.data.data.data);
        setTotal(tableDataResponse.data.data.total);
    }

    async function pageChange(page: number, pageSize?: number) {
        setPage(page);
        if (pageSize !== undefined && size !== pageSize) {
            setPage(1);
            setSize(pageSize);
            await initTableAsync(1, pageSize!, selectedJobState, selectedJobName);
        } else {
            await initTableAsync(page, size, selectedJobState, selectedJobName);
        }
    }

    async function jobNameChange(value: any) {
        setSelectedJobName(value);
        await initTableAsync(page, size, selectedJobState, value);
    }

    async function jobStateChange(event: any) {
        if (event.target.value === '') event.target.value = undefined;
        setSelectedJobState(event.target.value);
        await initTableAsync(page, size, event.target.value, selectedJobName);
    }


    function deleteRecord(id: number) {
        Modal.confirm({
            title: '是否删除该记录？',
            onOk: async () => {
                await JobRecordService.RemoveJobRecords({ recordIds: [id] });
                await initTableAsync(page, size, selectedJobState, selectedJobName);
            }
        })
    }

    function deleteRecords() {
        Modal.confirm({
            title: '是否删除这些记录？',
            onOk: async () => {
                await JobRecordService.RemoveJobRecords({ recordIds: selectedRowKeys });
                await initTableAsync(page, size, selectedJobState, selectedJobName);
            }
        })
    }

    function rowSelectChange(selectedRowKeys: any) {
        setSelectedRowKeys(selectedRowKeys);
    }

    return (
        <>
            <div style={{ marginBottom: '10px' }}>
                <Radio.Group defaultValue="" buttonStyle="solid" onChange={jobStateChange}>
                    <Radio.Button value="" style={{ width: "80px", textAlign: "center" }}>全部</Radio.Button>
                    <Radio.Button value="0" style={{ width: "80px", textAlign: "center" }}>就绪</Radio.Button>
                    <Radio.Button value="1" style={{ width: "80px", textAlign: "center" }}>运行中</Radio.Button>
                    <Radio.Button value="2" style={{ width: "80px", textAlign: "center" }}>成功</Radio.Button>
                    <Radio.Button value="3" style={{ width: "80px", textAlign: "center" }}>失败</Radio.Button>
                </Radio.Group>
                <Select style={{ width: 300, marginLeft: 20 }} placeholder="请选择job类型" allowClear onChange={jobNameChange}>
                    {jobNames.map(name => (
                        <Select.Option key={name} value={name}>{name}</Select.Option>
                    ))}
                </Select>
            </div>
            <div style={{ marginBottom: '10px' }}>
                <Button onClick={deleteRecords}><Space><FontAwesomeIcon icon={faTrash} />删除记录</Space></Button>
            </div>
            <Table style={{ marginBottom: '10px' }} columns={taskTableColumns} dataSource={taskTableData} pagination={false}
                bordered scroll={{ x: 1600 }} rowSelection={{ type: "checkbox", onChange: rowSelectChange }}></Table>
            <Pagination pageSize={size} total={total} current={page} showSizeChanger={true} onChange={pageChange} />
        </>
    );
}