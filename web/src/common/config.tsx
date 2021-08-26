import axios from "axios";

export class Configuration {

    static BaseUrl: string;

    static OauthSetting: {
        github: {
            client_id: string;
            redirect_uri: string;
        },
        baidu: {
            client_id: string;
            redirect_uri: string;
        },
        
    };

    static init() {
        let that = this;
        return new Promise(
            function (resolve, reject) {
                axios.get("/config/config.json")
                    .then(result => {
                        that.BaseUrl = result.data.server_url;
                        that.OauthSetting = result.data.oauth;
                        resolve('success');
                    }, error => {
                        console.error("无法找到配置文件！");
                        reject(fail);
                    });
            }
        );
    }

}