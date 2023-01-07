import { faChessKnight } from "@fortawesome/free-regular-svg-icons";
import { faCircleLeft, faCircleRight, faEdit, faMoon, faOutdent, faSun, faUser } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Avatar, Button, Card, Divider, Dropdown, Layout, Menu, MenuProps, Popover, Space, Switch } from "antd";
import { Content, Header } from "antd/es/layout/layout";
import Sider from "antd/es/layout/Sider";
import { useEffect, useState } from "react";
import { CirclePicker, ColorResult } from 'react-color';
import { Link, Outlet } from "react-router-dom";
import { Constants } from "../../../common/constants";
import { StorageService } from "../../../common/storage";
import { getUserInfo } from "../../../http/requests/account";
import { RefreshService } from "../../../service/refreshService";

import './layout.css';

interface IBasicLayout {
    onColorChange(color: string): void;
    onThemeChange(color: string): void;
}

export default function BasicLayout({ onColorChange, onThemeChange }: IBasicLayout) {

    type MenuItem = Required<MenuProps>['items'][number];

    const [collapsed, setCollapsed] = useState(false);
    const [realName, setRealName] = useState<string>('');

    const [primaryColor, setPrimaryColor] = useState<string>('');
    const [theme, setTheme] = useState<string>('');

    const menus: MenuProps['items'] = [
        {
            label:
                <Button type='link' style={{ padding: '4px 6px' }} >
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
        setRealName(info.data.data.realName);
    }

    function logout() {
        StorageService.clearLoginStore();
        window.location.replace('/');
    };

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

                            <Dropdown className="dropdown" menu={{ items: menus }} arrow={{ pointAtCenter: false }} trigger={['click']}>
                                <Avatar icon={<FontAwesomeIcon icon={faUser} />} style={{ marginRight: '4px' }} />
                            </Dropdown>

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
        </>
    );
}