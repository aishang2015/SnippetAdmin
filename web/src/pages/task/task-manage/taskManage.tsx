import { Button, Form, Input, Modal, Space, Switch, Table, Tag, Tooltip } from 'antd';
import { useEffect, useState } from 'react';
import { dateFormat } from '../../../common/time';

import './taskManage.less';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faPlay, faPlus, faSave, faTrash } from '@fortawesome/free-solid-svg-icons';
import InfoCircleOutlined from '@ant-design/icons/lib/icons/InfoCircleOutlined';
import { JobService } from '../../../http/requests/job';


export default function TaskManage(props: any) {

    const [page, setPage] = useState<number>(1);
    const [size, setSize] = useState<number>(20);

    const [editModalVisible, setEditModalVisible] = useState(false);
    const [editForm] = Form.useForm();

    const [taskTableData, setTaskTableData] = useState(new Array<any>());

    const taskTableColumns: any = [
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
            title: '操作', key: 'operate', align: 'center', width: '120px',fixed: 'right',
            render: (text: any, record: any) => (
                <Space size="middle">
                    <Tooltip title="编辑任务">
                        <a onClick={() => { editTask(record.id) }}><FontAwesomeIcon icon={faEdit} fixedWidth /></a>
                    </Tooltip>
                    <Tooltip title="删除任务">
                        <a onClick={() => { deleteTask(record.id) }}><FontAwesomeIcon icon={faTrash} fixedWidth /></a>
                    </Tooltip>
                    <Tooltip title="立即执行任务">
                        <a onClick={() => { runTask(record.id) }}><FontAwesomeIcon icon={faPlay} fixedWidth /></a>
                    </Tooltip>
                </Space>
            ),
        }
    ];

    useEffect(() => {
        initial();
    }, []);// eslint-disable-line react-hooks/exhaustive-deps

    async function initial() {

        let response = await JobService.GetJobs({ page: page, size: size });
        setTaskTableData(response.data.data.data);
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
                name: jobs["name"],
                describe: jobs["describe"],
                cron: jobs["cron"]
            });
        } else {
            await JobService.AddJob({
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

    return (
        <>
            <div style={{ marginBottom: '10px' }}>
                <Button type="primary" onClick={addTask}>
                    <Space><FontAwesomeIcon icon={faPlus} fixedWidth></FontAwesomeIcon>创建新任务</Space></Button>
            </div>
            <Table columns={taskTableColumns} dataSource={taskTableData} pagination={{ position: ["bottomLeft"], pageSize: 10 }}
                bordered scroll={{ x: 1600 }}></Table>

            <Modal visible={editModalVisible} destroyOnClose={true} onCancel={() => setEditModalVisible(false)}
                footer={null} title="任务信息编辑">
                <Form form={editForm} preserve={false} labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}
                    onFinish={submitJob}>
                    <Form.Item name="id" hidden ><Input /></Form.Item>
                    <Form.Item name="name" label="任务名称" rules={
                        [
                            { required: true, message: "请输入任务名称" },
                            { max: 40, message: "任务名称过长" },
                        ]}
                    >
                        <Input placeholder="请输入任务名" autoComplete="off2"
                            suffix={
                                <Tooltip title="任务名称为包含命名空间的类的全名">
                                    <InfoCircleOutlined style={{ color: 'rgba(0,0,0,.45)' }} />
                                </Tooltip>
                            } />
                    </Form.Item>
                    <Form.Item name="describe" label="任务描述" rules={
                        [
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
                            <Tooltip title="任务名称为包含命名空间的类的全名">
                                <InfoCircleOutlined style={{ color: 'rgba(0,0,0,.45)' }} />
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

