import { faCircleNotch, faClipboardCheck, faRefresh, faTrash } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Button, Divider, Modal, Pagination, Radio, Select, Space, Table, Tag, Tooltip } from 'antd';
import { useEffect, useState } from 'react';
import { dateFormat } from '../../../common/time';
import { JobService } from '../../../http/requests/task/job';
import { JobRecordService } from '../../../http/requests/task/job-record';
import { useToken } from 'antd/es/theme/internal';
import Title from 'antd/es/typography/Title';

export default function TaskRecord(props: any) {

    // !全局样式    
    const [_, token] = useToken();
    const [modal, contextHolder] = Modal.useModal();

    const [page, setPage] = useState(1);
    const [total, setTotal] = useState(0);
    const [size, setSize] = useState(10);

    const [jobNames, setJobNames] = useState(new Array<string>());
    const [selectedJobName, setSelectedJobName] = useState<any>(undefined);
    const [selectedJobState, setSelectedJobState] = useState<any>(undefined);

    const [selectedRowKeys, setSelectedRowKeys] = useState(new Array<any>());

    const [taskTableData, setTaskTableData] = useState(new Array<any>());

    const [jobTypeList, setJobTypeList] = useState(new Array<string>());

    const taskTableColumns: any = [
        {
            title: '编号', dataIndex: "id", align: 'center', width: '100px', fixed: 'left',
            render: (data: any, record: any, index: any) => (
                <span>{1 + index + size * (page - 1)} </span>
            )
        },
        {
            title: '状态', dataIndex: "jobState", align: 'center', width: '80px', fixed: 'left',
            render: (text: any, record: any) => {
                if (text === 1) {
                    return (<Tag color="green">成功</Tag>);
                } else if (text === 2) {
                    return (<Tag color="green">失败</Tag>);
                } else if (text === 3) {
                    return (<Tag color="green">运行中</Tag>);
                }
            },
        },
        { title: '任务类型', dataIndex: "jobType", align: 'center' },
        { title: '任务描述', dataIndex: "describe", align: 'center' },
        { title: '执行时长', dataIndex: "duration", align: 'center', width: '120px' },
        {
            title: '开始时间', dataIndex: "beginTime", align: 'center', width: '160px',
            render: (date: any) => dateFormat(date)
        },
        {
            title: '结束时间', dataIndex: "endTime", align: 'center', width: '160px',
            render: (date: any) => dateFormat(date)
        },
        { title: '执行结果', dataIndex: "infomation", align: 'center', width: '300px', fixed: "right" },
        {
            title: '操作', key: 'operate', align: 'center', width: '60px', fixed: 'right',
            render: (text: any, record: any) => (
                <div>
                    {record.jobState !== 3 &&
                        <Tooltip title="删除记录">
                            <Button type='link' style={{ padding: '4px 6px' }} onClick={() => { deleteRecord(record.id) }}><FontAwesomeIcon icon={faTrash} /></Button>
                        </Tooltip>
                    }
                </div>
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


        let jobTypeList = await JobService.GetJobTypeList();
        setJobTypeList(jobTypeList.data.data);
    }

    // 初始化表格数据
    async function initTableAsync(page: number, size: number, jobState: number | undefined, jobType: string) {
        let tableDataResponse = await JobRecordService.GetJobRecords({
            page: page,
            size: size,
            jobState: jobState,
            jobType: jobType
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
            {contextHolder}

            {/* 操作 */}
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: "14px" }}>

                <div style={{ display: 'flex', alignItems: 'center' }}>
                    <FontAwesomeIcon icon={faClipboardCheck} style={{ marginRight: '8px', fontSize: "18px" }} />
                    <Title level={4} style={{ marginBottom: 0 }}>任务管理</Title>
                </div>
                <div>
                    <Select style={{ width: 300, marginRight: 8 }} placeholder="请选择job类型" allowClear
                        onChange={jobNameChange}>
                        {
                            jobTypeList.map(o => (
                                <Select.Option value={o} key={o}>{o}</Select.Option>
                            ))
                        }
                    </Select>
                    <Radio.Group defaultValue="" buttonStyle="solid" onChange={jobStateChange} style={{ marginRight: '10px' }}>
                        <Radio.Button value="" style={{ width: "80px", textAlign: "center" }}>全部</Radio.Button>
                        <Radio.Button value="1" style={{ width: "80px", textAlign: "center" }}>成功</Radio.Button>
                        <Radio.Button value="2" style={{ width: "80px", textAlign: "center" }}>失败</Radio.Button>
                        <Radio.Button value="3" style={{ width: "80px", textAlign: "center" }}>运行中</Radio.Button>
                    </Radio.Group>
                    <Tooltip title="刷新" color={token.colorPrimary}>
                        <Button type="primary" icon={<FontAwesomeIcon icon={faRefresh} />} style={{ marginRight: '4px' }}
                            onClick={initial} />
                    </Tooltip>
                </div>
            </div>

            <Divider style={{ margin: '14px 0' }} />

            <Table style={{ marginBottom: '10px' }} columns={taskTableColumns} dataSource={taskTableData} pagination={false}
                bordered scroll={{ x: 1700 }} size="small"></Table>
            <Pagination pageSize={size} total={total} current={page} showSizeChanger={true} onChange={pageChange} />
        </>
    );
}