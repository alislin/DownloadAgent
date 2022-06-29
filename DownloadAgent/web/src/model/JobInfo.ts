export interface JobInfo {
    id: string;
    url: string;
    fileName: string;
    path: string;
    size: number;
    recievedByte: number;
    speed: number;
    processPercentage: number;
    jobState: JobState;
    start: string;
    end: string;
    dur: string;
    error: string | null;
}

export enum JobState {
    Wait = 0,
    Running = 1,
    Done = 2
}

export interface DownloadConfig {
    downloadPath: string;
    skipSameFile: boolean;
}