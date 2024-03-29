
import { useToken } from 'antd/es/theme/internal';
import { useForm } from 'antd/lib/form/Form';
import { Alert, Button, Card, Checkbox, DatePicker, Divider, Form, Input, InputNumber, message, Radio, Select, Space, Switch, Tooltip, Typography } from 'antd';

import { useEffect, useState } from 'react';
import FormItem from 'antd/es/form/FormItem';
import { SettingService } from '../../../../http/requests/system/setting';

export default function PwdSetting() {

    const [_, token] = useToken();
    
    const [isLoading, setIsLoading] = useState(false);
    const [form] = useForm();

    useEffect(() => {
        initial();
    }, []);

    async function initial() {
        let response = await SettingService.GetSettings({
            keyList: [
                "SecurityPwdContainsDigit",
                "SecurityPwdContainsUpperChar",
                "SecurityPwdContainsLowerChar",
                "SecurityPwdContainsSpecialChar",
                "SecurityPwdLength",
                
            ]
        });
        form.setFieldsValue({
            SecurityPwdContainsDigit: response.data.data.settings?.find(d => d.key === "SecurityPwdContainsDigit")?.value === 'true',
            SecurityPwdContainsUpperChar: response.data.data.settings?.find(d => d.key === "SecurityPwdContainsUpperChar")?.value === 'true',
            SecurityPwdContainsLowerChar: response.data.data.settings?.find(d => d.key === "SecurityPwdContainsLowerChar")?.value === 'true',
            SecurityPwdContainsSpecialChar: response.data.data.settings?.find(d => d.key === "SecurityPwdContainsSpecialChar")?.value === 'true',
            SecurityPwdLength: response.data.data.settings?.find(d => d.key === "SecurityPwdLength")?.value,
            
        });
    }

    async function formSubmit(values: any) {
        try {
            setIsLoading(true);
            await SettingService.UpdateSetting({
                settings: [
                    { key: "SecurityPwdContainsDigit", value: values.SecurityPwdContainsDigit?.toString() },
                    { key: "SecurityPwdContainsUpperChar", value: values.SecurityPwdContainsUpperChar?.toString() },
                    { key: "SecurityPwdContainsLowerChar", value: values.SecurityPwdContainsLowerChar?.toString() },
                    { key: "SecurityPwdContainsSpecialChar", value: values.SecurityPwdContainsSpecialChar?.toString() },
                    { key: "SecurityPwdLength", value: values.SecurityPwdLength?.toString() },
                ]
            });
        }finally{
            setIsLoading(false);
        }
    }

    return (<>
        <Typography.Title level={5}>密码复杂度配置</Typography.Title>
        <Form form={form} labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} layout="horizontal" onFinish={formSubmit}>
            
            <FormItem name="SecurityPwdContainsDigit" label="是否包含数字" required={false} hidden={false}
                valuePropName="checked" rules={
                    []
                } preserve={false}>
                <Switch />
            </FormItem>
            
            <FormItem name="SecurityPwdContainsUpperChar" label="是否包含大写字母" required={false} hidden={false}
                valuePropName="checked" rules={
                    []
                } preserve={false}>
                <Switch />
            </FormItem>
            
            <FormItem name="SecurityPwdContainsLowerChar" label="是否包含小写字母" required={false} hidden={false}
                valuePropName="checked" rules={
                    []
                } preserve={false}>
                <Switch />
            </FormItem>
            
            <FormItem name="SecurityPwdContainsSpecialChar" label="是否包含特殊字符" required={false} hidden={false}
                valuePropName="checked" rules={
                    []
                } preserve={false}>
                <Switch />
            </FormItem>
            
            <FormItem name="SecurityPwdLength" label="密码长度" required={false} hidden={false}
                valuePropName="value" rules={
                    []
                } preserve={false}>
                <InputNumber style={{ width: "100%" }} placeholder={"请输入SecurityPwdLength"} />
            </FormItem>
            
            <Form.Item wrapperCol={{ offset: 6, span: 16 }}>
                <Button type='primary' htmlType="submit" loading={isLoading}>保存</Button>
            </Form.Item>
        </Form>
    </>);
}
        