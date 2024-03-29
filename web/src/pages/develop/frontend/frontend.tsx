
import { Button, Checkbox, DatePicker, Form, Input, InputNumber, Radio, Select, Space, Switch, Table, Tabs, TreeSelect, Typography } from 'antd';
import './frontend.css';
import { MinusCircleOutlined, PlusOutlined } from '@ant-design/icons';
import { useState } from 'react';
import FormItem from 'antd/es/form/FormItem';
import { useForm } from 'antd/es/form/Form';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faMinus } from '@fortawesome/free-solid-svg-icons';

export default function Frontend() {

    const [tabKey, setTabKey] = useState<string>("form");


    const [properties, setProperties] = useState<Array<any>>([]);
    const [formSetting, setFormSetting] = useState<any>([]);
    const [formCode, setFormCode] = useState<string>('');
    const [elementForm] = useForm();
    const [couldGenerateCode, setCouldGenerateCode] = useState<boolean>(true);


    const [tableColumns, setTableColumns] = useState<Array<any>>([]);

    const items = [
        { label: '表单', key: 'form' },
        { label: '表格', key: 'table' },
        { label: '设置项', key: 'setting' },
    ];

    function tabKeyChanged(key: string) {
        setTabKey(key);
        elementForm.resetFields();
        setCouldGenerateCode(true);
        setFormCode('');
        setFormSetting({});
        setProperties([]);
        setTableColumns([]);
    }

    function generateForm(values: any) {
        if (values.properties && values.properties.length > 0) {
            setCouldGenerateCode(false);
            elementForm.resetFields();
            setFormSetting(values);
            setProperties(values.properties);
        }
    }

    function generateFormCode() {

        function getCom(type: string, label: string) {
            switch (type) {
                case "string":
                    return '<Input placeholder={"请输入' + label + '"} />';
                case "textarea":
                    return '<Input.TextArea placeholder={"请输入' + label + '"} />';
                case "number":
                    return '<InputNumber style={{ width: "100%" }} placeholder={"请输入' + label + '"} />';
                case "date":
                    return '<DatePicker style={{ width: "100%" }} />';
                case "range":
                    return '<DatePicker.RangePicker />';
                case "checkbox":
                    return '<Checkbox />';
                case "radio":
                    return `
                    <Radio.Group>
                        <Radio value="a">item 1</Radio>
                        <Radio value="b">item 2</Radio>
                        <Radio value="c">item 3</Radio>
                    </Radio.Group>
                    `;
                case "switch":
                    return '<Switch />';
                case "select":
                    return '<Select ></Select>';
                case "tree":
                    return '<TreeSelect />';
                default:
                    return '';
            }
        }

        let pHtml = '';
        for (const p of properties) {
            pHtml += `
            <FormItem name="${p.name}" label="${p.label}" required="${p.isRequired ?? false}" hidden="${p.isHidden ?? false}"
                valuePropName="${(p.type === "checkbox" || p.type === "switch") ? "checked" : "value"}" rules={
                    ${p.isRequired ? ("[{ required: true, message: \"请输入" + p.label + "\" }]") : ("[]")}
                } preserve={false}>
                ${getCom(p.type, p.name)}
            </FormItem>
            `;
        }
        let formData = '';
        for (const p of properties) {
            formData += `
                ${p.name}:values["${p.name}"],`;
        }

        let result = `
    const [isLoading, setIsLoading] = useState(false);
    const [form] = useForm();

    async function formEdit() {
        let values:any = {};
        form.setFieldsValue({
            ${formData}
        });
    }

    async function formSubmit(values: any) {
        try {
            setIsLoading(true);
            let formData = {
            ${formData}
            };
        }finally{
            setIsLoading(false);
        }
    }

    return (
        <Form form={form} labelCol={{ span: ${formSetting.labelCol} }} wrapperCol={{ span: ${formSetting.wrapperCol} }} layout="${formSetting.layout}">
            ${pHtml}
            <Form.Item wrapperCol={{ offset: ${formSetting.labelCol}, span: ${formSetting.wrapperCol} }}>
                <Button type='primary' htmlType="submit" loading={isLoading}>保存</Button>
            </Form.Item>
        </Form>
    );
        `;
        setFormCode(result);
    }

    function copyFormCode() {
        const textArea = document.createElement('textarea');
        textArea.value = formCode;
        // 使text area不在viewport，同时设置不可见
        textArea.style.position = 'absolute'
        textArea.style.opacity = '0'
        textArea.style.left = '-999999px'
        textArea.style.top = '-999999px'
        document.body.appendChild(textArea)
        textArea.focus()
        textArea.select()
        document.execCommand("copy");
        document.body.removeChild(textArea);
    }

    //#region 表格
    function generateTable(values: any) {
        if (values.properties && values.properties.length > 0) {
            setCouldGenerateCode(false);
            elementForm.resetFields();
            setFormSetting(values);
            setProperties(values.properties);
            if (values && values.properties && values.properties.length > 0) {
                let columns = values.properties.map((p: any) => {
                    let data: any = {
                        title: p.label,
                        dataIndex: p.name,
                        key: p.name,
                        align: p.align,
                    };
                    if (p.width) {
                        data.width = p.width
                    }

                    return data;
                });

                if (values.noCol) {
                    columns = [
                        {
                            title: "序号",
                            dataIndex: "no",
                            key: "no",
                            width: "100px"
                        }
                    ].concat(columns);
                }
                if (values.editCol) {
                    columns = columns.concat([
                        {
                            title: "操作",
                            dataIndex: "operate",
                            key: "operate"
                        }
                    ])
                }

                setTableColumns(columns);
            }
        }
    }

    function generateTableCode() {
        let columnHtml = '';
        for (const p of properties) {
            columnHtml += `
            { title: '${p.label}',key: "${p.name}", dataIndex: "${p.name}", align: "${p.align}",${p.width ? "width:'" + p.width + "'," : ''} ${p.render ? "render: (data: any, record: any, index: any) => {}" : ""}},`;
        }
        if (formSetting.noCol) {
            columnHtml = `
            {
                title: '序号', dataIndex: "num", align: 'left', width: '100px',
                render: (data: any, record: any, index: any) => (
                    <span>{(page - 1) * size + 1 + index}</span>
                )
            },` + columnHtml;
        }
        if (formSetting.editCol) {
            columnHtml = columnHtml + `
            {
                title: '操作', key: 'operate', align: 'center', width: '130px',
                render: (text: any, record: any) => (
                    <div>
                        <RightElement identify="" child={
                            <>
                                <Tooltip title="编辑">
                                    <Button type='link' style={{ padding: '4px 6px' }} onClick={() => edit(record.id)}><FontAwesomeIcon icon={faEdit} /></Button>
                                </Tooltip>
                            </>
                        }></RightElement>
                        <RightElement identify="" child={
                            <>
                                <Tooltip title="删除">
                                    <Button type='link' style={{ padding: '4px 6px' }} onClick={() => remove(record.id)}><FontAwesomeIcon icon={faTrash} /></Button>
                                </Tooltip>
                            </>
                        }></RightElement>
                    </div>
                ),
            }
            `;
        }

        let result = `
        import { Modal, Pagination, Space, Table, Tooltip } from 'antd';
        import { useEffect, useState } from 'react';
        import { RightElement } from '../../../components/right/rightElement';
        import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
        import { faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';
        
        export default function TablePage() {
            
            const [page, setPage] = useState(1);
            const [size, setSize] = useState(10);
            const [total, setTotal] = useState(0);

            const [modalVisible, setModalVisible] = useState(false);
            const [tableData, setTableData] = useState(new Array<any>());

            const tableColumns:any = [${columnHtml}];

            useEffect(() => {

            }, []);

            async function edit(id:any){
                setModalVisible(true);
            }

            async function remove(id:any){
                Modal.confirm({
                    title: '是否删除?',
                    onOk: async () => {
                    }
                })
            }

            return (
                <>
                    <Table columns={tableColumns} dataSource={tableData} pagination={false} size="${formSetting.size}" ></Table>
                    {total > 0 &&
                        <Pagination current={page} total={total} onChange={async (p, s) => { setPage(p); setSize(s); }} showSizeChanger={false} style={{ marginTop: '10px' }}></Pagination>
                    }
                    <Modal open={modalVisible} title="" footer={null} onCancel={() => setModalVisible(false)}
                        destroyOnClose={true} maskClosable={false}>
                    </Modal>
                </>
            );
        }
        `;
        setFormCode(result);
    }

    //#endregion

    //#region 配置文件窗口

    const [title, setTitle] = useState("");

    function generateSettingFile(values: any) {
        if (values.properties && values.properties.length > 0) {
            setCouldGenerateCode(false);
            elementForm.resetFields();
            setFormSetting(values);
            setProperties(values.properties);
            setTitle(values.settingTitle);
        }

    }

    function generateSettingFormCode() {

        function getCom(type: string, label: string) {
            switch (type) {
                case "string":
                    return '<Input placeholder={"请输入' + label + '"} />';
                case "textarea":
                    return '<Input.TextArea placeholder={"请输入' + label + '"} />';
                case "number":
                    return '<InputNumber style={{ width: "100%" }} placeholder={"请输入' + label + '"} />';
                case "date":
                    return '<DatePicker style={{ width: "100%" }} />';
                case "range":
                    return '<DatePicker.RangePicker />';
                case "checkbox":
                    return '<Checkbox />';
                case "radio":
                    return `
                    <Radio.Group>
                        <Radio value="a">item 1</Radio>
                        <Radio value="b">item 2</Radio>
                        <Radio value="c">item 3</Radio>
                    </Radio.Group>
                    `;
                case "switch":
                    return '<Switch />';
                case "select":
                    return '<Select ></Select>';
                case "tree":
                    return '<TreeSelect />';
                default:
                    return '';
            }
        }

        let pHtml = '';
        for (const p of properties) {
            pHtml += `
            <FormItem name="${p.name}" label="${p.label}" required={${p.isRequired ?? false}} hidden={${p.isHidden ?? false}}
                valuePropName="${(p.type === "checkbox" || p.type === "switch") ? "checked" : "value"}" rules={
                    ${p.isRequired ? ("[{ required: true, message: \"请输入" + p.label + "\" }]") : ("[]")}
                } preserve={false}>
                ${getCom(p.type, p.label)}
            </FormItem>
            `;
        }

        let nameList = '';
        for (const p of properties) {
            nameList += `\"${p.name}\",\n                `
        }
        let fieldSetList = '';
        for (const p of properties) {
            if (p.type === 'switch') {
                fieldSetList += `${p.name}: response.data.data.settings?.find(d => d.key === \"${p.name}\")?.value === 'true',\n            `
            } else {
                fieldSetList += `${p.name}: response.data.data.settings?.find(d => d.key === \"${p.name}\")?.value,\n            `
            }
        }
        let updateList = '';
        for (const p of properties) {
            updateList += `{ key: \"${p.name}\", value: values.${p.name}?.toString() },\n                    `
        }


        let result = `
import { useToken } from 'antd/es/theme/internal';
import { useForm } from 'antd/lib/form/Form';
import { Alert, Button, Card, Checkbox, DatePicker, Divider, Form, Input, InputNumber, message, Radio, Select, Space, Switch, Tooltip, Typography } from 'antd';

import { useEffect, useState } from 'react';
import FormItem from 'antd/es/form/FormItem';
import { SettingService } from '../../../../http/requests/system/setting';

export default function Setting() {

    const [_, token] = useToken();
    
    const [isLoading, setIsLoading] = useState(false);
    const [form] = useForm();

    useEffect(() => {
        initial();
    }, []);

    async function initial() {
        let response = await SettingService.GetSettings({
            keyList: [
                ${nameList}
            ]
        });
        form.setFieldsValue({
            ${fieldSetList}
        });
    }

    async function formSubmit(values: any) {
        try {
            setIsLoading(true);
            await SettingService.UpdateSetting({
                settings: [
                    ${updateList}
                ]
            });
        }finally{
            setIsLoading(false);
        }
    }

    return (<>
        <Typography.Title level={5}>${title}</Typography.Title>
        <Form form={form} labelCol={{ span: ${formSetting.labelCol} }} wrapperCol={{ span: ${formSetting.wrapperCol} }} 
                layout="${formSetting.layout}" onFinish={formSubmit}>
            ${pHtml}
            <Form.Item wrapperCol={{ offset: ${formSetting.labelCol}, span: ${formSetting.wrapperCol} }}>
                <Button type='primary' htmlType="submit" loading={isLoading}>保存</Button>
            </Form.Item>
        </Form>
    </>);
}
        `;
        setFormCode(result);
    }

    //#endregion


    return (
        <>
            <Tabs items={items} defaultActiveKey="form" onChange={tabKeyChanged} />
            {tabKey === "form" &&

                <div className="design-container">
                    <div className='input-area'>
                        <Form name='dynamic-form' autoComplete='off' onFinish={generateForm}>
                            <div style={{ display: 'flex' }}>
                                <Form.Item
                                    label="标签宽"
                                    name="labelCol"
                                    initialValue={6}
                                    rules={[{ required: true, message: '请输入' }]}
                                >
                                    <InputNumber style={{ width: '130px' }} />
                                </Form.Item>
                                <Form.Item
                                    label="内容宽"
                                    name="wrapperCol"
                                    initialValue={16}
                                    rules={[{ required: true, message: '请输入' }]}
                                >
                                    <InputNumber style={{ width: '130px' }} />
                                </Form.Item>
                                <Form.Item
                                    label="方向"
                                    name="layout"
                                    initialValue={'horizontal'}
                                    rules={[{ required: true, message: '请输入' }]}
                                >
                                    <Select style={{ width: '130px' }} >
                                        <Select.Option key='horizontal'>垂直</Select.Option>
                                        <Select.Option key='inline'>内联</Select.Option>
                                    </Select>
                                </Form.Item>
                            </div>
                            <Form.List name="properties">
                                {
                                    (fields, { add, remove }) => (
                                        <>
                                            {
                                                fields.map(
                                                    (field) => (
                                                        <Space key={field.key} align="baseline" style={{ display: 'flex' }}>
                                                            <Form.Item
                                                                {...field}
                                                                label="标签"
                                                                name={[field.name, 'label']}
                                                                rules={[{ required: true, message: '请输入' }]}
                                                            >
                                                                <Input style={{ width: '130px' }} />
                                                            </Form.Item>
                                                            <Form.Item
                                                                {...field}
                                                                label="名称"
                                                                name={[field.name, 'name']}
                                                                rules={[{ required: true, message: '请输入' }]}
                                                            >
                                                                <Input style={{ width: '130px' }} />
                                                            </Form.Item>
                                                            <Form.Item
                                                                {...field}
                                                                label="类型"
                                                                name={[field.name, 'type']}
                                                                rules={[{ required: true, message: '请输入' }]}
                                                            >
                                                                <Select style={{ width: '130px' }} >
                                                                    <Select.Option key="string">文本</Select.Option>
                                                                    <Select.Option key="textarea">多行文本</Select.Option>
                                                                    <Select.Option key="number">数字</Select.Option>
                                                                    <Select.Option key="date">日期</Select.Option>
                                                                    <Select.Option key="range">日期范围</Select.Option>
                                                                    <Select.Option key="checkbox">复选框</Select.Option>
                                                                    <Select.Option key="radio">单选框</Select.Option>
                                                                    <Select.Option key="switch">开关</Select.Option>
                                                                    <Select.Option key="select">选择</Select.Option>
                                                                    <Select.Option key="tree">树</Select.Option>
                                                                </Select>
                                                            </Form.Item>
                                                            <Form.Item name={[field.name, 'isHidden']} valuePropName="checked">
                                                                <Checkbox>隐藏</Checkbox>
                                                            </Form.Item>
                                                            <Form.Item name={[field.name, 'isRequired']} valuePropName="checked">
                                                                <Checkbox>必须</Checkbox>
                                                            </Form.Item>
                                                            <Button icon={<FontAwesomeIcon icon={faMinus}></FontAwesomeIcon>} onClick={() => remove(field.name)}></Button>
                                                        </Space>
                                                    )
                                                )
                                            }
                                            <Form.Item>
                                                <Button type="dashed" onClick={() => add()} block >
                                                    添加字段
                                                </Button>
                                            </Form.Item>
                                            <Form.Item>
                                                <div>
                                                    <Button type="primary" htmlType='submit' style={{ marginRight: '8px' }} >
                                                        生成表单
                                                    </Button>
                                                    <Button disabled={couldGenerateCode} onClick={generateFormCode} type="primary" htmlType='button' style={{ marginRight: '8px' }}  >
                                                        生成代码
                                                    </Button>
                                                    <Button disabled={couldGenerateCode} onClick={copyFormCode} type="primary" htmlType='button' style={{ marginRight: '8px' }}  >
                                                        复制代码
                                                    </Button>
                                                </div>
                                            </Form.Item>
                                        </>
                                    )
                                }
                            </Form.List>
                        </Form>
                    </div>
                    <div className='output-area'>
                        <Form form={elementForm} labelCol={{ span: formSetting.labelCol }}
                            wrapperCol={{ span: formSetting.wrapperCol }} layout={formSetting.layout}>
                            {
                                properties.map(p =>
                                (
                                    <>
                                        <FormItem name={p.name} label={p.label} required={p.isRequired} hidden={p.isHidden}
                                            valuePropName={p.type === "checkbox" || "switch" ? "checked" : "value"} rules={
                                                p.isRequired ?
                                                    [
                                                        { required: true, message: "请输入" + p.label },
                                                    ] : []
                                            }>
                                            {p.type === "string" &&
                                                <Input placeholder={"请输入" + p.label} />
                                            }
                                            {p.type === "textarea" &&
                                                <Input.TextArea placeholder={"请输入" + p.label} />
                                            }
                                            {p.type === "number" &&
                                                <InputNumber style={{ width: "100%" }} placeholder={"请输入" + p.label} />
                                            }
                                            {p.type === "date" &&
                                                <DatePicker style={{ width: "100%" }} />
                                            }
                                            {p.type === "range" &&
                                                <DatePicker.RangePicker />
                                            }
                                            {p.type === "checkbox" &&
                                                <Checkbox />
                                            }
                                            {p.type === "radio" &&
                                                <Radio.Group>
                                                    <Radio value="a">item 1</Radio>
                                                    <Radio value="b">item 2</Radio>
                                                    <Radio value="c">item 3</Radio>
                                                </Radio.Group>
                                            }
                                            {p.type === "switch" &&
                                                <Switch />
                                            }
                                            {p.type === "select" &&
                                                <Select ></Select>
                                            }
                                            {p.type === "tree" &&
                                                <TreeSelect />
                                            }
                                        </FormItem>
                                    </>
                                )
                                )
                            }
                            {properties.length > 0 &&
                                <Form.Item wrapperCol={{ offset: formSetting.labelCol, span: formSetting.wrapperCol }}>
                                    <Button type='primary' htmlType="submit">保存</Button>
                                </Form.Item>
                            }
                        </Form>
                    </div>
                    <div id="code-area">
                        <pre>
                            {formCode}
                        </pre>
                    </div>
                </div>
            }
            {tabKey === "table" &&

                <div className="design-container">
                    <div className='input-area'>
                        <Form name='dynamic-form' autoComplete='off' onFinish={generateTable}>
                            <div style={{ display: 'flex' }}>
                                <Form.Item
                                    label="checkBox列"
                                    name="checkCol"
                                    initialValue={true}
                                    valuePropName="checked"
                                >
                                    <Checkbox style={{ marginRight: '8px' }} />
                                </Form.Item>
                                <Form.Item
                                    label="序号列"
                                    name="noCol"
                                    initialValue={true}
                                    valuePropName="checked"
                                >
                                    <Checkbox style={{ marginRight: '8px' }} />
                                </Form.Item>
                                <Form.Item
                                    label="操作列"
                                    name="editCol"
                                    initialValue={true}
                                    valuePropName="checked"
                                >
                                    <Checkbox style={{ marginRight: '8px' }} />
                                </Form.Item>
                                <Form.Item
                                    label="大小"
                                    name="size"
                                    initialValue={"large"}
                                >
                                    <Select>
                                        <Select.Option key="large">大</Select.Option>
                                        <Select.Option key="middle">中</Select.Option>
                                        <Select.Option key="small">小</Select.Option>
                                    </Select>
                                </Form.Item>
                            </div>
                            <Form.List name="properties">
                                {
                                    (fields, { add, remove }) => (
                                        <>
                                            {
                                                fields.map(
                                                    (field) => (
                                                        <Space key={field.key} align="baseline" style={{ display: 'flex' }}>
                                                            <Form.Item
                                                                {...field}
                                                                label="标签"
                                                                name={[field.name, 'label']}
                                                                rules={[{ required: true, message: '请输入' }]}
                                                            >
                                                                <Input style={{ width: '130px' }} />
                                                            </Form.Item>
                                                            <Form.Item
                                                                {...field}
                                                                label="名称"
                                                                name={[field.name, 'name']}
                                                                rules={[{ required: true, message: '请输入' }]}
                                                            >
                                                                <Input style={{ width: '130px' }} />
                                                            </Form.Item>
                                                            <Form.Item
                                                                {...field}
                                                                label="对齐"
                                                                name={[field.name, 'align']}
                                                                initialValue="center"
                                                            >
                                                                <Select style={{ width: '130px' }} >
                                                                    <Select.Option key="left">左对齐</Select.Option>
                                                                    <Select.Option key="center">居中</Select.Option>
                                                                    <Select.Option key="right">右对齐</Select.Option>
                                                                </Select>
                                                            </Form.Item>
                                                            <Form.Item name={[field.name, 'width']} label="宽度">
                                                                <Input style={{ width: '130px' }} />
                                                            </Form.Item>
                                                            <Form.Item name={[field.name, 'render']} initialValue={false} valuePropName="checked">
                                                                <Checkbox>自定义渲染</Checkbox>
                                                            </Form.Item>

                                                            <Button icon={<FontAwesomeIcon icon={faMinus}></FontAwesomeIcon>} onClick={() => remove(field.name)}></Button>
                                                        </Space>
                                                    )
                                                )
                                            }
                                            <Form.Item>
                                                <Button type="dashed" onClick={() => add()} block >
                                                    添加字段
                                                </Button>
                                            </Form.Item>
                                            <Form.Item>
                                                <div>
                                                    <Button type="primary" htmlType='submit' style={{ marginRight: '8px' }} >
                                                        生成表格
                                                    </Button>
                                                    <Button disabled={couldGenerateCode} onClick={generateTableCode} type="primary" htmlType='button' style={{ marginRight: '8px' }}  >
                                                        生成代码
                                                    </Button>
                                                    <Button disabled={couldGenerateCode} onClick={copyFormCode} type="primary" htmlType='button' style={{ marginRight: '8px' }}  >
                                                        复制代码
                                                    </Button>
                                                </div>
                                            </Form.Item>
                                        </>
                                    )
                                }
                            </Form.List>
                        </Form>
                    </div>
                    <div className='output-area'>
                        {properties.length > 0 &&
                            <>
                                {formSetting.checkCol &&
                                    <Table rowSelection={{ type: 'checkbox' }} columns={tableColumns} size={formSetting.size}></Table>
                                }
                                {!formSetting.checkCol &&
                                    <Table columns={tableColumns} size={formSetting.size}></Table>
                                }
                            </>
                        }

                    </div>
                    <div id="code-area">
                        <pre>
                            {formCode}
                        </pre>
                    </div>
                </div>

            }
            {tabKey === "setting" &&
                <div className="design-container">
                    <div className='input-area'>
                        <Form name='dynamic-form' autoComplete='off' onFinish={generateSettingFile}>
                            <div style={{ display: 'flex' }}>
                                <Form.Item
                                    label="配置标题"
                                    name="settingTitle"
                                    initialValue={''}
                                    rules={[{ required: true, message: '请输入' }]}
                                >
                                    <Input style={{ width: '130px' }} />
                                </Form.Item>
                                <Form.Item
                                    label="标签宽"
                                    name="labelCol"
                                    initialValue={6}
                                    rules={[{ required: true, message: '请输入' }]}
                                >
                                    <InputNumber style={{ width: '130px' }} />
                                </Form.Item>
                                <Form.Item
                                    label="内容宽"
                                    name="wrapperCol"
                                    initialValue={16}
                                    rules={[{ required: true, message: '请输入' }]}
                                >
                                    <InputNumber style={{ width: '130px' }} />
                                </Form.Item>
                                <Form.Item
                                    label="方向"
                                    name="layout"
                                    initialValue={'horizontal'}
                                    rules={[{ required: true, message: '请输入' }]}
                                >
                                    <Select style={{ width: '130px' }} >
                                        <Select.Option key='horizontal'>垂直</Select.Option>
                                        <Select.Option key='inline'>内联</Select.Option>
                                    </Select>
                                </Form.Item>
                            </div>
                            <Form.List name="properties">
                                {
                                    (fields, { add, remove }) => (
                                        <>
                                            {
                                                fields.map(
                                                    (field) => (
                                                        <Space key={field.key} align="baseline" style={{ display: 'flex' }}>
                                                            <Form.Item
                                                                {...field}
                                                                label="标签"
                                                                name={[field.name, 'label']}
                                                                rules={[{ required: true, message: '请输入' }]}
                                                            >
                                                                <Input style={{ width: '130px' }} />
                                                            </Form.Item>
                                                            <Form.Item
                                                                {...field}
                                                                label="名称"
                                                                name={[field.name, 'name']}
                                                                rules={[{ required: true, message: '请输入' }]}
                                                            >
                                                                <Input style={{ width: '130px' }} />
                                                            </Form.Item>
                                                            <Form.Item
                                                                {...field}
                                                                label="类型"
                                                                name={[field.name, 'type']}
                                                                rules={[{ required: true, message: '请输入' }]}
                                                            >
                                                                <Select style={{ width: '130px' }} >
                                                                    <Select.Option key="string">文本</Select.Option>
                                                                    <Select.Option key="textarea">多行文本</Select.Option>
                                                                    <Select.Option key="number">数字</Select.Option>
                                                                    <Select.Option key="date">日期</Select.Option>
                                                                    <Select.Option key="range">日期范围</Select.Option>
                                                                    <Select.Option key="checkbox">复选框</Select.Option>
                                                                    <Select.Option key="radio">单选框</Select.Option>
                                                                    <Select.Option key="switch">开关</Select.Option>
                                                                    <Select.Option key="select">选择</Select.Option>
                                                                    <Select.Option key="tree">树</Select.Option>
                                                                </Select>
                                                            </Form.Item>
                                                            <Form.Item name={[field.name, 'isHidden']} valuePropName="checked">
                                                                <Checkbox>隐藏</Checkbox>
                                                            </Form.Item>
                                                            <Form.Item name={[field.name, 'isRequired']} valuePropName="checked">
                                                                <Checkbox>必须</Checkbox>
                                                            </Form.Item>
                                                            <Button icon={<FontAwesomeIcon icon={faMinus}></FontAwesomeIcon>} onClick={() => remove(field.name)}></Button>
                                                        </Space>
                                                    )
                                                )
                                            }
                                            <Form.Item>
                                                <Button type="dashed" onClick={() => add()} block >
                                                    添加字段
                                                </Button>
                                            </Form.Item>
                                            <Form.Item>
                                                <div>
                                                    <Button type="primary" htmlType='submit' style={{ marginRight: '8px' }} >
                                                        生成配置页面
                                                    </Button>
                                                    <Button disabled={couldGenerateCode} onClick={generateSettingFormCode} type="primary" htmlType='button' style={{ marginRight: '8px' }}  >
                                                        生成代码
                                                    </Button>
                                                    <Button disabled={couldGenerateCode} onClick={copyFormCode} type="primary" htmlType='button' style={{ marginRight: '8px' }}  >
                                                        复制代码
                                                    </Button>
                                                </div>
                                            </Form.Item>
                                        </>
                                    )
                                }
                            </Form.List>
                        </Form>
                    </div>
                    <div className='output-area'>
                        <Form form={elementForm} labelCol={{ span: formSetting.labelCol }}
                            wrapperCol={{ span: formSetting.wrapperCol }} layout={formSetting.layout}>

                            <Typography.Title level={5}>{title}</Typography.Title>
                            {
                                properties.map(p =>
                                (
                                    <>
                                        <FormItem name={p.name} label={p.label} required={p.isRequired} hidden={p.isHidden}
                                            valuePropName={p.type === "checkbox" || "switch" ? "checked" : "value"} rules={
                                                p.isRequired ?
                                                    [
                                                        { required: true, message: "请输入" + p.label },
                                                    ] : []
                                            }>
                                            {p.type === "string" &&
                                                <Input placeholder={"请输入" + p.label} />
                                            }
                                            {p.type === "textarea" &&
                                                <Input.TextArea placeholder={"请输入" + p.label} />
                                            }
                                            {p.type === "number" &&
                                                <InputNumber style={{ width: "100%" }} placeholder={"请输入" + p.label} />
                                            }
                                            {p.type === "date" &&
                                                <DatePicker style={{ width: "100%" }} />
                                            }
                                            {p.type === "range" &&
                                                <DatePicker.RangePicker />
                                            }
                                            {p.type === "checkbox" &&
                                                <Checkbox />
                                            }
                                            {p.type === "radio" &&
                                                <Radio.Group>
                                                    <Radio value="a">item 1</Radio>
                                                    <Radio value="b">item 2</Radio>
                                                    <Radio value="c">item 3</Radio>
                                                </Radio.Group>
                                            }
                                            {p.type === "switch" &&
                                                <Switch />
                                            }
                                            {p.type === "select" &&
                                                <Select ></Select>
                                            }
                                            {p.type === "tree" &&
                                                <TreeSelect />
                                            }
                                        </FormItem>
                                    </>
                                )
                                )
                            }
                            {properties.length > 0 &&
                                <Form.Item wrapperCol={{ offset: formSetting.labelCol, span: formSetting.wrapperCol }}>
                                    <Button type='primary' htmlType="submit">保存</Button>
                                </Form.Item>
                            }
                        </Form>
                    </div>
                    <div id="code-area">
                        <pre>
                            {formCode}
                        </pre>
                    </div>
                </div>
            }
        </>
    );
}