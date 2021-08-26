import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';
import 'moment/locale/zh-cn';
import zhCN from 'antd/lib/locale/zh_CN';

import { Provider } from 'react-redux';
import { Configuration } from './common/config';
import { Axios } from './http/request';
import { ReduxStore } from './redux/reduxStore';
import { ConfigProvider } from 'antd';
import { OauthService } from './common/oauth';

Configuration.init().then(
  success => {

    // 初始化axio实例
    Axios.initAxiosInstance();

    // 初始化认证配置
    OauthService.initUserManager();

    // 初始化redux
    const store = ReduxStore.initReduxStore();

    ReactDOM.render(
      <React.StrictMode>
        <Provider store={store}>
          <ConfigProvider locale={zhCN}>
            <App />
          </ConfigProvider>
        </Provider>
      </React.StrictMode>,
      document.getElementById('root')
    );

    // If you want to start measuring performance in your app, pass a function
    // to log results (for example: reportWebVitals(console.log))
    // or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
    reportWebVitals();
  },
  fail => {
    ReactDOM.render(
      <div>无法加载配置文件！</div>,
      document.getElementById('root')
    );
  }
)
