import { Button, Divider, Form, message, Select } from "antd";
import { useEffect, useState } from "react";
import { CodeService } from "../../../http/requests/develop/code";
import MonacoEditor from 'react-monaco-editor';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCode, faCopy } from "@fortawesome/free-solid-svg-icons";

export default function CodePage() {

    const [controllers, setControllers] = useState<Array<string>>([]);
    const [editorValue, seteditorValue] = useState('');

    useEffect(() => {
        initData();
    }, []);

    // !初始化
    async function initData() {
        let response = await CodeService.GetControllers();
        setControllers(response.data.data);
    }

    // !表单提交
    async function codeGenerateFormSubmit(values: any) {
        let response = await CodeService.GetTsRequestCode({
            controllerName: values['controller']
        });
        seteditorValue(response.data.data.requestCode ?? '');
    }

    // !复制
    async function copy() {
        let textNode = document.createElement('textarea');
        textNode.value = editorValue;
        textNode.setAttribute('id', 'copyEle');
        textNode.setAttribute('class', 'allow-copy');
        textNode.setAttribute('style', 'width: 0px; height: 0px; z-index: 0; position: fixed;');
        document.getElementsByTagName('body')[0].appendChild(textNode);
        (document.getElementById('copyEle') as any).select();
        document.execCommand("Copy");
        document.getElementsByTagName('body')[0].removeChild(textNode);
        await message.success("复制成功！");
    }

    return (
        <>
            <div style={{ display: "flex" }}>

                <Form layout="inline" onFinish={codeGenerateFormSubmit}>
                    <Form.Item label="控制器" name="controller" required
                        rules={[{ required: true, message: '请选择控制器!' }]}>
                        <Select style={{ width: '180px' }} placeholder="请选择控制器">
                            {controllers.map(c =>
                                <Select.Option key={c} value={c}>{c}</Select.Option>
                            )}
                        </Select>
                    </Form.Item>

                    <Form.Item>
                        <Button type="primary" htmlType="submit" icon={<FontAwesomeIcon icon={faCode} />} >
                            生成接口请求代码
                        </Button>
                    </Form.Item>
                </Form>
                <Button type="default" onClick={copy} icon={<FontAwesomeIcon icon={faCopy} />}>
                    复制
                </Button>
            </div>
            <Divider />
            <MonacoEditor
                height="600"
                language="javascript"
                theme="vs-dark"
                value={editorValue}
                onChange={value => seteditorValue(value)}
                options={{
                    selectOnLineNumbers: true
                }}
            />
        </>
    );
}