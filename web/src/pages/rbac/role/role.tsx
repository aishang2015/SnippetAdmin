import { Button, Divider, Form, Input, Modal, Pagination, Space, Switch, Table, Tooltip, Tree, TreeSelect } from 'antd';

import './role.css';
import { useEffect, useState } from 'react';
import { useForm } from 'antd/lib/form/Form';
import { RoleService } from '../../../http/requests/rbac/role';
import { ElementService } from '../../../http/requests/rbac/element';
import { RightElement } from '../../../components/right/rightElement';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBroom, faCircleNotch, faEdit, faPlus, faRefresh, faSave, faSearch, faTrash, faUserTag } from '@fortawesome/free-solid-svg-icons';
import Title from 'antd/es/typography/Title';
import { useToken } from 'antd/es/theme/internal';

export default function Role() {

    // !全局样式    
    const [_, token] = useToken();
    const [modal, contextHolder] = Modal.useModal();

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

    const [searchObject, setSearchObject] = useState<any>({});

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
                await getRoles();
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
            await getRoles();
            setRoleModalVisible(false);
        }
        finally {
            setIsLoading(false);
        }
    }

    useEffect(() => {
        init();
    }, []);// eslint-disable-line react-hooks/exhaustive-deps

    useEffect(() => {
        getRoles();
    }, [page, size, searchObject])

    async function init() {
        await getRoles();
        await getTreeData();
    }

    async function getRoles() {
        let result = await RoleService.getRoles({
            page: page,
            size: size,
            name: searchObject.name,
            code: searchObject.code,
        });
        setTotal(result.data.data.total);
        setRoleTableData(result.data.data.data);
    }

    // 取得权限数据
    async function getTreeData() {
        let response = await ElementService.getElementTree();
        setRightTree(response.data.data);
    }

    //#region  搜索

    const [searchModalVisible, setSearchModalVisible] = useState(false);
    const [searchForm] = useForm();

    async function openSearchModal() {
        setSearchModalVisible(true);
    }

    async function searchSubmit(values: any) {
        setPage(1);
        setSearchObject({
            name: values["roleName"],
            code: values["roleCode"],
        });
        setSearchModalVisible(false);
    }

    //#endregion


    return (
        <>
            {contextHolder}

            {/* 操作 */}
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: "6px" }}>
                <div style={{ display: 'flex', alignItems: 'center' }}>
                    <FontAwesomeIcon icon={faUserTag} style={{ marginRight: '8px', fontSize: "18px" }} />
                    <Title level={4} style={{ marginBottom: 0 }}>角色信息</Title>
                </div>
                <div>
                    <Tooltip title="搜索" color={token.colorPrimary}>
                        <Button type="primary" icon={<FontAwesomeIcon icon={faSearch} />} style={{ marginRight: '4px' }} onClick={openSearchModal} />
                    </Tooltip>
                    <Tooltip title="重置条件并搜索" color={token.colorPrimary}>
                        <Button type="primary" icon={<FontAwesomeIcon icon={faBroom} />} style={{ marginRight: '4px' }}
                            onClick={() => { setPage(1); setSearchObject({}); }} />
                    </Tooltip>
                    <Tooltip title="刷新" color={token.colorPrimary}>
                        <Button type="primary" icon={<FontAwesomeIcon icon={faRefresh} />} style={{ marginRight: '4px' }} onClick={init} />
                    </Tooltip>
                    <RightElement identify="create-role" child={
                        <>
                            <Tooltip title="新建" color={token.colorPrimary}>
                                <Button type="primary" icon={<FontAwesomeIcon icon={faPlus} />} style={{ marginRight: '4px' }} onClick={createRole} />
                            </Tooltip>
                        </>
                    }></RightElement>
                </div>
            </div>

            <Divider style={{ margin: '14px 0' }} />

            <div id="role-container">
                <Table columns={roleTableColumns} dataSource={roleTableData} pagination={false} size="small" ></Table>
                {total > 0 &&
                    <Pagination current={page} total={total} showSizeChanger={false} style={{ marginTop: '10px' }}
                        onChange={async (p, s) => { setPage(p); setSize(s); }}></Pagination>
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
                    <Form.Item name="rights" label="权限">
                        <TreeSelect maxTagCount={3} placeholder="请选择权限" treeData={rightTree} treeCheckable={true} showCheckedStrategy="SHOW_ALL"></TreeSelect>
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6 }}>
                        <Button type='primary' icon={<FontAwesomeIcon fixedWidth icon={faSave} />} htmlType="submit" loading={isLoading}>保存</Button>
                    </Form.Item>
                </Form>
            </Modal>

            <Modal open={rightModalVisible} onCancel={() => setRightModalVisible(false)} title="角色权限" footer={null}>
                <Tree checkable checkedKeys={rights} defaultExpandAll={true} treeData={rightTree} />
            </Modal>

            {/*搜索模态框*/}
            <Modal open={searchModalVisible} onCancel={() => setSearchModalVisible(false)}
                footer={null} title="搜索">
                <Form form={searchForm} onFinish={searchSubmit} labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} preserve={false}>
                    <Form.Item name="roleName" label="角色名">
                        <Input autoComplete="off" placeholder="请输入角色名" />
                    </Form.Item>
                    <Form.Item name="roleCode" label="角色编码">
                        <Input autoComplete="off" placeholder="请输入角色编码" />
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6 }}>
                        <Button type='primary' icon={<FontAwesomeIcon fixedWidth icon={faSearch} />}
                            htmlType="submit" loading={isLoading}>搜索</Button>
                    </Form.Item>
                </Form>
            </Modal>
        </>
    );
}