
import { Button, Checkbox, DatePicker, Form, Input, InputNumber, Radio, Select, Space, Switch, Tabs, TreeSelect } from 'antd';
import './frontend.css';
import { MinusCircleOutlined, PlusOutlined } from '@ant-design/icons';
import { useState } from 'react';
import FormItem from 'antd/es/form/FormItem';
import { useForm } from 'antd/es/form/Form';

export default function Frontend() {

    const [tabKey, setTabKey] = useState<string>("form");

    const [properties, setProperties] = useState<Array<any>>([]);
    const [formSetting, setFormSetting] = useState<any>([]);
    const [formCode, setFormCode] = useState<string>('');
    const [elementForm] = useForm();

    const items = [
        { label: '表单', key: 'form' },
    ];

    function tabKeyChanged(key: string) {
        setTabKey(key);
    }

    function generateForm(values: any) {
        if (values.properties && values.properties.length > 0) {
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
        <Form form={form} labelCol={{ span: ${formSetting.labelCol} }} wrapperCol={{ span: ${formSetting.wrapperCol} }}>
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
                                                            <MinusCircleOutlined onClick={() => remove(field.name)} />
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
                                                <Button type="primary" htmlType='submit' block >
                                                    生成表单
                                                </Button>
                                            </Form.Item>
                                        </>
                                    )
                                }
                            </Form.List>
                        </Form>
                    </div>
                    <div className='output-area'>
                        <Form form={elementForm} labelCol={{ span: formSetting.labelCol }}
                            wrapperCol={{ span: formSetting.wrapperCol }}>
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
                        {properties.length > 0 &&
                            <>
                                <Button type="primary" style={{ marginRight: '10px' }} onClick={generateFormCode}  >
                                    生成表单的代码
                                </Button>
                                <Button type="primary" onClick={copyFormCode}  >
                                    复制代码
                                </Button>
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
        </>
    );
}