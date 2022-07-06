import { Picker } from 'emoji-mart';
import 'emoji-mart/css/emoji-mart.css';
import './org.less';

import { faEdit, faObjectGroup, faPlus, faSave, faTrash } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Button, Descriptions, Divider, Form, Input, InputNumber, Modal, Select, Space, Table, Tooltip, Tree, TreeSelect } from 'antd';
import { useForm } from 'antd/lib/form/Form';
import { useEffect, useState } from 'react';
import { RightElement } from '../../../components/right/rightElement';
import { getOrganizationResult, OrganizationService } from '../../../http/requests/organization';

export default function Org() {

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
            render: (data: any, record: any, index: any) => (
                <span>{1 + index}</span>
            )
        },
        { title: '名称', dataIndex: "name", align: 'center', width: '160px' },
        { title: '编码', dataIndex: "code", align: 'center', width: '160px' },

        {
            title: '操作', dataIndex: "operate", align: 'center', width: '130px', fixed: 'right',
            render: (data: any, record: any) => (
                <Space size="middle">
                    <RightElement identify="add-update-org-type" child={
                        <>
                            <Tooltip title="编辑"><a onClick={() => orgTypeEdit(record)}><FontAwesomeIcon fixedWidth icon={faEdit} /></a></Tooltip>
                        </>
                    }></RightElement>
                    <RightElement identify="remove-org-type" child={
                        <>
                            <Tooltip title="删除"><a onClick={() => deleteOrgType(record.id)}><FontAwesomeIcon fixedWidth icon={faTrash} /></a></Tooltip>
                        </>
                    }></RightElement>
                </Space>
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
    async function showOrgTypes(values: any) {
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
                            <Button icon={<FontAwesomeIcon icon={faPlus} fixedWidth />} onClick={createOrg}>创建组织</Button>
                        </>
                    }></RightElement>
                    <RightElement identify="org-page" child={
                        <>
                            <Button icon={<FontAwesomeIcon icon={faObjectGroup} fixedWidth />} onClick={showOrgTypes} style={{ marginLeft: '5px' }}>组织类型</Button>
                        </>
                    }></RightElement>
                    <Divider style={{ margin: "10px 0" }} />
                    <Tree showLine={true} showIcon={true} treeData={treeData} onSelect={elementSelect} />
                </div>
                <div id="org-detail-container">
                    {orgDetail !== null &&
                        <>
                            <div>
                                <RightElement identify="edit-org" child={
                                    <>
                                        <Button onClick={editOrg} icon={<FontAwesomeIcon fixedWidth icon={faEdit} />} style={{ marginRight: '10px' }}>编辑组织</Button>
                                    </>
                                }></RightElement>
                                <RightElement identify="remove-org" child={
                                    <>
                                        <Button onClick={deleteOrg} icon={<FontAwesomeIcon fixedWidth icon={faTrash} />} style={{ marginRight: '10px' }}>删除组织</Button>
                                    </>
                                }></RightElement>
                            </div>
                            <Divider style={{ margin: "10px 0" }} />
                            <Descriptions title="组织信息" bordered>
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

            <Modal visible={orgEditVisible} destroyOnClose={true} onCancel={() => setOrgEditVisible(false)} footer={null}
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
                    <Form.Item name="orgIcon" label="组织图标" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                        <a onClick={() => setEmojiModalVisible(true)} style={{ fontSize: "20px" }}>{orgIcon}</a>
                    </Form.Item>
                    <Form.Item name="orgPhone" label="联系电话" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                        <Input placeholder="请输入联系电话" allowClear={true} autoComplete="off2"></Input>
                    </Form.Item>
                    <Form.Item name="orgAddress" label="地址" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                        <Input placeholder="请输入地址" allowClear={true} autoComplete="off2"></Input>
                    </Form.Item>
                    <Form.Item name="sorting" label="排序" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} rules={
                        [
                            { required: true, message: "请输入排序值" },
                        ]
                    } >
                        <InputNumber style={{ width: '100%' }} autoComplete="off2" placeholder="请输入排序值" />
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6, span: 16 }}>
                        <Button icon={<FontAwesomeIcon fixedWidth icon={faSave} />} htmlType="submit" loading={isLoading}>保存</Button>
                    </Form.Item>
                </Form>
            </Modal>

            <Modal width={390} visible={emojiModalVisible} footer={null} title="选择图标" destroyOnClose={true}
                onCancel={() => setEmojiModalVisible(false)} maskClosable={false}>
                <Picker native={true} autoFocus={true} emoji={orgIconId} onSelect={(e: any) => {
                    setOrgIcon(e.native);
                    setOrgIconId(e.id);
                    setEmojiModalVisible(false);
                }} i18n={{
                    search: '搜索',
                    notfound: '没找到您想要的Emoji',
                    categories: {
                        search: '搜索结果',
                        recent: '经常使用',
                        people: '人',
                        nature: '动物和自然',
                        foods: '食品和饮料',
                        activity: '活动',
                        places: '旅行和地点',
                        objects: '物体',
                        symbols: '符号',
                        flags: '旗帜',
                        custom: '自定义',
                    }
                }} />
            </Modal>

            <Modal width={600} visible={orgTypeTableVisible} onCancel={() => setOrgTypeTableVisible(false)} footer={null} title="组织类型" >

                <RightElement identify="add-update-org-type" child={
                    <>
                        <Button icon={<FontAwesomeIcon icon={faPlus} fixedWidth />} onClick={() => orgTypeEdit(null)} style={{ marginBottom: '10px' }}>创建组织类型</Button>
                    </>
                }></RightElement>
                <Table size="small" columns={orgTypeTableColumns} dataSource={orgTypeData} pagination={false}></Table>
            </Modal>
            <Modal width={500} visible={orgTypeEditVisible} destroyOnClose={true} onCancel={() => setOrgTypeEditVisible(false)} footer={null}
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
                        <Input placeholder="请输入组织类型名称" allowClear={true} autoComplete="off2"></Input>
                    </Form.Item>
                    <Form.Item name="code" label="组织类型编码" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} required rules={
                        [
                            { required: true, message: "请输入组织类型编码" },
                            { max: 50, message: "组织类型编码过长" },
                        ]
                    }>
                        <Input placeholder="请输入组织类型编码" allowClear={true} autoComplete="off2"></Input>
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6, span: 16 }}>
                        <Button icon={<FontAwesomeIcon fixedWidth icon={faSave} />} htmlType="submit" loading={isLoading}>保存</Button>
                    </Form.Item>
                </Form>
            </Modal>

        </>
    );
}