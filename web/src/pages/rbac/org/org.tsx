
import data from '@emoji-mart/data';
//import 'emoji-mart/css/emoji-mart.css';
import './org.css';

import { faEdit, faObjectGroup, faPlus, faSave, faSitemap, faTrash } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Button, Descriptions, Divider, Form, Input, InputNumber, Modal, Select, Table, Tooltip, Tree, TreeSelect } from 'antd';
import { useForm } from 'antd/lib/form/Form';
import React, { useEffect, useState } from 'react';
import { RightElement } from '../../../components/right/rightElement';
import { getOrganizationResult, OrganizationService } from '../../../http/requests/rbac/organization';
import Title from 'antd/es/typography/Title';
import { useToken } from 'antd/es/theme/internal';
import DraggableModal from '../../../components/common/draggableModal';
import { faBuilding } from '@fortawesome/free-regular-svg-icons';

// @ts-ignore - alternatively, add `declare module "@emoji-mart/react"` to your project's type declarations
const Picker = React.lazy(() => import("@emoji-mart/react"));

export default function Org() {

    // !全局样式    
    const [_, token] = useToken();
    const [modal, contextHolder] = Modal.useModal();

    const [orgEditVisible, setOrgEditVisible] = useState(false);
    const [orgForm] = useForm();

    const [orgTypeTableVisible, setOrgTypeTableVisible] = useState(false);
    const [orgTypeEditVisible, setOrgTypeEditVisible] = useState(false);
    const [orgTypeData, setOrgTypeData] = useState<any>([]);
    const [orgTypeForm] = useForm();

    const [orgIcon, setOrgIcon] = useState("🏢");
    const [orgIconId, setOrgIconId] = useState("office");
    const [emojiModalVisible, setEmojiModalVisible] = useState(false);

    const [treeData, setTreeData] = useState<Array<any>>([]);
    const [orgDetail, setOrgDetail] = useState<getOrganizationResult | null>(null);

    const [isLoading, setIsLoading] = useState(false);

    const orgTypeTableColumns: any = [

        {
            title: '序号', dataIndex: "num", align: 'center', width: '90px', fixed: "left",
            render: (_data: any, _record: any, index: any) => (
                <span>{1 + index}</span>
            )
        },
        { title: '名称', dataIndex: "name", align: 'center', width: '160px' },
        { title: '编码', dataIndex: "code", align: 'center', width: '160px' },

        {
            title: '操作', dataIndex: "operate", align: 'center', width: '130px', fixed: 'right',
            render: (_data: any, record: any) => (
                <div>
                    <RightElement identify="add-update-org-type" child={
                        <>
                            <Tooltip title="编辑">
                                <Button type='link' style={{ padding: '4px 6px' }} onClick={() => orgTypeEdit(record)}><FontAwesomeIcon fixedWidth icon={faEdit} /></Button>
                            </Tooltip>
                        </>
                    }></RightElement>
                    <RightElement identify="remove-org-type" child={
                        <>
                            <Tooltip title="删除">
                                <Button type='link' style={{ padding: '4px 6px' }} onClick={() => deleteOrgType(record.id)}><FontAwesomeIcon fixedWidth icon={faTrash} /></Button>
                            </Tooltip>
                        </>
                    }></RightElement>
                </div>
            ),
        }
    ];

    useEffect(() => {
        init();
    }, []);// eslint-disable-line react-hooks/exhaustive-deps

    async function init() {
        await getTreeData();
        let result = await OrganizationService.GetOrganizationTypes();
        setOrgTypeData(result.data.data);
    }

    // 取得树数据
    async function getTreeData() {
        let response = await OrganizationService.getOrganizationTree();
        makeTreeData(response.data.data);
        setTreeData(response.data.data);
    }

    // 创建新组织
    function createOrg() {
        setOrgIcon("🏢");
        setOrgIconId("office");
        setOrgEditVisible(true);
    }

    // 编辑组织
    function editOrg() {
        orgForm.setFieldsValue({
            id: orgDetail?.id,
            upOrg: orgDetail?.upId,
            orgName: orgDetail?.name,
            orgCode: orgDetail?.code,
            orgType: orgDetail?.type,
            orgPhone: orgDetail?.phone,
            orgAddress: orgDetail?.address,
            sorting: orgDetail?.sorting
        });
        setOrgIcon(orgDetail!.icon);
        setOrgIconId(orgDetail!.iconId);

        setOrgEditVisible(true);
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
        try {
            setIsLoading(true);

            if (values['id']) {

                // 更新
                await OrganizationService.updateOrganization({
                    id: values["id"],
                    upId: values["upOrg"],
                    name: values["orgName"],
                    code: values["orgCode"],
                    type: values["orgType"],
                    icon: orgIcon,
                    iconId: orgIconId,
                    phone: values["orgPhone"],
                    address: values["orgAddress"],
                    sorting: values["sorting"]
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
                    type: values["orgType"],
                    icon: orgIcon,
                    iconId: orgIconId,
                    phone: values["orgPhone"],
                    address: values["orgAddress"],
                    sorting: values["sorting"]
                });
                await getTreeData();
            }
            setOrgEditVisible(false);
        }
        finally {
            setIsLoading(false);
        }
    }

    // 删除组织类型
    async function deleteOrgType(id: number) {
        Modal.confirm({
            title: "是否删除该组织类型？",
            onOk: async () => {

                await OrganizationService.RemoveOrganizationType({ id: id });
                let result = await OrganizationService.GetOrganizationTypes();
                setOrgTypeData(result.data.data);
            }
        });
    }

    // 展示组织类型列表
    async function showOrgTypes(_values: any) {
        setOrgTypeTableVisible(true);
        let result = await OrganizationService.GetOrganizationTypes();
        setOrgTypeData(result.data.data);
    }

    // 组织类型编辑
    async function orgTypeEdit(data: any) {

        orgTypeForm.setFieldsValue({
            id: data?.id,
            name: data?.name,
            code: data?.code
        });
        setOrgTypeEditVisible(true);
    }

    // 组织类型提交
    async function orgTypeSubmit(values: any) {

        try {
            setIsLoading(true);

            // 更新
            await OrganizationService.AddOrUpdateOrganizationType({
                id: values["id"],
                name: values["name"],
                code: values["code"]
            });
            let result = await OrganizationService.GetOrganizationTypes();
            setOrgTypeData(result.data.data);
            setOrgTypeEditVisible(false);
        }
        finally {
            setIsLoading(false);
        }
    }

    // 将后端数据转为树格式
    function makeTreeData(data: any) {
        for (const d of data) {
            d.value = d.key;
            d.icon = (<>{<FontAwesomeIcon icon={faBuilding}></FontAwesomeIcon>}</>);
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
            {contextHolder}

            {/* 操作 */}
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: "14px" }}>

                <div style={{ display: 'flex', alignItems: 'center' }}>
                    <FontAwesomeIcon icon={faSitemap} style={{ marginRight: '8px', fontSize: "18px" }} />
                    <Title level={4} style={{ marginBottom: 0 }}>组织信息</Title>
                </div>
                <div>
                    {orgDetail !== null &&
                        <>
                            <RightElement identify="edit-org" child={
                                <>
                                    <Tooltip title="编辑当前组织" color={token.colorPrimary}>
                                        <Button type="primary" onClick={editOrg} icon={<FontAwesomeIcon fixedWidth icon={faEdit} />} style={{ marginRight: '4px' }}></Button>
                                    </Tooltip>
                                </>
                            }></RightElement>
                            <RightElement identify="remove-org" child={
                                <>
                                    <Tooltip title="删除当前组织" color={token.colorPrimary}>
                                        <Button type="primary" onClick={deleteOrg} icon={<FontAwesomeIcon fixedWidth icon={faTrash} />} style={{ marginRight: '4px' }}></Button>
                                    </Tooltip>
                                </>
                            }></RightElement>
                        </>
                    }
                    <RightElement identify="create-org" child={
                        <>
                            <Tooltip title="创建组织" color={token.colorPrimary}>
                                <Button type="primary" icon={<FontAwesomeIcon icon={faPlus} />} style={{ marginRight: '4px' }} onClick={createOrg} />
                            </Tooltip>
                        </>
                    }></RightElement>
                    <RightElement identify="org-page" child={
                        <>
                            <Tooltip title="组织类型" color={token.colorPrimary}>
                                <Button type="primary" icon={<FontAwesomeIcon icon={faObjectGroup} />} style={{ marginRight: '4px' }} onClick={showOrgTypes} />
                            </Tooltip>
                        </>
                    }></RightElement>
                </div>
            </div>

            <Divider style={{ margin: '14px 0 0 0' }} />

            <div id="org-container">
                <div id='org-tree-container'>
                    <Tree showLine={true} showIcon={true} treeData={treeData} onSelect={elementSelect} />
                </div>
                <Divider type='vertical' style={{ height: '100%' }} />
                <div id="org-detail-container">
                    {orgDetail !== null &&
                        <>
                            <Descriptions bordered>
                                <Descriptions.Item label="组织名称" labelStyle={{ width: "200px" }} span={3}>{orgDetail.name}</Descriptions.Item>
                                <Descriptions.Item label="组织编码" span={3}>{orgDetail.code}</Descriptions.Item>
                                <Descriptions.Item label="组织类型" span={3}>{orgDetail.type}</Descriptions.Item>
                                <Descriptions.Item label="组织类型编码" span={3}>{orgDetail.typeName}</Descriptions.Item>
                                <Descriptions.Item label="电话" span={3}>{orgDetail.phone}</Descriptions.Item>
                                <Descriptions.Item label="地址" span={3}>{orgDetail.address}</Descriptions.Item>
                                <Descriptions.Item label="排序" span={3}>{orgDetail.sorting}</Descriptions.Item>
                            </Descriptions>
                        </>
                    }
                </div>
            </div>

            <Modal modalRender={(modal) => { return <DraggableModal ><div>{modal}</div></DraggableModal> }}
                open={orgEditVisible} destroyOnClose={true} onCancel={() => setOrgEditVisible(false)} footer={null}
                title="组织信息编辑" width={800} maskClosable={false}>
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
                            { max: 32, message: "组织编码过长" },
                        ]
                    }>
                        <Input placeholder="请输入组织编码" allowClear={true} autoComplete="off"></Input>
                    </Form.Item>
                    <Form.Item name="orgType" label="组织类型" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                        <Select placeholder="请选择组织类型" allowClear={true}>
                            {
                                orgTypeData.map((o: any) => (
                                    <Select.Option value={o.code} key={o.code}>{o.name}</Select.Option>
                                ))
                            }
                        </Select>
                    </Form.Item>
                    <Form.Item name="orgPhone" label="联系电话" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                        <Input placeholder="请输入联系电话" allowClear={true} autoComplete="off"></Input>
                    </Form.Item>
                    <Form.Item name="orgAddress" label="地址" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                        <Input placeholder="请输入地址" allowClear={true} autoComplete="off"></Input>
                    </Form.Item>
                    <Form.Item name="sorting" label="排序" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} rules={
                        [
                            { required: true, message: "请输入排序值" },
                        ]
                    } >
                        <InputNumber style={{ width: '100%' }} autoComplete="off" placeholder="请输入排序值" />
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6, span: 16 }}>
                        <Button type='primary' icon={<FontAwesomeIcon fixedWidth icon={faSave} />} htmlType="submit" loading={isLoading}>保存</Button>
                    </Form.Item>
                </Form>
            </Modal>

            <Modal modalRender={(modal) => { return <DraggableModal ><div>{modal}</div></DraggableModal> }}
                width={390} open={emojiModalVisible} footer={null} title="选择图标" destroyOnClose={true}
                onCancel={() => setEmojiModalVisible(false)} maskClosable={false}>
                <Picker data={data} emoji={orgIconId} theme="light" locale={"zh"} onEmojiSelect={
                    (value: any) => {
                        console.log(value);
                        setOrgIcon(value.native);
                        setOrgIconId(value.id);
                        setEmojiModalVisible(false);
                    }
                } />
            </Modal>

            <Modal modalRender={(modal) => { return <DraggableModal ><div>{modal}</div></DraggableModal> }}
                width={600} open={orgTypeTableVisible} onCancel={() => setOrgTypeTableVisible(false)} footer={null} title="组织类型" >

                <RightElement identify="add-update-org-type" child={
                    <>
                        <Button icon={<FontAwesomeIcon icon={faPlus} fixedWidth />} onClick={() => orgTypeEdit(null)} style={{ marginBottom: '10px' }}>创建组织类型</Button>
                    </>
                }></RightElement>
                <Table size="small" columns={orgTypeTableColumns} dataSource={orgTypeData} pagination={false}></Table>
            </Modal>
            <Modal modalRender={(modal) => { return <DraggableModal ><div>{modal}</div></DraggableModal> }}
                width={500} open={orgTypeEditVisible} destroyOnClose={true} onCancel={() => setOrgTypeEditVisible(false)} footer={null}
                title="组织类型编辑" maskClosable={false}>

                <Form preserve={false} form={orgTypeForm} onFinish={orgTypeSubmit}>

                    <Form.Item name="id" hidden>
                        <Input />
                    </Form.Item>
                    <Form.Item name="name" label="组织类型名称" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} required rules={
                        [
                            { required: true, message: "请输入组织类型名称" },
                            { max: 50, message: "组织类型名称过长" },
                        ]
                    }>
                        <Input placeholder="请输入组织类型名称" allowClear={true} autoComplete="off"></Input>
                    </Form.Item>
                    <Form.Item name="code" label="组织类型编码" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} required rules={
                        [
                            { required: true, message: "请输入组织类型编码" },
                            { max: 50, message: "组织类型编码过长" },
                        ]
                    }>
                        <Input placeholder="请输入组织类型编码" allowClear={true} autoComplete="off"></Input>
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6, span: 16 }}>
                        <Button type='primary' icon={<FontAwesomeIcon fixedWidth icon={faSave} />} htmlType="submit" loading={isLoading}>保存</Button>
                    </Form.Item>
                </Form>
            </Modal>

        </>
    );
}