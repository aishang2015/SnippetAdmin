import React, { FC, Suspense } from 'react';
import {
  BrowserRouter,
  createBrowserRouter,
  Route,
  RouterProvider,
  Routes
} from "react-router-dom";
import 'antd/dist/reset.css';
import zhCN from 'antd/locale/zh_CN';
import './App.css';
import BasicLayout from './pages/layout/layout';
import Callback from './pages/callback/callback';
import { Bind } from './pages/bind/bind';
import { ConfigProvider } from 'antd';
import Login from './pages/login/login';

const App: FC = () => {

  const HomePage = React.lazy(() => import('./pages/home/home'));
  const AboutPage = React.lazy(() => import('./pages/about/about'));
  const TablePage = React.lazy(() => import('./pages/table/table'));
  const FlowPage = React.lazy(() => import('./pages/flow/flow'));
  const ChatPage = React.lazy(() => import('./pages/chat/chat'));

  const UserPage = React.lazy(() => import('./pages/rbac/user/user'));
  const RolePage = React.lazy(() => import('./pages/rbac/role/role'));
  const PagePage = React.lazy(() => import('./pages/rbac/page/page'));
  const OrgPage = React.lazy(() => import('./pages/rbac/org/org'));
  const PosPage = React.lazy(() => import('./pages/rbac/pos/position'));
  const StatePage = React.lazy(() => import('./pages/rbac/state/state'));

  const TaskManagePage = React.lazy(() => import('./pages/task/task-manage/taskManage'));
  const TaskRecordPage = React.lazy(() => import('./pages/task/task-record/taskRecord'));

  const SettingPage = React.lazy(() => import('./pages/system/setting/setting'));
  const AccessLogPage = React.lazy(() => import('./pages/system/access/access'));
  const ExceptionedPage = React.lazy(() => import('./pages/system/exception/exception'));
  const LoginLogPage = React.lazy(() => import('./pages/system/login/login'));
  const DictionaryPage = React.lazy(() => import('./pages/system/dictionary/dictionary'));
  const ExportPage = React.lazy(() => import('./pages/system/export/export'));

  const loadingContent = "加载中...";

  var noAuthorizedRouter = createBrowserRouter([
    { path: '', element: <Login /> },
    { path: '/login', element: <Login /> },
    { path: "/*", element: <Login /> },
  ]);

  var authorizedRouter = createBrowserRouter([
    {
      path: '/',
      element: <BasicLayout />,
      children: [
        { path: "", element: <Suspense fallback={loadingContent}><HomePage /></Suspense> },
        { path: "/home", element: <Suspense fallback={loadingContent}><HomePage /></Suspense> },
        { path: "/flow", element: <Suspense fallback={loadingContent}><FlowPage /></Suspense> },
        { path: "/chat", element: <Suspense fallback={loadingContent}><ChatPage /></Suspense> },
        { path: "/about", element: <Suspense fallback={loadingContent}><AboutPage /></Suspense> },

        { path: "/user", element: <Suspense fallback={loadingContent}><UserPage /></Suspense> },
        { path: "/role", element: <Suspense fallback={loadingContent}><RolePage /></Suspense> },
        { path: "/page", element: <Suspense fallback={loadingContent}><PagePage /></Suspense> },
        { path: "/org", element: <Suspense fallback={loadingContent}><OrgPage /></Suspense> },
        { path: "/pos", element: <Suspense fallback={loadingContent}><PosPage /></Suspense> },
        { path: "/state", element: <Suspense fallback={loadingContent}><StatePage /></Suspense> },

        { path: "/taskMange", element: <Suspense fallback={loadingContent}><TaskManagePage /></Suspense> },
        { path: "/taskRecord", element: <Suspense fallback={loadingContent}><TaskRecordPage /></Suspense> },

        { path: "/access", element: <Suspense fallback={loadingContent}><AccessLogPage /></Suspense> },
        { path: "/exception", element: <Suspense fallback={loadingContent}><ExceptionedPage /></Suspense> },
        { path: "/loginlog", element: <Suspense fallback={loadingContent}><LoginLogPage /></Suspense> },
        { path: "/dictionary", element: <Suspense fallback={loadingContent}><DictionaryPage /></Suspense> },
        { path: "/setting", element: <Suspense fallback={loadingContent}><SettingPage /></Suspense> },
        { path: "/export", element: <Suspense fallback={loadingContent}><ExportPage /></Suspense> },
        { path: "/*", element: <Suspense fallback={loadingContent}><HomePage /></Suspense> },
      ]
    }
  ]);


  return (

    <ConfigProvider
      locale={zhCN}
      theme={{
        token: {
          colorPrimary: '#00b96b',
          colorLink:'#00b96b'
        },
      }}>

      <RouterProvider router={localStorage.getItem('token') ? authorizedRouter : noAuthorizedRouter} />

    </ConfigProvider>
  );
}

export default App;