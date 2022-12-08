import { faEdit, faPlus, faSave, faTrash } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Button, Divider, Form, Input, InputNumber, List, Modal, Space, Table, Tooltip } from 'antd';
import { useForm } from 'antd/lib/form/Form';
import { useEffect, useState } from 'react';
import { RightElement } from '../../../components/right/rightElement';
import { DictionaryService, GetDicTypeListResponse, GetDicValueListResponse } from '../../../http/requests/dictionary';
import './dictionary.css';


export default function Dictionary() {

    const [isLoading, setIsLoading] = useState(false);

    const [selectedTypeId, setSelectedTypeId] = useState<number | null>(null);

    const [dicTypeVisible, setDicTypeVisible] = useState(false);
    const [dicValueVisible, setDicValueVisible] = useState(false);

    const [typeForm] = useForm();
    const [valueForm] = useForm();

    const [typeData, setTypeData] = useState(new Array<GetDicTypeListResponse>());
    const [valueData, setValueData] = useState(new Array<GetDicValueListResponse>());

    const tableColumns: any = [
        {
            title: '序号', dataIndex: "num", align: 'center', width: '90px',
            render: (data: any, record: any, index: any) => (
                <span>{1 + index}</span>
            )
        },
        { title: '字典项目名称', dataIndex: "name", align: 'center', width: '300px' },
        { title: '字典项目代码', dataIndex: "code", align: 'center' },
        { title: '排序', dataIndex: "sorting", align: 'center', width: '100px' },
        {
            title: '操作', dataIndex: "operate", align: 'center', width: '130px', fixed: 'right',
            render: (data: any, record: any) => (
                <Space size="middle">
                    <Tooltip title="编辑">
                        <RightElement identify="update-dicvalue" child={
                            <>
                                <a onClick={() => editValue(record)}>
                                    <FontAwesomeIcon icon={faEdit} />
                                </a>
                            </>
                        }></RightElement>
                    </Tooltip>
                    <Tooltip title="删除">
                        <RightElement identify="delete-dicvalue" child={
                            <>
                                <a onClick={() => deleteValue(record.id)}>
                                    <FontAwesomeIcon icon={faTrash} />
                                </a>
                            </>
                        }></RightElement>
                    </Tooltip>
                </Space>
            ),
        },
    ];

    useEffect(() => {
        initDicType();
    }, []);// eslint-disable-line react-hooks/exhaustive-deps

    //#region dicvalue

    async function initDicType() {
        let result = await DictionaryService.getDicTypeList();
        setTypeData(result.data.data);
    }

    async function addDicType() {
        typeForm.resetFields();
        setDicTypeVisible(true);
    }

    async function editType(data: GetDicValueListResponse) {
        typeForm.setFieldsValue({
            id: data.id,
            name: data.name,
            code: data.code
        });
        setDicTypeVisible(true);
    }

    async function deleteType(id: number) {
        Modal.confirm({
            title: "是否删除该字典类型？",
            onOk: async () => {
                await DictionaryService.deleteDicType({ id: id });
                await initDicType();
                setSelectedTypeId(null);
                setValueData([]);
            }
        })
    }

    async function submitType(values: any) {
        try {
            setIsLoading(true);

            if (values["id"] !== null && values["id"] !== undefined) {
                await DictionaryService.updateDicType({
                    id: values["id"],
                    code: values["code"],
                    name: values["name"],
                });
            } else {
                await DictionaryService.addDicType({
                    code: values["code"],
                    name: values["name"],
                });
            }
            await initDicType();
            setDicTypeVisible(false);
        } finally {
            setIsLoading(false);
        }
    }

    //#endregion

    //#region dicvalue
    async function initValueList(id: number) {
        setSelectedTypeId(id);
        let result = await DictionaryService.getDicValueList({ id: id });
        setValueData(result.data.data);
    }

    async function initDicValue() {
        let result = await DictionaryService.getDicValueList({ id: selectedTypeId! });
        setValueData(result.data.data);
    }

    async function addDicValue() {
        valueForm.resetFields();
        setDicValueVisible(true);
    }

    async function editValue(data: GetDicValueListResponse) {
        valueForm.setFieldsValue({
            id: data.id,
            name: data.name,
            code: data.code,
            sorting: data.sorting
        });
        setDicValueVisible(true);
    }

    async function deleteValue(id: number) {
        Modal.confirm({
            title: '是否删除该字典项目？',
            onOk: async () => {
                await DictionaryService.deleteDicValue({ id: id });
                await initDicValue();
            }
        })
    }

    async function submitValue(values: any) {
        try {
            setIsLoading(true);

            if (values["id"] !== null && values["id"] !== undefined) {
                await DictionaryService.updateDicValue({
                    id: values["id"],
                    typeId: selectedTypeId!,
                    code: values["code"],
                    name: values["name"],
                    sorting: values["sorting"]
                });
            } else {
                await DictionaryService.addDicValue({
                    typeId: selectedTypeId!,
                    code: values["code"],
                    name: values["name"],
                    sorting: values["sorting"]
                });
            }
            await initDicValue();
            setDicValueVisible(false);
        } finally {
            setIsLoading(false);
        }

    }

    //#endregion

    return (
        <>
            <div id="dic-container">
                <div id="dic-type-container">
                    <RightElement identify="add-dictype" child={
                        <>
                            <div>
                                <Button icon={<FontAwesomeIcon icon={faPlus} fixedWidth />} onClick={addDicType}>创建字典类型</Button>
                            </div>
                            <Divider style={{ margin: "10px 0" }} />
                        </>
                    }></RightElement>
                    <List itemLayout='horizontal' dataSource={typeData}
                        renderItem={item => (
                            <List.Item actions={[
                                <RightElement identify="edit-dictype" child={
                                    <>
                                        <a onClick={() => editType(item)}><FontAwesomeIcon icon={faEdit} /></a>
                                    </>
                                }></RightElement>,
                                <RightElement identify="delete-dictype" child={
                                    <>
                                        <a onClick={() => deleteType(item.id!)}><FontAwesomeIcon icon={faTrash} /></a>
                                    </>
                                }></RightElement>
                            ]}>
                                <List.Item.Meta title={<a onClick={() => initValueList(item.id!)}>{item.name}</a>} description={item.code} />
                            </List.Item>
                        )}
                    />
                </div>
                <Divider type="vertical" style={{ height: '100%' }} />
                <div id="dic-value-container">
                    <RightElement identify="add-dicvalue" child={
                        <>
                            <div>
                                <Button disabled={selectedTypeId === null} icon={<FontAwesomeIcon icon={faPlus} fixedWidth />} onClick={addDicValue}>创建字典项目</Button>
                            </div>
                            <Divider style={{ margin: "10px 0" }} />
                        </>
                    }></RightElement>
                    <Table size="small" columns={tableColumns} dataSource={valueData} scroll={{ x: 900 }} pagination={false}></Table>
                </div>
            </div>
            <Modal open={dicTypeVisible} destroyOnClose={true} maskClosable={false} footer={null}
                title={"编辑字典类型"} onCancel={() => setDicTypeVisible(false)}>
                <Form form={typeForm} preserve={false} onFinish={submitType}>
                    <Form.Item name="id" hidden>
                        <Input />
                    </Form.Item>
                    <Form.Item name="name" label="名称" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }}
                        rules={
                            [
                                { required: true, message: '请输入字典类型名称' },
                                { max: 50, message: "字典类型名称过长" }
                            ]
                        }>
                        <Input placeholder="请输入名称" allowClear={true} autoComplete="off2"></Input>
                    </Form.Item>
                    <Form.Item name="code" label="编码" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} rules={
                        [
                            { required: true, message: '请输入字典类型编码' },
                            { max: 80, message: "字典类型编码过长" },
                            { pattern: /^[A-Za-z0-9-_]+$/g, message: '字典类型编码只允许数字字母下划线' },
                        ]
                    }>
                        <Input placeholder="请输入编码" allowClear={true} autoComplete="off2"></Input>
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6, span: 16 }}>
                        <Button type='primary' icon={<FontAwesomeIcon icon={faSave} fixedWidth />} htmlType="submit" loading={isLoading}>保存</Button>
                    </Form.Item>
                </Form>
            </Modal>
            <Modal open={dicValueVisible} destroyOnClose={true} maskClosable={false} footer={null}
                title={"编辑字典项目"} onCancel={() => setDicValueVisible(false)}>
                <Form form={valueForm} preserve={false} onFinish={submitValue}>
                    <Form.Item name="id" hidden>
                        <Input />
                    </Form.Item>
                    <Form.Item name="name" label="名称" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} rules={
                        [
                            { required: true, message: '请输入字典项目名称' },
                            { max: 50, message: "字典项目名称过长" }
                        ]
                    }>
                        <Input placeholder="请输入名称" allowClear={true} autoComplete="off2"></Input>
                    </Form.Item>
                    <Form.Item name="code" label="编码" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} rules={
                        [
                            { required: true, message: '请输入字典项目编码' },
                            { max: 80, message: "字典项目编码过长" },
                            { pattern: /^[A-Za-z0-9-_]+$/g, message: '字典项目编码只允许数字字母下划线' },
                        ]
                    }>
                        <Input placeholder="请输入编码" allowClear={true} autoComplete="off2"></Input>
                    </Form.Item>
                    <Form.Item name="sorting" label="排序" labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} rules={
                        [
                            { required: true, message: '请输入字典项目排序' }
                        ]
                    }>
                        <InputNumber style={{ width: "100%" }} placeholder="请输入排序" autoComplete="off2" />
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6, span: 16 }}>
                        <Button type='primary' icon={<FontAwesomeIcon icon={faSave} fixedWidth />} htmlType="submit" loading={isLoading}>保存</Button>
                    </Form.Item>
                </Form>
            </Modal>
        </>
    );
}