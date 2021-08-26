import React from "react";
import { StorageService } from "../../common/storage";
import { handleThirdPartyCode } from "../../http/requests/account";

export class Callback extends React.Component<any>{

    _code: string;

    _source: string;


    constructor(props: any) {
        super(props);

        // 取得返回的code
        let querys = props.location.search.substring(1).split("&");
        this._source = querys[0].split("=")[1];
        this._code = querys[1].split("=")[1];
    }

    componentDidMount() {
        handleThirdPartyCode(this._code, this._source).then(result => {

            if (result.data.code === "ACCOUNT_INFO_0001") {
                localStorage.setItem('token', result.data.data.accessToken);
                localStorage.setItem('user-name', result.data.data.userName);
                window.location.reload();
            } else if (result.data.code === "ACCOUNT_INFO_0002") {
                StorageService.setOauthStore(
                    result.data.data.thirdPartyUserName,
                    result.data.data.thirdPartyType,
                    result.data.data.thirdPartyInfoCacheKey);
                this.props.history.push("/binding");
            }

        });
    }

    render() {
        return (
            <div style={{
                width: '100vw',
                height: '100vh',
                display: "flex",
                justifyContent: 'center',
                alignItems: 'center',
            }}>
                <p>正在处理用户信息，请稍等!</p>
            </div>
        );
    }
}

export default Callback;