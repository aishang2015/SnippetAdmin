import { Avatar, Button, Divider, Form, Input, Modal, Pagination, Radio, Select, Space, Switch, Table, Tag, Tooltip, Tree, TreeSelect } from 'antd';
import {
    HomeOutlined, PlusOutlined, ManOutlined, WomanOutlined, UserOutlined,
    SearchOutlined, ClearOutlined, EditOutlined, DeleteOutlined, KeyOutlined,
    SaveOutlined, MinusCircleOutlined
} from "@ant-design/icons";

import './user.less';
import { useEffect, useState } from 'react';
import { useForm } from 'antd/lib/form/Form';
import { DebounceSelect } from '../../../components/common/debounceSelect';

export default function User() {

    const [page, setPage] = useState(0);
    const [total, setTotal] = useState(0);
    const [size, setSize] = useState(0);

    const [orgSettingModal, setOrgSettingModal] = useState(false);
    const [orgSettingForm] = useForm();
    const [searchForm] = useForm();

    const [userEditModal, setUserEditModal] = useState(false);
    const [userEditForm] = useForm();

    const [pwdEditVisible, setPwdEditVisible] = useState(false);
    const [pwdEditForm] = useForm();

    const [userTableData, setUserTableData] = useState(new Array<any>());

    const treeData = [
        {
            title: '集团', key: '0-0-0', icon: <HomeOutlined />, children: [
                { title: '北京公司', key: '0-1-0', icon: (<>🏙</>), switcherIcon: (<></>) },
                { title: '上海公司', key: '0-2-0', icon: (<>🏙</>), switcherIcon: (<></>) },
                {
                    title: '天津公司', key: '0-3-0', icon: (<>🏙</>), children: [
                        { title: '行政部', key: '0-3-1', icon: (<>👬</>), switcherIcon: (<></>) },
                        { title: '开发一部', key: '0-3-2', icon: (<>👬</>), switcherIcon: (<></>) },
                        { title: '开发二部', key: '0-3-3', icon: (<>👬</>), switcherIcon: (<></>) }
                    ]
                },
            ]
        }
    ];

    const userTableColumns: any = [
        { title: '序号', dataIndex: "num", align: 'center', width: '100px', fixed: "left" },
        {
            title: '头像', dataIndex: "avatar", align: 'center', width: '120px',
            render: (data: any, record: any) => (
                data === null ?
                    <Avatar icon={<UserOutlined />} /> :
                    <Avatar src={data} />
            ),

        },
        { title: '账号', dataIndex: "userName", align: 'center', width: '160px' },
        { title: '姓名', dataIndex: "name", align: 'center', width: '120px' },
        {
            title: '性别', dataIndex: "gender", align: 'center', width: '80px',
            render: (text: any, record: any) => (
                text === 0 ?
                    <ManOutlined style={{ color: "blue" }} /> :
                    <WomanOutlined style={{ color: "red" }} />
            ),
        },
        { title: '电话', dataIndex: "phone", align: 'center', width: '130px' },
        {
            title: '角色', dataIndex: "role", align: 'center', width: '220px',
            render: (array: any, record: any) => (
                array?.map((s: any) => (<Tag key={s} style={{ marginBottom: '5px' }} color="#2db7f5">{s}</Tag>))
            ),
        },
        {
            title: '部门/职位', dataIndex: "orgPost", align: 'center', width: '440px',
            render: (array: any, record: any) => (
                array?.map((s: any) => (
                    <Tag key={s.org + s.post} style={{ marginBottom: '5px' }} color="#f50">{s.org + "/" + s.post}</Tag>)
                )
            ),
        },
        {
            title: '启用', dataIndex: "isActive", align: 'center', width: '120px', fixed: "right",
            render: (data: any, record: any) => (
                <Switch checked={data}></Switch>
            ),
        },
        {
            title: '操作', key: 'operate', align: 'center', fixed: "right",
            render: (text: any, record: any) => (
                <Space size="middle">
                    <Tooltip title="编辑"><a onClick={() => editUser(record.id)}><EditOutlined /></a></Tooltip>
                    <Tooltip title="删除"><a onClick={() => deleteUser(record.id)}><DeleteOutlined /></a></Tooltip>
                    <Tooltip title="设定密码"><a onClick={() => setPwd(record.id)}><KeyOutlined /></a></Tooltip>
                    <Tooltip title="移出组织"><a onClick={() => moveOutOrg(record.id)}><MinusCircleOutlined /></a></Tooltip>
                </Space>
            ),
        },
    ];

    useEffect(() => {
        setUserTableData([
            {
                num: 1, avatar: null, userName: "toknod", name: "王蛋八蛋", gender: 1, phone: 15900318989, role: ["管理"],
                orgPost: [{ org: "董事办", post: "秘书" }], isActive: true
            },
            {
                num: 2, avatar: "https://zos.alipayobjects.com/rmsportal/ODTLcjxAfvqbxHnVXCYX.png", userName: "toknod", name: "李报那个", gender: 0, phone: 15900318989, role: ["超级管理员"],
                orgPost: [{ org: "行政办", post: "行政主管" }, { org: "人力资源办", post: "人力资源专员" }], isActive: false
            }
        ]);

        setPage(1);
        setTotal(2);
        setSize(10)
    }, []);

    async function loadUser(username: string): Promise<Array<any>> {
        return new Promise<Array<any>>((resolve, reject) => {
            resolve([
                { label: '张三', value: 'zhangs' },
                { label: '李四', value: 'lis' },
                { label: '王五', value: 'wangw' }
            ]);
        });
    }

    function setOrgMember() {
        setOrgSettingModal(true);
    }

    // 搜索提交
    function searchSubmit(values: any) {
    }

    // 点击搜索按钮
    function searchUser() {
        searchForm.submit();
    }

    // 点击重置搜索按钮
    function resetSearchForm() {
        searchForm.resetFields();
    }

    // 创建用户
    function createUser() {
        setUserEditModal(true);
    }

    // 编辑用户
    function editUser(id: number) {
        setUserEditModal(true);
    }

    // 用户密码设定
    function setPwd(id: number) {
        setPwdEditVisible(true);
    }

    // 移出组织
    function moveOutOrg(id: number) {
        Modal.confirm({
            title: '是否将该用户移出组织'
        });
    }

    // 删除用户确认
    function deleteUser(id: number) {
        Modal.confirm({
            title: "确认删除用户",
            content: "是否删除该系统用户？"
        });
    }

    // 提交用户编辑信息
    function userInfoSubmit(values: any) {

    }

    // 提交用户密码信息
    function pwdSubmit(values: any) {

    }

    return (
        <>
            <div id="user-container">
                <div id="user-group-container">
                    <div>
                        <Button icon={<PlusOutlined />} onClick={setOrgMember}>成员设定</Button>
                    </div>
                    <Divider style={{ margin: "10px 0" }} />
                    <Tree showLine={true} showIcon={true} treeData={treeData} />
                </div>
                <div id="user-list-container">
                    <Form form={searchForm} layout="inline" onFinish={searchSubmit}>
                        <Form.Item name="userName">
                            <Input className="searchInput" autoComplete="off2" placeholder="请输入账号" />
                        </Form.Item>
                        <Form.Item name="name">
                            <Input className="searchInput" autoComplete="off2" placeholder="请输入姓名" />
                        </Form.Item>
                        <Form.Item name="phone">
                            <Input className="searchInput" placeholder="请输入电话" />
                        </Form.Item>
                        <Form.Item name="role">
                            <Select allowClear={true} className="searchInput" placeholder="请选择角色">
                                <Select.Option value="1">管理员</Select.Option>
                                <Select.Option value="2">super管理员</Select.Option>
                            </Select>
                        </Form.Item>
                    </Form>
                    <Space style={{ marginTop: "10px" }}>
                        <Button icon={<SearchOutlined />} onClick={searchUser}>查找</Button>
                        <Button icon={<ClearOutlined />} onClick={resetSearchForm}>重置</Button>
                        <Button icon={<PlusOutlined />} onClick={createUser}>创建</Button>
                    </Space>
                    <Divider style={{ margin: "10px 0" }} />
                    <Table columns={userTableColumns} dataSource={userTableData} scroll={{ x: 1700 }} pagination={false}></Table>
                    <Pagination current={page} total={total} pageSize={size} showSizeChanger={true} style={{ marginTop: '10px' }}></Pagination>
                </div>
            </div>

            <Modal visible={orgSettingModal} onCancel={() => setOrgSettingModal(false)} title="组织成员编辑" footer={null}
                destroyOnClose={true}>
                <Form form={orgSettingForm} >
                    <Form.Item label="成员" name="member" labelCol={{ span: 6 }} wrapperCol={{ span: 14 }}>
                        <DebounceSelect mode="multiple" fetchOptions={loadUser} placeholder="请选择成员" />
                    </Form.Item>
                    <Form.Item label="职位" labelCol={{ span: 6 }} wrapperCol={{ span: 14 }}>
                        <Select mode="multiple" placeholder="请选择职位" ></Select>
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6, span: 14 }}>
                        <Button type="primary">确定</Button>
                        <Button style={{ marginLeft: '10px' }} onClick={() => setOrgSettingModal(false)}>取消</Button>
                    </Form.Item>
                </Form>
            </Modal>

            <Modal visible={userEditModal} title="用户信息" footer={null} onCancel={() => setUserEditModal(false)}
                destroyOnClose={true}>
                <Form form={userEditForm} onFinish={userInfoSubmit} labelCol={{ span: 6 }}
                    wrapperCol={{ span: 16 }} preserve={false}>
                    <Form.Item name="id" hidden >
                        <Input />
                    </Form.Item>
                    <Form.Item name="userName" label="用户名">
                        <Input autoComplete="off2" placeholder="请输入用户名" />
                    </Form.Item>
                    <Form.Item name="name" label="姓名">
                        <Input autoComplete="off2" placeholder="请输入姓名" />
                    </Form.Item>
                    <Form.Item name="gender" label="性别">
                        <Radio.Group>
                            <Radio value={0}>男</Radio>
                            <Radio value={1}>女</Radio>
                        </Radio.Group>
                    </Form.Item>
                    <Form.Item name="phone" label="电话">
                        <Input placeholder="请输入电话" />
                    </Form.Item>
                    <Form.Item name="role" label="角色">
                        <Select allowClear={true} placeholder="请选择角色">
                            <Select.Option value="1">管理员</Select.Option>
                            <Select.Option value="2">super管理员</Select.Option>
                        </Select>
                    </Form.Item>
                    <Form.Item name="phone" wrapperCol={{ offset: 6 }}>
                        <Button icon={<SaveOutlined />}>保存</Button>
                    </Form.Item>
                </Form>
            </Modal>

            <Modal visible={pwdEditVisible} title="密码设置" footer={null} onCancel={() => setPwdEditVisible(false)}>
                <Form form={pwdEditForm} onFinish={pwdSubmit} labelCol={{ span: 6 }}
                    wrapperCol={{ span: 16 }}>
                    <Form.Item name="id" hidden >
                        <Input />
                    </Form.Item>
                    <Form.Item name="pwd" label="密码">
                        <Input autoComplete="off2" placeholder="请输入密码" type="password" />
                    </Form.Item>
                    <Form.Item name="confirmPwd" label="确认密码">
                        <Input autoComplete="off2" placeholder="请输入确认密码" type="password" />
                    </Form.Item>
                    <Form.Item name="phone" wrapperCol={{ offset: 6 }}>
                        <Button icon={<SaveOutlined />}>保存</Button>
                    </Form.Item>
                </Form>
            </Modal>
        </>
    );
}