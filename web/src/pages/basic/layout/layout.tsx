import { faChessKnight, faSave } from "@fortawesome/free-regular-svg-icons";
import { faCircleLeft, faCircleRight, faEdit, faMoon, faOutdent, faPlus, faSun, faTruckLoading, faUser } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Avatar, Button, Card, Divider, Dropdown, Form, Input, Layout, Menu, MenuProps, message, Modal, Popover, Space, Switch, Upload } from "antd";
import { useForm } from "antd/es/form/Form";
import { Content, Header } from "antd/es/layout/layout";
import Sider from "antd/es/layout/Sider";
import { useToken } from "antd/es/theme/internal";
import { RcFile, UploadChangeParam, UploadFile } from "antd/es/upload";
import { useEffect, useState } from "react";
import { CirclePicker, ColorResult } from 'react-color';
import { Link, Outlet } from "react-router-dom";
import { Configuration } from "../../../common/config";
import { Constants } from "../../../common/constants";
import { StorageService } from "../../../common/storage";
import { getUserInfo, modifyPassword, updateUserInfo, UserInfoResult } from "../../../http/requests/account";
import { RefreshService } from "../../../service/refreshService";

import './layout.css';

interface IBasicLayout {
    onColorChange(color: string): void;
    onThemeChange(color: string): void;
}

