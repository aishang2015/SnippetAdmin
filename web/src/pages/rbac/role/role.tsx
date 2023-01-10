import { Button, Divider, Form, Input, Modal, Pagination, Space, Switch, Table, Tooltip, Tree, TreeSelect } from 'antd';

import './role.css';
import { useEffect, useState } from 'react';
import { useForm } from 'antd/lib/form/Form';
import { RoleService } from '../../../http/requests/role';
import { ElementService } from '../../../http/requests/element';
import { RightElement } from '../../../components/right/rightElement';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faPlus, faSave, faSearch, faTrash } from '@fortawesome/free-solid-svg-icons';

export default function Role() {

    const [rightTree, setRightTree] = useState(new Array<any>());
    const [page, setPage] = useState(1);
    const [size, setSize] = useState(10);
    const [total, setTotal] = useState(0);

    const { TextArea } = Input;

    const [roleTableData, setRoleTableData] = useState(new Array<any>());

    const [isLoading, setIsLoading] = useState(false);

    const roleTableColumns: any = [
        {
            title: '序号', dataIndex: "num", align: 'center', width: '90px',
            render: (data: any, record: any, index: any) => (
                <span>{(page - 1) * size + 1 + index}</span>
            )
        },
        { title: '名称', dataIndex: "name", align: 'center', width: '220px' },
        { title: '角色编码', dataIndex: "code", align: 'center', width: '220px' },
        { title: '备注', dataIndex: "remark", align: 'center' },
        {
            title: '启用', dataIndex: "isActive", align: 'center', width: '90px',
            render: (data: any, record: any) => (
                <Switch defaultChecked={data} onChange={(checked, event) => activeChange(checked, record.id, record.isActive)}></Switch>
            ),
        },
        {
            title: '操作', key: 'operate', align: 'center', width: '130px',
            render: (text: any, record: any) => (
                <div>
                    <RightElement identify="edit-role" child={
                        <>
                            <Tooltip title="编辑角色">
                                <Button type='link' style={{ padding: '4px 6px' }} onClick={() => editRole(record.id)}><FontAwesomeIcon icon={faEdit} /></Button>
                            </Tooltip>
                        </>
                    }></RightElement>
                    <RightElement identify="remove-role" child={
                        <>
                            <Tooltip title="删除角色">
                                <Button type='link' style={{ padding: '4px 6px' }} onClick={() => deleteRole(record.id)}><FontAwesomeIcon icon={faTrash} /></Button>
                            </Tooltip>
                        </>
                    }></RightElement>
                    <Tooltip title="查看权限">
                        <Button type='link' style={{ padding: '4px 6px' }} onClick={() => viewRight(record.id)}><FontAwesomeIcon icon={faSearch} /></Button>
                    </Tooltip>
                </div>
            ),
        }
    ];

    const [roleModalVisible, setRoleModalVisible] = useState(false);
    const [roleEditForm] = useForm();

    const [rightModalVisible, setRightModalVisible] = useState(false);
    const [rights, setRights] = useState<any>();

    function createRole() {
        setRoleModalVisible(true);
    }

    async function activeChange(checked: boolean, roleId: number, isActive: boolean) {
        await RoleService.activeRole({ id: roleId, isActive: checked });
    }

    async function editRole(id: number) {
        let role = await RoleService.getRole({ id: id });
        roleEditForm.setFieldsValue({
            id: role.data.data.id,
            roleName: role.data.data.name,
            code: role.data.data.code,
            remark: role.data.data.remark,
            rights: role.data.data.rights
        });
        setRoleModalVisible(true);
    }

    // 删除角色
    function deleteRole(id: number) {
        Modal.confirm({
            title: '是否删除该角色?',
            onOk: async () => {
                await RoleService.removeRole({ id: id });
                await getRoles(page, size);
            }
        })
    }

    // 查看权限
    async function viewRight(id: number) {
        let role = await RoleService.getRole({ id: id });
        setRights(role.data.data.rights);
        setRightModalVisible(true);
    }

    // 提交角色信息
    async function roleInfoSubmit(values: any) {
        try {
            setIsLoading(true);
            await RoleService.addOrUpdateRole({
                id: values["id"],
                name: values["roleName"],
                code: values["code"],
                remark: values["remark"],
                rights: values["rights"]
            });
            await getRoles(page, size);
            setRoleModalVisible(false);
        }
        finally {
            setIsLoading(false);
        }
    }

    useEffect(() => {
        init();
    }, []);// eslint-disable-line react-hooks/exhaustive-deps

    async function init() {
        await getRoles(page, size);
        await getTreeData();
    }

    async function getRoles(page: number, size: number) {
        let result = await RoleService.getRoles({ page: page, size: size });
        setTotal(result.data.data.total);
        setRoleTableData(result.data.data.data);
    }

    // 取得权限数据
    async function getTreeData() {
        let response = await ElementService.getElementTree();
        setRightTree(response.data.data);
    }


    return (
        <>
            <div id="role-container">
                <RightElement identify="create-role" child={
                    <>
                        <Space style={{ marginTop: "10px" }}>
                            <Button icon={<FontAwesomeIcon fixedWidth icon={faPlus} />} onClick={createRole}>创建</Button>
                        </Space>
                        <Divider style={{ margin: "10px 0" }} />
                    </>
                }></RightElement>
                <Table columns={roleTableColumns} dataSource={roleTableData} pagination={false} size="small" ></Table>
                {total > 0 &&
                    <Pagination current={page} total={total} showSizeChanger={false} style={{ marginTop: '10px' }}
                        onChange={async (p, s) => { setPage(p); setSize(s); await getRoles(p, s); }}></Pagination>
                }
            </div>

            <Modal open={roleModalVisible} title="角色信息" footer={null} onCancel={() => setRoleModalVisible(false)}
                destroyOnClose={true} maskClosable={false} width={700}>
                <Form form={roleEditForm} onFinish={roleInfoSubmit} labelCol={{ span: 6 }}
                    wrapperCol={{ span: 16 }} preserve={false}>
                    <Form.Item name="id" hidden >
                        <Input />
                    </Form.Item>
                    <Form.Item name="roleName" label="角色名" rules={
                        [
                            { required: true, message: "请输入角色名" },
                            { max: 20, message: "角色名过长" },
                        ]
                    }>
                        <Input autoComplete="off" placeholder="请输入角色名" />
                    </Form.Item>
                    <Form.Item name="code" label="角色编码" rules={
                        [
                            { required: true, message: "请输入角色编码" },
                            { max: 32, message: "角色编码过长" },
                        ]
                    }>
                        <Input autoComplete="off" placeholder="请输入角色编码" />
                    </Form.Item>
                    <Form.Item name="remark" label="备注" rules={
                        [
                            { max: 200, message: "备注过长" }
                        ]
                    }>
                        <TextArea />
                    </Form.Item>
                    <Form.Item name="rights" label="页面权限">
                        <TreeSelect placeholder="请选择权限" treeData={rightTree} treeCheckable={true} showCheckedStrategy="SHOW_ALL"></TreeSelect>
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6 }}>
                        <Button type='primary' icon={<FontAwesomeIcon fixedWidth icon={faSave} />} htmlType="submit" loading={isLoading}>保存</Button>
                    </Form.Item>
                </Form>
            </Modal>

            <Modal open={rightModalVisible} onCancel={() => setRightModalVisible(false)} title="角色权限" footer={null}>
                <Tree checkable checkedKeys={rights} defaultExpandAll={true} treeData={rightTree} />
            </Modal>
        </>
    );
}