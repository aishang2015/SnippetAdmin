
import './page.less';

import { Button, Descriptions, Divider, Form, Input, Modal, Select, Tag, Tree, TreeSelect } from 'antd';
import { DeleteOutlined, EditOutlined, ExportOutlined, LinkOutlined, MenuOutlined, PlusOutlined, SaveOutlined } from '@ant-design/icons';
import { useEffect, useState } from 'react';
import { useForm } from 'antd/lib/form/Form';
import { ApiInfoService } from '../../../http/requests/apiInfo';
import { ElementService } from '../../../http/requests/element';
import { join, split } from 'lodash';
import { Constants } from '../../../common/constants';
import { downloadBlob } from '../../../common/file';
import { RightElement } from '../../../components/right/rightElement';

export default function Page() {

    const [apiInfo, setApiInfo] = useState(new Array<string>());
    const [treeData, setTreeData] = useState(new Array<any>());
    const [elementDetail, setElementDetail] = useState<any>(null);

    const [elementEditVisible, setElementEditVisible] = useState(false);
    const [elementForm] = useForm();

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
            elementInterfaces: elementDetail.accessApi ? split(elementDetail.accessApi, ',') : []
        });
    }

    // 删除元素
    function deleteElement(id: number) {
        Modal.confirm({
            title: '是否删除该元素?',
            content: '删除页面元素信息后，会导致角色的页面权限以及接口权限不可用，请谨慎操作！',
            onOk: async () => {
                await ElementService.deleteElement(elementDetail.id);
                setElementDetail(null);
                await getTreeData();
            }
        });
    }

    // 导出元素
    async function exportElement() {
        let response = await ElementService.exportElementData();
        downloadBlob(response.data, "element.txt");
    }

    // 元素编辑表单提交
    async function elementFormSubmit(values: any) {

        if (values['id']) {
            await ElementService.updateElement({
                id: values['id'],
                name: values['elementName'],
                type: values['elementType'],
                identity: values['elementIdentity'],
                accessApi: join(values['elementInterfaces'], ',')
            });
            let response = await ElementService.getElement(elementDetail.id);
            setElementDetail(response.data.data);
        } else {
            await ElementService.createElement({
                upId: values['upId'],
                name: values['elementName'],
                type: values['elementType'],
                identity: values['elementIdentity'],
                accessApi: join(values['elementInterfaces'], ',')
            });
        }
        setElementEditVisible(false);
        await getTreeData();
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
            if (d.type === 1) {
                d.icon = (<MenuOutlined />)
            } else if (d.type === 2) {
                d.icon = (<LinkOutlined />)
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
            <div id="page-container">
                <div id="page-tree-container">
                    <div>
                        <RightElement identify="add-element" child={
                            <>
                                <Button icon={<PlusOutlined />} style={{ marginRight: '10px' }} onClick={addElement}>添加</Button>
                            </>
                        }></RightElement>
                        <RightElement identify="export-element" child={
                            <>
                                <Button icon={<ExportOutlined />} style={{ marginRight: '10px' }} onClick={exportElement}>导出</Button>
                            </>
                        }></RightElement>
                    </div>
                    <Divider style={{ margin: "10px 0" }} />
                    <Tree showLine={true} showIcon={true} treeData={treeData} onSelect={elementSelect} />
                </div>
                <div id="page-detail-container">
                    {elementDetail !== null &&
                        <>
                            <div>
                                <RightElement identify="edit-element" child={
                                    <>
                                        <Button icon={<EditOutlined />} style={{ marginRight: '10px' }} onClick={() => editElement(1)}>编辑</Button>
                                    </>
                                }></RightElement>
                                <RightElement identify="remove-element" child={
                                    <>
                                        <Button icon={<DeleteOutlined />} style={{ marginRight: '10px' }} onClick={() => deleteElement(1)}>删除</Button>
                                    </>
                                }></RightElement>
                            </div>
                            <Divider style={{ margin: "10px 0" }} />
                            <Descriptions title="页面元素信息" bordered column={3}>
                                <Descriptions.Item label="元素名称" span={3} labelStyle={{ width: "200px" }}>{elementDetail.name}</Descriptions.Item>
                                <Descriptions.Item label="元素类型" span={3}>{elementDetail.type}</Descriptions.Item>
                                <Descriptions.Item label="元素标识" span={3}>{elementDetail.identity}</Descriptions.Item>
                                <Descriptions.Item label="接口信息" span={3}>
                                    {elementDetail.accessApi && split(elementDetail.accessApi, ',')?.map((api, index) => (
                                        <div key={index} style={{ marginBottom: '5px' }}>
                                            <Tag color="#87d068">POST</Tag><Tag color="#108ee9">{api}</Tag>
                                        </div>
                                    ))
                                    }
                                </Descriptions.Item>
                            </Descriptions>
                        </>
                    }
                </div>
            </div>

            <Modal visible={elementEditVisible} destroyOnClose={true} onCancel={() => setElementEditVisible(false)} footer={null}
                title="组织信息编辑" width={600}>
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
                    <Form.Item wrapperCol={{ offset: 6, span: 16 }}>
                        <Button icon={<SaveOutlined />} htmlType="submit">保存</Button>
                    </Form.Item>
                </Form>
            </Modal>
        </>
    );
}