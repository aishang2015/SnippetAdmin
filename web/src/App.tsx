import React, { FC, Suspense } from 'react';
import {
  createBrowserRouter,
  RouterProvider
} from "react-router-dom";
import 'antd/dist/reset.css';
import zhCN from 'antd/locale/zh_CN';
import './App.css';
import BasicLayout from './pages/basic/layout/layout';
import { ConfigProvider, theme } from 'antd';
import Login from './pages/basic/login/login';

type ThemeData = {
  colorPrimary: string;
  colorLink: string;
};

const defaultData: ThemeData = {
  colorPrimary: '#1677ff',
  colorLink: '#1677ff'
};

const App: FC = () => {

  const HomePage = React.lazy(() => import('./pages/basic/home/home'));
  const AboutPage = React.lazy(() => import('./pages/basic/about/about'));
  const TablePage = React.lazy(() => import('./pages/test/table/table'));
  const FlowPage = React.lazy(() => import('./pages/test/flow/flow'));
  const ChatPage = React.lazy(() => import('./pages/test/chat/chat'));

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

  const ExportPage = React.lazy(() => import('./pages/develop/export/export'));
  const CodePage = React.lazy(() => import('./pages/develop/code/code'));
  const FrontToolPage = React.lazy(() => import('./pages/develop/frontend/frontend'));


  const [colorData, setColorData] = React.useState<ThemeData>(defaultData);
  const [themeData, setThemeData] = React.useState<string>('');

  const loadingContent = "加载中...";

  var noAuthorizedRouter = createBrowserRouter([
    { path: '', element: <Login /> },
    { path: '/login', element: <Login /> },
    { path: "/*", element: <Login /> },
  ]);

  var authorizedRouter = createBrowserRouter([
    {
      path: '/',
      element: <BasicLayout onColorChange={(color: string) => {
        setColorData({
          colorPrimary: color,
          colorLink: color
        })
      }} onThemeChange={(themeStr: string) => {
        setThemeData(themeStr);

      }} />,
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
        { path: "/code", element: <Suspense fallback={loadingContent}><CodePage /></Suspense> },
        { path: '/frontend', element: <Suspense fallback={loadingContent}><FrontToolPage /></Suspense> },

        { path: "/*", element: <Suspense fallback={loadingContent}><HomePage /></Suspense> },
      ]
    }
  ]);

  React.useEffect(() => {

    let color = localStorage.getItem("primaryColor");
    if (color === null) {
      color = "#1677ff";
      localStorage.setItem("primaryColor", color);
    }
    setColorData({
      colorPrimary: color,
      colorLink: color,
    });

    let themeStr = localStorage.getItem("theme");
    if (themeStr === null) {
      themeStr = "light";
      localStorage.setItem("theme", themeStr);
    }
    setThemeData(themeStr);

  }, []);

  return (

    <ConfigProvider
      locale={zhCN}
      theme={{
        token: {
          colorPrimary: colorData.colorPrimary,
          colorLink: colorData.colorLink,
        },
        algorithm: themeData === "dark" ? theme.darkAlgorithm : theme.defaultAlgorithm
      }}>

      <RouterProvider router={localStorage.getItem('token') ? authorizedRouter : noAuthorizedRouter} />

    </ConfigProvider>
  );
}

export default App;