import { faCircleNotch, faDatabase, faEdit, faFilter, faPlug, faRefresh, faSearch } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Button, Card, DatePicker, Form, Input, InputNumber, Modal, Pagination, Select, Table, Tag, Tooltip, Typography } from 'antd';
import { useForm } from 'antd/lib/form/Form';
import { useEffect, useState } from 'react';
import { dateFormat } from '../../../common/time';
import { AccessLogService, GetDataDetailLogsOutputModel } from '../../../http/requests/system/access';
import 'dayjs/locale/zh-cn';
import dayjs from 'dayjs';
import { UserService } from '../../../http/requests/rbac/user';
import Title from 'antd/es/typography/Title';
import { RightElement } from '../../../components/right/rightElement';
import { useToken } from 'antd/es/theme/internal';
dayjs.locale('zh-cn');

export default function Access() {

    // !全局样式    
    const [_, token] = useToken();
    const [modal, contextHolder] = Modal.useModal();

    const [page, setPage] = useState(1);
    const [size, setSize] = useState(10);
    const [total, setTotal] = useState(0);

    const [tableData, setTableData] = useState(new Array<any>());

    const [searchModalVisible, setSearchModalVisible] = useState(false);
    const [searchForm] = useForm();

    const [moduleNames, setModuleNames] = useState<Array<string>>([]);
    const [methodNames, setMethodNames] = useState<Array<string>>([]);
    const [methodDic, setMethodDic] = useState<any>();

    const [userDic, setUserDic] = useState<any>([]);

    const [searchObj, setSearchObj] = useState({} as any);

    const tableColumns: any = [
        {
            title: '序号', dataIndex: "num", align: 'center', width: '80px', fixed: 'left',
            render: (data: any, record: any, index: any) => (
                <span>{(page - 1) * size + 1 + index}</span>
            )
        },
        { title: '模块', dataIndex: "module", align: 'center', width: '200px' },
        { title: '方法', dataIndex: "method", align: 'center' },
        { title: '路径', dataIndex: "path", align: 'center', width: '200px' },
        { title: '用户姓名', dataIndex: "realName", align: 'center', width: '120px' },
        { title: '访问者ip', dataIndex: "remoteIp", align: 'center', width: '100px' },
        {
            title: '访问时间', dataIndex: "accessedTime", align: 'center', width: '180px',
            render: (date: any) => dateFormat(date)
        },
        { title: '状态码', dataIndex: "statusCode", align: 'center', width: '90px' },
        { title: '耗时', dataIndex: "elapsedTime", align: 'center', width: '80px', fixed: 'right', },

        {
            title: '操作', dataIndex: "operate", align: 'center', width: '80px', fixed: 'right',
            render: (_data: any, record: any) => (
                <div>
                    <Tooltip title="数据日志" color={token.colorPrimary}>
                        <Button type='link' style={{ padding: '4px 6px' }} onClick={() => viewDataLog(record.traceIdentifier)}>
                            <FontAwesomeIcon fixedWidth icon={faDatabase} /></Button>
                    </Tooltip>
                </div>
            ),
        }
    ];

    useEffect(() => {
        initAsync();
    }, [page, size, searchObj]);// eslint-disable-line react-hooks/exhaustive-deps

    async function initAsync() {
        let response = await AccessLogService.getAccessLogs({
            page: page,
            size: size,
            sorts: [
                {
                    propertyName: 'AccessedTime',
                    isAsc: false
                }
            ],
            month: searchObj.searchMonth ? searchObj.searchMonth.format('YYYYMM') : dayjs().format('YYYYMM'),
            lowerElapsedTime: searchObj.searchElapsed,
            equalPath: searchObj.searchPath,
            equalUserId: searchObj.searchUserId,
            equalRemoteIp: searchObj.searchIp,
            equalModule: searchObj.searchModule,
            equalMethod: searchObj.searchMethod
        });
        setTotal(response.data.data.total);
        response.data.data.data.forEach((d: any) => d.key = d.id);
        setTableData(response.data.data.data);

        let moduleMethodDicResponse = await AccessLogService.GetModuleMethodDic();
        setModuleNames(Object.keys(moduleMethodDicResponse.data.data));
        setMethodDic(moduleMethodDicResponse.data.data);
    }

    async function openSearchModal() {
        setSearchModalVisible(true);
        let userDic = await UserService.getUserDic();
        setUserDic(userDic.data.data);
    }

    function handleModulesChange(moduleName: string) {
        setMethodNames(methodDic[moduleName] ?? []);
    }

    function searchSubmit(values: any) {
        let obj: any = {};
        obj.searchMonth = values['month'];
        obj.searchPath = values["path"];
        obj.searchUserId = values["userId"];
        obj.searchIp = values["ip"];
        obj.searchElapsed = values["elapsed"];
        obj.searchModule = values["method"]?.module;
        obj.searchMethod = values["method"]?.method;
        setSearchObj(obj);
        setSearchModalVisible(false);
    }

    function resetSearchForm() {
        setSearchObj({});
        searchForm.resetFields();
    }

    async function refresh() {
        await initAsync();
    }

    //#region 数据日志

    const [dataLogVisible, setDataLogVisible] = useState(false);
    const [dataLogData, setDataLogData] = useState<Array<GetDataDetailLogsOutputModel>>([]);

    async function viewDataLog(identify: string) {
        let response = await AccessLogService.GetDataDetailLogs({
            traceIdentifier: identify
        });
        setDataLogData(response.data.data);
        setDataLogVisible(true);
    }

    //#endregion

    return (
        <>
            {contextHolder}

            {/* 操作 */}
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div style={{ display: 'flex', alignItems: 'center' }}>
                    <FontAwesomeIcon icon={faPlug} style={{ marginRight: '8px', fontSize: "18px" }} />
                    <Title level={4} style={{ marginBottom: 0 }}>接口日志</Title>
                </div>
                <div>
                    <Tooltip title="查找">
                        <Button type="primary" icon={<FontAwesomeIcon icon={faSearch} />} style={{ marginRight: '4px' }} onClick={openSearchModal} />
                    </Tooltip>
                    <Tooltip title="刷新">
                        <Button type="primary" icon={<FontAwesomeIcon icon={faRefresh} />} style={{ marginRight: '4px' }} onClick={refresh} />
                    </Tooltip>
                </div>
            </div>

            <Table style={{ marginTop: '10px' }} bordered={true} columns={tableColumns} dataSource={tableData} scroll={{ x: 1400 }}
                expandable={{
                    expandedRowRender: record =>
                        <div>
                            {record.requestBody &&
                                <>
                                    <p style={{ margin: 0 }}>请求数据：</p>
                                    <pre style={{ margin: 0 }}>{JSON.stringify(JSON.parse(record.requestBody), null, 2)}</pre>
                                </>

                            }
                            {record.responseBody &&
                                <>
                                    <p style={{ margin: 0 }}>响应数据：</p>
                                    <pre style={{ margin: 0 }}>{JSON.stringify(JSON.parse(record.responseBody), null, 2)}</pre>
                                </>
                            }
                        </div>
                }}
                pagination={false} size="small" ></Table>
            {total > 0 &&
                <Pagination current={page} total={total} showSizeChanger={true} style={{ marginTop: '10px' }}
                    onChange={async (p, s) => { setPage(p); setSize(s); }}></Pagination>
            }
            <Modal open={searchModalVisible} onCancel={() => setSearchModalVisible(false)} title="搜索条件" footer={null}
                width={600}>
                <Form form={searchForm} onFinish={searchSubmit} labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} >
                    <Form.Item label="方法">
                        <Input.Group compact>
                            <Form.Item
                                name={['method', 'module']}
                                noStyle>
                                <Select allowClear defaultValue={null} placeholder="选择模块"
                                    style={{ width: 140 }} onChange={handleModulesChange} options={moduleNames.map((module) => ({ label: module, value: module }))} />
                            </Form.Item>
                            <Form.Item name={['method', 'method']} noStyle>
                                <Select allowClear defaultValue={null} placeholder="选择方法"
                                    style={{ width: 228 }} options={methodNames.map((method) => ({ label: method, value: method }))} />
                            </Form.Item>
                        </Input.Group>
                    </Form.Item>
                    <Form.Item name="path" label="请求路径">
                        <Input className="searchInput" autoComplete="off" placeholder="请输入请求路径" />
                    </Form.Item>
                    <Form.Item name="userId" label="用户" >
                        <Select allowClear defaultValue={null} placeholder="请选择用户"
                            options={userDic.map((user: any) => ({ label: user.realName, value: user.userId }))} />
                    </Form.Item>
                    <Form.Item name="ip" label="访问者ip" >
                        <Input className="searchInput" autoComplete="" placeholder="请输入ip地址" />
                    </Form.Item>
                    <Form.Item name="elapsed" label="最小请求时长">
                        <InputNumber placeholder="请输入最小请求时长" style={{ width: '100%' }} />
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6, span: 14 }}>
                        <Button type="primary" htmlType="submit">确定</Button>
                        <Button style={{ marginLeft: '10px' }} onClick={() => resetSearchForm()}>重置</Button>
                        <Button style={{ marginLeft: '10px' }} onClick={() => setSearchModalVisible(false)}>取消</Button>
                    </Form.Item>
                </Form>
            </Modal>

            <Modal open={dataLogVisible} onCancel={() => setDataLogVisible(false)} title="数据日志" footer={null}
                width={800}>
                {dataLogData.map(d => {
                    return <Card style={{ marginBottom: '8px' }}>
                        <Title level={5}>{d.entityName}
                            {d.operation === "删除数据" && <Tag color="red">{d.operation}</Tag>}
                            {d.operation === "修改数据" && <Tag color="orange">{d.operation}</Tag>}
                            {d.operation === "添加数据" && <Tag color="green">{d.operation}</Tag>}
                        </Title>

                        <div style={{ display: 'flex' }}>
                            <div style={{ flex: "1", fontWeight: 'bold' }}>属性名</div>
                            <div style={{ flex: "1", fontWeight: 'bold' }}>旧值</div>
                            <div style={{ flex: "1", fontWeight: 'bold' }}>新值</div>
                        </div>
                        {d.dataDetailList?.map(dd => {
                            return <>
                                <div style={{ display: 'flex' }}>
                                    <div style={{ flex: "1" }}>{dd.propertyName}</div>
                                    <div style={{ flex: "1", wordBreak: 'break-all' }}>{dd.oldValue}</div>
                                    <div style={{ flex: "1", wordBreak: 'break-all' }}>{dd.newValue}</div>
                                </div>
                            </>;
                        })}
                    </Card>;
                })}
            </Modal>
        </>
    );
}