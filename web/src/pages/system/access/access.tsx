import { faCircleNotch, faFilter } from '@fortawesome/free-solid-svg-icons';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { Button, DatePicker, Form, Input, InputNumber, Modal, Pagination, Table } from 'antd';
import { useForm } from 'antd/lib/form/Form';
import { useEffect, useState } from 'react';
import { dateFormat } from '../../../common/time';
import { AccessLogService } from '../../../http/requests/access';
import 'dayjs/locale/zh-cn';
import dayjs from 'dayjs';
dayjs.locale('zh-cn');

export default function Access() {

    const [page, setPage] = useState(1);
    const [size, setSize] = useState(10);
    const [total, setTotal] = useState(0);

    const [tableData, setTableData] = useState(new Array<any>());

    const [searchModalVisible, setSearchModalVisible] = useState(false);
    const [searchForm] = useForm();

    const [searchObj, setSearchObj] = useState({} as any);

    const tableColumns: any = [
        {
            title: '序号', dataIndex: "num", align: 'center', width: '90px',
            render: (data: any, record: any, index: any) => (
                <span>{(page - 1) * size + 1 + index}</span>
            )
        },
        { title: '请求方法', dataIndex: "method", align: 'center', width: '100px' },
        { title: '方法描述', dataIndex: "description", align: 'center' },
        { title: '请求路径', dataIndex: "path", align: 'center' },
        { title: '用户名', dataIndex: "username", align: 'center', width: '220px' },
        { title: '访问者ip', dataIndex: "remoteIp", align: 'center', width: '100px' },
        {
            title: '访问时间', dataIndex: "accessedTime", align: 'center', width: '180px',
            render: (date: any) => dateFormat(date)
        },
        { title: '响应时间', dataIndex: "elapsedTime", align: 'center', width: '100px' },
        { title: '状态码', dataIndex: "statusCode", align: 'center', width: '90px' }
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
            equalUsername: searchObj.searchUsername,
            equalRemoteIp: searchObj.searchIp
        });
        setTotal(response.data.data.total);
        response.data.data.data.forEach((d: any) => d.key = d.id);
        setTableData(response.data.data.data);
    }

    function openSearchModal() {
        setSearchModalVisible(true);
    }

    function searchSubmit(values: any) {
        let obj: any = {};
        obj.searchMonth = values['month'];
        obj.searchPath = values["path"];
        obj.searchUsername = values["username"];
        obj.searchIp = values["ip"];
        obj.searchElapsed = values["elapsed"];
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

    return (
        <>
            <div style={{ marginBottom: 10 }}>
                <Button style={{ marginRight: '10px' }} icon={<FontAwesomeIcon icon={faCircleNotch} fixedWidth />}
                    onClick={refresh}>刷新</Button>
                <Button style={{ marginRight: '10px' }} icon={<FontAwesomeIcon icon={faFilter} fixedWidth />}
                    onClick={openSearchModal}>查找</Button>
            </div>
            <Table bordered={true} columns={tableColumns} dataSource={tableData} scroll={{ x: 1600 }}
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
            <Modal open={searchModalVisible} onCancel={() => setSearchModalVisible(false)} title="搜索条件" footer={null}>
                <Form form={searchForm} onFinish={searchSubmit} labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} >
                    <Form.Item name="month" label="月份" >
                        <DatePicker picker="month" defaultValue={dayjs(new Date())} style={{ width: "100%" }} />
                    </Form.Item>
                    <Form.Item name="path" label="请求路径">
                        <Input className="searchInput" autoComplete="off" placeholder="请输入请求路径" />
                    </Form.Item>
                    <Form.Item name="username" label="用户名" >
                        <Input className="searchInput" autoComplete="off2" placeholder="请输入用户名" />
                    </Form.Item>
                    <Form.Item name="ip" label="访问者ip" >
                        <Input className="searchInput" autoComplete="off2" placeholder="请输入ip地址" />
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
        </>
    );
}