export default function BasicLayout({ onColorChange, onThemeChange }: IBasicLayout) {

    type MenuItem = Required<MenuProps>['items'][number];

    const [collapsed, setCollapsed] = useState(false);
    const [userInfo, setUserInfo] = useState<UserInfoResult>();
    const [avatar, setAvatar] = useState<string | null>(null);
    const [realName, setRealName] = useState<string>('');

    const [primaryColor, setPrimaryColor] = useState<string>('');
    const [theme, setTheme] = useState<string>('');

    const [_, token] = useToken();
    const [userSettingModalVisible, setUserSettingModalVisible] = useState(false);
    const [settingMenuIndex, setSettingMenuIndex] = useState<number>(1);

    const [isLoading, setIsLoading] = useState(false);
    const [imageUrl, setImageUrl] = useState<string>();


    const [pwdEditForm] = useForm();
    const [infoEditForm] = useForm();

    const menus: MenuProps['items'] = [
        {
            label:
                <Button type='link' style={{ padding: '4px 6px' }} onClick={() => userSetting()}>
                    <Space><FontAwesomeIcon icon={faEdit} fixedWidth />个人设置</Space>
                </Button>
            , key: '1'
        }, // 菜单项务必填写 key
        {
            label:
                <Button type='link' style={{ padding: '4px 6px' }} onClick={() => logout()}>
                    <Space><FontAwesomeIcon icon={faOutdent} fixedWidth />注销</Space>
                </Button>
            , key: '2'
        },
    ];

    useEffect(() => {
        init();
        let color = localStorage.getItem("primaryColor")!;
        setPrimaryColor(color);

        let theme = localStorage.getItem("theme")!;
        setTheme(theme);
    }, []);

    async function init() {
        await RefreshService.refreshTokenAsync();
        let info = await getUserInfo();
        setUserInfo(info.data.data);
        setAvatar(info.data.data.avatar)
        setRealName(info.data.data.realName);
    }

    function logout() {
        StorageService.clearLoginStore();
        window.location.replace('/');
    };

    async function userSetting() {
        setUserSettingModalVisible(true);
        let info = await getUserInfo();
        setUserInfo(info.data.data);
        setAvatar(info.data.data.avatar)
        setRealName(info.data.data.realName);

        if (info.data.data.avatar) {
            setImageUrl(`${Configuration.BaseUrl}/store/${info.data.data.avatar}`);
        }
    }

    // 提交用户密码信息
    async function pwdSubmit(values: any) {
        try {
            if (values.pwd !== values.confirmPwd) {
                message.warning("两次密码输入不一致，请重新输入");
                return;
            }

            setIsLoading(true);
            await modifyPassword({
                oldPassword: values.oldpwd,
                newPassword: values.pwd
            });
            pwdEditForm.resetFields();

            await userSetting();

        }
        finally {
            setIsLoading(false);
        }
    }

    // 提交用户信息
    async function infoSubmit(values: any) {
        try {
            setIsLoading(true);
            await updateUserInfo({
                phoneNumber: values['phoneNumber']
            });

            await userSetting();
        }
        finally {
            setIsLoading(false);
        }
    }

    function getItem(
        label: React.ReactNode,
        key: React.Key,
        icon?: React.ReactNode,
        children?: MenuItem[],
        type?: 'group',
    ): MenuItem {
        return {
            key,
            icon,
            children,
            label,
            type,
        } as unknown as MenuItem;
    }

    function getMenuItems() {
        let items = new Array<MenuItem>();
        let index = 0;
        for (const routeInfo of Constants.GetRouteInfo()) {
            if (!StorageService.getRights().find(right => right === routeInfo.identify) && routeInfo.identify) {
                continue;
            }
            if (routeInfo.children != undefined) {
                let childItems = new Array<MenuItem>();
                for (const child of routeInfo.children) {
                    if (!StorageService.getRights().find(right => right === child.identify) && child.identify) {
                        continue;
                    }
                    childItems.push(getItem(<Link to={child.path}>{child.name}</Link>, index++, child.icon));
                }
                items.push(getItem(routeInfo.name, index++, routeInfo.icon, childItems));
            } else {
                items.push(getItem(<Link to={routeInfo.path}>{routeInfo.name}</Link>, index++, routeInfo.icon));
            }
        }
        return items;
    }

    function handleColorChanged(color: ColorResult) {
        onColorChange(color.hex);
        setPrimaryColor(color.hex);
        localStorage.setItem("primaryColor", color.hex);
    }

    function handleThemeChange(isDark: boolean) {
        let theme = isDark ? "dark" : "light";
        onThemeChange(theme);
        setTheme(theme);
        localStorage.setItem("theme", theme);
    }

    function menuIndexChanged(index: number) {
        setSettingMenuIndex(index);

        infoEditForm.setFieldsValue({
            'phoneNumber': userInfo!.phoneNumber
        });
    }

    return (
        <>
            {/* all the other elements */}
            <Layout style={{ minHeight: '100vh', maxHeight: '100vh' }}>
                <Sider trigger={null} collapsible collapsed={collapsed} theme="dark">
                    {collapsed ?
                        <div className="logo" >Admin</div> :
                        <div className="logo large-logo-font" >SnippetAdmin</div>
                    }
                    <div style={{ display: 'flex', flexDirection: 'column', justifyContent: "space-between", userSelect: "none" }}>
                        <Menu theme="dark" mode="inline" defaultSelectedKeys={[localStorage.getItem('activeKey') ?? "/home"]}
                            items={getMenuItems() as any}>
                        </Menu>
                    </div>
                </Sider>
                <Layout>
                    <Header className="site-layout" style={{ padding: 0, display: 'flex', alignItems: 'center' }}>

                        <Button type="text" onClick={() => setCollapsed(!collapsed)} icon={collapsed ?
                            <FontAwesomeIcon icon={faCircleRight} style={{ color: 'white' }} /> :
                            <FontAwesomeIcon icon={faCircleLeft} style={{ color: 'white' }} />} />
                        <div style={{
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center'
                        }}>

                            <Switch style={{ marginRight: '12px', backgroundColor: primaryColor }}
                                checked={theme === "dark"}
                                defaultChecked={theme === "dark"}
                                checkedChildren={<FontAwesomeIcon icon={faMoon} />}
                                unCheckedChildren={<FontAwesomeIcon icon={faSun} />}
                                onChange={handleThemeChange} />

                            <Popover content={(
                                <CirclePicker onChangeComplete={handleColorChanged} />
                            )}>
                                <div style={{
                                    width: '20px', height: '20px', backgroundColor: primaryColor,
                                    borderRadius: '4px', marginRight: '10px'
                                }}>

                                </div>
                            </Popover>
                            {(avatar === null || avatar === '') &&
                                <Dropdown className="dropdown" menu={{ items: menus }} arrow={{ pointAtCenter: false }} trigger={['click']}>
                                    <Avatar icon={<FontAwesomeIcon icon={faUser} />} style={{ marginRight: '4px' }} />
                                </Dropdown>
                            }
                            {(avatar !== null && avatar !== '') &&
                                <Dropdown className="dropdown" menu={{ items: menus }} arrow={{ pointAtCenter: false }} trigger={['click']}>
                                    <Avatar src={`${Configuration.BaseUrl}/store/${avatar}`} style={{ marginRight: '4px' }} />
                                </Dropdown>
                            }

                            <div style={{
                                color: 'white', marginRight: '30px', userSelect: 'none', width: '80px',
                                overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap'
                            }} >{realName}</div>
                        </div>
                    </Header >
                    <Card className="screen_container" size="small">
                        <Outlet />
                    </Card>
                </Layout>
            </Layout>

            <Modal open={userSettingModalVisible} width={800} footer={null} title="用户信息设置"
                onCancel={() => setUserSettingModalVisible(false)} >
                <div style={{ display: "flex", height: "600px", overflow: "auto" }}>
                    <div style={{ width: "200px" }}>
                        <ul style={{ listStyle: "none", margin: '0', padding: '0' }}>
                            <li style={{
                                backgroundColor: settingMenuIndex === 1 ? token.colorPrimary : token.colorBgElevated,
                                color: settingMenuIndex === 1 ? "white" : token.colorText,
                                display: 'block', padding: "10px", cursor: "pointer",
                                borderRadius: '4px', transition: "all 0.3s",

                            }} onClick={() => menuIndexChanged(1)}>修改头像</li>
                            <li style={{
                                backgroundColor: settingMenuIndex === 2 ? token.colorPrimary : token.colorBgElevated,
                                color: settingMenuIndex === 2 ? "white" : token.colorText,
                                display: 'block', padding: "10px", cursor: "pointer",
                                borderRadius: '4px', transition: "all 0.3s",

                            }} onClick={() => menuIndexChanged(2)}>基本信息</li>
                            <li style={{
                                backgroundColor: settingMenuIndex === 3 ? token.colorPrimary : token.colorBgElevated,
                                color: settingMenuIndex === 3 ? "white" : token.colorText,
                                display: 'block', padding: "10px", cursor: "pointer",
                                borderRadius: '4px', transition: "all 0.3s",
                            }} onClick={() => menuIndexChanged(3)}>修改密码</li>
                        </ul>
                    </div>
                    <Divider type="vertical" style={{ height: "100%" }} />
                    <div style={{ flex: 1 }}>
                        {settingMenuIndex === 1 &&
                            <>
                                <Upload
                                    name="avatar"
                                    listType="picture-circle"
                                    className="avatar-uploader"
                                    showUploadList={false}
                                    headers={{
                                        Authorization: `Bearer ${localStorage.getItem("token")}`
                                    }}
                                    action={`${Configuration.BaseUrl}/api/account/UploadAvatar`}
                                    beforeUpload={(file: RcFile) => {
                                        const isJpgOrPng = file.type === 'image/jpeg' || file.type === 'image/png';
                                        if (!isJpgOrPng) {
                                            message.error('只能上传 JPG/PNG 文件!');
                                        }
                                        const isLt2M = file.size / 1024 / 1024 < 2;
                                        if (!isLt2M) {
                                            message.error('图片必须小于 2MB!');
                                        }
                                        return isJpgOrPng && isLt2M;
                                    }}
                                    onChange={(info: UploadChangeParam<UploadFile>) => {
                                        if (info.file.status === 'uploading') {
                                            setIsLoading(true);
                                            return;
                                        }
                                        if (info.file.status === 'done') {
                                            setImageUrl(`${Configuration.BaseUrl}/store/${info.file.response.data}`);
                                            setIsLoading(false);
                                            message.success("上传成功");
                                        }
                                    }}
                                >
                                    {imageUrl ? <img src={imageUrl} alt="avatar" style={{ width: '100%' }} /> :
                                        <div>
                                            {isLoading ? <FontAwesomeIcon icon={faTruckLoading} /> : <FontAwesomeIcon icon={faPlus} />}
                                            <div style={{ marginTop: 8 }}>上传</div>
                                        </div>
                                    }
                                </Upload>
                            </>
                        }
                        {settingMenuIndex === 2 &&
                            <>

                                <Form form={infoEditForm} onFinish={infoSubmit} labelCol={{ span: 6 }}
                                    wrapperCol={{ span: 16 }} preserve={false}>
                                    <Form.Item name="phoneNumber" label="电话" rules={
                                        [
                                            { pattern: /^[0-9-]+$/g, message: '电话只能包含数字和-' },
                                            { max: 20, message: "电话过长" },
                                        ]
                                    }>
                                        <Input autoComplete="off2" placeholder="请输入电话" />
                                    </Form.Item>
                                    <Form.Item wrapperCol={{ offset: 6 }}>
                                        <Button type='primary' icon={<FontAwesomeIcon fixedWidth icon={faSave} />} htmlType="submit" loading={isLoading}>保存</Button>
                                    </Form.Item>
                                </Form>
                            </>
                        }
                        {settingMenuIndex === 3 &&
                            <>
                                <Form form={pwdEditForm} onFinish={pwdSubmit} labelCol={{ span: 6 }}
                                    wrapperCol={{ span: 16 }} preserve={false}>

                                    <Form.Item name="oldpwd" label="旧密码" rules={
                                        [
                                            { required: true, message: "请输入密码" },
                                        ]
                                    }>
                                        <Input autoComplete="off2" placeholder="请输入旧密码" type="password" />
                                    </Form.Item>
                                    <Form.Item name="pwd" label="密码" rules={
                                        [
                                            { required: true, message: "请输入密码" },
                                            { max: 100, message: "密码过长" },
                                            { min: 4, message: "密码最小4位" },
                                        ]
                                    }>
                                        <Input autoComplete="off2" placeholder="请输入密码" type="password" />
                                    </Form.Item>
                                    <Form.Item name="confirmPwd" label="确认密码" rules={
                                        [
                                            { required: true, message: "请输入确认密码" },
                                        ]
                                    }>
                                        <Input autoComplete="off2" placeholder="请输入确认密码" type="password" />
                                    </Form.Item>
                                    <Form.Item wrapperCol={{ offset: 6 }}>
                                        <Button type='primary' icon={<FontAwesomeIcon fixedWidth icon={faSave} />} htmlType="submit" loading={isLoading}>保存</Button>
                                    </Form.Item>
                                </Form>
                            </>
                        }
                    </div>
                </div>
            </Modal>
        </>
    );
}