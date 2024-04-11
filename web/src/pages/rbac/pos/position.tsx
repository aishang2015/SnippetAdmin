import { Button, Divider, Form, Input, InputNumber, Modal, Pagination, Space, Table, Tooltip } from 'antd';
import { RightElement } from '../../../components/right/rightElement';
import './position.css';
import { useEffect, useState } from 'react';
import { useForm } from 'antd/lib/form/Form';
import { PositionService } from '../../../http/requests/rbac/position';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBroom, faCircleNotch, faEdit, faPlus, faRefresh, faSave, faSearch, faTrash, faUserFriends } from '@fortawesome/free-solid-svg-icons';
import Title from 'antd/es/typography/Title';
import { useToken } from 'antd/es/theme/internal';

export default function Position() {

    // !全局样式    
    const [_, token] = useToken();
    const [modal, contextHolder] = Modal.useModal();

    const [page, setPage] = useState(1);
    const [size, setSize] = useState(10);
    const [total, setTotal] = useState(0);
    const [positionModalVisible, setPositionModalVisible] = useState(false);
    const [positionEditForm] = useForm();
    const [positionTableData, setPositionTableData] = useState(new Array<any>());

    const [isLoading, setIsLoading] = useState(false);

    const [searchObject, setSearchObject] = useState<any>({});

    const positionTableColumns: any = [
        {
            title: '序号', dataIndex: "num", align: 'center', width: '90px',
            render: (data: any, record: any, index: any) => (
                <span>{(page - 1) * size + 1 + index}</span>
            )
        },
        { title: '名称', dataIndex: "name", align: 'center' },
        { title: '职位编码', dataIndex: "code", align: 'center', width: '220px' },
        { title: '排序', dataIndex: "sorting", align: 'center', width: '100px' },
        {
            title: '操作', key: 'operate', align: 'center', width: '130px',
            render: (text: any, record: any) => (
                <div>
                    <RightElement identify="edit-role" child={
                        <>
                            <Tooltip title="编辑职位">
                                <Button type='link' style={{ padding: '4px 6px' }} onClick={() => editPosition(record.id)}><FontAwesomeIcon icon={faEdit} /></Button>
                            </Tooltip>
                        </>
                    }></RightElement>
                    <RightElement identify="remove-role" child={
                        <>
                            <Tooltip title="删除职位">
                                <Button type='link' style={{ padding: '4px 6px' }} onClick={() => deletePosition(record.id)}><FontAwesomeIcon icon={faTrash} /></Button>
                            </Tooltip>
                        </>
                    }></RightElement>
                </div>
            ),
        }
    ];

    useEffect(() => {
        getPositions();
    }, []);// eslint-disable-line react-hooks/exhaustive-deps

    useEffect(() => {
        getPositions();
    }, [page, size, searchObject]);// eslint-disable-line react-hooks/exhaustive-deps

    async function getPositions() {
        let result = await PositionService.getPositions({ page, size, name: searchObject.name });
        setPositionTableData(result.data.data.data);
        setTotal(result.data.data.total);
    }

    function createPosition() {
        setPositionModalVisible(true);
    }

    async function editPosition(id: any) {
        let position = await PositionService.getPosition({ id });
        positionEditForm.setFieldsValue({
            id: position.data.data.id,
            name: position.data.data.name,
            code: position.data.data.code,
            sorting: position.data.data.sorting
        });
        setPositionModalVisible(true);
    }

    function deletePosition(id: any) {
        Modal.confirm({
            title: '是否删除该角色?',
            onOk: async () => {
                await PositionService.deletePosition({ id });
                await getPositions();
            }
        })
    }

    async function positionSubmit(values: any) {
        try {
            setIsLoading(true);
            await PositionService.addOrUpdatePosition({
                id: values["id"],
                name: values["name"],
                code: values["code"],
                sorting: values["sorting"]
            });
            await getPositions();
            setPositionModalVisible(false);
        }
        finally {
            setIsLoading(false);
        }
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
            name: values["posName"],
        });
        setSearchModalVisible(false);
    }

    //#endregion

    return (
        <>
            {contextHolder}

            {/* 操作 */}
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: "6px" }}>
                <div style={{ display: 'flex', alignItems: 'center' }}>
                    <FontAwesomeIcon icon={faUserFriends} style={{ marginRight: '8px', fontSize: "18px" }} />
                    <Title level={4} style={{ marginBottom: 0 }}>职位信息</Title>
                </div>
                <div>
                    <Tooltip title="搜索" color={token.colorPrimary}>
                        <Button type="primary" icon={<FontAwesomeIcon icon={faSearch} />} style={{ marginRight: '4px' }} onClick={openSearchModal} />
                    </Tooltip>
                    <Tooltip title="重置条件并搜索" color={token.colorPrimary}>
                        <Button type="primary" icon={<FontAwesomeIcon icon={faBroom} />} style={{ marginRight: '4px' }}
                            onClick={() => { setPage(1); setSearchObject({}); }} />
                    </Tooltip>
                    <Tooltip title="刷新" color={token.colorPrimary}>
                        <Button type="primary" icon={<FontAwesomeIcon icon={faRefresh} />} style={{ marginRight: '4px' }} onClick={getPositions} />
                    </Tooltip>
                    <RightElement identify="add-position" child={
                        <>
                            <Tooltip title="新建订单" color={token.colorPrimary}>
                                <Button type="primary" icon={<FontAwesomeIcon icon={faPlus} />} onClick={createPosition} className="mr-4" />
                            </Tooltip>
                        </>
                    }></RightElement>
                </div>
            </div>

            <Divider style={{ margin: '14px 0' }} />


            <div id="position-container">

                <Table columns={positionTableColumns} dataSource={positionTableData} pagination={false} size="small" ></Table>
                {total > 0 &&
                    <Pagination current={page} total={total} onChange={async (p, s) => { setPage(p); setSize(s); }} showSizeChanger={false} style={{ marginTop: '10px' }}></Pagination>
                }
            </div>
            <Modal open={positionModalVisible} title="职位信息" footer={null} onCancel={() => setPositionModalVisible(false)}
                destroyOnClose={true} maskClosable={false}>
                <Form form={positionEditForm} onFinish={positionSubmit} labelCol={{ span: 6 }}
                    wrapperCol={{ span: 16 }} preserve={false}>
                    <Form.Item name="id" hidden >
                        <Input />
                    </Form.Item>
                    <Form.Item name="name" label="职位名称" rules={
                        [
                            { required: true, message: "请输入职位名称" },
                            { max: 30, message: "职位名称过长" },
                        ]
                    }>
                        <Input autoComplete="off" placeholder="请输入职位名称" />
                    </Form.Item>
                    <Form.Item name="code" label="职位编码" rules={
                        [
                            { required: true, message: "请输入职位编码" },
                            { max: 32, message: "职位编码过长" },
                        ]
                    }>
                        <Input autoComplete="off" placeholder="请输入职位编码" />
                    </Form.Item>
                    <Form.Item name="sorting" label="排序" rules={
                        [
                            { required: true, message: "请输入排序值" },
                        ]
                    } >
                        <InputNumber style={{ width: '100%' }} autoComplete="off" placeholder="请输入排序值" />
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6 }}>
                        <Button type='primary' icon={<FontAwesomeIcon fixedWidth icon={faSave} />} htmlType="submit" loading={isLoading}>保存</Button>
                    </Form.Item>
                </Form>
            </Modal>

            {/*搜索模态框*/}
            <Modal open={searchModalVisible} onCancel={() => setSearchModalVisible(false)}
                footer={null} title="搜索">
                <Form form={searchForm} onFinish={searchSubmit} labelCol={{ span: 6 }} wrapperCol={{ span: 16 }} preserve={false}>
                    <Form.Item name="posName" label="职位名称">
                        <Input autoComplete="off" placeholder="请输入职位名称" />
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6 }}>
                        <Button type='primary' icon={<FontAwesomeIcon fixedWidth icon={faSearch} />}
                            htmlType="submit" loading={isLoading}>搜索</Button>
                    </Form.Item>
                </Form>
            </Modal>
        </>
    );
}