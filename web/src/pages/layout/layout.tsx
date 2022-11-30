import { faArrowRightLong, faArrowLeftLong, faUser, faEdit, faOutdent } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Avatar, Divider, Dropdown, Layout, Menu, MenuProps, Space } from "antd";
import { Content, Header } from "antd/es/layout/layout";
import Sider from "antd/es/layout/Sider";
import MenuItem from "antd/es/menu/MenuItem";
import { useState } from "react";
import { Link, Outlet } from "react-router-dom";
import { Constants } from "../../common/constants";
import { StorageService } from "../../common/storage";

import './layout.css';

export default function BasicLayout() {

    const [collapsed, setCollapsed] = useState(false);

    const menus: MenuProps['items'] = [
        {
            label:
                <a>
                    <Space><FontAwesomeIcon icon={faEdit} fixedWidth />个人设置</Space>
                </a>
            , key: '1'
        }, // 菜单项务必填写 key
        {
            label:
                <a onClick={() => logout()}>
                    <Space><FontAwesomeIcon icon={faOutdent} fixedWidth />注销</Space>
                </a>
            , key: '2'
        },
    ];

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
        for (const routeInfo of Constants.RouteInfo) {
            if(!StorageService.getRights().find(right => right === routeInfo.identify)){
                continue;
            }
            if (routeInfo.children != undefined) {
                let childItems = new Array<MenuItem>();
                for (const child of routeInfo.children) {
                    if(!StorageService.getRights().find(right => right === child.identify)){
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

    return (
        <>
            {/* all the other elements */}
            <Layout style={{ minHeight: '100vh', maxHeight: '100vh' }}>
                <Sider trigger={null} collapsible collapsed={collapsed}>
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
                        {collapsed ?
                            <FontAwesomeIcon icon={faArrowRightLong} style={{ lineHeight: '64px', fontSize: '20px', margin: '22px' }} onClick={() => setCollapsed(!collapsed)} /> :
                            <FontAwesomeIcon icon={faArrowLeftLong} style={{ lineHeight: '64px', fontSize: '20px', margin: '22px' }} onClick={() => setCollapsed(!collapsed)} />
                        }
                        <div style={{
                            display: 'flex',
                            alignItems: 'center'
                        }}>

                            <Dropdown className="dropdown" menu={{ items: menus }} arrow={{ pointAtCenter: false }} trigger={['click']}>
                                <Avatar icon={<FontAwesomeIcon icon={faUser} />} style={{ marginRight: '30px' }} />
                            </Dropdown>

                        </div>
                    </Header >
                    <Content className="screen_container">
                        <Outlet />
                    </Content>
                </Layout>
            </Layout>
        </>
    );
}