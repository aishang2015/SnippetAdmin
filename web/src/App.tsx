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
import { StorageService } from './common/storage';

interface app {
}

class App extends React.Component<any, app> {

  constructor(props: any) {
    super(props);
    this.state = {
    }
  }

  render() {

    // 客户端判断token是否过期，过期则清理登录信息
    let isOutOfDate = false;
    let expireString = StorageService.getExpire();
    if (expireString) {
      let expireTime = new Date(expireString);
      if (expireTime < new Date()) {
        isOutOfDate = true;
      }
    } else {
      isOutOfDate = true;
    }

    if (isOutOfDate) {
      StorageService.clearLoginStore();
    }

    const HomePage = React.lazy(() => import('./pages/home/home'));
    const AboutPage = React.lazy(() => import('./pages/about/about'));
    const TablePage = React.lazy(() => import('./pages/table/table'));
    const FlowPage = React.lazy(() => import('./pages/flow/flow'));
    const ChatPage = React.lazy(() => import('./pages/chat/chat'));

    return (
      <Router>
        <Switch>
          <Route path="/" render={
            ({ location }) => localStorage.getItem('token') ? (
              <BasicLayout>
                <Switch>
                  <Route exact={true} path="/home"><Suspense fallback="加载中..."><HomePage /></Suspense></Route>
                  <Route exact={true} path="/table"><Suspense fallback="加载中..."><TablePage /></Suspense></Route>
                  <Route exact={true} path="/flow"><Suspense fallback="加载中..."><FlowPage /></Suspense></Route>
                  <Route exact={true} path="/chat"><Suspense fallback="加载中..."><ChatPage /></Suspense></Route>
                  <Route exact={true} path="/about"><Suspense fallback="加载中..."><AboutPage /></Suspense></Route>

                  <Route exact={true} path="/submenu1"><Suspense fallback="加载中..."><AboutPage /></Suspense></Route>
                  <Route exact={true} path="/submenu2"><Suspense fallback="加载中..."><AboutPage /></Suspense></Route>
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
