#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SnippetAdmin/SnippetAdmin.csproj", "SnippetAdmin/"]
COPY ["SnippetAdmin.Base/SnippetAdmin.CommonModel/SnippetAdmin.CommonModel.csproj", "SnippetAdmin.Base/SnippetAdmin.CommonModel/"]
COPY ["SnippetAdmin.Base/SnippetAdmin.DynamicApi/SnippetAdmin.DynamicApi.csproj", "SnippetAdmin.Base/SnippetAdmin.DynamicApi/"]
COPY ["SnippetAdmin.Base/SnippetAdmin.EntityFrameworkCore/SnippetAdmin.EntityFrameworkCore.csproj", "SnippetAdmin.Base/SnippetAdmin.EntityFrameworkCore/"]
COPY ["SnippetAdmin.Base/SnippetAdmin.Orleans/SnippetAdmin.Orleans.csproj", "SnippetAdmin.Base/SnippetAdmin.Orleans/"]
COPY ["SnippetAdmin.Base/SnippetAdmin.PluginBase/SnippetAdmin.PluginBase.csproj", "SnippetAdmin.Base/SnippetAdmin.PluginBase/"]
COPY ["SnippetAdmin.Base/SnippetAdmin.Quartz/SnippetAdmin.Quartz.csproj", "SnippetAdmin.Base/SnippetAdmin.Quartz/"]
COPY ["SnippetAdmin.Endpoint/SnippetAdmin.Endpoint.csproj", "SnippetAdmin.Endpoint/"]
RUN curl https://api.nuget.org/v3/index.json -v
RUN dotnet restore "SnippetAdmin/SnippetAdmin.csproj"
COPY . .
WORKDIR "/src/SnippetAdmin"
RUN dotnet build "SnippetAdmin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SnippetAdmin.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SnippetAdmin.dll"]