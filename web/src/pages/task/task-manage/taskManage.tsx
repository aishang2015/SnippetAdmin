import { Button, Form, Input, Modal, Space, Switch, Table, Tag, Tooltip } from 'antd';
import { useEffect, useState } from 'react';
import { EditOutlined } from "@ant-design/icons";

import './taskManage.less';
import { initial } from 'lodash';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faPlay, faPlus, faRemoveFormat, faSave, faStop, faTrash } from '@fortawesome/free-solid-svg-icons';
import SaveOutlined from '@ant-design/icons/lib/icons/SaveOutlined';


export default function TaskManage(props: any) {

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
        { title: '上次执行时间', dataIndex: "lastTime", align: 'center', width: '180px' },
        { title: '创建时间', dataIndex: "createTime", align: 'center', width: '180px' },
        {
            title: '状态', dataIndex: "isActive", align: 'center', width: '120px',
            render: (data: any, record: any) => (
                <Switch defaultChecked={data}
                    checkedChildren="启动"
                    unCheckedChildren="禁用"
                    onChange={(checked, event) => activeChange(checked, event, record.id, record.isActive)}></Switch>
            ),
        },
        {
            title: '操作', key: 'operate', align: 'center', width: '120px',
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
    }, []);

    function initial() {
        setTaskTableData([
            { key: "1", name: 'SnippetAdmin.Data.Entity.Scheduler.SiNGeognoegeonJob', describe: "定时下达开关栓指令", cron: "0 0/10 * * * ?", isActive: true },
            { key: "2", name: '定时下达开关栓指令', describe: "定时下达开关栓指令", cron: "0 0/10 * * * ?", lastTime: "2021/10/22 08:12:12", createTime: "2021/10/22 08:12:12", isActive: true },
            { key: "3", name: '定时下达开关栓指令', describe: "定时下达开关栓指令", cron: "0 0/10 * * * ?", isActive: true },
            { key: "4", name: '定时下达开关栓指令', describe: "定时下达开关栓指令", cron: "0 0/10 * * * ?", isActive: true },
            { key: "5", name: '定时下达开关栓指令', describe: "定时下达开关栓指令", cron: "0 0/10 * * * ?", isActive: true },
            { key: "6", name: '定时下达开关栓指令', describe: "定时下达开关栓指令", cron: "0 0/10 * * * ?", isActive: true },
            { key: "7", name: '定时下达开关栓指令', describe: "定时下达开关栓指令", cron: "0 0/10 * * * ?", isActive: true },
            { key: "8", name: '定时下达开关栓指令', describe: "定时下达开关栓指令", cron: "0 0/10 * * * ?", isActive: true },
            { key: "9", name: '定时下达开关栓指令', describe: "定时下达开关栓指令", cron: "0 0/10 * * * ?", isActive: true },
            { key: "10", name: '定时下达开关栓指令', describe: "定时下达开关栓指令", cron: "0 0/10 * * * ?", isActive: true },
            { key: "11", name: '定时下达开关栓指令', describe: "定时下达开关栓指令", cron: "0 0/10 * * * ?", isActive: true },
            { key: "12", name: '定时下达开关栓指令', describe: "定时下达开关栓指令", cron: "0 0/10 * * * ?", isActive: true },
        ]);
    }

    async function activeChange(checked: boolean, event: Event, roleId: number, isActive: boolean) {

    }

    function addTask() {
        setEditModalVisible(true);
    }

    function editTask(id: number) {
        setEditModalVisible(true);
    }

    function deleteTask(id: number) {

    }

    function runTask(id: number) {

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
                <Form form={editForm} preserve={false} labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} >
                    <Form.Item name="id" hidden ><Input /></Form.Item>
                    <Form.Item name="name" label="任务名" >
                        <Input placeholder="请输入任务名" autoComplete="off2" />
                    </Form.Item>
                    <Form.Item name="describe" label="任务描述" >
                        <Input placeholder="请输入任务描述" autoComplete="off" />
                    </Form.Item>
                    <Form.Item name="cron" label="执行计划" >
                        <Input placeholder="请输入Cron表达式" autoComplete="off" />
                    </Form.Item>
                    <Form.Item name="isActive" label="是否启用" >
                        <Switch></Switch>
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6 }}>
                        <Button htmlType="submit"><Space><FontAwesomeIcon icon={faSave} /> 保存</Space></Button>
                    </Form.Item>
                </Form>
            </Modal>
        </>
    );
}