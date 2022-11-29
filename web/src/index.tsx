import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';
import { Configuration } from './common/config';
import { Axios } from './http/request';
import { OauthService } from './common/oauth';

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
Configuration.init().then(

  async success => {

    // 初始化axio实例
    Axios.initAxiosInstance();

    // 初始化认证配置
    OauthService.initUserManager();
    
    root.render(

        <App />

    );
  },
  fail => {
    root.render(
      <div>无法加载配置文件！</div>
    );
  }
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
