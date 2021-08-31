
import './page.less';

import { Button, Descriptions, Divider, Form, Input, Modal, Select, Tag, Tree } from 'antd';
import { DeleteOutlined, EditOutlined, ExportOutlined, LinkOutlined, MenuOutlined, PlusOutlined, SaveOutlined } from '@ant-design/icons';
import { useState } from 'react';
import { useForm } from 'antd/lib/form/Form';

export default function Page() {

    const [elementEditVisible, setElementEditVisible] = useState(false);
    const [elementForm] = useForm();

    const treeData = [
        {
            title: '主页', key: '1-0-0', icon: <MenuOutlined />, switcherIcon: (<></>)
        },
        {
            title: '系统管理', key: '2-0-0', icon: <MenuOutlined />, children: [
                {
                    title: '系统用户', key: '2-1-0', icon: <MenuOutlined />, children: [
                        { title: '成员设定', key: '2-1-1', icon: <LinkOutlined />, switcherIcon: (<></>) },
                        { title: '创建用户', key: '2-1-2', icon: <LinkOutlined />, switcherIcon: (<></>) },
                        { title: '编辑用户', key: '2-1-3', icon: <LinkOutlined />, switcherIcon: (<></>) },
                        { title: '删除用户', key: '2-1-4', icon: <LinkOutlined />, switcherIcon: (<></>) },
                        { title: '设置密码', key: '2-1-5', icon: <LinkOutlined />, switcherIcon: (<></>) },
                        { title: '移出组织', key: '2-1-6', icon: <LinkOutlined />, switcherIcon: (<></>) },

                    ]
                },
                { title: '角色管理', key: '2-2-0', icon: <MenuOutlined />, switcherIcon: (<></>) },
                { title: '组织管理', key: '2-3-0', icon: <MenuOutlined />, switcherIcon: (<></>) },
                { title: '页面权限', key: '2-4-0', icon: <MenuOutlined />, switcherIcon: (<></>) },
                { title: '登录管理', key: '2-5-0', icon: <MenuOutlined />, switcherIcon: (<></>) },
            ]
        },
        {
            title: '关于', key: '3-0-0', icon: <MenuOutlined />, switcherIcon: (<></>)
        },
    ];

    // 添加元素
    function addElement() {
        setElementEditVisible(true);
    }

    // 编辑元素
    function editElement(id: number) {
        setElementEditVisible(true);
    }

    // 删除元素
    function deleteElement(id: number) {
        Modal.confirm({
            title: '是否删除该元素?'
        });
    }

    return (
        <>
            <div id="page-container">
                <div id="page-tree-container">
                    <div>
                        <Button icon={<PlusOutlined />} onClick={addElement}>添加页面元素</Button>
                    </div>
                    <Divider style={{ margin: "10px 0" }} />
                    <Tree showLine={true} showIcon={true} treeData={treeData} />
                </div>
                <div id="page-detail-container">
                    <div>
                        <Button icon={<EditOutlined />} style={{ marginRight: '10px' }} onClick={() => editElement(1)}>编辑</Button>
                        <Button icon={<DeleteOutlined />} style={{ marginRight: '10px' }} onClick={() => deleteElement(1)}>删除</Button>
                        <Button icon={<ExportOutlined />} style={{ marginRight: '10px' }}>导出</Button>
                    </div>
                    <Divider style={{ margin: "10px 0" }} />
                    <Descriptions title="页面元素信息" bordered>
                        <Descriptions.Item label="元素名称" span={3}>创建用户</Descriptions.Item>
                        <Descriptions.Item label="元素类型" span={3}>菜单</Descriptions.Item>
                        <Descriptions.Item label="元素标识" span={3}>user-create</Descriptions.Item>
                        <Descriptions.Item label="接口信息" span={3}><Tag color="#87d068">POST</Tag><Tag color="#108ee9">/api/user/createUser</Tag></Descriptions.Item>
                    </Descriptions>
                </div>
            </div>

            <Modal visible={elementEditVisible} destroyOnClose={true} onCancel={() => setElementEditVisible(false)} footer={null}
                title="组织信息编辑" width={600}>
                <Form preserve={false} form={elementForm}>
                    <Form.Item name="elementName" label="元素名称" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                        <Input placeholder="请输入元素名称"></Input>
                    </Form.Item>
                    <Form.Item name="elementType" label="元素类型" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                        <Select placeholder="请选择元素类型" allowClear={true}>
                            <Select.Option value="menu">菜单</Select.Option>
                            <Select.Option value="link">按钮/链接</Select.Option>
                        </Select>
                    </Form.Item>
                    <Form.Item name="elementIdentity" label="元素标识" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                        <Input placeholder="请输入元素标识" allowClear={true}></Input>
                    </Form.Item>
                    <Form.Item name="elementInterfaces" label="使用接口" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                        <Select placeholder="请选择使用接口" allowClear={true} mode="multiple">
                            <Select.Option value="1" label="China">
                                <Tag color="#87d068">POST</Tag><Tag color="#108ee9">/api/user/createUser</Tag>
                            </Select.Option>
                            <Select.Option value="2" label="China">
                                <Tag color="#87d068">POST</Tag><Tag color="#108ee9">/api/user/createUser</Tag>
                            </Select.Option>
                        </Select>
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6, span: 16 }}>
                        <Button icon={<SaveOutlined />}>保存</Button>
                    </Form.Item>
                </Form>
            </Modal>
        </>
    );
}