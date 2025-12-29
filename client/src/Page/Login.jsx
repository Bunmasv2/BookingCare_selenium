import React, { useEffect, useState } from 'react'
import axios from "../Util/AxiosConfig"
import { Button } from 'react-bootstrap'
import { jwtDecode } from "jwt-decode"

function Login() {
    const [token, setToken] = useState()
    
    useEffect(() => {
        const fetchData = async () => {
            const respone = await axios.get("/auth/login")
            console.log(respone.data.token)
            setToken(respone.data.token)
            const decoded = jwtDecode(respone.data.token)
            console.log("Decoded Token:", decoded);
        }

        fetchData()
    }, [])

    const Auth = async () => {
        try {
            const respone = await axios.post('/auth/auth_user', { email: "wef", password: "agsegf" }, {
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            })
            console.log(respone)
        } catch (error) {
            
        }
    }

    return (
        <>
            <div>Login</div>
            <Button onClick={Auth}>Auth</Button>
        </>
    )
}

export default Login