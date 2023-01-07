import { faPlus } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Button, Form, Input, Upload, Image, Divider } from 'antd';
import { useToken } from 'antd/es/theme/internal';
import { useForm } from 'antd/lib/form/Form';
import { RcFile, UploadFile } from 'antd/lib/upload/interface';
import { useEffect, useState } from 'react';
import { Configuration } from '../../../common/config';
import { SettingService } from '../../../http/requests/setting';
import './setting.css';

export default function Setting() {

    const token = useToken();
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

        await SettingService.saveLoginPageSetting(formData);
        await loadLoginSetting();
    }

    function selectSetting(index: number) {
        setSelectItem(index);
    }

    function selectClass(index: number) {
        return selectItem === index ? 'li-selected' : '';
    }

    useEffect(() => {
        if (selectItem === 0) {
            loadLoginSetting();
        }
    }, [selectItem]);// eslint-disable-line react-hooks/exhaustive-deps

    async function loadLoginSetting() {
    }

    return (
        <>
            <div id="setting-container">
                <div id="setting-item-container">
                    <ul>
                        <li onClick={() => selectSetting(0)} style={{
                            borderLeft: selectItem === 0 ? '5px solid ' + token[1].colorPrimary : ""
                        }}>通用设置</li>
                    </ul>
                </div>
                <Divider type='vertical' style={{ height: '100%' }} />
                <div id="setting-detail-container">
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