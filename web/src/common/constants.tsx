import { faArrowRight, faBug, faClipboardCheck, faCog, faColumns, faDiceOne, faFileAlt, faFileExport, faHome, faInfo, faSitemap, faTasks, faThumbtack, faUser, faUserFriends, faUsers, faUserTag } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';

export class Constants {

    static RouteInfo = [
        { path: '/home', name: '主页', identify: 'home', icon: <FontAwesomeIcon icon={faHome} fixedWidth /> },
        {
            path: '', name: '权限管理', identify: 'rbac', icon: <FontAwesomeIcon icon={faUsers} fixedWidth />, children: [
                { path: '/user', name: '用户信息', identify: 'user', icon: <FontAwesomeIcon icon={faUser} fixedWidth /> },
                { path: '/role', name: '角色信息', identify: 'role', icon: <FontAwesomeIcon icon={faUserTag} fixedWidth /> },
                { path: '/org', name: '组织信息', identify: 'org', icon: <FontAwesomeIcon icon={faSitemap} fixedWidth /> },
                { path: '/pos', name: '职位信息', identify: 'org', icon: <FontAwesomeIcon icon={faUserFriends} fixedWidth /> },
                { path: '/page', name: '页面权限', identify: 'permission', icon: <FontAwesomeIcon icon={faColumns} fixedWidth /> },
            ]
        },
        {
            path: '', name: '任务调度', identify: 'task', icon: <FontAwesomeIcon icon={faTasks} fixedWidth />, children: [
                { path: '/taskMange', name: '任务管理', identify: 'taskManage', icon: <FontAwesomeIcon icon={faThumbtack} fixedWidth /> },
                { path: '/taskRecord', name: '任务记录', identify: 'taskRecord', icon: <FontAwesomeIcon icon={faClipboardCheck} fixedWidth /> },
            ]
        },
        // {
        //     path: '', name: '系统设置', icon: <FontAwesomeIcon icon={faCog} fixedWidth />, children: [
        //         { path: '/state', name: '登录管理', icon: <FontAwesomeIcon icon={faSignInAlt} fixedWidth /> },
        //     ]
        // },
        {
            path: '', name: '日志中心', identify: 'systemLog', icon: <FontAwesomeIcon icon={faFileAlt} fixedWidth />, children: [
                { path: '/access', name: '接口日志', identify: 'accessRecord', icon: <FontAwesomeIcon icon={faArrowRight} fixedWidth /> },
                { path: '/exception', name: '异常日志', identify: 'exceptionRecord', icon: <FontAwesomeIcon icon={faBug} fixedWidth /> },
                { path: '/loginlog', name: '登录日志', identify: 'loginRecord', icon: <FontAwesomeIcon icon={faInfo} fixedWidth /> },
            ]
        },
        {
            path: '', name: '系统配置', identify: 'system-config', icon: <FontAwesomeIcon icon={faCog} fixedWidth />, children: [
                { path: '/dictionary', name: '字典配置', identify: 'dic-config', icon: <FontAwesomeIcon icon={faDiceOne} fixedWidth /> },
                { path: '/export', name: '数据导出', identify: 'dic-config', icon: <FontAwesomeIcon icon={faFileExport} fixedWidth /> },
            ]
        },
        // {
        //     path: '', name: '临时', identify: 'rbac', icon: <FontAwesomeIcon icon={faTasks} fixedWidth />, children: [
        //         //{ path: '/flow', name: '流程图', identify: 'user', icon: <FontAwesomeIcon icon={faProjectDiagram} fixedWidth /> },
        //         { path: '/table', name: '表格', identify: 'user', icon: <FontAwesomeIcon icon={faTable} fixedWidth /> },
        //         //{ path: '/chat', name: '聊天', identify: 'user', icon: <FontAwesomeIcon icon={faChalkboardTeacher} fixedWidth /> },
        //     ]
        // },
    ];

    static flatRoute = function (array: any) {
        let result = new Array<any>();
        if (array.length > 0) {
            array.forEach((route: any) => {
                result = result.concat(route);
                if (route.children && route.children.length > 0) {
                    result = result.concat(Constants.flatRoute(route.children));
                }
            });
        }
        return result;
    }

    static FlatRouteInfo: Array<any> = Constants.flatRoute(Constants.RouteInfo);

    static ElementTypeArray = [
        { key: 1, value: '菜单' },
        { key: 2, value: '按钮/链接' }
    ]

    static ElementTypeDic: { [key: number]: string } = {
        1: '菜单',
        2: '按钮/链接'
    }
}