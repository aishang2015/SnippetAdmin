
import './page.css';

import { Button, Descriptions, Divider, Form, Input, InputNumber, Modal, Select, Tag, Tooltip, Tree, TreeSelect } from 'antd';
import { useEffect, useState } from 'react';
import { useForm } from 'antd/lib/form/Form';
import { ApiInfoService } from '../../../http/requests/develop/apiInfo';
import { ElementService } from '../../../http/requests/rbac/element';
import { join, split } from 'lodash';
import { Constants } from '../../../common/constants';
import { RightElement } from '../../../components/right/rightElement';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBars, faEdit, faLink, faPlus, faSave, faShieldHalved, faTrash } from '@fortawesome/free-solid-svg-icons';
import { useToken } from 'antd/es/theme/internal';
import Title from 'antd/es/typography/Title';
import DraggableModal from '../../../components/common/draggableModal';

export default function Page() {

    // !全局样式    
    const [_, token] = useToken();
    const [modal, contextHolder] = Modal.useModal();

    const [apiInfo, setApiInfo] = useState(new Array<string>());
    const [treeData, setTreeData] = useState(new Array<any>());
    const [elementDetail, setElementDetail] = useState<any>(null);

    const [elementEditVisible, setElementEditVisible] = useState(false);
    const [elementForm] = useForm();

    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        init();
    }, []);// eslint-disable-line react-hooks/exhaustive-deps

    async function init() {
        await initApiInfo();
        await getTreeData();
    }

    // 取得所有接口数据
    async function initApiInfo() {
        let response = await ApiInfoService.getApiInfo();
        setApiInfo(response.data.data);
    }

    // 取得树数据
    async function getTreeData() {
        let response = await ElementService.getElementTree();
        makeTreeData(response.data.data);
        setTreeData(response.data.data);
    }

    // 添加元素
    function addElement() {
        setElementEditVisible(true);
    }

    // 编辑元素
    function editElement(id: number) {
        setElementEditVisible(true);
        elementForm.setFieldsValue({
            id: elementDetail.id,
            elementName: elementDetail.name,
            elementType: elementDetail.type,
            elementIdentity: elementDetail.identity,
            elementInterfaces: elementDetail.accessApi ? split(elementDetail.accessApi, ',') : [],
            sorting: elementDetail.sorting
        });
    }

    // 删除元素
    function deleteElement(id: number) {
        Modal.confirm({
            title: '是否删除?',
            content: '删除权限信息后，可能会导致用户无法访问页面以及数据，请谨慎操作！',
            onOk: async () => {
                await ElementService.deleteElement(elementDetail.id);
                setElementDetail(null);
                await getTreeData();
            }
        });
    }

    // 元素编辑表单提交
    async function elementFormSubmit(values: any) {

        try {

            setIsLoading(true);

            if (values['id']) {
                await ElementService.updateElement({
                    id: values['id'],
                    name: values['elementName'],
                    type: values['elementType'],
                    identity: values['elementIdentity'],
                    accessApi: join(values['elementInterfaces'], ','),
                    sorting: values['sorting']
                });
                let response = await ElementService.getElement(elementDetail.id);
                setElementDetail(response.data.data);
            } else {
                await ElementService.createElement({
                    upId: values['upId'],
                    name: values['elementName'],
                    type: values['elementType'],
                    identity: values['elementIdentity'],
                    accessApi: join(values['elementInterfaces'], ','),
                    sorting: values['sorting']
                });
            }
            setElementEditVisible(false);
            await getTreeData();
        } finally {
            setIsLoading(false);
        }
    }

    // 树元素选中
    async function elementSelect(selectedKeys: any, e: { selected: boolean }) {
        if (e.selected) {
            let response = await ElementService.getElement(selectedKeys[0]);
            setElementDetail(response.data.data);
        } else {
            setElementDetail(null);
        }
    }

    // 将后端数据转为树格式
    function makeTreeData(data: any) {
        for (const d of data) {
            d.value = d.key;
            if (d.type === 1) {
                d.icon = (<FontAwesomeIcon fixedWidth icon={faBars} />)
            } else if (d.type === 2) {
                d.icon = (<FontAwesomeIcon fixedWidth icon={faLink} />)
            }

            if (d.children.length === 0) {
                d.switcherIcon = (<></>)
            } else {
                makeTreeData(d.children);
            }
        }
    }

    return (
        <>
            {contextHolder}

            {/* 操作 */}
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: "6px" }}>
                <div style={{ display: 'flex', alignItems: 'center' }}>
                    <FontAwesomeIcon icon={faShieldHalved} style={{ marginRight: '8px', fontSize: "18px" }} />
                    <Title level={4} style={{ marginBottom: 0 }}>权限信息</Title>
                </div>
                <div>
                    {elementDetail !== null &&
                        <>
                            <RightElement identify="edit-element" child={
                                <>
                                    <Tooltip title="编辑当前权限" color={token.colorPrimary}>
                                        <Button type="primary" icon={<FontAwesomeIcon fixedWidth icon={faEdit} />}
                                            style={{ marginRight: '4px' }} onClick={() => editElement(1)}></Button>
                                    </Tooltip>
                                </>
                            }></RightElement>
                            <RightElement identify="remove-element" child={
                                <>
                                    <Tooltip title="删除当前权限" color={token.colorPrimary}>
                                        <Button type="primary" icon={<FontAwesomeIcon fixedWidth icon={faTrash} />}
                                            style={{ marginRight: '4px' }} onClick={() => deleteElement(1)}></Button>
                                    </Tooltip>
                                </>
                            }></RightElement>
                        </>
                    }
                    <RightElement identify="add-position" child={
                        <>
                            <Tooltip title="创建" color={token.colorPrimary}>
                                <Button type="primary" icon={<FontAwesomeIcon icon={faPlus} />} onClick={addElement} className="mr-4" />
                            </Tooltip>
                        </>
                    }></RightElement>
                </div>
            </div>

            <Divider style={{ margin: '14px 0 0 0' }} />

            <div id="page-container">
                <div id="page-tree-container">
                    <Tree showLine={true} showIcon={true} treeData={treeData} onSelect={elementSelect} />
                </div>
                <Divider type='vertical' style={{ height: '100%' }} />
                <div id="page-detail-container">
                    {elementDetail !== null &&
                        <>
                            <Descriptions title="" bordered column={3}>
                                <Descriptions.Item label="元素名称" span={3} labelStyle={{ width: "200px" }}>{elementDetail.name}</Descriptions.Item>
                                <Descriptions.Item label="元素类型" span={3}>{elementDetail.type === 1 ? '菜单' : '按钮/链接'}</Descriptions.Item>
                                <Descriptions.Item label="元素标识" span={3}>{elementDetail.identity}</Descriptions.Item>
                                <Descriptions.Item label="接口信息" span={3}>
                                    {elementDetail.accessApi && split(elementDetail.accessApi, ',')?.map((api, index) => (
                                        <div key={index} style={{ marginBottom: '5px' }}>
                                            <Tag color="#87d068">POST</Tag><Tag color="#108ee9">{api}</Tag>
                                        </div>
                                    ))
                                    }
                                </Descriptions.Item>
                                <Descriptions.Item label="元素排序" span={3}>{elementDetail.sorting}</Descriptions.Item>
                            </Descriptions>
                        </>
                    }
                </div>
            </div>

            <Modal modalRender={(modal) => { return <DraggableModal ><div>{modal}</div></DraggableModal> }}
                open={elementEditVisible} destroyOnClose={true} onCancel={() => setElementEditVisible(false)} footer={null}
                title="页面元素编辑" width={600} maskClosable={false}>
                <Form form={elementForm} onFinish={elementFormSubmit} preserve={false} >
                    <Form.Item name="id" hidden>
                        <Input />
                    </Form.Item>
                    {!elementForm.getFieldValue('id') &&
                        <Form.Item name="upId" label="上级元素" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                            <TreeSelect treeData={treeData} placeholder="请选择上级元素" allowClear treeLine={true} treeIcon={true}></TreeSelect>
                        </Form.Item>
                    }
                    <Form.Item name="elementName" label="元素名称" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} required
                        rules={
                            [
                                { required: true, message: "请输入元素名称" },
                                { max: 50, message: "元素名称过长" },
                            ]
                        }>
                        <Input placeholder="请输入元素名称" autoComplete="off"></Input>
                    </Form.Item>
                    <Form.Item name="elementType" label="元素类型" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} required
                        rules={
                            [
                                { required: true, message: '请选择元素类型' }
                            ]
                        }>
                        <Select placeholder="请选择元素类型" allowClear={true}>
                            {Constants.ElementTypeArray.map(e => (
                                <Select.Option value={e.key} key={e.key}>{e.value}</Select.Option>
                            ))}
                        </Select>
                    </Form.Item>
                    <Form.Item name="elementIdentity" label="元素标识" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} required
                        rules={
                            [
                                { required: true, message: '请输入元素标识' },
                                { max: 80, message: "元素标识过长" },
                                { pattern: /^[A-Za-z0-9-_]+$/g, message: '元素标识只允许数字字母下划线' },
                            ]
                        }>
                        <Input placeholder="请输入元素标识" autoComplete="off"></Input>
                    </Form.Item>
                    <Form.Item name="elementInterfaces" label="使用接口" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}>
                        <Select placeholder="请选择使用接口" allowClear={true} mode="multiple">
                            {apiInfo.map(
                                (api, index) =>
                                (
                                    <Select.Option value={api} key={api}>
                                        <Tag color="#87d068">POST</Tag><Tag color="#108ee9">{api}</Tag>
                                    </Select.Option>
                                )
                            )
                            }
                        </Select>
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
        </>
    );
}