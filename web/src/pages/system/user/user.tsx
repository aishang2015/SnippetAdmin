import { Avatar, Button, Divider, Form, Input, Modal, Pagination, Radio, Select, Space, Switch, Table, Tag, Tree, TreeSelect } from 'antd';
import {
    HomeOutlined, PlusOutlined, ManOutlined, WomanOutlined, UserOutlined,
    SearchOutlined, ClearOutlined, EditOutlined, DeleteOutlined, KeyOutlined
} from "@ant-design/icons";

import './user.less';
import { DebounceSelect } from '../../../components/common/debounceSelect';
import { useEffect, useState } from 'react';
import { useForm } from 'antd/lib/form/Form';

export default function User() {

    const [page, setPage] = useState(0);
    const [total, setTotal] = useState(0);
    const [size, setSize] = useState(0);

    const [groupSettingModal, setGroupSettingModal] = useState(false);
    const [groupSettingForm] = useForm();
    const [searchForm] = useForm();

    const [userEditModal, setUserEditModal] = useState(false);
    const [userEditForm] = useForm();

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
        { title: '序号', dataIndex: "num", align: 'center', width: '100px' },
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
                array.map((s: any) => (<Tag key={s} style={{ marginBottom: '5px' }} color="#2db7f5">{s}</Tag>))
            ),
        },
        {
            title: '启用', dataIndex: "isActive", align: 'center', width: '120px',
            render: (data: any, record: any) => (
                <Switch checked={data}></Switch>
            ),
        },
        {
            title: '操作', key: 'operate', align: 'center',
            render: (text: any, record: any) => (
                <Space size="middle">
                    <a><EditOutlined /></a>
                    <a><DeleteOutlined /></a>
                    <a><KeyOutlined /></a>
                </Space>
            ),
        },
    ];

    useEffect(() => {
        setUserTableData([
            { num: 1, avatar: null, userName: "toknod", name: "王蛋八蛋", gender: 1, phone: 15900318989, role: ["管理"], isActive: true },
            { num: 2, avatar: "https://zos.alipayobjects.com/rmsportal/ODTLcjxAfvqbxHnVXCYX.png", userName: "toknod", name: "李报那个", gender: 0, phone: 15900318989, role: ["超级管理员"], isActive: false }
        ]);

        setPage(1);
        setTotal(2);
        setSize(10)
    }, []);

    function loadUser(username: string): Promise<Array<any>> {
        return new Promise<Array<any>>((resolve, reject) => [
            { label: '张三', value: 'zhangs' },
            { label: '李四', value: 'lis' },
            { label: '王五', value: 'wangw' }
        ]);
    }

    function setGroupMember() {
        setGroupSettingModal(true);
    }

    // 组织成员设定
    function groupSettingSubmit(values: any) {

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

    // 提交用户编辑信息
    function userInfoSubmit(values: any) {

    }

    return (
        <>
            <div id="user-container">
                <div id="user-group-container">
                    <div>
                        <Button icon={<PlusOutlined />} onClick={setGroupMember}>组织成员设定</Button>
                    </div>
                    <Divider style={{ margin: "10px 0" }} />
                    <Tree showLine={true} showIcon={true} treeData={treeData} />
                </div>
                <div id="user-list-container">
                    <Form form={searchForm} layout="inline" onFinish={searchSubmit}>
                        <Form.Item name="userName">
                            <Input className="searchInput" autoComplete="off2" placeholder="请输入用户名" />
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
                        <Button icon={<SearchOutlined />} onClick={searchUser}>查找用户</Button>
                        <Button icon={<ClearOutlined />} onClick={resetSearchForm}>重置条件</Button>
                        <Button icon={<PlusOutlined />} onClick={createUser}>创建用户</Button>
                    </Space>
                    <Divider style={{ margin: "10px 0" }} />
                    <Table columns={userTableColumns} dataSource={userTableData} scroll={{ x: 1300 }} pagination={false}></Table>
                    <Pagination current={page} total={total} pageSize={size} showSizeChanger={true} style={{ marginTop: '10px' }}></Pagination>
                </div>
            </div>

            <Modal visible={groupSettingModal} onCancel={() => setGroupSettingModal(false)} title="组织成员设定" footer={null}>
                <Form form={groupSettingForm} onFinish={groupSettingSubmit}>
                    <Form.Item label="组织" name="group" labelCol={{ span: 6 }} wrapperCol={{ span: 14 }}>
                        <TreeSelect placeholder="请选择组织"></TreeSelect>
                    </Form.Item>
                    <Form.Item label="负责人" name="leader" labelCol={{ span: 6 }} wrapperCol={{ span: 14 }}>
                        <DebounceSelect mode="multiple" fetchOptions={loadUser} placeholder="请选择负责人" />
                    </Form.Item>
                    <Form.Item label="成员" name="member" labelCol={{ span: 6 }} wrapperCol={{ span: 14 }}>
                        <DebounceSelect mode="multiple" fetchOptions={loadUser} placeholder="请选择负责人" />
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6, span: 14 }}>
                        <Button type="primary">确定</Button>
                        <Button style={{ marginLeft: '10px' }} onClick={() => setGroupSettingModal(false)}>取消</Button>
                    </Form.Item>
                </Form>
            </Modal>

            <Modal visible={userEditModal} title="用户信息编辑" footer={null} onCancel={() => setUserEditModal(false)}>
                <Form form={userEditForm} onFinish={userInfoSubmit} labelCol={{ span: 6 }}
                    wrapperCol={{ span: 16 }}>
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
                        <Button>确定</Button>
                    </Form.Item>
                </Form>
            </Modal>
        </>
    );
}