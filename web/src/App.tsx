import BasicLayout from './pages/layout/layout'
import {
  BrowserRouter as Router,
  Redirect,
  Route,
  Switch,
} from "react-router-dom";
import './App.less';
import Login from './pages/login/login';
import React, { Suspense } from 'react';
import Callback from './pages/callback/callback';
import { Bind } from './pages/bind/bind';

interface app {
}

class App extends React.Component<any, app> {

  constructor(props: any) {
    super(props);
    this.state = {
    }
  }

  render() {

    // // 客户端判断token是否过期，过期则清理登录信息
    // let isOutOfDate = false;
    // let expireString = StorageService.getExpire();
    // if (expireString) {
    //   let expireTime = new Date(expireString);
    //   if (expireTime < new Date()) {
    //     isOutOfDate = true;
    //   }
    // } else {
    //   isOutOfDate = true;
    // }

    // if (isOutOfDate) {
    //   StorageService.clearLoginStore();
    // }

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

    return (
      <Router>
        <Switch>
          <Route path="/" render={
            ({ location }) => localStorage.getItem('token') ? (
              <BasicLayout>
                <Switch>
                  <Route exact={true} path="/home"><Suspense fallback={loadingContent}><HomePage /></Suspense></Route>
                  <Route exact={true} path="/table"><Suspense fallback={loadingContent}><TablePage /></Suspense></Route>
                  <Route exact={true} path="/flow"><Suspense fallback={loadingContent}><FlowPage /></Suspense></Route>
                  <Route exact={true} path="/chat"><Suspense fallback={loadingContent}><ChatPage /></Suspense></Route>
                  <Route exact={true} path="/about"><Suspense fallback={loadingContent}><AboutPage /></Suspense></Route>

                  <Route exact={true} path="/user"><Suspense fallback={loadingContent}><UserPage /></Suspense></Route>
                  <Route exact={true} path="/role"><Suspense fallback={loadingContent}><RolePage /></Suspense></Route>
                  <Route exact={true} path="/page"><Suspense fallback={loadingContent}><PagePage /></Suspense></Route>
                  <Route exact={true} path="/org"><Suspense fallback={loadingContent}><OrgPage /></Suspense></Route>
                  <Route exact={true} path="/pos"><Suspense fallback={loadingContent}><PosPage /></Suspense></Route>
                  <Route exact={true} path="/state"><Suspense fallback={loadingContent}><StatePage /></Suspense></Route>

                  <Route exact={true} path="/taskMange"><Suspense fallback={loadingContent}><TaskManagePage /></Suspense></Route>
                  <Route exact={true} path="/taskRecord"><Suspense fallback={loadingContent}><TaskRecordPage /></Suspense></Route>

                  <Route exact={true} path="/access"><Suspense fallback={loadingContent}><AccessLogPage /></Suspense></Route>
                  <Route exact={true} path="/exception"><Suspense fallback={loadingContent}><ExceptionedPage /></Suspense></Route>
                  <Route exact={true} path="/loginlog"><Suspense fallback={loadingContent}><LoginLogPage /></Suspense></Route>
                  <Route exact={true} path="/dictionary"><Suspense fallback={loadingContent}><DictionaryPage /></Suspense></Route>
                  <Route exact={true} path="/setting"><Suspense fallback={loadingContent}><SettingPage /></Suspense></Route>
                  <Route exact={true} path="/export"><Suspense fallback={loadingContent}><ExportPage /></Suspense></Route>

                  <Route path="*">
                    <Redirect to="/home"></Redirect>
                  </Route>
                </Switch>
              </BasicLayout>
            ) : (
              <Switch>
                <Route exact={true} path="/login" component={Login}></Route>
                <Route exact={true} path="/callback" component={Callback}></Route>
                <Route exact={true} path="/binding" component={Bind}></Route>
                <Route path="*">
                  <Redirect to="/login"></Redirect>
                </Route>
              </Switch>
            )
          } />
        </Switch>
      </Router>
    );
  }
}

export default App;
