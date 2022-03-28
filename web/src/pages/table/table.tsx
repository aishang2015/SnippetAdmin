import { Alert, Button, Checkbox, DatePicker, Input, InputNumber, Pagination, Popover, Select, Space, Table } from 'antd';
import { useEffect, useState } from 'react';
import { cloneDeep, sortBy } from 'lodash';
import { FilterOutlined, EyeOutlined, CheckSquareOutlined, SearchOutlined, UndoOutlined } from "@ant-design/icons";
import './table.less';
import { DynamicService } from '../../http/requests/dynamic';


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

    // 分页器
    const [page, setPage] = useState(1);
    const [total, setTotal] = useState(0);
    const [size, setSize] = useState(10);

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
                    width: '100px',
                    sorter: { multiple: index + 1 },
                    isDisplay: true,
                    displayIndex: index + 1,
                }
            });

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
                            <a>编辑</a>
                            <a>删除</a>
                        </Space>
                    ),
                },
            ];
            setDisplayColumns(allColumns);
            setAllColumns(allColumns);

            // let data = new Array<any>();
            // for (let i = 0; i < 15; i++) {
            //     data.push({
            //         key: i + 1, num: i + 1, p1: `p1-${i + 1}`, p2: `p2-${i + 1}`, p3: `p3-${i + 1}`, p4: `p4-${i + 1}`, p5: `p5-${i + 1}`, p6: `p6-${i + 1}`, p7: `p7-${i + 1}`
            //     });
            // }
            await loadData(page, size);

            // 初始化过滤组输入属性值
            initFilterGroupInputProperty(allColumns);

        }

        initialAsync();

    }, []);

    async function loadData(currentpage: number, currentsize: number, sorts: any = null) {

        let result = await DynamicService.getMany(entityName, {
            page: currentpage,
            size: currentsize,
            sorts: sorts
        });

        // 添加序号
        result.data.data.data.forEach((element: any, index: number) => {
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
        searchContent.find(p => p.key === dataIndex).compareType = value;
        setSearchContent(cloneDeep(searchContent));
    }

    function onInputValueChange(event: any, dataIndex: any) {
        searchContent.find(p => p.key === dataIndex).inputValue = event.target.value;
        setSearchContent(cloneDeep(searchContent));
    }

    function onInputNumberValueChange(value: any, dataIndex: any) {
        searchContent.find(p => p.key === dataIndex).inputValue = value;
        setSearchContent(cloneDeep(searchContent));
    }

    function onComparePrecisionChange(value: any, dataIndex: any) {
        searchContent.find(p => p.key === dataIndex).comparePrecision = value;
        setSearchContent(cloneDeep(searchContent));
    }

    function onSelectValuesChange(value: any, dataIndex: any) {
        searchContent.find(p => p.key === dataIndex).selectValues = value;
        setSearchContent(cloneDeep(searchContent));
    }

    function onDateValueChange(event: any, dataIndex: any) {
        searchContent.find(p => p.key === dataIndex).inputValue = event;
        setSearchContent(cloneDeep(searchContent));
    }

    function onRangeValueChange(event: any, dataIndex: any) {
        searchContent.find(p => p.key === dataIndex).rangeValue = event;
        setSearchContent(cloneDeep(searchContent));
    }

    function property(property: string, dataIndex: any) {
        return searchContent.find(p => p.key === dataIndex)[property];
    }

    //#endregion

    // 列过滤表单属性初始化
    function initFilterGroupInputProperty(displayProps: any) {
        let array = new Array<any>();
        for (let index = 0; index < displayProps.length; index++) {
            let prop = displayProps[index];
            if (prop.filterType !== undefined && prop.filterType !== null && prop.filterType !== "") {
                switch (prop.filterType) {
                    case "string":
                        array.push({
                            key: prop.dataIndex,
                            compareType: null,
                            inputValue: null,
                        });
                        break;
                    case "number":
                        array.push({
                            key: prop.dataIndex,
                            compareType: null,
                            inputValue: null,
                        });
                        break;
                    case "date":
                        array.push({
                            key: prop.dataIndex,
                            compareType: null,
                            comparePrecision: null,
                            inputValue: null,
                            rangeValue: [],
                        });
                        break;
                    case "dictionary":
                        array.push({
                            key: prop.dataIndex,
                            compareType: null,
                            selectValues: [],
                        });
                        break;
                    case "bool":
                        array.push({
                            key: prop.dataIndex,
                            compareType: null
                        });
                        break;
                    default:
                        break;
                }
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
                                            <Input allowClear style={{ width: '45%' }} placeholder="请输入值" value={searchContent[index].inputValue}
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
                                                <Select.Option value={2}>小于等于</Select.Option>
                                                <Select.Option value={3}>小于</Select.Option>
                                                <Select.Option value={4}>小于等于</Select.Option>
                                                <Select.Option value={5}>等于</Select.Option>
                                                <Select.Option value={6}>不等于</Select.Option>
                                            </Select>
                                            <InputNumber style={{ width: '45%' }} placeholder="请输入值" value={searchContent[index].inputValue}
                                                onChange={(e) => onInputNumberValueChange(e, prop.dataIndex)} />
                                        </Input.Group>
                                    );
                                case "date":
                                    return (
                                        <Input.Group compact style={{ marginTop: '5px' }} key={index}>
                                            <span style={{ lineHeight: 2, marginRight: '10px', width: '20%', textAlign: 'right' }}>{prop.title}（{"日期"}）:</span>
                                            <Select style={{ width: '15%' }} allowClear placeholder="选择方式" value={property("compareType", prop.dataIndex)}
                                                onChange={(value) => onCompareTypeChange(value, prop.dataIndex)}>
                                                <Select.Option value={1}>大于</Select.Option>
                                                <Select.Option value={2}>小于等于</Select.Option>
                                                <Select.Option value={3}>小于</Select.Option>
                                                <Select.Option value={4}>小于等于</Select.Option>
                                                <Select.Option value={5}>等于</Select.Option>
                                                <Select.Option value={6}>不等于</Select.Option>
                                                <Select.Option value={7}>日期区间</Select.Option>
                                            </Select>
                                            <Select style={{ width: '15%' }} allowClear placeholder="选择精度" value={property("comparePrecision", prop.dataIndex)}
                                                onChange={(value) => onComparePrecisionChange(value, prop.dataIndex)}>
                                                <Select.Option value={1}>年</Select.Option>
                                                <Select.Option value={2}>年月</Select.Option>
                                                <Select.Option value={3}>年月日</Select.Option>
                                                <Select.Option value={4}>年月日时分</Select.Option>
                                            </Select>
                                            {property("compareType", prop.dataIndex) !== 7 && <DatePicker showTime={property("comparePrecision", prop.dataIndex) === 4}
                                                style={{ width: '45%' }} value={property("inputValue", prop.dataIndex)}
                                                onChange={(date, dateString) => onDateValueChange(date, prop.dataIndex)} picker={
                                                    ((): "date" | "year" | "month" => {
                                                        switch (property("comparePrecision", prop.dataIndex)) {
                                                            case 1:
                                                                return "year";
                                                            case 2:
                                                                return "month";
                                                            case 3:
                                                                return "date";
                                                            default:
                                                                return "date";
                                                        }
                                                    })()
                                                } />}
                                            {property("compareType", prop.dataIndex) === 7 && <DatePicker.RangePicker showTime={property("comparePrecision", prop.dataIndex) === 4}
                                                style={{ width: '45%' }} value={property("rangeValue", prop.dataIndex)}
                                                onChange={(date, dateString) => onRangeValueChange(date, prop.dataIndex)} picker={
                                                    ((): "date" | "year" | "month" => {
                                                        switch (property("comparePrecision", prop.dataIndex)) {
                                                            case 1:
                                                                return "year";
                                                            case 2:
                                                                return "month";
                                                            case 3:
                                                                return "date";
                                                            default:
                                                                return "date";
                                                        }
                                                    })()
                                                } />}
                                        </Input.Group>
                                    );
                                case "dictionary":
                                    return (
                                        <Input.Group compact style={{ marginTop: '5px' }} key={index}>
                                            <span style={{ lineHeight: 2, marginRight: '10px', width: '20%', textAlign: 'right' }}>{prop.title}（{"可选值"}）:</span>
                                            <Select style={{ width: '30%' }} allowClear placeholder="选择比较方式" value={property("compareType", prop.dataIndex)}
                                                onChange={(value) => onCompareTypeChange(value, prop.dataIndex)}>
                                                <Select.Option value={1}>相等</Select.Option>
                                                <Select.Option value={2}>任一相等</Select.Option>
                                                <Select.Option value={3}>不等</Select.Option>
                                                <Select.Option value={4}>任一不等</Select.Option>
                                            </Select>
                                            <Select allowClear style={{ width: '45%' }} placeholder="请选择值" mode="multiple" value={property("selectValues", prop.dataIndex)}
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
                                            <Select style={{ width: '75%' }} allowClear placeholder="选择比较方式" value={property("compareType", prop.dataIndex)}
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
                    <Button style={{ marginRight: '10px' }} onClick={resetCondition} icon={<UndoOutlined />}>重置</Button>
                    <Button style={{ marginRight: '10px' }} onClick={searchData} icon={<SearchOutlined />} type="primary">搜索</Button>
                </div>
            </>
        );
    }

    // 重置搜索条件
    function resetCondition() {
        searchContent.forEach(c => {
            c.compareType = null;
            c.comparePrecision = null;
            c.inputValue = null;
            c.rangeValue = [];
            c.selectValues = [];
        });
        setSearchContent(cloneDeep(searchContent));
    }

    // 搜索数据
    function searchData() {
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

        let sortData: any = new Array<any>();
        if (sorter instanceof Array) {
            sorter.forEach(d => {
                sortData.push({
                    propertyName: d.field.replace(/^\S/, (s: string) => s.toLocaleUpperCase()),
                    IsAsc: d.order === "ascend"
                });
            });
        } else {
            if (sorter.order === undefined) {
                sortData = null;
            } else {
                sortData.push({
                    propertyName: sorter.field.replace(/^\S/, (s: string) => s.toLocaleUpperCase()),
                    IsAsc: sorter.order === "ascend"
                });
            }

        }
        await loadData(page, size, sortData);
    }

    // 分页码发生变化
    async function pageChange(newpage: number, newsize: number) {
        if (size !== newsize) {
            newpage = 1;
        }
        await loadData(newpage, newsize);
        setPage(newpage);
        setSize(newsize);
    }

    return (
        <>
            <div style={{ display: 'flex' }}>
                <Button onClick={cleanSelect} style={{ marginRight: "10px" }} type="primary" icon={<CheckSquareOutlined />}>批量选择</Button>
                <Popover content={getSelectGroup()} title="属性显示" trigger="click" placement="bottomLeft" overlayStyle={{ width: '200px' }}>
                    <Button style={{ marginRight: "10px" }} type="primary" icon={<EyeOutlined />}>显示列</Button>
                </Popover>
                <Popover content={getFilterGroup()} title="数据过滤" trigger="click" placement="bottomLeft" overlayStyle={{ width: '800px' }}>
                    <Button style={{ marginRight: "10px" }} type="primary" icon={<FilterOutlined />}>数据过滤</Button>
                </Popover>
            </div>
            {batchSelect && <Alert style={{ marginTop: '10px' }} message="所有分页数据都已经被批量选择，请谨慎操作" type="warning" showIcon />}
            <Table style={{ marginTop: '10px' }} columns={displayColumns} pagination={false} rowSelection={rowSelection}
                dataSource={tableData} bordered={true} size='small' onChange={tableChange}></Table>
            <Pagination defaultCurrent={1} pageSize={size} total={total} current={page} style={{ marginTop: '10px' }}
                showSizeChanger={true} onChange={pageChange} />
        </>
    );
}