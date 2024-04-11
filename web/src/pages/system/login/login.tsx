import { faBroom, faCircleNotch, faRefresh, faSearch, faSignInAlt } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Button, Form, Input, Modal, Pagination, Table, Tag, Tooltip } from "antd";
import { useEffect, useState } from "react";
import { dateFormat } from "../../../common/time";
import { LoginLogService } from "../../../http/requests/system/login-log";
import Title from "antd/es/typography/Title";
import { useToken } from "antd/es/theme/internal";
import { useForm } from "antd/es/form/Form";



export default function Login() {

    // !全局样式    
    const [_, token] = useToken();
    const [modal, contextHolder] = Modal.useModal();

    const [page, setPage] = useState(1);
    const [size, setSize] = useState(10);
    const [total, setTotal] = useState(0);

    const [tableData, setTableData] = useState(new Array<any>());

    const tableColumns: any = [
        {
            title: '序号', dataIndex: "num", align: 'center', width: '90px',
            render: (data: any, record: any, index: any) => (
                <span>{(page - 1) * size + 1 + index}</span>
            )
        },
        { title: '登录名', dataIndex: "username", align: 'center', width: '300px' },
        { title: '登录ip', dataIndex: "remoteIp", align: 'center' },
        {
            title: '登录时间', dataIndex: "accessedTime", align: 'center', width: '180px',
            render: (date: any) => dateFormat(date)
        },
        {
            title: '是否成功', dataIndex: "isSucceed", align: 'center', width: '100px',
            render: (date: any) => date ? <Tag color="success">是</Tag> : <Tag color="error">否</Tag>
        },
    ];

    const [searchObject, setSearchObject] = useState<any>({});

    useEffect(() => {
        initAsync();
    }, [page, size, searchObject]);// eslint-disable-line react-hooks/exhaustive-deps

    async function initAsync() {
        let response = await LoginLogService.getLoginLogs({
            page: page,
            size: size,
            equalUsername: searchObject.loginName,
            sorts: [
                {
                    propertyName: 'AccessedTime',
                    isAsc: false
                }
            ]
        });
        setTotal(response.data.data.total);
        response.data.data.data.forEach((d: any) => d.key = d.id);
        setTableData(response.data.data.data);
    }

    //#region  搜索

    const [searchModalVisible, setSearchModalVisible] = useState(false);
    const [searchForm] = useForm();

    async function openSearchModal() {
        setSearchModalVisible(true);
    }

    async function searchSubmit(values: any) {
        setPage(1);
        setSearchObject({
            loginName: values["loginName"],
        });
        setSearchModalVisible(false);
    }

    //#endregion

    return (
        <>
            {contextHolder}

            {/* 操作 */}
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div style={{ display: 'flex', alignItems: 'center' }}>
                    <FontAwesomeIcon icon={faSignInAlt} style={{ marginRight: '8px', fontSize: "18px" }} />
                    <Title level={4} style={{ marginBottom: 0 }}>登录日志</Title>
                </div>
                <div>
                    <Tooltip title="查找">
                        <Button type="primary" icon={<FontAwesomeIcon icon={faSearch} />} style={{ marginRight: '4px' }} onClick={openSearchModal} />
                    </Tooltip>
                    <Tooltip title="重置条件并搜索" color={token.colorPrimary}>
                        <Button type="primary" icon={<FontAwesomeIcon icon={faBroom} />} style={{ marginRight: '4px' }}
                            onClick={() => { setPage(1); setSearchObject({}); }} />
                    </Tooltip>
                    <Tooltip title="刷新">
                        <Button type="primary" icon={<FontAwesomeIcon icon={faRefresh} />} style={{ marginRight: '4px' }}
                            onClick={initAsync} />
                    </Tooltip>
                </div>
            </div>

            <Table style={{ marginBottom: 10, marginTop: '10px' }} bordered={true} size="small" columns={tableColumns} dataSource={tableData} scroll={{ x: 930 }}
                pagination={false}  ></Table>
            {total > 0 &&
                <Pagination current={page} total={total} showSizeChanger={true} style={{ marginTop: '10px' }}
                    onChange={async (p, s) => { setPage(p); setSize(s); await initAsync(); }}></Pagination>
            }

            {/*搜索模态框*/}
            <Modal open={searchModalVisible} onCancel={() => setSearchModalVisible(false)}
                footer={null} title="搜索">
                <Form form={searchForm} onFinish={searchSubmit} labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} preserve={false}>
                    <Form.Item name="loginName" label="登录名">
                        <Input autoComplete="off" placeholder="请输入登录名" />
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6 }}>
                        <Button type='primary' icon={<FontAwesomeIcon fixedWidth icon={faSearch} />}
                            htmlType="submit">搜索</Button>
                    </Form.Item>
                </Form>
            </Modal>
        </>
    );
}