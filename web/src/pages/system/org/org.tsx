import './org.less';
import 'emoji-mart/css/emoji-mart.css';
import { Picker } from 'emoji-mart';

import { Modal, Button, Descriptions, Divider, Form, Tag, Tree, Input, Space, Checkbox, TreeSelect, message } from 'antd';
import {
    PlusOutlined, EditOutlined, DeleteOutlined, ContactsOutlined, MinusCircleOutlined,
    SaveOutlined
} from "@ant-design/icons";
import { useEffect, useState } from 'react';
import { useForm } from 'antd/lib/form/Form';
import { getOrganizationResult, OrganizationService } from '../../../http/requests/organization';
import { uniq } from 'lodash';
import { RightElement } from '../../../components/right/rightElement';

export default function Org() {

    const [postModalVisible, setPostModalVisible] = useState(false);
    const [postForm] = useForm();

    const [orgEditVisible, setOrgEditVisible] = useState(false);
    const [orgForm] = useForm();

    const [orgIcon, setOrgIcon] = useState("🏟");
    const [emojiModalVisible, setEmojiModalVisible] = useState(false);

    const [treeData, setTreeData] = useState<Array<any>>([]);
    const [orgDetail, setOrgDetail] = useState<getOrganizationResult | null>(null);

    useEffect(() => {
        init();
    }, []);// eslint-disable-line react-hooks/exhaustive-deps

    async function init() {
        await getTreeData();
    }

    // 取得树数据
    async function getTreeData() {
        let response = await OrganizationService.getOrganizationTree();
        makeTreeData(response.data.data);
        setTreeData(response.data.data);
    }

    // 创建新组织
    function createOrg() {
        setOrgIcon("🏟");
        setOrgEditVisible(true);
    }

    function editOrg() {
        orgForm.setFieldsValue({
            id: orgDetail?.id,
            upOrg: orgDetail?.upId,
            orgName: orgDetail?.name,
            orgCode: orgDetail?.code,
            orgPhone: orgDetail?.phone,
            orgAddress: orgDetail?.address
        });
        setOrgIcon(orgDetail!.icon);

        setOrgEditVisible(true);
    }

    // 职位设置
    function setPost() {
        postForm.setFieldsValue({
            positions: orgDetail!.positions
        });

        setPostModalVisible(true);
    }

    // 删除组织
    function deleteOrg() {
        Modal.confirm({
            title: "是否删除该组织?",
            onOk: async () => {
                await OrganizationService.deleteOrganization(orgDetail!.id);
                setOrgDetail(null);
                await getTreeData();
            }
        });
    }

    // 组织提交
    async function orgSubmit(values: any) {

        if (values['id']) {

            // 更新
            await OrganizationService.updateOrganization({
                id: values["id"],
                upId: values["upOrg"],
                name: values["orgName"],
                code: values["orgCode"],
                icon: orgIcon,
                phone: values["orgPhone"],
                address: values["orgAddress"],
            });
            await getTreeData();
            let response = await OrganizationService.getOrganization(orgDetail!.id);
            setOrgDetail(response.data.data);

        } else {

            // 创建
            await OrganizationService.createOrganization({
                upId: values["upOrg"],
                name: values["orgName"],
                code: values["orgCode"],
                icon: orgIcon,
                phone: values["orgPhone"],
                address: values["orgAddress"],
            });
            await getTreeData();
        }
        setOrgEditVisible(false);
    }

    // 职位表单提交
    async function postFormSubmit(values: any) {

        var uniqArray = uniq(values["positions"].map((v: any) => v.name));
        if (uniqArray.length !== values["positions"].length) {
            message.error("职位名称冲突！");
            return;
        }

        await OrganizationService.setPosition({
            organizationId: orgDetail!.id,
            positions: values["positions"]
        });
        setPostModalVisible(false);
        let response = await OrganizationService.getOrganization(orgDetail!.id);
        setOrgDetail(response.data.data);
    }

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

    // 树元素选中
    async function elementSelect(selectedKeys: any, e: { selected: boolean }) {
        if (e.selected) {
            let response = await OrganizationService.getOrganization(selectedKeys[0]);
            setOrgDetail(response.data.data);
        } else {
            setOrgDetail(null);
        }
    }

    return (
        <>
            <div id="org-container">
                <div id='org-tree-container'>
                    <RightElement identify="create-org" child={
                        <>
                            <Button icon={<PlusOutlined />} onClick={createOrg}>创建组织</Button>
                            <Divider style={{ margin: "10px 0" }} />
                        </>
                    }></RightElement>
                    <Tree showLine={true} showIcon={true} treeData={treeData} onSelect={elementSelect} />
                </div>
                <div id="org-detail-container">
                    {orgDetail !== null &&
                        <>
                            <div>
                                <RightElement identify="edit-org" child={
                                    <>
                                        <Button onClick={editOrg} icon={<EditOutlined />} style={{ marginRight: '10px' }}>编辑组织</Button>
                                    </>
                                }></RightElement>
                                <RightElement identify="remove-org" child={
                                    <>
                                        <Button onClick={deleteOrg} icon={<DeleteOutlined />} style={{ marginRight: '10px' }}>删除组织</Button>
                                    </>
                                }></RightElement>
                                <RightElement identify="set-pos" child={
                                    <>
                                        <Button onClick={setPost} icon={<ContactsOutlined />} style={{ marginRight: '10px' }}>职位设置</Button>
                                    </>
                                }></RightElement>
                            </div>
                            <Divider style={{ margin: "10px 0" }} />
                            <Descriptions title="组织信息" bordered>
                                <Descriptions.Item label="组织名称" labelStyle={{ width: "200px" }} span={3}>{orgDetail.name}</Descriptions.Item>
                                <Descriptions.Item label="组织代码" span={3}>{orgDetail.code}</Descriptions.Item>
                                <Descriptions.Item label="电话" span={3}>{orgDetail.phone}</Descriptions.Item>
                                <Descriptions.Item label="地址" span={3}>{orgDetail.address}</Descriptions.Item>
                                <Descriptions.Item label="可用上级职位" span={3}>{orgDetail.upPositions.map(p => <Tag color="#87d068" key={p}>{p}</Tag>)}</Descriptions.Item>
                                <Descriptions.Item label="当前组织职位" span={3}>{orgDetail.positions.map(p => <Tag color={p.visibleToChild ? "#87d068" : "grey"} key={p.name}>{p.name}({p.code})</Tag>)}</Descriptions.Item>
                            </Descriptions>
                        </>
                    }
                </div>
            </div>

            <Modal destroyOnClose={true} visible={postModalVisible} onCancel={() => setPostModalVisible(false)}
                footer={null} title="职位设置" width={900}>
                <Form form={postForm} preserve={false} onFinish={postFormSubmit}>
                    <Form.List name="positions">
                        {(fields, { add, remove }, { errors }) => (
                            <>
                                {fields.map((field, index) => (
                                    <Form.Item label={index === 0 ? '职位名称' : ''} key={field.key}
                                        labelCol={{ span: index === 0 ? 6 : 0 }}
                                        wrapperCol={{ offset: index === 0 ? 0 : 6 }} >
                                        <Space>
                                            <Form.Item {...field} validateTrigger={['onChange', 'onBlur']}
                                                name={[field.name, 'name']}
                                                fieldKey={[field.fieldKey, 'name']} noStyle>
                                                <Input placeholder="职位名称" autoComplete="off" />
                                            </Form.Item>
                                            <Form.Item {...field} validateTrigger={['onChange', 'onBlur']}
                                                name={[field.name, 'code']}
                                                fieldKey={[field.fieldKey, 'code']} noStyle>
                                                <Input placeholder="职位编码" autoComplete="off" />
                                            </Form.Item>
                                            <Form.Item {...field} validateTrigger={['onChange', 'onBlur']}
                                                name={[field.name, 'visibleToChild']}
                                                fieldKey={[field.fieldKey, 'visibleToChild']}
                                                initialValue={false}
                                                valuePropName="checked" noStyle>
                                                <Checkbox>下级可用</Checkbox>
                                            </Form.Item>
                                            {fields.length > 0 ? (
                                                <MinusCircleOutlined style={{ marginLeft: "10px", fontSize: "24px", color: "#999" }} onClick={() => remove(field.name)} />
                                            ) : null}
                                        </Space>
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
                    <Form.Item wrapperCol={{ offset: 6 }}>
                        <Button icon={<SaveOutlined />} htmlType="submit">保存</Button>
                    </Form.Item>
                </Form>
            </Modal>

            <Modal visible={orgEditVisible} destroyOnClose={true} onCancel={() => setOrgEditVisible(false)} footer={null}
                title="组织信息编辑" width={800}>
                <Form preserve={false} form={orgForm} onFinish={orgSubmit}>
                    <Form.Item name="id" hidden>
                        <Input />
                    </Form.Item>
                    <Form.Item name="upOrg" label="上级组织" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                        <TreeSelect treeData={treeData} placeholder="请选择上级组织" allowClear={true} treeLine={true} treeIcon={true}></TreeSelect>
                    </Form.Item>
                    <Form.Item name="orgName" label="组织名称" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} required rules={
                        [
                            { required: true, message: "请输入组织名称" },
                            { max: 50, message: "组织名称过长" },
                        ]
                    }>
                        <Input placeholder="请输入组织名称" allowClear={true} autoComplete="off"></Input>
                    </Form.Item>
                    <Form.Item name="orgCode" label="组织编码" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} required rules={
                        [
                            { required: true, message: "请输入组织编码" },
                            { max: 50, message: "组织编码过长" },
                            { pattern: /^[A-Za-z0-9-_]+$/g, message: "请输入数字字母" },
                        ]
                    }>
                        <Input placeholder="请输入组织编码" allowClear={true} autoComplete="off"></Input>
                    </Form.Item>
                    <Form.Item name="orgIcon" label="组织图标" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                        <a onClick={() => setEmojiModalVisible(true)} style={{ fontSize: "20px" }}>{orgIcon}</a>
                    </Form.Item>
                    <Form.Item name="orgPhone" label="联系电话" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                        <Input placeholder="请输入联系电话" allowClear={true} autoComplete="off2"></Input>
                    </Form.Item>
                    <Form.Item name="orgAddress" label="地址" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                        <Input placeholder="请输入地址" allowClear={true} autoComplete="off2"></Input>
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6, span: 16 }}>
                        <Button icon={<SaveOutlined />} htmlType="submit">保存</Button>
                    </Form.Item>
                </Form>
            </Modal>

            <Modal width={390} visible={emojiModalVisible} footer={null} title="选择图标" destroyOnClose={true}
                onCancel={() => setEmojiModalVisible(false)} >
                <Picker native={true} onSelect={(e: any) => {
                    setOrgIcon(e.native);
                    setEmojiModalVisible(false);
                }} />
            </Modal>

        </>
    );
}