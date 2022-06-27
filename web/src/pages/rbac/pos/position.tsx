import { Button, Divider, Form, Input, InputNumber, Modal, Pagination, Space, Table, Tooltip } from 'antd';
import { RightElement } from '../../../components/right/rightElement';
import { SaveOutlined, PlusOutlined, EditOutlined, DeleteOutlined, SearchOutlined } from "@ant-design/icons";
import './position.less';
import { useEffect, useState } from 'react';
import { useForm } from 'antd/lib/form/Form';
import { PositionService } from '../../../http/requests/position';

export default function () {

    const [page, setPage] = useState(1);
    const [size, setSize] = useState(10);
    const [total, setTotal] = useState(0);
    const [positionModalVisible, setPositionModalVisible] = useState(false);
    const [positionEditForm] = useForm();
    const [positionTableData, setPositionTableData] = useState(new Array<any>());

    const positionTableColumns: any = [
        {
            title: '序号', dataIndex: "num", align: 'center', width: '90px',
            render: (data: any, record: any, index: any) => (
                <span>{(page - 1) * size + 1 + index}</span>
            )
        },
        { title: '名称', dataIndex: "name", align: 'center', width: '220px' },
        { title: '职位编码', dataIndex: "code", align: 'center', width: '220px' },
        { title: '排序', dataIndex: "sorting", align: 'center', width: '100px' },
        {
            title: '操作', key: 'operate', align: 'center', width: '130px',
            render: (text: any, record: any) => (
                <Space size="middle">
                    <RightElement identify="edit-role" child={
                        <>
                            <Tooltip title="编辑职位"><a onClick={() => editPosition(record.id)}><EditOutlined /></a></Tooltip>
                        </>
                    }></RightElement>
                    <RightElement identify="remove-role" child={
                        <>
                            <Tooltip title="删除职位"><a onClick={() => deletePosition(record.id)}><DeleteOutlined /></a></Tooltip>
                        </>
                    }></RightElement>
                </Space>
            ),
        }
    ];

    useEffect(() => {
        getPositions(page, size);
    }, []);// eslint-disable-line react-hooks/exhaustive-deps

    async function getPositions(page: number, size: number) {
        let result = await PositionService.getPositions({ page, size });
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
                await getPositions(page, size);
            }
        })
    }

    async function positionSubmit(values: any) {
        await PositionService.addOrUpdatePosition({
            id: values["id"],
            name: values["name"],
            code: values["code"],
            sorting: values["sorting"]
        });
        await getPositions(page, size);
        setPositionModalVisible(false);
    }

    return (
        <>
            <div id="position-container">
                <RightElement identify="create-role" child={
                    <>
                        <Space style={{ marginTop: "10px" }}>
                            <Button icon={<PlusOutlined />} onClick={createPosition}>创建</Button>
                        </Space>
                        <Divider style={{ margin: "10px 0" }} />
                    </>
                }></RightElement>
                <Table columns={positionTableColumns} dataSource={positionTableData} pagination={false} size="small" ></Table>
                {total > 0 &&
                    <Pagination current={page} total={total} onChange={async (p, s) => { setPage(p); setSize(s); await getPositions(p, s); }} showSizeChanger={false} style={{ marginTop: '10px' }}></Pagination>
                }
            </div>
            <Modal visible={positionModalVisible} title="职位信息" footer={null} onCancel={() => setPositionModalVisible(false)}
                destroyOnClose={true}>
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
                        <Input autoComplete="off2" placeholder="请输入职位名称" />
                    </Form.Item>
                    <Form.Item name="code" label="职位编码" rules={
                        [
                            { required: true, message: "请输入职位编码" },
                            { max: 32, message: "职位编码过长" },
                        ]
                    }>
                        <Input autoComplete="off2" placeholder="请输入职位编码" />
                    </Form.Item>
                    <Form.Item name="sorting" label="排序" rules={
                        [
                            { required: true, message: "请输入排序值" },
                        ]
                    } >
                        <InputNumber style={{ width: '100%' }} autoComplete="off2" placeholder="请输入排序值" />
                    </Form.Item>
                    <Form.Item wrapperCol={{ offset: 6 }}>
                        <Button icon={<SaveOutlined />} htmlType="submit">保存</Button>
                    </Form.Item>
                </Form>
            </Modal>
        </>
    );
}