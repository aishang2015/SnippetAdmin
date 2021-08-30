import './org.less';

import { Modal, Button, Descriptions, Divider, Form, Tag, Tree, Input, Switch, Select } from 'antd';
import {
    HomeOutlined, PlusOutlined, EditOutlined, DeleteOutlined, ContactsOutlined, MinusCircleOutlined,
    SaveOutlined
} from "@ant-design/icons";
import { useState } from 'react';
import { useForm } from 'antd/lib/form/Form';

export default function Org() {

    const [postModalVisible, setPostModalVisible] = useState(false);
    const [postForm] = useForm();

    const [orgEditVisible, setOrgEditVisible] = useState(false);
    const [orgForm] = useForm();

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

    // 创建新组织
    function createOrg() {
        setOrgEditVisible(true);
    }

    function editOrg() {
        setOrgEditVisible(true);
    }

    // 职位设置
    function setPost() {
        setPostModalVisible(true);
    }

    // 删除组织
    function deleteOrg() {
        Modal.confirm({
            title: "是否删除该组织"
        });
    }

    // 表单提交
    function postFormSubmit() {
    }

    return (
        <>
            <div id="org-container">
                <div id='org-tree-container'>
                    <Button icon={<PlusOutlined />} onClick={createOrg}>创建组织</Button>
                    <Divider style={{ margin: "10px 0" }} />
                    <Tree showLine={true} showIcon={true} treeData={treeData} />
                </div>
                <div id="org-detail-container">
                    <div>
                        <Button onClick={editOrg} icon={<EditOutlined />} style={{ marginRight: '10px' }}>编辑</Button>
                        <Button onClick={deleteOrg} icon={<DeleteOutlined />} style={{ marginRight: '10px' }}>删除</Button>
                        <Button onClick={setPost} icon={<ContactsOutlined />} style={{ marginRight: '10px' }}>职位设置</Button>
                    </div>
                    <Divider style={{ margin: "10px 0" }} />
                    <Descriptions title="组织信息" bordered>
                        <Descriptions.Item label="组织名称">行政部</Descriptions.Item>
                        <Descriptions.Item label="电话" span={2}>1810000000</Descriptions.Item>
                        <Descriptions.Item label="地址" span={3}>XX大厦1101</Descriptions.Item>
                        <Descriptions.Item label="上级部门">总公司</Descriptions.Item>
                        <Descriptions.Item label="下级部门" span={2}>总公司</Descriptions.Item>
                        <Descriptions.Item label="可用上级职位" span={3}><Tag>总经理</Tag><Tag>部门长</Tag></Descriptions.Item>
                        <Descriptions.Item label="当前组织职位" span={3}><Tag>行政主管</Tag><Tag>行政助理</Tag></Descriptions.Item>
                    </Descriptions>
                </div>
            </div>

            <Modal destroyOnClose={true} visible={postModalVisible} onCancel={() => setPostModalVisible(false)}
                footer={null} title="职位设置" width={800}>
                <Form form={postForm} preserve={false} onFinish={postFormSubmit}>
                    <Form.Item name="visibleToChild" label="下级组织是否可用" labelCol={{ span: 6 }}>
                        <Switch checkedChildren="可用" unCheckedChildren="不可" />
                    </Form.Item>
                    <Form.List name="posts">
                        {(fields, { add, remove }, { errors }) => (
                            <>
                                {fields.map((field, index) => (
                                    <Form.Item label={index === 0 ? '职位名称' : ''} required={true} key={field.key}
                                        labelCol={{ span: index === 0 ? 6 : 0 }}
                                        wrapperCol={{ offset: index === 0 ? 0 : 6 }} >
                                        <Form.Item {...field} validateTrigger={['onChange', 'onBlur']} noStyle                                        >
                                            <Input placeholder="职位名称" style={{ width: '60%' }} />
                                        </Form.Item>
                                        {fields.length > 0 ? (
                                            <MinusCircleOutlined style={{ marginLeft: "10px", fontSize: "24px", color: "#999" }} onClick={() => remove(field.name)} />
                                        ) : null}
                                    </Form.Item>
                                ))}
                                <Form.Item wrapperCol={{ offset: 6 }}>
                                    <Button type="dashed" onClick={() => add()} style={{ width: '60%' }} icon={<PlusOutlined />}                                >
                                        添加新职位
                                    </Button>
                                    <Form.ErrorList errors={errors} />
                                </Form.Item>
                            </>
                        )}
                    </Form.List>
                    <Form.Item name="phone" wrapperCol={{ offset: 6 }}>
                        <Button icon={<SaveOutlined />}>保存</Button>
                    </Form.Item>
                </Form>
            </Modal>

            <Modal visible={orgEditVisible} destroyOnClose={true} onCancel={() => setOrgEditVisible(false)} footer={null}
                title="组织信息编辑" width={800}>
                <Form preserve={false} form={orgForm}>
                    <Form.Item name="upOrg" label="上级组织" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                        <Select placeholder="请选择上级组织" allowClear={true}></Select>
                    </Form.Item>
                    <Form.Item name="orgName" label="组织名称" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                        <Input placeholder="请输入组织名称" allowClear={true}></Input>
                    </Form.Item>
                    <Form.Item name="orgPhone" label="联系电话" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                        <Input placeholder="请输入联系电话" allowClear={true}></Input>
                    </Form.Item>
                    <Form.Item name="orgAddress" label="地址" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                        <Input placeholder="请输入地址" allowClear={true}></Input>
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6, span: 16 }}>
                        <Button icon={<SaveOutlined />}>保存</Button>
                    </Form.Item>
                </Form>
            </Modal>

        </>
    );
}