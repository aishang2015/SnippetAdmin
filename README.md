# SnippetAdmin

基于.net 7,react18,ant-design 5,自用开发模板，有权限管理，定时任务管理，日志，动态接口，代码生成器等功能

体验地址：http://39.105.56.25:31000/

用户名：admin

密码：admin

### 启动方式

- docker-compose up -d
- 使用visual studio打开api，vscode打开web项目，在appsetting中配置mysql地址。code first模式，程序会自动生成数据库以及数据

### 创建模板

- cd api                                  #进入目录

- dotnet new -u .                 #卸载

- dotnet new -i .                  #安装

- dotnet new sa -n 你自己的项目名
