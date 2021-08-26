import { Layout, Tabs } from 'antd';
import React from 'react';
import { withRouter } from 'react-router-dom';
import NavMenu from '../../components/menu/navMenu';
import NavHeader from '../../components/header/navHeader';
import { Configuration } from '../../common/config';
import './layout.less';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { onReceiveMessage } from '../../redux/notification/notificationCreator';
import { Constants } from '../../common/constants';
import { EventService } from '../../common/event';

const signalR = require("@microsoft/signalr");

const { Content } = Layout;


class BasicLayout extends React.Component<any, any> {

    newTabIndex = 0;

    initialPanes = [
        { title: '主页', key: '/home', closable: false }
    ];

    constructor(props: any) {
        super(props);
        this.state = {
            collapsed: false,
            onLogout: () => { },
            activeKey: localStorage.getItem('activeKey') ?? this.initialPanes[0].key,
            panes: localStorage.getItem('panes') ? JSON.parse(localStorage.getItem('panes')!) : this.initialPanes,
        }
        this.props.history.push(this.state.activeKey);
    }

    async componentDidMount() {
        let connection = new signalR.HubConnectionBuilder()
            .withUrl(`${Configuration.BaseUrl}/broadcast`, { accessTokenFactory: () => localStorage.getItem("token") })
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        let startFun = async () => {
            try {
                await connection.start();
            } catch (err) {
                console.log(err);
                setTimeout(startFun, 5000);
            }
        }

        connection.on("HandleMessage", (message: string) => {
            this.props.receiveMessage(message);
        });

        connection.onclose(startFun);

        await startFun();
    }

    componentDidUpdate() {

        // 路由发生变化时切换tab页
        if (this.state.activeKey !== this.props.location.pathname) {
            if (this.state.panes.find((p: any) => p.key === this.props.location.pathname)) {
                this.setState({
                    activeKey: this.props.location.pathname
                });
                this.saveTabData(this.props.location.pathname);
            } else {
                const newPanes = [...this.state.panes];
                const activeKey = this.props.location.pathname;
                let route = Constants.FlatRouteInfo.find((r: any) => r.path === activeKey);
                newPanes.push({ title: route?.name, key: activeKey });
                this.setState({
                    panes: newPanes,
                    activeKey
                });
                this.saveTabData(activeKey, newPanes);
            }

            // 发送事件到菜单组件，修改菜单选择状态
            EventService.Emit("tabChange", [this.props.location.pathname]);
        }
    }

    // 选择tab页
    onChange = (activeKey: any) => {
        this.setState({ activeKey });
        this.props.history.push(activeKey);
        this.saveTabData(activeKey);

        // 发送事件到菜单组件，修改菜单选择状态
        EventService.Emit("tabChange", [activeKey]);
    };

    // 删除tab页
    onEdit = (targetKey: any, action: any) => {
        if (action === "remove") {
            const { panes, activeKey } = this.state;
            let newActiveKey = activeKey;
            let lastIndex = 0;
            panes.forEach((pane: any, i: any) => {
                if (pane.key === targetKey) {
                    lastIndex = i - 1;
                }
            });
            const newPanes = panes.filter((pane: any) => pane.key !== targetKey);
            if (newPanes.length && newActiveKey === targetKey) {
                if (lastIndex >= 0) {
                    newActiveKey = newPanes[lastIndex].key;
                } else {
                    newActiveKey = newPanes[0].key;
                }
            }
            this.props.history.push(newActiveKey);
            this.setState({
                panes: newPanes,
                activeKey: newActiveKey,
            });
            this.saveTabData(newActiveKey, newPanes);
            
            // 发送事件到菜单组件，修改菜单选择状态
            EventService.Emit("tabChange", [newActiveKey]);
        }
    };

    // 保存tab数据
    saveTabData(key = this.state.activeKey, panes = this.state.panes) {
        localStorage.setItem('activeKey', key);
        localStorage.setItem('panes', JSON.stringify(panes));
    }

    render = () => (
        <Layout style={{ minHeight: '100vh', maxHeight: '100vh' }}>
            <NavMenu />
            <Layout>
                <NavHeader />
                <Content>
                    <Content className="tab_container">
                        <Tabs type="editable-card" hideAdd={true} onChange={this.onChange} activeKey={this.state.activeKey}
                            onEdit={this.onEdit}>
                            {this.state.panes.map((pane: any) => (
                                <Tabs.TabPane tab={
                                    <>
                                        {Constants.FlatRouteInfo.find(r => r.path === pane.key)?.icon}{pane.title}
                                    </>
                                } key={pane.key} closable={pane.closable}>
                                </Tabs.TabPane>
                            ))}
                        </Tabs>
                    </Content>
                    <Content className="screen_container">
                        {this.props.children}
                    </Content>
                </Content>
            </Layout>
        </Layout>
    );
}

// 通过withRouter能把一些路由信息放入到当前页面的props内
export default connect(
    (state: any) => ({
        notifications: state.NotificationReducer.notifications
    }),
    (dispatch: Dispatch) => ({
        receiveMessage: (msg: string) => dispatch(onReceiveMessage(msg)),
    })
)(withRouter(BasicLayout));