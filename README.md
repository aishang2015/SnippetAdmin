# SnippetAdmin

基于.net 7,react18,ant-design 5,自用开发模板

### 功能特点

- 动态接口，在实体上添加注解生成动态接口

- 实体缓存，在实体上添加注解即可以在缓存中访问实时同步的数据

- ef分表，基于EF的SharedTypeEntity的功能实现

- 代码生成器

- 基于quartz的定时任务管理

- 支持项目模板

- 指标监控

- 引入了Orleans

- 基础的rbac管理，支持切换数据据库类型

- 基于表配置的系统配置管理

### 启动方式

- docker-compose up -d
- 使用visual studio打开api，vscode打开web项目，在appsetting中配置mysql地址。启动后，程序会自动生成数据库以及数据

### 使用此项目创建项目模板

- cd 项目根目录                            #进入目录

- dotnet new uninstall .              #卸载

- dotnet new install .                   #安装

- dotnet new sa -n 你自己的项目名
