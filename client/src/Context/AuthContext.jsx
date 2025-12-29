import { createContext, useEffect, useState } from "react"
import { useLocation, useNavigate } from "react-router-dom"

const AuthContext = createContext()

const AuthProvider = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState(false)
    const [UserName, setUserName] = useState("")
    const [role, setRole] = useState()
    const location = useLocation()
    const navigate = useNavigate()

    useEffect(() => {
        const storedUserName = localStorage.getItem("UserName")
        const user_role = localStorage.getItem("userRole")
        if (storedUserName) {
            setIsAuthenticated(true)
            setUserName(storedUserName || "Người dùng")
            setRole(user_role)
        }
    }, [])

    const login = (name, role) => {
        localStorage.setItem("UserName", name)
        localStorage.setItem("userRole", role)
        setIsAuthenticated(true)
        setUserName(name)
        setRole(role)

        setTimeout(() => {
            const redirectPath = localStorage.getItem("prevPage") || "/"
            localStorage.removeItem("prevPage")
            navigate(redirectPath || "/")
        }, 200)
    }

    const logout = () => {
        localStorage.removeItem("token")
        localStorage.removeItem("UserName")
        setIsAuthenticated(false)
        setUserName("")
    }

    const hanelePrevPage = () => {
        console.log(location.pathname)
    }

    return (
        <AuthContext.Provider value={{ isAuthenticated, UserName, login, logout, hanelePrevPage, role }}>
            {children}
        </AuthContext.Provider>
    )
}

export { AuthContext, AuthProvider }


