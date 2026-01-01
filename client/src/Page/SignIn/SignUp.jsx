import React, { useContext, useState } from 'react';
import { Button, InputGroup, Form } from 'react-bootstrap';
import { ValideFormContext} from '../../Context/ValideFormContext';
import axios from '../../Util/AxiosConfig';

function SignUp({ setIsLogin, onSuccessfulSignup }) {
    const { validateField, validateForm, formErrors } = useContext(ValideFormContext);
    const [registerData, setRegisterData] = useState({
        fullname: "", phone: "", email: "", signup_password: "", passwordConfirmed: ""
    });
    const [loading, setLoading] = useState(false);
    const [showPasswords, setShowPasswords] = useState(false);

    const passwordsMatch = () => {
        return registerData.signup_password === registerData.passwordConfirmed && registerData.signup_password !== "";
    };

    // OTP DISABLED - Direct registration without OTP
    const handleRegister = async (e) => {
        e.preventDefault();
        const errors = validateForm(registerData);
        if (errors > 0) return;

        if (!passwordsMatch()) {
            return;
        }

        try {
            setLoading(true);
            // OTP DISABLED - Send register request directly without OTP
            const payload = { ...registerData, otp: "000000" }; // Dummy OTP for testing
            await axios.post("/auth/register", payload);
            
            // Gọi callback để truyền dữ liệu sang SignIn
            if (onSuccessfulSignup) {
                onSuccessfulSignup(registerData);
            } else {
                setIsLogin(true);
            }
        } catch (error) {
            console.log(error);
        } finally {
            setLoading(false);
        }
    };

    const passwordMatchError = !passwordsMatch() && registerData.passwordConfirmed !== "" 
        ? "Mật khẩu xác nhận không khớp với mật khẩu đã nhập" 
        : null;

    return (
        <>
            <h2>Đăng Ký</h2>
            <Form className="auth-form" onSubmit={handleRegister}>
                <Form.Control
                    type="text"
                    id="fullName"
                    placeholder="Họ và Tên"
                    value={registerData.fullname}
                    isInvalid={!!formErrors.fullname}
                    onChange={(e) => {
                        const value = e.target.value;
                        setRegisterData({ ...registerData, fullname: value });
                        validateField("fullname", value);
                    }}

                />
                <Form.Control.Feedback type="invalid">{formErrors.fullname}</Form.Control.Feedback>

                <Form.Control
                    id="phone"
                    type="text"
                    placeholder="Số điện thoại"
                    value={registerData.phone}
                    isInvalid={!!formErrors.phone}
                    onChange={(e) => {
                        const value = e.target.value;
                        setRegisterData({ ...registerData, phone: value });
                        validateField("phone", value);
                    }}
                />
                <Form.Control.Feedback type="invalid">{formErrors.phone}</Form.Control.Feedback>

                <Form.Control
                    type="email"
                    id="email"
                    placeholder="Email (Example@gmail.com)"
                    value={registerData.email}
                    isInvalid={!!formErrors.email}
                    onChange={(e) => {
                        const value = e.target.value;
                        setRegisterData({ ...registerData, email: value });
                        validateField("email", value);
                    }}
                />
                <Form.Control.Feedback type="invalid">{formErrors.email}</Form.Control.Feedback>

                <InputGroup className="my-1">
                    <Form.Control
                        id="password"
                        type={showPasswords ? "text" : "password"}
                        placeholder="Mật khẩu"
                        value={registerData.signup_password}
                        isInvalid={!!formErrors.signup_password}
                        onChange={(e) => {
                            const value = e.target.value;
                            setRegisterData({ ...registerData, signup_password: value });
                            validateField("signup_password", value);
                        }}
                    />
                    <Form.Control.Feedback type="invalid">{formErrors.signup_password}</Form.Control.Feedback>
                </InputGroup>

                <InputGroup className="my-1">
                    <Form.Control
                        id="confirmPassword"
                        type={showPasswords ? "text" : "password"}
                        placeholder="Xác nhận mật khẩu"
                        value={registerData.passwordConfirmed}
                        isInvalid={!!formErrors.passwordConfirmed || passwordMatchError}
                        onChange={(e) => {
                            const value = e.target.value;
                            setRegisterData({ ...registerData, passwordConfirmed: value });
                            validateField("passwordConfirmed", value);
                        }}
                    />
                    <Form.Control.Feedback type="invalid">
                        {formErrors.passwordConfirmed || passwordMatchError}
                    </Form.Control.Feedback>
                </InputGroup>

                <div className="checkbox-container">
                    <input
                        type="checkbox"
                        id="showPasswordToggle"
                        className="small-checkbox"
                        checked={showPasswords}
                        onChange={() => setShowPasswords(!showPasswords)}
                    />
                    <label htmlFor="showPasswordToggle">
                        {showPasswords ? "Ẩn mật khẩu" : "Hiển thị mật khẩu"}
                    </label>
                </div>

                <Button id="btnSignUp" variant="primary" type="submit" disabled={loading}  onClick={handleRegister}>
                    {loading ? "Đang xử lý..." : "Đăng Ký"}
                </Button>
            </Form>

            <p>
                Đã có tài khoản?{" "}
                <Button variant="link" onClick={() => {
                    setIsLogin(true);
                }}>
                    Đăng nhập
                </Button>
            </p>
            </>
    );
}

export default SignUp;