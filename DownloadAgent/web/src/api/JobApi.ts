import type { DownloadConfig, JobInfo } from "../model/JobInfo";
import Fetcher from "../util/Fetcher";

class JobApi {
    constructor() {
        Fetcher.BaseUrl = "/";
        Fetcher.Hearders = {
            "Content-Type": "application/json",
            "Accept": "application/json"
        };
        Fetcher.ErrorResponse = (error: string) => {
            return {
                Error: error,
                ErrorCode: "",
            };
        }
    }

    public async LoadList(): Promise<JobInfo[]> {
        let url = "api/manage/list";
        let params = "";

        let result = await Fetcher.Get<JobInfo[]>(url)
            .then(res => {
                if (res) {
                    return res;
                }
                return [];
            });

        return result;
    }

    public async LoadConfig(): Promise<DownloadConfig> {
        let url = "api/manage/config";
        let params = "";

        let result = await Fetcher.Get<DownloadConfig>(url)
            .then(res => {
                if (res) {
                    return res;
                }
                return null;
            });

        return result;
    }

    public async SaveConfig(config: DownloadConfig): Promise<boolean> {
        let url = "api/manage/config";

        let result = await Fetcher.Post<boolean>(url, config)
            .then(res => {
                if (res) {
                    return res;
                }
                return null;
            });

        return result;
    }
}

export default new JobApi();