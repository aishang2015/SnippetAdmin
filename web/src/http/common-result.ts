export interface CommonResult<T> {
    isSuccess: boolean;
    code: string;
    message: string;
    data: T;
}