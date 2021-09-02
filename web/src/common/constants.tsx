import {
    HomeOutlined, VideoCameraOutlined, UserOutlined, PartitionOutlined, SolutionOutlined,
    SettingOutlined, SafetyCertificateOutlined, LoginOutlined
} from '@ant-design/icons';

export class Constants {

    static RouteInfo = [
        { path: '/home', name: '主页', icon: <HomeOutlined /> },
        {
            path: '', name: 'RBAC管理', icon: <SettingOutlined />, children: [
                { path: '/user', name: '系统用户', icon: <UserOutlined /> },
                { path: '/role', name: '角色管理', icon: <SolutionOutlined /> },
                { path: '/org', name: '组织管理', icon: <PartitionOutlined /> },
                { path: '/page', name: '页面权限', icon: <SafetyCertificateOutlined /> },
            ]
        },
        {
            path: '', name: '系统设置', icon: <SettingOutlined />, children: [
                { path: '/state', name: '登录管理', icon: <LoginOutlined /> },
            ]
        },
        { path: '/about', name: '关于', icon: <VideoCameraOutlined /> }
    ];

    static FlatRouteInfo = [
        ...Constants.RouteInfo,
        ...Constants.RouteInfo[1].children!,
        ...Constants.RouteInfo[2].children!
    ];

    static ElementTypeArray = [
        { key: 1, value: '菜单' },
        { key: 2, value: '按钮/链接' }
    ]

    static ElementTypeDic: { [key: number]: string } = {
        1: '菜单',
        2: '按钮/链接'
    }
}