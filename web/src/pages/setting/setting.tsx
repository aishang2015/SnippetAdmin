import { useState } from 'react';
import './setting.less';

export default function Setting() {

    const [selectItem, setSelectItem] = useState<number>(0);

    function selectSetting(index: number) {
        setSelectItem(index);
    }

    function selectClass(index: number) {
        return selectItem === index ? 'li-selected' : '';
    }

    return (
        <>
            <div id="setting-container">
                <div id="setting-item-container">
                    <ul>
                        <li onClick={() => selectSetting(0)} className={selectClass(0)}>系统</li>
                        <li onClick={() => selectSetting(1)} className={selectClass(1)}>认证</li>
                        <li onClick={() => selectSetting(2)} className={selectClass(2)}>定时任务</li>
                    </ul>
                </div>
                <div id="setting-detail-container">

                </div>
            </div>
        </>
    );
}