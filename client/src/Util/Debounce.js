const debounce = (func) => {
    let timerId = null
    
    return (...args) => {
        if (timerId) {
            clearTimeout(timerId)
            timerId = null
        }

        timerId = setTimeout(() => {
            func(...args)
        }, 300);
    }
}

export default debounce