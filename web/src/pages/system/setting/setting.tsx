
import { Checkbox, DatePicker, Divider, Input, InputNumber, message, Radio, Select, Switch, Typography } from 'antd';
import { useToken } from 'antd/es/theme/internal';
import { useEffect, useState } from 'react';
import './setting.css';
import dayjs from 'dayjs';

import 'dayjs/locale/zh-cn';
import locale from 'antd/locale/zh_CN';
import PwdSetting from './coms/pwdSetting';

export default function Setting() {

    const [_, token] = useToken();

    const [selectItem, setSelectItem] = useState<number>(1);
    const [groups, setGroups] = useState<Array<any>>([
        { key: 1, name: "密码配置", code: "mail-template-config" },
    ]);

    async function selectSetting(index: number, groupCode: string) {
        setSelectItem(index);
    }

    return (
        <>
            <div id="setting-container">
                <div id="setting-item-container">
                    <ul>
                        {groups.map((g, i) => (
                            <li key={g.key} onClick={() => selectSetting(g.key, g.code!)} style={{
                                borderLeft: selectItem === g.key ? '5px solid ' + token.colorPrimary : ""
                            }}>{g.name}</li>
                        ))}
                    </ul>
                </div>
                <Divider type='vertical' style={{ height: '100%' }} />
                <div id="setting-detail-container">
                    {selectItem === 1 && <><PwdSetting /></>}


                </div>
            </div>
        </>
    );
}