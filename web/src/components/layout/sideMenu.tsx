import Sider from "antd/lib/layout/Sider";
import React from "react";
import { Link } from 'react-router-dom';
import { Button, Menu } from "antd";
import { connect } from "react-redux";
import DynamicAntdTheme from 'dynamic-antd-theme';
import { Constants } from "../../common/constants";
import { EventService } from "../../common/event";
import { StorageService } from "../../common/storage";

interface ISideMenuProps {
    collapsed: boolean;
}

class SideMenu extends React.Component<ISideMenuProps, any>{

    constructor(props: any) {
        super(props);
        this.state = {
            selectedKeys: [localStorage.getItem('activeKey') ?? Constants.RouteInfo[0].path]
        };
    }

    componentDidMount() {
        EventService.Subscribe("tabChange", (params: any) => {
            this.setState({
                selectedKeys: params
            });
        });
    }

    onSelect = (event: any) => {
        this.setState({
            selectedKeys: event.selectedKeys
        });
    }


    render = () => (

        <Sider trigger={null} collapsible collapsed={this.props.collapsed}>
            {this.props.collapsed ?
                <div className="logo" >Admin</div> :
                <div className="logo large-logo-font" >SnippetAdmin</div>
            }
            <div style={{ display: 'flex', flexDirection: 'column', justifyContent: "space-between", userSelect: "none" }}>
                <Menu theme="dark" mode="inline" defaultSelectedKeys={[localStorage.getItem('activeKey') ?? "/home"]}
                    selectedKeys={this.state.selectedKeys} onSelect={this.onSelect.bind(this)}>
                    {Constants.RouteInfo.map((r, index) => {
                        if (r.children !== undefined) {
                            return (
                                <>
                                    {StorageService.getRights().find(right => right === r.identify) &&
                                        <Menu.SubMenu key={index} icon={r.icon} title={r.name}>
                                            {
                                                r.children.map(item => {
                                                    return (
                                                        <>
                                                            {StorageService.getRights().find(r => r === item.identify) &&
                                                                <Menu.Item key={item.path} icon={item.icon}>
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
            <DynamicAntdTheme primaryColor={"#52c41a"} placement={"topRight"}
                style={{ position: "absolute", bottom: '10px', left: "calc(50% - 25px)" } as any} />
        </Sider>
    );
}

export default connect(
    (state: any) => ({
        collapsed: state.NavCollapsedReducer.collapsed
    })
)(SideMenu);