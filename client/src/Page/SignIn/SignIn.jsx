import React, { useContext, useState, useEffect } from 'react'
import { Button, Form } from 'react-bootstrap'
import { useNavigate } from 'react-router-dom'
import { AuthContext } from '../../Context/AuthContext'
import { ValideFormContext } from '../../Context/ValideFormContext'
import axios from '../../Util/AxiosConfig'

function SignIn({ setIsLogin, transferData, clearTransferData }) {
    const { login } = useContext(AuthContext)
    const { validateForm, formErrors } = useContext(ValideFormContext)
    const [loginData, setLoginData] = useState({ email: "", password: "" })
    const [loading, setLoading] = useState(false)
    const [showSuccessMessage, setShowSuccessMessage] = useState(false)
    const navigate = useNavigate()

    // Auto-fill data từ signup khi có transferData
    useEffect(() => {
        if (transferData) {
            setLoginData({
                email: transferData.email,
                password: transferData.password
            })
            setShowSuccessMessage(true)
            
            // Ẩn thông báo sau 5 giây
            const timer = setTimeout(() => {
                setShowSuccessMessage(false)
                if (clearTransferData) {
                    clearTransferData()
                }
            }, 5000)

            return () => clearTimeout(timer)
        }
    }, [transferData, clearTransferData])

    const handleLogin = async (e) => {
        e.preventDefault()
        const errors = validateForm(loginData)
           
        if (errors > 0) return

        try {
            const response = await axios.post("/auth/Signin", loginData)
            login(response.data.userName, response.data.role)
            setLoading(true)
            navigate("/")
        } catch (error) {
            console.log(error)
            setLoading(false)
        }
    }

    return (
        <>
            <h2>Đăng Nhập</h2>
            
            {/* Thông báo đăng ký thành công */}
            {showSuccessMessage && (
                <div className="alert alert-success" role="alert">
                    <strong>Đăng ký thành công!</strong> Thông tin đăng nhập đã được điền sẵn cho bạn.
                </div>
            )}
            
            <Form className="auth-form" onSubmit={handleLogin}>
                <Form.Control
                    type="email"
                    name="email"
                    placeholder="Email"
                    value={loginData.email}
                    onChange={(e) => setLoginData({ ...loginData, email: e.target.value })}
                    isInvalid={!!formErrors.email}
                />
                <Form.Control.Feedback className="mb-3" type="invalid">{formErrors.email}</Form.Control.Feedback>

                <Form.Control
                    type="password"
                    placeholder="Mật khẩu"
                    value={loginData.password}
                    onChange={(e) => setLoginData({ ...loginData, password: e.target.value })}
                    isInvalid={!!formErrors.password}
                />
                <Form.Control.Feedback className="mb-3" type="invalid">{formErrors.password}</Form.Control.Feedback>

                <Button variant="primary" disabled={loading} type="submit">
                    {loading ? "Đang xử lý..." : "Đăng Nhập"}
                </Button>
            </Form>
            <p style={{ cursor: "pointer", color: "blue" }} onClick={() => navigate("/auth/forgot-password")}>
                Quên mật khẩu?
            </p>
            <p>
                Chưa có tài khoản? <Button variant="link" onClick={() => setIsLogin(false)}>Đăng ký</Button>
            </p>
        </>
    )
}

export default SignIn