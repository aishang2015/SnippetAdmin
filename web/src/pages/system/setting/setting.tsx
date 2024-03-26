
import { Checkbox, DatePicker, Divider, Input, InputNumber, message, Radio, Select, Switch, Typography } from 'antd';
import { useToken } from 'antd/es/theme/internal';
import { useForm } from 'antd/lib/form/Form';
import { RcFile, UploadFile } from 'antd/lib/upload/interface';
import { useEffect, useState } from 'react';
import { GetSettingGroupsOutputModel, GetSettingsOutputModel, SettingModel, SettingService } from '../../../http/requests/system/setting';
import './setting.css';
import dayjs from 'dayjs';

import 'dayjs/locale/zh-cn';
import locale from 'antd/locale/zh_CN';

export default function Setting() {

    const [_,token] = useToken();
    const [selectItem, setSelectItem] = useState<number>(0);

    const [iconFileList, setIconFileList] = useState<UploadFile[]>([]);
    const iconProps = {
        onRemove: (file: UploadFile<any>) => {
            const index = iconFileList.indexOf(file);
            const newFileList = iconFileList.slice();
            newFileList.splice(index, 1);
            setIconFileList(newFileList);
        },
        beforeUpload: (file: UploadFile<any>) => {
            setIconFileList([file]);
            return false;
        },
        iconFileList,
    };
    const [backgroundFileList, setBackgroundFileList] = useState<UploadFile[]>([]);
    const backgroundProps = {
        onRemove: (file: UploadFile<any>) => {
            const index = backgroundFileList.indexOf(file);
            const newFileList = backgroundFileList.slice();
            newFileList.splice(index, 1);
            setBackgroundFileList(newFileList);
        },
        beforeUpload: (file: UploadFile<any>) => {
            setBackgroundFileList([file]);
            return false;
        },
        backgroundFileList,
    };
    const [backgroundUrl, setBackgroundUrl] = useState<string>('');
    const [iconUrl, setIconUrl] = useState<string>('');

    const [loginPageForm] = useForm();

    async function loginPageFormSubmit(values: any) {
        const formData = new FormData();
        formData.append('background', backgroundFileList.length > 0 ? backgroundFileList.at(0) as RcFile : '');
        formData.append('icon', iconFileList.length > 0 ? iconFileList.at(0) as RcFile : '');
        formData.append('title', values['title']);

        //await SettingService.saveLoginPageSetting(formData);
        await loadLoginSetting();
    }

    async function selectSetting(index: number, groupCode: string) {
        setSelectItem(index);
        let settingsResponse = await SettingService.GetSettings({ groupCode: groupCode });
        setSettings(settingsResponse.data.data);
    }

    function selectClass(index: number) {
        return selectItem === index ? 'li-selected' : '';
    }

    const [groups, setGroups] = useState<Array<GetSettingGroupsOutputModel>>([]);
    const [settings, setSettings] = useState<Array<GetSettingsOutputModel>>([]);

    useEffect(() => {
        if (selectItem === 0) {
            loadLoginSetting();
        }
    }, [selectItem]);// eslint-disable-line react-hooks/exhaustive-deps

    async function loadLoginSetting() {

        let response = await SettingService.GetSettingGroups();
        setGroups(response.data.data);

        if (response.data.data && response.data.data.length > 0) {
            let firstGroup = response.data.data[0];
            let settingsResponse = await SettingService.GetSettings({ groupCode: firstGroup.code });
            setSettings(settingsResponse.data.data);
        }
    }

    async function saveSettingValue(settingCode: string, settingValue: string) {
        await SettingService.SaveSetting({
            code: settingCode,
            value: settingValue
        });
    }

    function getEditComponent(setting: SettingModel) {
        if (setting.inputType === 1) {
            return <Input style={{ minWidth: '230px' }} defaultValue={setting.value!}
                onPressEnter={async (event) => {
                    let value = (event.target as any).value;
                    if (!value) {
                        await message.warning("配置值不能为空");
                        return;
                    }
                    if (setting.max && value.length > setting.max) {
                        await message.warning(`最大${setting.max}位`);
                        return;
                    }
                    if (setting.min && value.length < setting.min) {
                        await message.warning(`最少需要${setting.min}位`);
                        return;
                    }
                    if(setting.regex && !RegExp(setting.regex).test(value)){
                        await message.warning(`不匹配表达式${setting.regex}`);
                        return;
                    }
                    await saveSettingValue(setting.code!, value);
                }} />
        } else if (setting.inputType === 2) {
            return <InputNumber style={{ minWidth: '230px' }} defaultValue={setting.value!}
                onPressEnter={async (event) => {
                    let value = (event.target as any).value;
                    if (!value) {
                        await message.warning("配置值不能为空");
                        return;
                    }
                    if (setting.max && value > setting.max) {
                        await message.warning(`最大值为${setting.max}`);
                        return;
                    }
                    if (setting.min && value < setting.min) {
                        await message.warning(`最小值为${setting.min}`);
                        return;
                    }
                    await saveSettingValue(setting.code!, value);
                }} />
        } else if (setting.inputType === 3) {
            return <Input.TextArea style={{ minWidth: '230px' }} defaultValue={setting.value!} onBlur={async (event) => {
                let value = (event.target as any).value;
                if (!value) {
                    await message.warning("配置值不能为空");
                    return;
                }
                if (setting.max && value.length > setting.max) {
                    await message.warning(`最大${setting.max}位`);
                    return;
                }
                if (setting.min && value.length < setting.min) {
                    await message.warning(`最少需要${setting.min}位`);
                    return;
                }
                if(setting.regex && !RegExp(setting.regex).test(value)){
                    await message.warning(`不匹配表达式${setting.regex}`);
                    return;
                }
                await saveSettingValue(setting.code!, value);
            }} />
        } else if (setting.inputType === 4) {
            return <Switch defaultChecked={setting.value == "true" ? true : false} onChange={async (value) => {

                await saveSettingValue(setting.code!, value.toString());
            }} />
        } else if (setting.inputType === 5) {
            let options = setting.options?.split(',');
            return (
                <Radio.Group defaultValue={setting.value!} onChange={async (value) => {
                    await saveSettingValue(setting.code!, value.target.value);
                }}>
                    {options?.map(o => <Radio.Button value={o}>{o}</Radio.Button>)}
                </Radio.Group>)
        } else if (setting.inputType === 6) {
            let options = setting.options?.split(',');
            let checkedValues = setting.value?.split(',');
            return (
                <Checkbox.Group defaultValue={checkedValues} onChange={async (values) => {
                    let value = values.join(',');
                    await saveSettingValue(setting.code!, value);
                }}>
                    {options?.map(o => <Checkbox value={o}>{o}</Checkbox>)}
                </Checkbox.Group>)
        } else if (setting.inputType === 7) {
            let options = setting.options?.split(',');
            return (
                <Select style={{ minWidth: '230px' }} defaultValue={setting.value!} onChange={async (value) => {
                    await saveSettingValue(setting.code!, value);
                }}>
                    {options?.map(o => <Select.Option value={o}>{o}</Select.Option>)}
                </Select>)

        } else if (setting.inputType === 8) {
            return <DatePicker defaultValue={dayjs(setting.value)} style={{ minWidth: '230px' }} onChange={async (value) => {
                let str = value?.format();
                await saveSettingValue(setting.code!, str!);
            }} />
        } else if (setting.inputType === 9) {
            return <DatePicker defaultValue={dayjs(setting.value)} showTime style={{ minWidth: '230px' }} onChange={async (value) => {
                let str = value?.format();
                await saveSettingValue(setting.code!, str!);
            }} />
        }
    }

    return (
        <>
            <div id="setting-container">
                <div id="setting-item-container">
                    <ul>
                        {groups.map((g, i) => (
                            <li key={i.toString()} onClick={() => selectSetting(i, g.code!)} style={{
                                borderLeft: selectItem === i ? '5px solid ' + token.colorPrimary : ""
                            }}>{g.name}</li>
                        ))}
                    </ul>
                </div>
                <Divider type='vertical' style={{ height: '100%' }} />
                <div id="setting-detail-container">
                    {settings.map((s, i) => (
                        <>
                            <Typography.Title level={4}>{s.name}</Typography.Title>
                            {s.describe &&
                                <Typography.Text type='secondary'>{s.name}</Typography.Text>
                            }
                            {s.settings?.map(
                                (setting, index) => (
                                    <>
                                        <div style={{
                                            display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start',
                                            marginTop: '10px'
                                        }}>
                                            <div >
                                                <div>{setting.name}</div>
                                                {setting.describe &&
                                                    <div>
                                                        <Typography.Text type='secondary'>{setting.describe}</Typography.Text>
                                                    </div>
                                                }
                                            </div>
                                            <div>
                                                {getEditComponent(setting)}
                                            </div>
                                        </div>
                                    </>
                                )
                            )
                            }
                            <Divider />
                        </>
                    ))

                    }

                    {selectItem === 0 &&
                        <>
                            {/* 此配置不再使用
                                <Form form={loginPageForm} onFinish={loginPageFormSubmit} layout='vertical'>
                                <Form.Item name="title" label="网站标题" required rules={
                                    [
                                        { required: true, message: "请输入网站标题" },
                                        { max: 30, message: "网站标题过长" },
                                    ]
                                }>
                                    <Input placeholder='请输入网站标题' />
                                </Form.Item>
                                {iconUrl !== '' && iconUrl !== null && iconUrl !== undefined &&
                                    <Image width={100} src={iconUrl} />
                                }
                                <Form.Item label="系统图标">
                                    <Upload {...iconProps}>
                                        <Button icon={<FontAwesomeIcon icon={faPlus} fixedWidth />}>选择文件</Button>
                                    </Upload>
                                </Form.Item>
                                {backgroundUrl !== '' && backgroundUrl !== null && backgroundUrl !== undefined &&
                                    <Image width={400} src={backgroundUrl} />
                                }
                                <Form.Item label="登录页背景">
                                    <Upload {...backgroundProps}>
                                        <Button icon={<FontAwesomeIcon icon={faPlus} fixedWidth />}>选择文件</Button>
                                    </Upload>
                                </Form.Item>
                                <Form.Item>
                                    <Button type='primary' htmlType="submit" >保存</Button>
                                </Form.Item>
                            </Form> */}
                        </>
                    }
                </div>
            </div>
        </>
    );
}