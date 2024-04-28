import { useRef, useState } from "react";
import Draggable, { DraggableBounds, DraggableData, DraggableEvent } from "react-draggable";


export default function DraggableModal(props: any) {

    const [bound, setBound] = useState<DraggableBounds>({
        left: 0, top: 0, bottom: 0, right: 0
    });

    const resizeableSpaceWidth = 5;

    const onStart = (event: DraggableEvent, draggableData: DraggableData) => {
        const { clientWidth, clientHeight } = window?.document?.documentElement;
        const targetRect = (props.draggableRef?.current as any)?.getBoundingClientRect();
        setBound({
            left: -targetRect?.left + draggableData?.x,
            right: clientWidth - (targetRect?.right - draggableData?.x) - resizeableSpaceWidth,
            top: -targetRect?.top + draggableData?.y,
            bottom: clientHeight - (targetRect?.bottom - draggableData?.y)
        })
    };


    return (
        <>
            <Draggable handle={'.ant-modal-header'}
                bounds={bound}
                onStart={(event, uiData) => onStart(event, uiData)}>
                { props.children }
            </Draggable>

        </>
    );
}