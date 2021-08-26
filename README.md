# SnippetAdmin

使用.net 5和react创建的初始模板。

#### 使用技术

后端：AutoMapper，FluentValidation，Identity，EntityFrameworkCore，Refit，Serilog，SkyAPM，Swashbuckle，SignalR，BackgroundService，IDistributedCache，MiniProfiler

前端：create-react-app，antd，craco，axios，redux，react-router，signalr，lodash，events

#### 从docker启动

docker-compose up -d --build 

然后打开连接

[Web](http://localhost:21001/home)

[SkyWalking](http://localhost:9898/)

#### 功能说明

- 配置redux和eventemitter
- 一个动态列信息的表格
- 一个基于jsplumb和panzoom的流程图
- 一个手机仿app聊天界面
- 前端主题色运行时切换
- 百度和Github的Oauth认证
- jwt认证
- Identity和EF配置
- 分布式缓存和内存缓存扩展
- 宿主服务扩展
- 链路跟踪
- 程序初始化器
- 一些扩展方法，中间件和帮助类
- docker compose配置以及本地卷

#### 其他

百度Oauth设置地址：[管理控制台 - 百度开放云平台 (baidu.com)](http://developer.baidu.com/console#app/project)

githubOauth设置地址：[Developer applications (github.com)](https://github.com/settings/developers)
