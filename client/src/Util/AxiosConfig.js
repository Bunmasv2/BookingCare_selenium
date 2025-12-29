import axios from "axios"
import { SuccessNotify, WarningNotify, ErrorNotify } from "./ToastConfig"

const instance = axios.create({
    baseURL: "http://127.0.0.1:5140/api",
    // baseURL: "https://clinic-cj96.onrender.com/api",
    withCredentials: true,
})

// Add a request interceptor
instance.interceptors.request.use(function (config) {
    // Do something before request is sent
    return config
}, function (error) {
    // Do something with request error
    return Promise.reject(error)
})

instance.interceptors.response.use(
    function (response) {
        if (response.status === 200 && response.data.message) {
            if (response.data.message === "Đăng nhập thành công!") return response
            alert(response.data.message);
        }
        return response;
    },
    function (error) {
        if (error && error.response && error.response.data) {
            const errorMessage = error.response.data.ErrorMessage || "Lỗi không xác định";
            if (error.response.status === 401 || errorMessage.includes("Vui lòng đăng nhập")) {
                alert(errorMessage)
                window.location.href = "/đăng%20nhập";
                return Promise.reject(error);
            }
            alert(errorMessage);
        }
        return Promise.reject(error);
    });

export default instance