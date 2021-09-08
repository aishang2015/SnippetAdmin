import { Button, Divider, Form, Input, Modal, Pagination, Space, Switch, Table, Tooltip, TreeSelect } from 'antd';
import { SaveOutlined, PlusOutlined, EditOutlined, DeleteOutlined } from "@ant-design/icons";

import './role.less';
import { useEffect, useState } from 'react';
import TextArea from 'antd/lib/input/TextArea';
import { useForm } from 'antd/lib/form/Form';

export default function Role() {

    const [page, setPage] = useState(0);
    const [total, setTotal] = useState(0);

    const [roleTableData, setRoleTableData] = useState(new Array<any>());

    const roleTableColumns: any = [
        { title: '序号', dataIndex: "num", align: 'center', width: '100px' },
        { title: '名称', dataIndex: "name", align: 'center', width: '220px' },
        { title: '备注', dataIndex: "remark", align: 'center' },
        {
            title: '启用', dataIndex: "isActive", align: 'center', width: '120px',
            render: (data: any, record: any) => (
                <Switch checked={data}></Switch>
            ),
        },
        {
            title: '操作', key: 'operate', align: 'center', width: '120px',
            render: (text: any, record: any) => (
                <Space size="middle">
                    <Tooltip title="编辑"><a onClick={() => editRole(record.id)}><EditOutlined /></a></Tooltip>
                    <Tooltip title="删除"><a onClick={() => deleteRole(record.id)}><DeleteOutlined /></a></Tooltip>
                </Space>
            ),
        }
    ];

    const [roleModalVisible, setRoleModalVisible] = useState(false);
    const [userEditForm] = useForm();

    function createRole() {
        setRoleModalVisible(true);
    }

    function editRole(id: number) {
        setRoleModalVisible(true);
    }

    // 删除角色
    function deleteRole(id: number) {
        Modal.confirm({
            title: '是否删除该角色?'
        })
    }

    // 提交角色信息
    function roleInfoSubmit(id: number) {

    }

    useEffect(() => {
        setPage(1);
        setTotal(1);
        setRoleTableData([
            { num: 1, name: '超级管理员', remark: "全部权限", isActive: true }
        ]);
    }, []);


    return (
        <>
            <div id="role-container">
                <Space style={{ marginTop: "10px" }}>
                    <Button icon={<PlusOutlined />} onClick={createRole}>创建</Button>
                </Space>
                <Divider style={{ margin: "10px 0" }} />
                <Table columns={roleTableColumns} dataSource={roleTableData} pagination={false}></Table>
                <Pagination current={page} total={total} showSizeChanger={false} style={{ marginTop: '10px' }}></Pagination>

            </div>

            <Modal visible={roleModalVisible} title="角色信息" footer={null} onCancel={() => setRoleModalVisible(false)}
                destroyOnClose={true}>
                <Form form={userEditForm} onFinish={roleInfoSubmit} labelCol={{ span: 6 }}
                    wrapperCol={{ span: 16 }} preserve={false}>
                    <Form.Item name="id" hidden >
                        <Input />
                    </Form.Item>
                    <Form.Item name="roleName" label="角色名">
                        <Input autoComplete="off2" placeholder="请输入角色名" />
                    </Form.Item>
                    <Form.Item name="remark" label="备注">
                        <TextArea placeholder="请输入姓名" />
                    </Form.Item>
                    <Form.Item name="role" label="权限">
                        <TreeSelect placeholder="请选择权限"></TreeSelect>
                    </Form.Item>
                    <Form.Item name="phone" wrapperCol={{ offset: 6 }}>
                        <Button icon={<SaveOutlined />}>保存</Button>
                    </Form.Item>
                </Form>
            </Modal>
        </>
    );
}