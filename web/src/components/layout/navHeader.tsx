import { Avatar, Badge, Button, Divider, Dropdown, List, Menu, Space } from "antd";
import { Header } from "antd/lib/layout/layout";
import React from "react";
import { connect } from "react-redux";
import { RouteComponentProps, withRouter } from "react-router-dom";
import { Dispatch } from "redux";
import { onToggle } from "../../redux/navCollapsed/navCollapsedCreator";
import { onClearMessage } from "../../redux/notification/notificationCreator";
import { StorageService } from "../../common/storage";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faArrowLeft, faArrowRight, faBell, faCompress, faEdit, faExpand, faOutdent, faUser } from "@fortawesome/free-solid-svg-icons";

type INavHeaderProps = {
    collapsed: boolean;
    toggle: () => {};
    clearMessage: () => {};
    notifications: string[];
} & RouteComponentProps<{}>

type INavHeaderState = {
    visible: boolean;
    isExpand: boolean;
}


class NavHeader extends React.Component<INavHeaderProps, INavHeaderState>{

    state = {
        visible: false,
        isExpand: false
    };

    render = () => {
        const menu = (
            <Menu>
                <Menu.Item>
                    <a>
                        <Space><FontAwesomeIcon icon={faEdit} fixedWidth />个人设置</Space>
                    </a>
                </Menu.Item>
                <Divider style={{ marginTop: 4, marginBottom: 4 }} />
                <Menu.Item>
                    <a onClick={() => this.logout()}>
                        <Space><FontAwesomeIcon icon={faOutdent} fixedWidth />注销</Space>
                    </a>
                </Menu.Item>
            </Menu >
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
                {/* {React.createElement(this.props.collapsed ? MenuUnfoldOutlined : MenuFoldOutlined, {
                    className: 'trigger',
                    onClick: this.props.toggle,
                })} */}
                {this.props.collapsed ?
                    <FontAwesomeIcon icon={faArrowRight} style={{ lineHeight: '64px', fontSize: '20px', margin: '22px' }} onClick={this.props.toggle} /> :
                    <FontAwesomeIcon icon={faArrowLeft} style={{ lineHeight: '64px', fontSize: '20px', margin: '22px' }} onClick={this.props.toggle} />
                }
                <div style={{
                    display: 'flex',
                    alignItems: 'center'
                }}>
                    <Button icon={this.state.isExpand ?
                        <FontAwesomeIcon icon={faCompress} /> :
                        <FontAwesomeIcon icon={faExpand} />} shape="circle" style={{ marginRight: '10px' }}
                        onClick={() => this.showFullScreen()}></Button>
                    <Badge count={this.props.notifications.length}>
                        <Dropdown overlay={message} placement="bottomRight" visible={this.state?.visible}
                            onVisibleChange={(flag) => { this.setState({ visible: flag }) }}
                            overlayStyle={{
                                width: 500,
                                background: "white",
                                border: "1px solid gray",
                                padding: 10
                            }}>
                            <Button shape="circle" icon={<FontAwesomeIcon icon={faBell} />} style={{ marginRight: '10px' }} />
                        </Dropdown>
                    </Badge>
                    <Dropdown className="dropdown" overlay={menu} arrow={{ pointAtCenter: false }} trigger={['click']}>
                        <Avatar icon={<FontAwesomeIcon icon={faUser} />} style={{ marginRight: '30px' }} />
                    </Dropdown>

                </div>
            </Header >
        );
    }

    showFullScreen() {

        let element: any = document.documentElement;

        let doc = document as any;
        let isFullScreen = (
            doc.fullscreenElement ||
            doc.mozFullscreenElement ||
            doc.msFullscreenElement ||
            doc.webkitFullscreenElement ||
            null
        )

        if (!isFullScreen) {
            if (element.requestFullscreen) {
                element.requestFullscreen();
            } else if (element.msRequestFullscreen) {
                element.msRequestFullscreen();
            } else if (element.mozRequestFullScreen) {
                element.mozRequestFullScreen();
            } else if (element.webkitRequestFullscreen) {
                element.webkitRequestFullscreen();
            }
        } else {
            let doc: any = document;
            if (doc.exitFullscreen) {
                doc.exitFullscreen();
            } else if (doc.msExitFullscreen) {
                doc.msExitFullscreen();
            } else if (doc.mozCancelFullScreen) {
                doc.mozCancelFullScreen();
            } else if (doc.webkitExitFullscreen) {
                doc.webkitExitFullscreen();
            }
        }
        this.setState({ isExpand: !isFullScreen });
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