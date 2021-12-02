import { Avatar, Button, Divider, Form, Input, Modal, Pagination, Radio, Select, Space, Switch, Table, Tag, Tooltip, Tree } from 'antd';
import {
    PlusOutlined, ManOutlined, WomanOutlined, UserOutlined,
    SearchOutlined, ClearOutlined, EditOutlined, DeleteOutlined, KeyOutlined,
    SaveOutlined, MinusCircleOutlined
} from "@ant-design/icons";

import './user.less';
import { useEffect, useRef, useState } from 'react';
import { useForm } from 'antd/lib/form/Form';
import { DebounceSelect } from '../../../components/common/debounceSelect';
import { UserService } from '../../../http/requests/user';
import { OrganizationService } from '../../../http/requests/organization';
import { RoleService } from '../../../http/requests/role';
import { RightElement } from '../../../components/right/rightElement';

export default function User() {

    const searchOption = useRef<any>({});

    const [page, setPage] = useState(1);
    const [total, setTotal] = useState(10);
    const [size, setSize] = useState(10);

    const [orgAddVisible, setOrgAddVisible] = useState(false);
    const [orgSettingForm] = useForm();
    const [searchForm] = useForm();

    const [userEditVisible, setUserEditVisible] = useState(false);
    const [userEditForm] = useForm();

    const [roleOptions, setRoleOptions] = useState<Array<any>>([]);

    const [pwdEditVisible, setPwdEditVisible] = useState(false);
    const [pwdEditForm] = useForm();

    const [userTableData, setUserTableData] = useState(new Array<any>());

    const [treeData, setTreeData] = useState<any>();

    const [selectedOrg, setSelectedOrg] = useState<number | null>(null);
    const [positionOptions, setPositionOptions] = useState<Array<any>>([]);

    const userTableColumns: any = [
        {
            title: '序号', dataIndex: "num", align: 'center', width: '90px', fixed: "left",
            render: (data: any, record: any, index: any) => (
                <span>{(page - 1) * size + 1 + index}</span>
            )
        },
        {
            title: '头像', dataIndex: "avatar", align: 'center', width: '80px',
            render: (data: any, record: any) => (
                data === null ?
                    <Avatar icon={<UserOutlined />} /> :
                    <Avatar src={data} />
            ),

        },
        { title: '账号', dataIndex: "userName", align: 'center', width: '160px' },
        { title: '姓名', dataIndex: "realName", align: 'center', width: '120px' },
        {
            title: '性别', dataIndex: "gender", align: 'center', width: '80px',
            render: (text: any, record: any) => {
                if (text === 0) {
                    return (<span></span>);
                } else if (text === 1) {
                    return (<ManOutlined style={{ color: "blue" }} />);
                } else {
                    return (<WomanOutlined style={{ color: "red" }} />);
                }
            },
        },
        { title: '电话', dataIndex: "phoneNumber", align: 'center', width: '130px' },
        {
            title: '角色', dataIndex: "roles", align: 'center', width: '220px',
            render: (array: any, record: any) => (
                array?.map((s: any) => (s.isActive ? <Tag key={s} style={{ marginBottom: '5px' }} color="#2db7f5">{s.roleName}</Tag>
                    : <Tag key={s} style={{ marginBottom: '5px' }} color="gray">{s.roleName}</Tag>))
            ),
        },
        {
            title: '部门/职位', dataIndex: "orgPositions", align: 'center',
            render: (array: any, record: any) => (
                array?.map((s: any) => (
                    <Tag key={s.org + s.position} style={{ marginBottom: '5px' }} color="#f50">{s.org + "/" + s.position}</Tag>)
                )
            ),
        },
        {
            title: '启用', dataIndex: "isActive", align: 'center', width: '90px', fixed: 'right',
            render: (data: any, record: any) => (
                <RightElement identify="active-user" child={
                    <>
                        <Switch defaultChecked={data} onChange={(checked, event) => { activeChange(checked, record.id) }}></Switch>
                    </>
                }></RightElement>
            ),
        },
        {
            title: '操作', dataIndex: "operate", align: 'center', width: '130px', fixed: 'right',
            render: (data: any, record: any) => (
                <Space size="middle">
                    <RightElement identify="edit-user" child={
                        <>
                            <Tooltip title="编辑"><a onClick={() => editUser(record.id)}><EditOutlined /></a></Tooltip>
                        </>
                    }></RightElement>
                    <RightElement identify="remove-user" child={
                        <>
                            <Tooltip title="删除"><a onClick={() => deleteUser(record.id)}><DeleteOutlined /></a></Tooltip>
                        </>
                    }></RightElement>
                    <RightElement identify="set-password" child={
                        <>
                            <Tooltip title="设定密码"><a onClick={() => setPwd(record.id)}><KeyOutlined /></a></Tooltip>
                        </>
                    }></RightElement>
                    <RightElement identify="move-out" child={
                        <>
                            {selectedOrg !== null && <Tooltip title="移出组织"><a onClick={() => moveOutOrg(record.id)}><MinusCircleOutlined /></a></Tooltip>}
                        </>
                    }></RightElement>
                </Space>
            ),
        },
    ];

    useEffect(() => {
        init();
    }, []);// eslint-disable-line react-hooks/exhaustive-deps

    async function init() {
        await getOrgTree();
        await getRoles();
        await getUsers();
    }

    async function getUsers() {
        let userResponse = await UserService.searchUser({
            page: page,
            size: size,
            userName: searchOption.current?.userName,
            realName: searchOption.current?.realName,
            phone: searchOption.current?.phoneNumber,
            role: searchOption.current?.role,
            org: searchOption.current?.org
        });
        userResponse.data.data.data.forEach((d: any) => d.key = d.id);
        setUserTableData(userResponse.data.data.data);
        setTotal(userResponse.data.data.total);
    }

    async function getOrgTree() {
        let response = await OrganizationService.getOrganizationTree();
        makeTreeData(response.data.data);
        setTreeData(response.data.data);

        // 将后端数据转为树格式
        function makeTreeData(data: any) {
            for (const d of data) {
                d.icon = (<>{d.icon}</>);
                if (d.children.length === 0) {
                    d.switcherIcon = (<></>)
                } else {
                    makeTreeData(d.children);
                }
            }
        }
    }

    async function getRoles() {
        let roleDicResponse = await RoleService.getRoleDic();
        setRoleOptions(roleDicResponse.data.data);
    }

    async function activeChange(checked: boolean, id: number) {
        await UserService.activeUser({ id: id, isActive: checked });
    }

    async function loadUser(username: string): Promise<Array<any>> {
        return UserService.searchUser({ page: 1, size: 20, realName: username }).then(r => r.data.data)
            .then(result => result.data.map(d => ({
                label: d.realName ?? " ",
                value: d.id,
            })))
    }

    // 添加组织成员
    async function setOrgMember() {
        let response = await OrganizationService.GetPositionDic({ id: selectedOrg! });
        setPositionOptions(response.data.data);
        setOrgAddVisible(true);
    }

    async function submitOrgMember(values: any) {
        await UserService.addOrgMember({ orgId: selectedOrg!, userIds: values["members"].map((v: any) => v.value), positions: values["positions"] });
        setOrgAddVisible(false);
        await getUsers();
    }

    // 搜索提交
    async function searchSubmit(values: any) {
        searchOption.current.userName = values['userName'];
        searchOption.current.realName = values['realName'];
        searchOption.current.phoneNumber = values['phoneNumber'];
        searchOption.current.role = values['role'];
        await getUsers();
    }

    // 选中组织
    async function orgSelect(selectedKeys: any, event: any) {
        if (event.selected) {
            searchOption.current.org = selectedKeys[0];
            setSelectedOrg(selectedKeys[0]);
        } else {
            searchOption.current.org = null;
            setSelectedOrg(null);
        }
        await getUsers();
    }

    // 点击搜索按钮
    function searchUser() {
        searchForm.submit();
    }

    // 点击重置搜索按钮
    function resetSearchForm() {
        searchOption.current.userName = null;
        searchOption.current.realName = null;
        searchOption.current.phoneNumber = null;
        searchOption.current.role = null;
        searchForm.resetFields();
    }

    // 创建用户
    function createUser() {
        setUserEditVisible(true);
    }

    // 编辑用户
    async function editUser(id: number) {
        let userResponse = await UserService.getUser({ id: id });
        userEditForm.setFieldsValue({
            id: userResponse.data.data.id,
            userName: userResponse.data.data.userName,
            realName: userResponse.data.data.realName,
            gender: userResponse.data.data.gender,
            phoneNumber: userResponse.data.data.phoneNumber,
            roles: userResponse.data.data.roles
        });
        await getUsers();
        setUserEditVisible(true);
    }

    // 用户密码设定
    function setPwd(id: number) {
        pwdEditForm.setFieldsValue({
            id: id
        });
        setPwdEditVisible(true);
    }

    // 移出组织
    function moveOutOrg(id: number) {
        Modal.confirm({
            title: '是否将该用户移出组织',
            onOk: async () => {
                await UserService.removeOrgMember({ orgId: selectedOrg!, userId: id });
                await getUsers();
            }
        });
    }

    // 删除用户确认
    function deleteUser(id: number) {
        Modal.confirm({
            title: "确认删除用户",
            content: "是否删除该系统用户？",
            onOk: async () => {
                await UserService.removeUser({ id: id });
                await getUsers();
            }
        });
    }

    // 提交用户编辑信息
    async function userInfoSubmit(values: any) {
        await UserService.addOrUpdateUser({
            id: values["id"],
            userName: values["userName"],
            realName: values["realName"],
            gender: values["gender"],
            phoneNumber: values["phoneNumber"],
            roles: values["roles"]
        });
        setUserEditVisible(false);
        await getUsers();
    }

    // 提交用户密码信息
    async function pwdSubmit(values: any) {
        await UserService.setUserPassword({
            id: values['id'],
            password: values['pwd'],
            confirmPassword: values['confirmPwd']
        });
        setPwdEditVisible(false);
    }

    async function pageChange(page: number, size?: number) {
        setPage(page);
        setSize(size!);
    }

    useEffect(() => {
        getUsers();
    }, [page, size]); // 

    return (
        <>
            <div id="user-container">
                <div id="user-group-container">
                    <RightElement identify="add-member" child={
                        <>
                            <div>
                                <Button icon={<PlusOutlined />} onClick={setOrgMember} disabled={selectedOrg === null}>添加成员</Button>
                            </div>
                            <Divider style={{ margin: "10px 0" }} />
                        </>
                    }></RightElement>
                    <Tree showLine={true} showIcon={true} treeData={treeData} onSelect={(keys: any, event: any) => orgSelect(keys, event)} />
                </div>
                <div id="user-list-container">
                    <Form form={searchForm} layout="inline" onFinish={searchSubmit}>
                        <Form.Item name="userName" label="账号" labelCol={{ style: { width: '60px' } }}>
                            <Input className="searchInput" autoComplete="off" placeholder="请输入账号" />
                        </Form.Item>
                        <Form.Item name="realname" label="姓名" labelCol={{ style: { width: '60px' } }}>
                            <Input className="searchInput" autoComplete="off2" placeholder="请输入姓名" />
                        </Form.Item>
                        <Form.Item name="phoneNumber" label="电话" labelCol={{ style: { width: '60px' } }}>
                            <Input className="searchInput" autoComplete="off2" placeholder="请输入电话" />
                        </Form.Item>
                        <Form.Item name="role" label="角色" labelCol={{ style: { width: '60px' } }}>
                            <Select allowClear={true} className="searchInput" placeholder="请选择角色">
                                {
                                    roleOptions.map(o => (
                                        <Select.Option value={o.key} key={o.key}>{o.value}</Select.Option>
                                    ))
                                }
                            </Select>
                        </Form.Item>
                    </Form>
                    <Space style={{ marginTop: "10px" }}>
                        <Button icon={<SearchOutlined />} onClick={searchUser}>查找</Button>
                        <Button icon={<ClearOutlined />} onClick={resetSearchForm}>重置</Button>
                        <RightElement identify="create-user" child={
                            <>
                                <Button icon={<PlusOutlined />} onClick={createUser}>创建</Button>
                            </>
                        }></RightElement>
                    </Space>
                    <Divider style={{ margin: "10px 0" }} />
                    <Table size="small" columns={userTableColumns} dataSource={userTableData} scroll={{ x: 1700 }} pagination={false}></Table>
                    <Pagination current={page} total={total} pageSize={size} showSizeChanger={true} style={{ marginTop: '10px' }}
                        onChange={pageChange}></Pagination>
                </div>
            </div>

            <Modal visible={orgAddVisible} onCancel={() => setOrgAddVisible(false)} title="添加新的组织成员" footer={null}
                destroyOnClose={true}>
                <Form form={orgSettingForm} onFinish={submitOrgMember} preserve={false}>
                    <Form.Item label="成员" name="members" labelCol={{ span: 6 }} wrapperCol={{ span: 14 }} rules={
                        [
                            { required: true, message: "请选择成员" },
                        ]
                    }>
                        <DebounceSelect mode="multiple" fetchOptions={loadUser} placeholder="请选择成员" />
                    </Form.Item>
                    <Form.Item label="职位" name="positions" labelCol={{ span: 6 }} wrapperCol={{ span: 14 }}>
                        <Select mode="multiple" placeholder="请选择职位" >
                            {
                                positionOptions.map(o => (
                                    <Select.Option value={o.key} key={o.key}>{o.value}</Select.Option>
                                ))
                            }
                        </Select>
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6, span: 14 }}>
                        <Button type="primary" htmlType="submit">确定</Button>
                        <Button style={{ marginLeft: '10px' }} onClick={() => setOrgAddVisible(false)}>取消</Button>
                    </Form.Item>
                </Form>
            </Modal>

            <Modal visible={userEditVisible} title="用户信息" footer={null} onCancel={() => setUserEditVisible(false)}
                destroyOnClose={true}>
                <Form form={userEditForm} onFinish={userInfoSubmit} labelCol={{ span: 6 }}
                    wrapperCol={{ span: 16 }} preserve={false}>
                    <Form.Item name="id" hidden >
                        <Input />
                    </Form.Item>
                    <Form.Item name="userName" label="用户名" rules={
                        [
                            { required: true, message: "请输入用户名" },
                            { max: 20, message: "用户名过长" },
                            { pattern: /^[A-Za-z0-9]+$/g, message: '用户名只允许数字字母' },
                        ]
                    }>
                        <Input autoComplete="off" placeholder="请输入用户名" />
                    </Form.Item>
                    <Form.Item name="realName" label="姓名" rules={
                        [
                            { required: true, message: "请输入姓名" },
                            { max: 20, message: "姓名过长" },
                        ]
                    }>
                        <Input autoComplete="off2" placeholder="请输入姓名" />
                    </Form.Item>
                    <Form.Item name="gender" label="性别">
                        <Radio.Group defaultValue={0}>
                            <Radio value={0}>未知</Radio>
                            <Radio value={1}>男</Radio>
                            <Radio value={2}>女</Radio>
                        </Radio.Group>
                    </Form.Item>
                    <Form.Item name="phoneNumber" label="电话">
                        <Input autoComplete="off2" placeholder="请输入电话" />
                    </Form.Item>
                    <Form.Item name="roles" label="角色">
                        <Select allowClear={true} placeholder="请选择角色" mode="multiple" >
                            {
                                roleOptions.map(o => (
                                    <Select.Option value={o.key} key={o.key}>{o.value}</Select.Option>
                                ))
                            }
                        </Select>
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6 }}>
                        <Button icon={<SaveOutlined />} htmlType="submit">保存</Button>
                    </Form.Item>
                </Form>
            </Modal>

            <Modal visible={pwdEditVisible} title="密码设置" footer={null} onCancel={() => setPwdEditVisible(false)}
                destroyOnClose={true}>
                <Form form={pwdEditForm} onFinish={pwdSubmit} labelCol={{ span: 6 }}
                    wrapperCol={{ span: 16 }} preserve={false}>
                    <Form.Item name="id" hidden >
                        <Input />
                    </Form.Item>
                    <Form.Item name="pwd" label="密码" rules={
                        [
                            { required: true, message: "请输入密码" },
                            { max: 50, message: "密码过长" },
                        ]
                    }>
                        <Input autoComplete="off2" placeholder="请输入密码" type="password" />
                    </Form.Item>
                    <Form.Item name="confirmPwd" label="确认密码" rules={
                        [
                            { required: true, message: "请输入确认密码" },
                        ]
                    }>
                        <Input autoComplete="off2" placeholder="请输入确认密码" type="password" />
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6 }}>
                        <Button icon={<SaveOutlined />} htmlType="submit">保存</Button>
                    </Form.Item>
                </Form>
            </Modal>
        </>
    );
}