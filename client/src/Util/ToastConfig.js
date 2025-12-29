import { toast } from "react-toastify"

const toastConfig = {
    position: "top-right",
    autoClose: 800,
    hideProgressBar: false,
    closeOnClick: true,
    pauseOnHover: false,
    draggable: true,
    theme: "light",
}

export const SuccessNotify = (content) => {
    toast.success(`${content}`, toastConfig)
}

export const WarningNotify = (content) => {
    toast.warning(`${content}`, toastConfig)
}

export const ErrorNotify = (content) => {
    toast.error(`${content}`, toastConfig)
}