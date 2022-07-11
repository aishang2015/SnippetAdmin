import { Alert, Button, Checkbox, DatePicker, Form, Input, InputNumber, message, Modal, Pagination, Popover, Select, Space, Switch, Table } from 'antd';
import { useEffect, useState } from 'react';
import { cloneDeep, sortBy } from 'lodash';
import './table.less';
import { DynamicService } from '../../http/requests/dynamic';
import { dateFormat } from '../../common/time';
import { useForm } from 'antd/lib/form/Form';
import moment from 'moment';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faCheckSquare, faEye, faFilter, faPlus, faRotateLeft, faSave, faSearch } from '@fortawesome/free-solid-svg-icons';


export default function TablePage(props: any) {

    const entityName = "JobRecord";

    // 表格
    const [allColumns, setAllColumns] = useState(new Array<any>());
    const [displayColumns, setDisplayColumns] = useState(new Array<any>());
    const [displayProps, setDisplayProps] = useState(new Array<any>());
    const [tableData, setTableData] = useState(new Array<any>());
    const [selectedRowKeys, setSelectedRowKeys] = useState(new Array<any>());

    // 操作数据
    const [batchSelect, setBatchSelect] = useState(false);
    const [searchContent, setSearchContent] = useState(new Array<any>());

    // 排序
    const [sortData, setSortData] = useState(new Array<any>());

    // 分页器
    const [page, setPage] = useState(1);
    const [total, setTotal] = useState(0);
    const [size, setSize] = useState(10);

    // 弹出框可视
    const [filterPopoverVisible, setFilterPopoverVisible] = useState(false);

    // 编辑数据模态框
    const [editDataModal, setEditDataModal] = useState(false);

    // 编辑表单
    const [dataEditForm] = useForm();

    const rowSelection = {
        selectedRowKeys,
        onChange: (selectedRowKeys: React.Key[], selectedRows: any[]) => {
            setSelectedRowKeys(selectedRowKeys);
            setBatchSelect(false);
        }
    };

    useEffect(() => {

        async function initialAsync() {

            let columns = await DynamicService.getColumnInfo(entityName);
            const propertyColumns = columns.data.data.map((d, index) => {
                return {
                    title: d.propertyDescribe,
                    dataIndex: d.propertyName.replace(/^\S/, s => s.toLocaleLowerCase()),
                    align: 'center',
                    width: d.propertyType === 'date' ? "160px" : "100px",
                    sorter: { multiple: index + 1 },
                    isDisplay: true,
                    displayIndex: index + 1,
                    filterType: d.propertyType,
                    render: (text: any, record: any) => {
                        if (d.propertyType === 'date') {
                            return dateFormat(text);
                        } else {
                            return text;
                        }
                    }
                }
            });

            // 初始化过滤组输入属性值
            initFilterGroupInputProperty(propertyColumns);

            setDisplayProps(propertyColumns);

            const allColumns: any[] = [
                {
                    title: '序号', dataIndex: 'num', align: 'center', width: '100px', isDisplay: true, displayIndex: 0
                },
                ...sortBy(propertyColumns, o => o.displayIndex),
                {
                    title: '操作', key: 'operate', align: 'center', isDisplay: true, displayIndex: 999,
                    render: (text: any, record: any) => (
                        <Space size="middle">
                            <a onClick={async () => {
                                let data = await DynamicService.findOne(entityName, record.id);

                                let fieldValues: any = {};
                                propertyColumns.forEach(p => {
                                    if (p.filterType === "date") {
                                        let date = new Date(data.data.data[p.dataIndex]);
                                        fieldValues[p.dataIndex] = moment(date);
                                    } else {
                                        fieldValues[p.dataIndex] = data.data.data[p.dataIndex];
                                    }
                                });

                                dataEditForm.setFieldsValue(fieldValues);

                                setEditDataModal(true);

                            }}>编辑</a>
                            <a onClick={() => {
                                Modal.confirm({
                                    title: '是否删除该条数据?',
                                    onOk: async () => {
                                        await DynamicService.deleteOne(entityName, record.id);
                                        await loadData(page, size);
                                    },
                                });
                            }}>删除</a>
                        </Space>
                    ),
                },
            ];
            setDisplayColumns(allColumns);
            setAllColumns(allColumns);

            await loadData(page, size);
        }

        initialAsync();

    }, []);

    async function loadData(currentpage: number, currentsize: number,
        sorts: any = sortData, filters: any = searchContent) {

        let result = await DynamicService.getMany(entityName, {
            page: currentpage,
            size: currentsize,
            sorts: sorts,
            filters: filters.filter((p: any) => p.compareType !== null)
        });

        // 添加序号
        result.data.data.data.forEach((element: any, index: number) => {
            element.key = element.id;
            element.num = currentsize * (currentpage - 1) + index + 1;
        });
        setTableData(result.data.data.data);
        setTotal(result.data.data.total);
    }

    // 清理选择
    function cleanSelect() {
        setBatchSelect(true);
        setSelectedRowKeys(tableData.map(d => d.key));
    }

    // 列显示设置组
    function getSelectGroup() {
        return displayProps.map((prop: any) => {
            return (<div key={prop.dataIndex} ><Checkbox onChange={() => displayChange(prop)} checked={prop.isDisplay}>{prop.title}</Checkbox></div>);
        });
    }

    //#region 动态过滤条件 属性变化事件

    function onCompareTypeChange(value: any, dataIndex: any) {
        searchContent.find(p => p.propertyName === dataIndex).compareType = value;
        setSearchContent(cloneDeep(searchContent));
    }

    function onInputValueChange(event: any, dataIndex: any) {
        searchContent.find(p => p.propertyName === dataIndex).stringValue = event.target.value;
        setSearchContent(cloneDeep(searchContent));
    }

    function onInputNumberValueChange(value: any, dataIndex: any) {
        searchContent.find(p => p.propertyName === dataIndex).numberValue = value;
        setSearchContent(cloneDeep(searchContent));
    }

    function onDateValueChange(event: any, dataIndex: any) {
        searchContent.find(p => p.propertyName === dataIndex).dateTimeValue = event;
        setSearchContent(cloneDeep(searchContent));
    }

    function onComparePrecisionChange(value: any, dataIndex: any) {
        searchContent.find(p => p.propertyName === dataIndex).datePrecision = value;
        setSearchContent(cloneDeep(searchContent));
    }

    function onSelectValuesChange(value: any, dataIndex: any) {
        searchContent.find(p => p.propertyName === dataIndex).intArrayValue = value;
        setSearchContent(cloneDeep(searchContent));
    }

    function property(property: string, dataIndex: any) {
        return searchContent.find(p => p.propertyName === dataIndex)[property];
    }

    //#endregion

    // 列过滤表单属性初始化
    function initFilterGroupInputProperty(displayProps: any) {
        let array = new Array<any>();
        for (let index = 0; index < displayProps.length; index++) {
            let prop = displayProps[index];
            if (prop.filterType !== undefined && prop.filterType !== null && prop.filterType !== "") {

                let valueType = 1;

                switch (prop.filterType) {
                    case "string":
                        valueType = 1;
                        break;
                    case "number":
                        valueType = 2;
                        break;
                    case "date":
                        valueType = 3;
                        break;
                    case "bool":
                        valueType = 4;
                        break;
                    case "enum":
                        valueType = 5;
                        break;
                    default:
                        break;
                }
                array.push({
                    propertyName: prop.dataIndex,
                    valueType: valueType,
                    compareType: null,
                    stringValue: null,
                    numberValue: null,
                    dateTimeValue: null,
                    datePrecision: null,
                    intArrayValue: []
                });
            }
        }
        setSearchContent(array);
    }

    // 列过滤设置
    function getFilterGroup() {
        let index = 0;
        return (
            <>
                {
                    displayProps.map((prop: any) => {
                        index++;
                        if (prop.filterType !== undefined && prop.filterType !== null && prop.filterType !== "") {
                            switch (prop.filterType) {
                                case "string":
                                    return (
                                        <Input.Group compact style={{ marginTop: '5px' }} key={index}>
                                            <span style={{ lineHeight: 2, marginRight: '10px', width: '20%', textAlign: 'right' }}>{prop.title}（{"字符串"}）:</span>
                                            <Select style={{ width: '30%' }} allowClear placeholder="选择比较方式" value={property("compareType", prop.dataIndex)}
                                                onChange={(value) => onCompareTypeChange(value, prop.dataIndex)}>
                                                <Select.Option value={1}>包含</Select.Option>
                                                <Select.Option value={2}>不包含</Select.Option>
                                                <Select.Option value={3}>等于</Select.Option>
                                                <Select.Option value={4}>不等于</Select.Option>
                                            </Select>
                                            <Input allowClear style={{ width: '45%' }} placeholder="请输入值" value={property("stringValue", prop.dataIndex)}
                                                onChange={(e) => onInputValueChange(e, prop.dataIndex)} />
                                        </Input.Group>
                                    );
                                case "number":
                                    return (
                                        <Input.Group compact style={{ marginTop: '5px' }} key={index}>
                                            <span style={{ lineHeight: 2, marginRight: '10px', width: '20%', textAlign: 'right' }}>{prop.title}（{"数字"}）:</span>
                                            <Select style={{ width: '30%' }} allowClear placeholder="选择比较方式" value={property("compareType", prop.dataIndex)}
                                                onChange={(value) => onCompareTypeChange(value, prop.dataIndex)}>
                                                <Select.Option value={1}>大于</Select.Option>
                                                <Select.Option value={2}>大于等于</Select.Option>
                                                <Select.Option value={3}>小于</Select.Option>
                                                <Select.Option value={4}>小于等于</Select.Option>
                                                <Select.Option value={5}>等于</Select.Option>
                                                <Select.Option value={6}>不等于</Select.Option>
                                            </Select>
                                            <InputNumber style={{ width: '45%' }} placeholder="请输入值" value={property("numberValue", prop.dataIndex)}
                                                onChange={(e) => onInputNumberValueChange(e, prop.dataIndex)} />
                                        </Input.Group>
                                    );
                                case "date":
                                    return (
                                        <Input.Group compact style={{ marginTop: '5px' }} key={index}>
                                            <span style={{ lineHeight: 2, marginRight: '10px', width: '20%', textAlign: 'right' }}>{prop.title}（{"日期"}）:</span>
                                            <Select style={{ width: '15%' }} allowClear placeholder="选择方式"
                                                value={property("compareType", prop.dataIndex)}
                                                onChange={(value) => onCompareTypeChange(value, prop.dataIndex)}>
                                                <Select.Option value={1}>大于</Select.Option>
                                                <Select.Option value={2}>大于等于</Select.Option>
                                                <Select.Option value={3}>小于</Select.Option>
                                                <Select.Option value={4}>小于等于</Select.Option>
                                                <Select.Option value={5}>等于</Select.Option>
                                                <Select.Option value={6}>不等于</Select.Option>
                                                {/* <Select.Option value={7}>日期区间</Select.Option> */}
                                            </Select>
                                            <Select style={{ width: '15%' }} allowClear placeholder="选择精度"
                                                value={property("datePrecision", prop.dataIndex)}
                                                onChange={(value) => onComparePrecisionChange(value, prop.dataIndex)}>
                                                <Select.Option value={1}>年</Select.Option>
                                                <Select.Option value={2}>年月日</Select.Option>
                                                <Select.Option value={3}>年月日时分秒</Select.Option>
                                            </Select>
                                            <DatePicker showTime={property("datePrecision", prop.dataIndex) === 3}
                                                style={{ width: '45%' }} value={property("dateTimeValue", prop.dataIndex)}
                                                onChange={(date, dateString) => onDateValueChange(date, prop.dataIndex)} picker={
                                                    ((): "date" | "year" | "month" => {
                                                        switch (property("datePrecision", prop.dataIndex)) {
                                                            case 1:
                                                                return "year";
                                                            case 2:
                                                                return "date";
                                                            default:
                                                                return "date";
                                                        }
                                                    })()
                                                } />

                                        </Input.Group>
                                    );
                                case "enum":
                                    return (
                                        <Input.Group compact style={{ marginTop: '5px' }} key={index}>
                                            <span style={{ lineHeight: 2, marginRight: '10px', width: '20%', textAlign: 'right' }}>{prop.title}（{"可选值"}）:</span>
                                            <Select style={{ width: '30%' }} allowClear placeholder="选择比较方式" value={property("compareType", prop.dataIndex)}
                                                onChange={(value) => onCompareTypeChange(value, prop.dataIndex)}>
                                                <Select.Option value={1}>包含</Select.Option>
                                                <Select.Option value={2}>不包含</Select.Option>
                                            </Select>
                                            <Select allowClear style={{ width: '45%' }} placeholder="请选择值" mode="multiple" value={property("intArrayValue", prop.dataIndex)}
                                                onChange={(value) => onSelectValuesChange(value, prop.dataIndex)}>
                                                <Select.Option value={1}>选项1</Select.Option>
                                                <Select.Option value={2}>选项2</Select.Option>
                                                <Select.Option value={3}>选项3</Select.Option>
                                            </Select>
                                        </Input.Group>
                                    );
                                case "bool":
                                    return (
                                        <Input.Group compact style={{ marginTop: '5px' }} key={index}>
                                            <span style={{ lineHeight: 2, marginRight: '10px', width: '20%', textAlign: 'right' }}>{prop.title}（{"布尔"}）:</span>
                                            <Select style={{ width: '30%' }} allowClear placeholder="选择比较方式" value={property("compareType", prop.dataIndex)}
                                                onChange={(value) => onCompareTypeChange(value, prop.dataIndex)}>
                                                <Select.Option value={1}>是</Select.Option>
                                                <Select.Option value={2}>否</Select.Option>
                                            </Select>
                                        </Input.Group>
                                    );
                                default:
                                    return (<span key={index}>无法识别的属性类型</span>);
                            }
                        }
                        return (<></>);
                    })
                }
                <div style={{ textAlign: "center", padding: "10px 0" }}>
                    <Button style={{ marginRight: '10px' }} onClick={resetCondition} icon={<FontAwesomeIcon fixedWidth icon={faRotateLeft} />}>重置</Button>
                    <Button style={{ marginRight: '10px' }} onClick={searchData} icon={<FontAwesomeIcon fixedWidth icon={faSearch} />} type="primary">搜索</Button>
                </div>
            </>
        );
    }

    // 重置搜索条件
    function resetCondition() {
        searchContent.forEach(c => {
            c.compareType = null;
            c.stringValue = null;
            c.numberValue = null;
            c.dateTimeValue = null;
            c.datePrecision = null;
            c.boolValue = null;
            c.intArrayValue = [];
        });
        setSearchContent(cloneDeep(searchContent));
    }

    // 搜索数据
    function searchData() {
        loadData(page, size);
        setFilterPopoverVisible(false);
    }

    // 列显示变化
    function displayChange(prop: any) {
        prop.isDisplay = !prop.isDisplay;
        setDisplayProps(JSON.parse(JSON.stringify(displayProps)));

        // 找到隐藏的列，然后再过滤进行显示
        let hiddenProps = displayProps.filter(p => !p.isDisplay).map(p => p.dataIndex);
        let displayColumns = allColumns.filter(c => !hiddenProps.includes(c.dataIndex));
        sortBy(displayColumns, o => o.displayIndex);
        setDisplayColumns(displayColumns);
    }

    // 排序发生变化
    async function tableChange(pagination: any, filters: any, sorter: any, extra: any) {

        let sorts: any = new Array<any>();
        if (sorter instanceof Array) {
            sorter.forEach(d => {
                sorts.push({
                    propertyName: d.field.replace(/^\S/, (s: string) => s.toLocaleUpperCase()),
                    IsAsc: d.order === "ascend"
                });
            });
        } else {
            if (sorter.order === undefined) {
                sorts = null;
            } else {
                sorts.push({
                    propertyName: sorter.field.replace(/^\S/, (s: string) => s.toLocaleUpperCase()),
                    IsAsc: sorter.order === "ascend"
                });
            }
        }
        setSortData(sorts);
        await loadData(page, size, sorts);
    }

    // 分页码发生变化
    async function pageChange(newpage: number, newsize: number) {
        if (size !== newsize) {
            newpage = 1;
        }
        await loadData(newpage, newsize, sortData);
        setPage(newpage);
        setSize(newsize);
    }

    // 添加记录
    async function addRecord() {

        dataEditForm.resetFields();
        setEditDataModal(true);
    }

    // 提交数据
    async function dataEditSubmit(values: any) {

        if (values["id"] !== null && values["id"] != undefined && values["id"] != '') {
            await DynamicService.updateOne(entityName, values);
        } else {
            await DynamicService.addOne(entityName, values);
        }

        await loadData(page, size);
        setEditDataModal(false);
    }

    return (
        <>
            <div style={{ display: 'flex' }}>
                <Button onClick={() => loadData(page, size)} style={{ marginRight: "10px" }} type="primary" icon={<FontAwesomeIcon fixedWidth icon={faRotateLeft} />}>刷新数据</Button>
                <Button onClick={cleanSelect} style={{ marginRight: "10px" }} type="primary" icon={<FontAwesomeIcon fixedWidth icon={faCheckSquare} />}>批量选择</Button>
                <Popover content={getSelectGroup()} title="属性显示" trigger="click" placement="bottomLeft" overlayStyle={{ width: '200px' }}>
                    <Button style={{ marginRight: "10px" }} type="primary" icon={<FontAwesomeIcon fixedWidth icon={faEye} />}>显示隐藏</Button>
                </Popover>
                <Popover content={getFilterGroup()} title="数据过滤" trigger="click" placement="bottomLeft" overlayStyle={{ width: '800px' }}
                    visible={filterPopoverVisible} onVisibleChange={setFilterPopoverVisible}>
                    <Button style={{ marginRight: "10px" }} type="primary" icon={<FontAwesomeIcon fixedWidth icon={faFilter} />}>数据过滤</Button>
                </Popover>
                <Button onClick={() => addRecord()} style={{ marginRight: "10px" }} type="primary" icon={<FontAwesomeIcon fixedWidth icon={faPlus} />}>创建记录</Button>
            </div>
            {batchSelect && <Alert style={{ marginTop: '10px' }} message="所有分页数据都已经被批量选择，请谨慎操作" type="warning" showIcon />}
            <Table style={{ marginTop: '10px' }} columns={displayColumns} pagination={false} rowSelection={rowSelection}
                dataSource={tableData} bordered={true} size='small' onChange={tableChange}></Table>
            <Pagination defaultCurrent={1} pageSize={size} total={total} current={page} style={{ marginTop: '10px' }}
                showSizeChanger={true} onChange={pageChange} showTotal={(total, range) => `共计 ${total} 条数据`} />

            <Modal visible={editDataModal} onCancel={() => setEditDataModal(false)} title="编辑数据" footer={null}>
                <Form form={dataEditForm} onFinish={dataEditSubmit} labelCol={{ span: 6 }}
                    wrapperCol={{ span: 16 }} preserve={false}>
                    {
                        displayProps.map((prop: any) => {
                            switch (prop.filterType) {
                                case "string":
                                    return <Form.Item label={prop.title} name={prop.dataIndex} key={prop.dataIndex}>
                                        <Input placeholder={`请输入${prop.title}`} />
                                    </Form.Item>;
                                case "number":
                                    return prop.dataIndex === 'id' ?
                                        <Form.Item label={prop.title} name={prop.dataIndex} key={prop.dataIndex} hidden>
                                            <InputNumber placeholder={`请输入${prop.title}`} style={{ width: '100%' }} />
                                        </Form.Item>
                                        :
                                        <Form.Item label={prop.title} name={prop.dataIndex} key={prop.dataIndex}>
                                            <InputNumber placeholder={`请输入${prop.title}`} style={{ width: '100%' }} />
                                        </Form.Item>;
                                case "date":
                                    return <Form.Item label={prop.title} name={prop.dataIndex} key={prop.dataIndex}>
                                        <DatePicker placeholder={`请选择${prop.title}`} showTime={true} style={{ width: '100%' }} />
                                    </Form.Item>;
                                case "enum":
                                    return <Form.Item label={prop.title} name={prop.dataIndex} key={prop.dataIndex}>
                                        <Select placeholder={`请选择${prop.title}`} >
                                            <Select.Option value={0}>选项1</Select.Option>
                                            <Select.Option value={1}>选项2</Select.Option>
                                            <Select.Option value={2}>选项3</Select.Option>
                                        </Select>
                                    </Form.Item>;
                                case "bool":
                                    return <Form.Item label={prop.title} name={prop.dataIndex} key={prop.dataIndex}>
                                        <Switch></Switch>
                                    </Form.Item>;
                            }
                        })
                    }
                    <Form.Item wrapperCol={{ offset: 6 }}>
                        <Button icon={<FontAwesomeIcon fixedWidth icon={faSave} />} htmlType="submit">保存</Button>
                    </Form.Item>
                </Form>

            </Modal>
        </>
    );
}