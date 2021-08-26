import { Badge, Button, Dropdown, List, Menu } from "antd";
import { Header } from "antd/lib/layout/layout";
import React from "react";
import { connect } from "react-redux";
import {
    MenuUnfoldOutlined,
    MenuFoldOutlined,
    DownOutlined,
    NotificationOutlined
} from '@ant-design/icons';
import { RouteComponentProps, withRouter } from "react-router-dom";
import { Dispatch } from "redux";
import { onToggle } from "../../redux/navCollapsed/navCollapsedCreator";
import { onClearMessage } from "../../redux/notification/notificationCreator";
import { StorageService } from "../../common/storage";

type INavHeaderProps = {
    collapsed: boolean;
    toggle: () => {};
    clearMessage: () => {};
    notifications: string[];
} & RouteComponentProps<{}>

type INavHeaderState = {
    visible: boolean;
}


class NavHeader extends React.Component<INavHeaderProps, INavHeaderState>{

    state = {
        visible: false,
    };

    render = () => {
        const menu = (
            <Menu>
                <Menu.Item danger><a onClick={() => this.logout()}>注销</a></Menu.Item>
            </Menu>
        );


        const message = (
            <div>
                {this.props.notifications.length > 0 &&
                    <Button onClick={() => this.props.clearMessage()}>清理</Button>
                }
                <List
                    itemLayout="horizontal"
                    pagination={{ position: 'bottom' }}
                    dataSource={this.props.notifications}
                    renderItem={item => (
                        <List.Item>
                            <List.Item.Meta
                                title={item}
                            />
                        </List.Item>
                    )}
                />
            </div>
        );

        const userName = localStorage.getItem('user-name');

        return (
            <Header className="site-layout" style={{ padding: 0 }}>
                {React.createElement(this.props.collapsed ? MenuUnfoldOutlined : MenuFoldOutlined, {
                    className: 'trigger',
                    onClick: this.props.toggle,
                })}
                <div>
                    <Badge count={this.props.notifications.length}>
                        <Dropdown overlay={message} placement="bottomRight" visible={this.state?.visible}
                            onVisibleChange={(flag) => { this.setState({ visible: flag }) }}
                            overlayStyle={{
                                width: 500,
                                background: "white",
                                border: "1px solid gray",
                                padding: 10
                            }}>
                            <Button shape="circle" icon={<NotificationOutlined />} />
                        </Dropdown>
                    </Badge>
                    <Dropdown className="dropdown" overlay={menu}>
                        <a className="ant-dropdown-link" onClick={e => e.preventDefault()}>
                            {userName}<DownOutlined />
                        </a>
                    </Dropdown>
                </div>
            </Header>
        );
    }


    logout() {
        StorageService.clearLoginStore();
        this.props.history.push("/login");
    };
}

export default connect(
    (state: any) => ({
        collapsed: state.NavCollapsedReducer.collapsed,
        notifications: state.NotificationReducer.notifications
    }),
    (dispatch: Dispatch) => ({
        toggle: () => dispatch(onToggle()),
        clearMessage: () => dispatch(onClearMessage()),
    })
)(withRouter(NavHeader));