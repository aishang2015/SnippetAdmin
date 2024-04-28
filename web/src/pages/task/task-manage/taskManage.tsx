import { Button, Divider, Form, Input, Modal, Pagination, Select, Space, Switch, Table, Tag, Tooltip } from 'antd';
import { useEffect, useState } from 'react';
import { dateFormat } from '../../../common/time';

import { faCircleNotch, faEdit, faInfoCircle, faPlay, faPlus, faRefresh, faSave, faSearch, faThumbtack, faTrash } from '@fortawesome/free-solid-svg-icons';
import { JobService } from '../../../http/requests/task/job';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { useToken } from 'antd/es/theme/internal';
import Title from 'antd/es/typography/Title';
import { RightElement } from '../../../components/right/rightElement';
import DraggableModal from '../../../components/common/draggableModal';


export default function TaskManage(props: any) {

    // !全局样式    
    const [_, token] = useToken();
    const [modal, contextHolder] = Modal.useModal();

    const [page, setPage] = useState<number>(1);
    const [size, setSize] = useState<number>(20);
    const [total, setTotal] = useState<number>(0);

    const [jobTypeList, setJobTypeList] = useState(new Array<string>());

    const [editModalVisible, setEditModalVisible] = useState(false);
    const [editForm] = Form.useForm();

    const [taskTableData, setTaskTableData] = useState(new Array<any>());

    const taskTableColumns: any = [
        {
            title: '编号', dataIndex: "id", align: 'center', width: '80px', fixed: 'left',
            render: (data: any, record: any, index: any) => (
                <span>{1 + index + size * (page - 1)} </span>
            )
        },
        { title: '任务类型', dataIndex: "type", align: 'center' },
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
            title: '状态', dataIndex: "isActive", align: 'center', width: '100px', fixed: 'right',
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
                        <Button type='link' style={{ padding: '0px' }} onClick={() => { editTask(record.id) }}><FontAwesomeIcon icon={faEdit} /></Button>
                    </Tooltip>
                    <Tooltip title="删除任务">
                        <Button type='link' style={{ padding: '0px' }} onClick={() => { deleteTask(record.id) }}><FontAwesomeIcon icon={faTrash} /></Button>
                    </Tooltip>
                    <Tooltip title="立即执行">
                        <Button type='link' style={{ padding: '0px' }} onClick={() => { runTask(record.id) }}><FontAwesomeIcon icon={faPlay} /></Button>
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
                describe: jobs["describe"],
                cron: jobs["cron"]
            });
        } else {
            await JobService.AddJob({
                type: jobs["type"],
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
            {contextHolder}

            {/* 操作 */}
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: "14px" }}>

                <div style={{ display: 'flex', alignItems: 'center' }}>
                    <FontAwesomeIcon icon={faThumbtack} style={{ marginRight: '8px', fontSize: "18px" }} />
                    <Title level={4} style={{ marginBottom: 0 }}>任务管理</Title>
                </div>
                <div>
                    <Tooltip title="刷新" color={token.colorPrimary}>
                        <Button type="primary" icon={<FontAwesomeIcon icon={faRefresh} />} style={{ marginRight: '4px' }}
                            onClick={initial} />
                    </Tooltip>
                    <RightElement identify="create-user" child={
                        <>
                            <Tooltip title="新建" color={token.colorPrimary}>
                                <Button type="primary" icon={<FontAwesomeIcon icon={faPlus} />} style={{ marginRight: '4px' }}
                                    onClick={addTask} />
                            </Tooltip>
                        </>
                    }></RightElement>
                </div>
            </div>

            <Divider style={{ margin: '14px 0' }} />

            <Table style={{ marginBottom: '10px' }} columns={taskTableColumns} dataSource={taskTableData} pagination={false}
                bordered scroll={{ x: 1400 }} size="small"></Table>
            <Pagination pageSize={size} total={total} current={page} showSizeChanger={true} onChange={pageChange} />

            <Modal modalRender={(modal) => { return <DraggableModal ><div>{modal}</div></DraggableModal> }}
                open={editModalVisible} destroyOnClose={true} onCancel={() => setEditModalVisible(false)}
                footer={null} title="任务信息编辑" maskClosable={false}>
                <Form form={editForm} preserve={false} labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}
                    onFinish={submitJob}>
                    <Form.Item name="id" hidden ><Input /></Form.Item>
                    <Form.Item name="type" label="任务类型" rules={
                        [
                            { required: true, message: "请选择任务类型" },
                        ]}
                    >
                        <Select placeholder="请选择任务类型" disabled={editForm.getFieldValue('id')}>
                            {
                                jobTypeList.map(o => (
                                    <Select.Option value={o} key={o}>{o}</Select.Option>
                                ))
                            }
                        </Select>
                    </Form.Item>
                    <Form.Item name="describe" label="任务描述" rules={
                        [
                            { required: true, message: "请输入任务描述" },
                            { max: 80, message: "描述文字过长" },
                        ]}>
                        <Input placeholder="请输入任务描述" autoComplete="off" />
                    </Form.Item>
                    <Form.Item name="cron" label="执行计划" rules={
                        [
                            { required: true, message: "请输入执行计划" },
                            { max: 40, message: "执行计划过长" },
                            { pattern: /(@(annually|yearly|monthly|weekly|daily|hourly|reboot))|(@every (\d+(ns|us|µs|ms|s|m|h))+)|((((\d+,)+\d+|(\d+(\/|-)\d+)|\d+|\*) ?){5,7})/, message: "cron格式不对" }
                        ]}>
                        <Input placeholder="请输入Cron表达式" autoComplete="off" suffix={
                            <Tooltip title="cron表达式">
                                <FontAwesomeIcon icon={faInfoCircle} style={{ color: 'rgba(0,0,0,.45)' }} />
                            </Tooltip>
                        } />
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6 }}>
                        <Button type='primary' htmlType="submit"><Space><FontAwesomeIcon icon={faSave} /> 保存</Space></Button>
                    </Form.Item>
                </Form>
            </Modal>
        </>
    );
}

