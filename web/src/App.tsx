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
import { Loading } from './components/common/loading/loading';
import { Constants } from './common/constants';

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

  const loadingContent = <Loading />;

  var noAuthorizedRouter = createBrowserRouter([
    { path: '', element: <Login /> },
    { path: '/login', element: <Login /> },
    { path: "/*", element: <Login /> },
  ]);

  const loader = async (path: string) => {
    let pageName = Constants.FlatRouteInfo.find(r => r.path === path)?.name;
    PubSub.publish("navTo", { name: pageName, path: path });
  }

  var routeInfo = [
    { path: "",component:<HomePage/>},
    { path: "/home",component:<HomePage/>},
    { path: "/flow",component:<FlowPage/>},
    { path: "/chat",component:<ChatPage/>},
    { path: "/about",component:<AboutPage/>},

    { path: "/user",component:<UserPage/>},
    { path: "/role",component:<RolePage/>},
    { path: "/page",component:<PagePage/>},
    { path: "/org",component:<OrgPage/>},
    { path: "/pos",component:<PosPage/>},
    { path: "/state",component:<StatePage/>},

    { path: "/taskMange",component:<TaskManagePage/>},
    { path: "/taskRecord",component:<TaskRecordPage/>},

    { path: "/access",component:<AccessLogPage/>},
    { path: "/exception",component:<ExceptionedPage/>},
    { path: "/loginlog",component:<LoginLogPage/>},
    { path: "/dictionary",component:<DictionaryPage/>},
    { path: "/setting",component:<SettingPage/>},

    { path: "/export",component:<ExportPage/>},
    { path: "/code",component:<CodePage/>},
    { path: '/frontend',component:<FrontToolPage/>},

    { path: "/*",component:<HomePage/>},
  ];

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
      children: routeInfo.map(r=>
        {
          return {
            path: r.path,
            element: <Suspense fallback={loadingContent}>{r.component}</Suspense> ,
            loader: async ({ params }) => { await loader(r.path) }
          }
        }
      )
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