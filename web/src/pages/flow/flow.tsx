import { Button } from 'antd';
import { EndpointOptions, jsPlumb, jsPlumbInstance } from 'jsplumb';
import { PlusOutlined, MinusOutlined } from '@ant-design/icons';
import panzoom, { PanZoom, Transform } from 'panzoom';
import { useEffect, useRef, useState } from 'react';
import './flow.less';


export default function Flow(props: any) {

    // panzoom变换信息
    const [transform, setTransform] = useState({} as Transform);

    // 上一个选中的节点
    let lastNodeId = useRef("");

    // 节点id序号
    let nodeIndex = useRef(1);

    // jsplumb实例
    let jsplumbInstance: React.MutableRefObject<jsPlumbInstance | undefined> = useRef<jsPlumbInstance>();

    // panzoom实例
    let panzoomInstance: React.MutableRefObject<PanZoom | undefined> = useRef<PanZoom>();

    // 容器实例引用
    const containerRef = useRef({} as any);

    // 断点配置
    const endpointOption: EndpointOptions & {
        allowLoopback: boolean,
        filter: Function
    } = {
        maxConnections: -1,

        anchor: ["Continuous", { faces: ["top", "left", "bottom", "right"] }] as any,

        // 端点样式
        endpoint: 'Rectangle',
        paintStyle: { width: 10, height: 10, fill: 'transparent' } as any,
        hoverPaintStyle: { width: 10, height: 10, fill: '#666' } as any,

        dragOptions: { cursor: 'pointer' },

        isSource: true,
        isTarget: true,
        allowLoopback: false,

        // 链接线样式
        connector: ['Flowchart', { cornerRadius: 10, gap: 5, midpoint: 0.7 }],
        connectorStyle: { stroke: '#1890ff', strokeWidth: 2 },
        connectorHoverStyle: { stroke: '#003a8c', strokeWidth: 2 },
        connectorOverlays: [["Arrow", { location: 1, width: 10, length: 10 }]],

        // 过滤不拖拽的区域
        filter: (event: any, element: any) => {
            return !event.target.classList.contains('node-draggable');
        }
    };

    useEffect(() => {
        jsplumbInstance.current = jsPlumb.getInstance({
            DragOptions: { cursor: 'move', zIndex: 2000 },
            Container: "zoom-container"
        });
        panzoomInstance.current = panzoom(containerRef.current!, {
            maxZoom: 2,
            minZoom: 0.4,
            zoomSpeed: 0.1,
            smoothScroll: false,
            onDoubleClick: function (e) {
                // 禁用 双击放大缩小
                return false; // tells the library to not preventDefault, and not stop propagation
            }
        });
        jsplumbInstance.current!.bind('click', (conn, orignalEvent) => {
            jsplumbInstance.current!.deleteConnection(conn);
        });
        panzoomInstance.current.on('transform', (e: any) => {
            setTransform({
                x: e.getTransform().x,
                y: e.getTransform().y,
                scale: e.getTransform().scale,
            });
            jsplumbInstance.current!.setZoom(e.getTransform().scale);
        });
    }, []);// eslint-disable-line react-hooks/exhaustive-deps

    let selectedNodeType: string = '';

    // flow点击事件，隐藏选择框
    function flowClick() {
        let lastEle = document.getElementById(lastNodeId.current);
        lastEle?.classList.remove("node-selected");
    }

    // 拖拽开始
    function dragBegin(e: any, nodeType: string) {
        e.stopPropagation();
        panzoomInstance.current!.pause();
        selectedNodeType = nodeType;
    }

    // 拖拽中
    function dragOver(e: any) {
        e.preventDefault();
    }

    // 目标放置
    function mouseDrop(e: any) {
        let el = document.createElement("div");
        el.id = `node${nodeIndex.current++}`
        el.className = "node " + selectedNodeType;
        el.style.position = "absolute";
        el.style.top = `${((e.nativeEvent.layerY - transform.y) / transform.scale - 40).toString()}px`;
        el.style.left = `${((e.nativeEvent.layerX - transform.x) / transform.scale - 40).toString()}px`;
        el.innerText = "节点";

        let connectEl = document.createElement("div");
        connectEl.className = "node-draggable";
        connectEl.style.zIndex = '99';

        // 点击时出现选框
        el.onmousedown = (e) => {

            // 清除上一个点的选中状态
            if (lastNodeId.current !== el.id) {
                let lastEle = document.getElementById(lastNodeId.current);
                lastEle?.classList.remove("node-selected");
            }

            lastNodeId.current = el.id;
            el.classList.add("node-selected");
        }
        el.onclick = (e) => {
            e.stopPropagation();
        }
        el.ondblclick = (e) => {
            e.stopPropagation();
        }

        el.appendChild(connectEl);

        // 设置元素信息
        jsplumbInstance.current!.draggable(el, {
            filter: '.node-draggable',
            filterExclude: false
        } as any);
        jsplumbInstance.current!.makeSource(el, endpointOption);
        jsplumbInstance.current!.makeTarget(el, endpointOption);
        containerRef.current!.appendChild(el);
        panzoomInstance.current!.resume();
    }

    // 缩小
    function zoomIn() {
        panzoomInstance.current?.smoothZoom(transform.x, transform.y, 0.9);
    }

    // 放大
    function zoomOut() {
        panzoomInstance.current?.smoothZoom(transform.x, transform.y, 1.1);
    }

    // 键盘事件
    function catchKey(e: any) {
        var keyCode = e.keyCode || e.which || e.charCode;
        if (keyCode === 46) {
            if (lastNodeId.current !== "") {
                jsplumbInstance.current?.remove(lastNodeId.current);
                lastNodeId.current = "";
            }
        }
    }

    return (
        <>
            <div id="flow-container" onDrop={mouseDrop} onDragOver={dragOver} onClick={flowClick} onKeyDown={catchKey}>
                <div id="flow-component-bar">
                    <div draggable className="node mt-10 node-one" onMouseDown={(e) => dragBegin(e, "node-one")}>one</div>
                    <div draggable className="node mt-10 node-two" onMouseDown={(e) => dragBegin(e, "node-two")}>two</div>
                    <div draggable className="node mt-10 node-three" onMouseDown={(e) => dragBegin(e, "node-three")}>three</div>
                    <div draggable className="node mt-10 node-four" onMouseDown={(e) => dragBegin(e, "node-four")}>four</div>
                    <div draggable className="node mt-10 node-five" onMouseDown={(e) => dragBegin(e, "node-five")}>five</div>
                </div>

                <div id="zoom-container" ref={containerRef} >
                </div>

                <div id="zoom-controller">
                    <Button size="small" icon={<MinusOutlined />} onClick={zoomIn}></Button>
                    <div id="scale-value">{transform.scale?.toFixed(1)}</div>
                    <Button size="small" icon={<PlusOutlined />} onClick={zoomOut}></Button>
                </div>
            </div>
        </>
    );
}