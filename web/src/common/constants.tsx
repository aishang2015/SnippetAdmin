import { faChalkboardTeacher, faClipboardCheck, faCog, faColumns, faHome, faInfo, faSitemap, faTasks, faThumbtack, faUniversalAccess, faUser, faUsers, faUserTag } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

export class Constants {

    static RouteInfo = [
        { path: '/home', name: '主页', identify: 'home', icon: <FontAwesomeIcon icon={faHome} fixedWidth /> },
        {
            path: '', name: 'RBAC管理', identify: 'rbac', icon: <FontAwesomeIcon icon={faUniversalAccess} fixedWidth />, children: [
                { path: '/user', name: '用户信息', identify: 'user', icon: <FontAwesomeIcon icon={faUser} fixedWidth /> },
                { path: '/role', name: '角色信息', identify: 'role', icon: <FontAwesomeIcon icon={faUserTag} fixedWidth /> },
                { path: '/org', name: '组织信息', identify: 'org', icon: <FontAwesomeIcon icon={faSitemap} fixedWidth /> },
                { path: '/pos', name: '职位信息', identify: 'org', icon: <FontAwesomeIcon icon={faUsers} fixedWidth /> },
                { path: '/page', name: '页面权限', identify: 'permission', icon: <FontAwesomeIcon icon={faColumns} fixedWidth /> },
            ]
        },
        {
            path: '', name: '任务调度', identify: 'rbac', icon: <FontAwesomeIcon icon={faTasks} fixedWidth />, children: [
                { path: '/taskMange', name: '任务管理', identify: 'user', icon: <FontAwesomeIcon icon={faThumbtack} fixedWidth /> },
                { path: '/taskRecord', name: '任务记录', identify: 'role', icon: <FontAwesomeIcon icon={faClipboardCheck} fixedWidth /> },
            ]
        },
        // {
        //     path: '', name: '系统设置', icon: <FontAwesomeIcon icon={faCog} fixedWidth />, children: [
        //         { path: '/state', name: '登录管理', icon: <FontAwesomeIcon icon={faSignInAlt} fixedWidth /> },
        //     ]
        // },
        { path: '/setting', name: '系统设置', identify: 'about', icon: <FontAwesomeIcon icon={faCog} fixedWidth /> },
        { path: '/about', name: '关于', identify: 'about', icon: <FontAwesomeIcon icon={faInfo} fixedWidth /> },
        // {
        //     path: '', name: '临时', identify: 'rbac', icon: <FontAwesomeIcon icon={faTasks} fixedWidth />, children: [
        //         { path: '/flow', name: '流程图', identify: 'user', icon: <FontAwesomeIcon icon={faProjectDiagram} fixedWidth /> },
        //         { path: '/table', name: '表格', identify: 'user', icon: <FontAwesomeIcon icon={faTable} fixedWidth /> },
        //         { path: '/chat', name: '聊天', identify: 'user', icon: <FontAwesomeIcon icon={faChalkboardTeacher} fixedWidth /> },
        //     ]
        // },
    ];

    static FlatRouteInfo = [
        ...Constants.RouteInfo,
        ...Constants.RouteInfo[1].children!,
        ...Constants.RouteInfo[2].children!,
        //...Constants.RouteInfo[5].children!
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