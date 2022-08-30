# SnippetAdmin

使用.net 6和react创建，实现角色权限管理，低代码开发接口，任务调度等功能

体验地址：http://39.105.56.25:31000/

用户名：admin

密码：admin

使用github action 实现了基于docker，阿里云镜像仓库和阿里云ecs的 CICD 参考目录 /.github/workflows/docker-image.yml

### 启动方式

- docker-compose up -d
- 使用visual studio打开api，vscode打开web项目，在appsetting中配置mysql地址。code first模式，程序会自动生成数据库以及数据



### 创建模板

- cd api

- dotnet new -i .

- dotnet new sa -n 你自己的项目名


