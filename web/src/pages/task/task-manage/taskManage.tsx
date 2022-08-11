import { Button, Form, Input, Modal, Pagination, Select, Space, Switch, Table, Tag, Tooltip } from 'antd';
import { useEffect, useState } from 'react';
import { dateFormat } from '../../../common/time';

import './taskManage.less';
import { faCircleNotch, faEdit, faInfoCircle, faPlay, faPlus, faSave, faTrash } from '@fortawesome/free-solid-svg-icons';
import { JobService } from '../../../http/requests/job';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';


export default function TaskManage(props: any) {

    const [page, setPage] = useState<number>(1);
    const [size, setSize] = useState<number>(20);
    const [total, setTotal] = useState<number>(0);

    const [jobTypeList, setJobTypeList] = useState(new Array<string>());

    const [editModalVisible, setEditModalVisible] = useState(false);
    const [editForm] = Form.useForm();

    const [taskTableData, setTaskTableData] = useState(new Array<any>());

    const taskTableColumns: any = [
        {
            title: '编号', dataIndex: "id", align: 'center', width: '100px',
            render: (data: any, record: any, index: any) => (
                <span>{1 + index + size * (page - 1)} </span>
            )
        },
        { title: '任务名', dataIndex: "name", align: 'center', width: '300px' },
        { title: '任务描述', dataIndex: "describe", align: 'center' },
        {
            title: '执行计划', dataIndex: "cron", align: 'center', width: '120px',
            render: (data: any, record: any) => (
                <Tag color="magenta">{data}</Tag>
            ),
        },
        {
            title: '上次执行时间', dataIndex: "lastTime", align: 'center', width: '180px',
            render: (date: any) => dateFormat(date)
        },
        {
            title: '创建时间', dataIndex: "createTime", align: 'center', width: '180px',
            render: (date: any) => dateFormat(date)
        },
        {
            title: '状态', dataIndex: "isActive", align: 'center', width: '120px',
            render: (data: any, record: any) => (
                <Switch defaultChecked={data}
                    checkedChildren="启动"
                    unCheckedChildren="暂停"
                    onChange={(checked, event) => activeChange(checked, record.id)}></Switch>
            ),
        },
        {
            title: '操作', key: 'operate', align: 'center', width: '120px', fixed: 'right',
            render: (text: any, record: any) => (
                <Space size="middle">
                    <Tooltip title="编辑任务">
                        <a onClick={() => { editTask(record.id) }}><FontAwesomeIcon icon={faEdit} /></a>
                    </Tooltip>
                    <Tooltip title="删除任务">
                        <a onClick={() => { deleteTask(record.id) }}><FontAwesomeIcon icon={faTrash} /></a>
                    </Tooltip>
                    <Tooltip title="立即执行任务">
                        <a onClick={() => { runTask(record.id) }}><FontAwesomeIcon icon={faPlay} /></a>
                    </Tooltip>
                </Space>
            ),
        }
    ];

    useEffect(() => {
        initial();
    }, [page, size]);// eslint-disable-line react-hooks/exhaustive-deps

    async function initial() {

        let response = await JobService.GetJobs({ page: page, size: size });
        setTaskTableData(response.data.data.data);
        setTotal(response.data.data.total);

        let jobTypeList = await JobService.GetJobTypeList();
        setJobTypeList(jobTypeList.data.data);
    }

    async function activeChange(checked: boolean, id: number) {
        await JobService.ActiveJob({ id: id, isActive: checked });
    }

    function addTask() {
        setEditModalVisible(true);
    }

    async function editTask(id: number) {
        let job = await JobService.GetJob({ id: id });
        editForm.setFieldsValue({
            id: job.data.data.id,
            type: job.data.data.type,
            name: job.data.data.name,
            describe: job.data.data.describe,
            cron: job.data.data.cron
        });
        setEditModalVisible(true);
    }

    async function submitJob(jobs: any) {
        if (jobs["id"]) {
            await JobService.UpdateJob({
                id: jobs["id"],
                type: jobs["type"],
                name: jobs["name"],
                describe: jobs["describe"],
                cron: jobs["cron"]
            });
        } else {
            await JobService.AddJob({
                type: jobs["type"],
                name: jobs["name"],
                describe: jobs["describe"],
                cron: jobs["cron"]
            });
        }
        setEditModalVisible(false);
        await initial();
    }

    function deleteTask(id: number) {
        Modal.confirm({
            title: "是否删除该任务?删除该任务后，任务关联的执行记录也会被一并清除。",
            onOk: async () => {
                await JobService.DeleteJob({ id: id });
                await initial();
            }
        });
    }

    async function runTask(id: number) {
        await JobService.RunJob({ id: id });
    }

    async function pageChange(p: number, s?: number) {
        if (p !== page) {
            setPage(p);
        }
        if (s !== size && s !== undefined) {
            setPage(1);
            setSize(s);
        }
    }

    return (
        <>
            <div style={{ marginBottom: '10px' }}>
                <Button style={{ marginRight: '10px' }} icon={<FontAwesomeIcon icon={faCircleNotch} fixedWidth />}
                    onClick={initial}>刷新</Button>
                <Button type="primary" onClick={addTask}><FontAwesomeIcon icon={faPlus} fixedWidth></FontAwesomeIcon>创建新任务</Button>
            </div>
            <Table style={{ marginBottom: '10px' }} columns={taskTableColumns} dataSource={taskTableData} pagination={false}
                bordered scroll={{ x: 1600 }} size="small"></Table>
            <Pagination pageSize={size} total={total} current={page} showSizeChanger={true} onChange={pageChange} />

            <Modal visible={editModalVisible} destroyOnClose={true} onCancel={() => setEditModalVisible(false)}
                footer={null} title="任务信息编辑" maskClosable={false}>
                <Form form={editForm} preserve={false} labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}
                    onFinish={submitJob}>
                    <Form.Item name="id" hidden ><Input /></Form.Item>
                    <Form.Item name="type" label="任务类型" rules={
                        [
                            { required: true, message: "请选择任务类型" },
                        ]}
                    >
                        <Select placeholder="请选择任务类型">
                            {
                                jobTypeList.map(o => (
                                    <Select.Option value={o} key={o}>{o}</Select.Option>
                                ))
                            }
                        </Select>
                    </Form.Item>
                    <Form.Item name="name" label="任务名称" rules={
                        [
                            { required: true, message: "请输入任务名称" },
                            { max: 40, message: "任务名称过长" },
                        ]}
                    >
                        <Input placeholder="请输入任务名" autoComplete="off2" />
                    </Form.Item>
                    <Form.Item name="describe" label="任务描述" rules={
                        [
                            { required: true, message: "请输入任务名称" },
                            { max: 80, message: "描述文字过长" },
                        ]}>
                        <Input placeholder="请输入任务描述" autoComplete="off" />
                    </Form.Item>
                    <Form.Item name="cron" label="执行计划" rules={
                        [
                            { required: true, message: "请输入任务名称" },
                            { max: 40, message: "任务名称过长" },
                            { pattern: /(@(annually|yearly|monthly|weekly|daily|hourly|reboot))|(@every (\d+(ns|us|µs|ms|s|m|h))+)|((((\d+,)+\d+|(\d+(\/|-)\d+)|\d+|\*) ?){5,7})/, message: "cron格式不对" }
                        ]}>
                        <Input placeholder="请输入Cron表达式" autoComplete="off" suffix={
                            <Tooltip title="cron表达式">
                                <FontAwesomeIcon icon={faInfoCircle} style={{ color: 'rgba(0,0,0,.45)' }} />
                            </Tooltip>
                        } />
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6 }}>
                        <Button htmlType="submit"><Space><FontAwesomeIcon icon={faSave} /> 保存</Space></Button>
                    </Form.Item>
                </Form>
            </Modal>
        </>
    );
}

