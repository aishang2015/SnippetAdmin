import './home.css';

import dayjs from 'dayjs';
import React, { useEffect, useRef, useState } from "react";
import { Configuration } from '../../../common/config';
import { Card, Col, Row, Statistic, Typography } from 'antd';
import { round, takeRight } from "lodash";
import ReactECharts from 'echarts-for-react';

const signalR = require("@microsoft/signalr");

interface home {
    metrics?: any;
}

export default function Home(props: any) {

    // cpu使用率
    const [cpuUsageData, setCpuUsageData] = useState<Array<any>>([]);
    const [cpuUsageOption, setCpuUsageOption] = useState<any>({});

    // GC数据
    const [gcOption, setGcOption] = useState<any>({});

    // 物理内存使用
    const [memoryData, setMemoryData] = useState<Array<any>>([]);
    const [memoryOption, setMemoryOption] = useState<any>({});

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

                let data = message as any;

                // cpu
                cpuUsageData.push({
                    cpu: round(Number(data['runtimeMetric']['cpuUsage']) * 100),
                    time: dayjs().format("mm:ss")
                });
                let sliceData = takeRight(cpuUsageData, 20);
                setCpuUsageData(sliceData);
                setCpuUsageOption({
                    tooltip: {
                        formatter: '{b} : {c}%'
                    },
                    series: [
                        {
                            name: 'cpu使用率',
                            type: 'gauge',
                            detail: {
                                valueAnimation: true,
                                formatter: '{value}%'
                            },
                            data: [
                                {
                                    value: round(Number(data['runtimeMetric']['cpuUsage']) * 100),
                                    name: 'cpu使用率'
                                }
                            ]
                        }
                    ]
                });

                // gc
                setGcOption({
                    tooltip: {
                        trigger: 'item'
                    },
                    legend: {
                        top: '5%',
                        left: 'auto',
                        orient: 'vertical',
                        textStyle: {
                            color: 'inherit'
                        }
                    },
                    series: [
                        {
                            type: 'pie',
                            radius: ['40%', '70%'],
                            itemStyle: {
                                borderRadius: 10,
                                borderColor: '#fff',
                                borderWidth: 2
                            },
                            label: {
                                show: false,
                                position: 'center'
                            },
                            emphasis: {
                                label: {
                                    show: true,
                                    fontWeight: 'bold'
                                }
                            },
                            labelLine: {
                                show: false
                            },
                            data: [
                                { value: Number(data['runtimeMetric']['gen0Size']), name: '第0代' },
                                { value: Number(data['runtimeMetric']['gen1Size']), name: '第1代' },
                                { value: Number(data['runtimeMetric']['gen2Size']), name: '第2代' },
                                { value: Number(data['runtimeMetric']['lohSize']), name: '大对象堆' },
                            ]
                        }
                    ]

                });

                // 内存
                memoryData.push({
                    cpu: round(Number(data['runtimeMetric']['workingSet'])),
                    time: dayjs().format("mm:ss")
                });
                sliceData = takeRight(memoryData, 10);
                setMemoryData(sliceData);
                setMemoryOption({
                    xAxis: {
                        type: 'category',
                        boundaryGap: false,
                        data: sliceData.map(d => d.time)
                    },
                    tooltip: {
                        trigger: 'axis'
                    },
                    yAxis: {
                        type: 'value',
                    },
                    series: [
                        {
                            data: sliceData.map(d => d.cpu),
                            type: 'line',
                            areaStyle: {}
                        }
                    ]
                });
            });
            connection.current.onclose(startFun);
        } catch (err) {
            console.log(err);
            //setTimeout(startFun, 5000);
        }
    }

    return (
        <div>
            <div style={{ display: 'flex' }}>
                <div style={{ flex: 1 }}>
                    <Typography.Title level={4}>CPU使用率</Typography.Title>
                    <ReactECharts option={cpuUsageOption} />
                </div>
                <div style={{ flex: 1 }}>
                    <Typography.Title level={4}>内存使用</Typography.Title>
                    <ReactECharts option={memoryOption} />
                </div>
                <div style={{ flex: 1 }}>
                    <Typography.Title level={4}>GC统计</Typography.Title>
                    <ReactECharts option={gcOption} />
                </div>
            </div>
            <Typography.Title level={4}>程序指标</Typography.Title>
            <div style={{ display: 'flex', gap: '10px', marginBottom: '10px' }}>
                <div style={{ flex: 1 }}>
                    <Card size='small'>
                        <Statistic className='ellips' title="加载的 Assembly 数量" value={metrics?.runtimeMetric?.assemblyCount} />
                    </Card>
                </div>
                <div style={{ flex: 1 }}>
                    <Card size='small'>
                        <Statistic className='ellips' title="活动的 Timer 的数量" value={metrics?.runtimeMetric?.activeTimerCount} />
                    </Card>
                </div>
                <div style={{ flex: 1 }}>
                    <Card size='small'>
                        <Statistic className='ellips' title="线程池中的线程数" value={metrics?.runtimeMetric?.threadpoolThreadCount} />
                    </Card>
                </div>
            </div>
            <Typography.Title level={4}>数据库指标</Typography.Title>
            <div style={{ display: 'flex', gap: '10px', marginBottom: '10px' }}>
                <div style={{ flex: 1 }}>
                    <Card size='small'>
                        <Statistic className='ellips' title="活动中DbContexts" value={metrics?.entityFrameworkCoreMetric?.activeDbContexts} />
                    </Card>
                </div>
                <div style={{ flex: 1 }}>
                    <Card size='small'>
                        <Statistic className='ellips' title="每秒查询次数" value={metrics?.entityFrameworkCoreMetric?.queriesPerSecond} />
                    </Card>
                </div>
                <div style={{ flex: 1 }}>
                    <Card size='small'>
                        <Statistic className='ellips' title="总计查询次数" value={metrics?.entityFrameworkCoreMetric?.totalQueries} />
                    </Card>
                </div>
                <div style={{ flex: 1 }}>
                    <Card size='small'>
                        <Statistic className='ellips' title="每秒提交次数" value={metrics?.entityFrameworkCoreMetric?.saveChangesPerSecond} />
                    </Card>
                </div>
                <div style={{ flex: 1 }}>
                    <Card size='small'>
                        <Statistic className='ellips' title="总计提交次数" value={metrics?.entityFrameworkCoreMetric?.totalSaveChanges} />
                    </Card>
                </div>
            </div>
            <Typography.Title level={4}>HTTP请求</Typography.Title>
            <div style={{ display: 'flex', gap: '10px', marginBottom: '10px' }}>
                <div style={{ flex: 1 }}>
                    <Card size='small'>
                        <Statistic className='ellips' title="执行中的请求" value={metrics?.hostingMetric?.currentRequests} />
                    </Card>
                </div>
                <div style={{ flex: 1 }}>
                    <Card size='small'>
                        <Statistic className='ellips' title="每秒请求数" value={metrics?.hostingMetric?.requestRate} />
                    </Card>
                </div>
                <div style={{ flex: 1 }}>
                    <Card size='small'>
                        <Statistic className='ellips' title="全部请求数" value={metrics?.hostingMetric?.totalRequests} />
                    </Card>
                </div>
            </div>
        </div>
    );


}