import { faArrowRightLong, faArrowLeftLong, faCompress, faExpand, faBell, faUser, faEdit, faOutdent } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Avatar, Badge, Button, Divider, Dropdown, Layout, Menu, message, Space } from "antd";
import { Content, Header } from "antd/es/layout/layout";
import Sider from "antd/es/layout/Sider";
import menu from "antd/es/menu";
import { useState } from "react";
import { Link, Outlet } from "react-router-dom";
import { Constants } from "../../common/constants";
import { StorageService } from "../../common/storage";

import './layout.css';

export default function BasicLayout() {

    const [collapsed, setCollapsed] = useState(false);

    const menu = (
        <Menu>
            <Menu.Item>
                <a>
                    <Space><FontAwesomeIcon icon={faEdit} fixedWidth />个人设置</Space>
                </a>
            </Menu.Item>
            <Divider style={{ marginTop: 4, marginBottom: 4 }} />
            <Menu.Item>
                <a onClick={() => logout()}>
                    <Space><FontAwesomeIcon icon={faOutdent} fixedWidth />注销</Space>
                </a>
            </Menu.Item>
        </Menu >
    );

    function logout() {
        StorageService.clearLoginStore();
        window.location.reload();
    };

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
                        <Menu theme="dark" mode="inline" defaultSelectedKeys={[localStorage.getItem('activeKey') ?? "/home"]}>
                            {Constants.RouteInfo.map((r, index) => {
                                if (r.children !== undefined) {
                                    return (
                                        <>
                                            {StorageService.getRights().find(right => right === r.identify) &&
                                                <Menu.SubMenu key={index} icon={r.icon} title={r.name}>
                                                    {
                                                        r.children.map((item, i) => {
                                                            return (
                                                                <>
                                                                    {StorageService.getRights().find(r => r === item.identify) &&
                                                                        <Menu.Item key={i + item.path} icon={item.icon}>
                                                                            <Link to={item.path}>{item.name}</Link>
                                                                        </Menu.Item>
                                                                    }
                                                                </>
                                                            );
                                                        })
                                                    }
                                                </Menu.SubMenu>
                                            }
                                        </>
                                    );
                                } else {
                                    return (
                                        <>
                                            {StorageService.getRights().find(right => right === r.identify) &&
                                                <Menu.Item key={r.path} icon={r.icon}>
                                                    <Link to={r.path}>{r.name}</Link>
                                                </Menu.Item>
                                            }
                                        </>
                                    );
                                }

                            })}
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

                            <Dropdown className="dropdown" overlay={menu} arrow={{ pointAtCenter: false }} trigger={['click']}>
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