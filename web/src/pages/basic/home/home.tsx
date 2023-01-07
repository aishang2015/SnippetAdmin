import './home.css';

import React, { useEffect, useRef, useState } from "react";
import { Configuration } from '../../../common/config';
import { Card, Col, Row, Statistic, Typography } from 'antd';

const signalR = require("@microsoft/signalr");

interface home {
    metrics?: any;
}

export default function Home(props: any) {

    const [metrics, setMetrics] = useState<any>({});
    const connection = useRef<any>(
        new signalR.HubConnectionBuilder()
            .withUrl(`${Configuration.BaseUrl}/metrics`, { accessTokenFactory: () => localStorage.getItem("token") })
            .configureLogging(signalR.LogLevel.Warning)
            .build()
    );


    useEffect(() => {
        startFun();
        return () => {
            connection.current.off("ReceiveMetrics");
            connection.current.stop();
        };
    }, []);

    async function startFun() {
        try {

            await connection.current.start();
            connection.current.on("ReceiveMetrics", (message: object) => {
                setMetrics(message);
            });
            connection.current.onclose(startFun);
        } catch (err) {
            console.log(err);
            //setTimeout(startFun, 5000);
        }
    }

    return (
        <div>
            <Typography.Title level={3}>EF Core指标监控</Typography.Title>
            <Row gutter={5} style={{ marginTop: "5px" }}>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="活动 DbContexts" value={metrics?.entityFrameworkCoreMetric?.activeDbContexts} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="查询缓存命中率 (%)" value={metrics?.entityFrameworkCoreMetric?.compiledQueryCacheHitRate} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="全部乐观并发失败" value={metrics?.entityFrameworkCoreMetric?.totalExecutionStrategyOperationFailures} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="每秒乐观并发失败" value={metrics?.entityFrameworkCoreMetric?.executionStrategyOperationFailuresPerSecond} />
                    </Card>
                </Col>
            </Row>
            <Row gutter={5} style={{ marginTop: "5px" }}>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="总计查询次数" value={metrics?.entityFrameworkCoreMetric?.totalQueries} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="每秒查询次数" value={metrics?.entityFrameworkCoreMetric?.queriesPerSecond} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="总计提交次数" value={metrics?.entityFrameworkCoreMetric?.totalSaveChanges} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="每秒数据库执行错误" value={metrics?.entityFrameworkCoreMetric?.executionStrategyOperationFailuresPerSecond} />
                    </Card>
                </Col>
            </Row>
            <Row gutter={5} style={{ marginTop: "5px" }}>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="每秒事务提交次数" value={metrics?.entityFrameworkCoreMetric?.saveChangesPerSecond} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="总计事务提交次数" value={metrics?.entityFrameworkCoreMetric?.totalExecutionStrategyOperationFailures} />
                    </Card>
                </Col>
            </Row>

            <Typography.Title level={3}>运行时监控</Typography.Title>
            <Row gutter={5} style={{ marginTop: "5px" }}>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="自上次 GC 以来 GC 的时间百分比" value={metrics?.runtimeMetric?.timeInGc} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="每个更新间隔分配的字节数" value={metrics?.runtimeMetric?.allocRate} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="相对于所有系统 CPU 资源进程的 CPU 使用率百分比" value={metrics?.runtimeMetric?.cpuUsage} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="已发生的异常数" value={metrics?.runtimeMetric?.exceptionCount} />
                    </Card>
                </Col>
            </Row>
            <Row gutter={5} style={{ marginTop: "5px" }}>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="根据 GC.GetTotalMemory(Boolean) 认为要分配的字节数" value={metrics?.runtimeMetric?.gcHeapSize} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="每个更新间隔发生的第 0 代 GC 次数" value={metrics?.runtimeMetric?.gen0GcCount} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="第 0 代 GC 的字节数" value={metrics?.runtimeMetric?.gen0Size} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="每个更新间隔发生的第 1 代 GC 次数" value={metrics?.runtimeMetric?.gen1GcCount} />
                    </Card>
                </Col>
            </Row>
            <Row gutter={5} style={{ marginTop: "5px" }}>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="第 1 代 GC 的字节数" value={metrics?.runtimeMetric?.gen1Size} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="每个更新间隔发生的第 2 代 GC 次数" value={metrics?.runtimeMetric?.gen2GcCount} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="第 2 代 GC 的字节数" value={metrics?.runtimeMetric?.gen2Size} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="大型对象堆的字节数" value={metrics?.runtimeMetric?.lohSize} />
                    </Card>
                </Col>
            </Row>
            <Row gutter={5} style={{ marginTop: "5px" }}>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="已固定对象堆的字节数" value={metrics?.runtimeMetric?.pohSize} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="GC 堆碎片" value={metrics?.runtimeMetric?.gcFragmentation} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="尝试锁定监视器时出现争用的次数" value={metrics?.runtimeMetric?.monitorLockContentionCount} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="当前活动的 Timer 实例的计数" value={metrics?.runtimeMetric?.activeTimerCount} />
                    </Card>
                </Col>
            </Row>
            <Row gutter={5} style={{ marginTop: "5px" }}>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="加载到进程中的 Assembly 实例的计数" value={metrics?.runtimeMetric?.assemblyCount} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="ThreadPool 中已处理的工作项数" value={metrics?.runtimeMetric?.threadpoolCompletedItemsCount} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="ThreadPool 中当前已加入处理队列的工作项数" value={metrics?.runtimeMetric?.threadpoolQueueLength} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="ThreadPool 中当前存在的线程池线程数" value={metrics?.runtimeMetric?.threadpoolThreadCount} />
                    </Card>
                </Col>
            </Row>
            <Row gutter={5} style={{ marginTop: "5px" }}>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="映射到进程上下文的物理内存量" value={metrics?.runtimeMetric?.workingSet} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="JIT 编译的 IL 的总大小，以字节为单位" value={metrics?.runtimeMetric?.ilBytesJitted} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="JIT 编译的方法数" value={metrics?.runtimeMetric?.methodJittedCount} />
                    </Card>
                </Col>
                <Col span={6}>
                    <Card>
                        <Statistic className='ellips' title="由 GC 所提交的字节数" value={metrics?.gcCommittedBytes?.threadpoolThreadCount} />
                    </Card>
                </Col>
            </Row>
        </div>
    );


}