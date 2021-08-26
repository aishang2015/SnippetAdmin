import { Avatar, Button } from 'antd';
import TextArea from 'antd/lib/input/TextArea';
import './chat.less';
import { useState } from 'react';
import { concat, delay } from 'lodash';
import { dateFormat } from '../../common/time';


export default function ChatPage(props: any) {

    let i = 1;

    type messageType = {
        type: number,
        content: string,
        time: Date,
        avatar: string
    }

    const [messages, setMessages] = useState(new Array<messageType>());

    const [inputContent, setInputContent] = useState('');

    function sendInputChange(e: any) {
        setInputContent(e.target.value);
    }

    function sendMessage() {
        if (inputContent !== undefined && inputContent != null && inputContent !== "") {
            let newMsg = {
                type: 1,
                content: inputContent as string,
                time: new Date(),
                avatar: "A"
            };

            let feedbackMsg = {
                type: 2,
                content: inputContent as string,
                time: new Date(),
                avatar: "A"
            };

            let msgs = concat(messages, newMsg, feedbackMsg);
            setMessages(msgs);
            setInputContent('');

            delay(() => {
                let el = document.getElementById("message-area") as any;
                el.scrollTop = el.scrollHeight
            }, 100);
        }
    }

    function catchSendKey(e: any) {
        var keyCode = e.keyCode || e.which || e.charCode;
        var ctrlKey = e.ctrlKey || e.metaKey;
        if (ctrlKey && keyCode === 13) {
            sendMessage();
        }
    }

    return (
        <>
            <div id="chat-container" style={{ backgroundImage: "url(/images/ip.png)", backgroundRepeat: 'no-repeat' }}>
                <div id="message-area">
                    {messages.map(m =>
                        m.type === 1 ?

                            <div className="send-message-area" key={i++}>
                                <div className="send-message-content">
                                    <div className="send-message">
                                        {m.content}
                                    </div>
                                    <div className="send-message-time">{dateFormat(m.time.toString())}</div>
                                </div>
                                <div className="send-user-avatar">
                                    <Avatar style={{ backgroundColor: '#87d068' }}>{m.avatar}</Avatar>
                                </div>
                            </div>
                            :
                            <div className="receive-message-area" key={i++}>
                                <div className="receive-user-avatar">
                                    <Avatar style={{ backgroundColor: '#6887d0' }}>{m.avatar}</Avatar>
                                </div>
                                <div className="receive-message-content">
                                    <div className="receive-message">
                                        {m.content}
                                    </div>
                                    <div className="receive-message-time">{dateFormat(m.time.toString())}</div>
                                </div>
                            </div>

                    )}
                </div>
                <div id="send-area">
                    <TextArea autoSize={{ minRows: 1, maxRows: 4 }} bordered={false} value={inputContent}
                        onChange={sendInputChange} onKeyDown={catchSendKey} style={{ borderBottom: "1px solid lightgray", background: 'white' }} />
                    <Button onClick={sendMessage} >发送</Button>
                </div>
            </div>
        </>
    );
}