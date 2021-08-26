import { HomeOutlined, VideoCameraOutlined, TableOutlined, ApartmentOutlined, WechatOutlined } from '@ant-design/icons';

export class Constants {

    static RouteInfo = [
        { path: '/home', name: '主页', icon: <HomeOutlined /> },
        { path: '/table', name: '表格', icon: <TableOutlined /> },
        { path: '/flow', name: '流程', icon: <ApartmentOutlined /> },
        { path: '/chat', name: '对话', icon: <WechatOutlined /> },
        {
            path: '', name: '子菜单', icon: <WechatOutlined />, children: [
                { path: '/submenu1', name: '子菜单1', icon: <WechatOutlined /> },
                { path: '/submenu2', name: '子菜单2', icon: <WechatOutlined /> },
            ]
        },
        { path: '/about', name: '关于', icon: <VideoCameraOutlined /> }
    ];

    static FlatRouteInfo = [
        ...Constants.RouteInfo,
        ...Constants.RouteInfo[4].children!
    ];

}