export interface CommonResult<T> {
    isSuccess: boolean;
    code: string;
    message: string;
    data: T;
}

export interface CommonResultNoData {
    isSuccess: boolean;
    code: string;
    message: string;
}