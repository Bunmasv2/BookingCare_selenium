import React, { useContext, useEffect, useState, useRef } from 'react';
import { Button, InputGroup, Modal, Form } from 'react-bootstrap';
import { ValideFormContext} from '../../Context/ValideFormContext';
import axios from '../../Util/AxiosConfig';

function SignUp({ setIsLogin, onSuccessfulSignup }) {
    const { validateField, validateForm, formErrors } = useContext(ValideFormContext);
    const [registerData, setRegisterData] = useState({
        fullname: "", phone: "", email: "", signup_password: "", passwordConfirmed: ""
    });
    const [otpInputs, setOtpInputs] = useState(["", "", "", "", "", ""]);
    const [showOtpModal, setShowOtpModal] = useState(false);
    const [loading, setLoading] = useState(false);
    const [showPasswords, setShowPasswords] = useState(false);
    const [resendCooldown, setResendCooldown] = useState(0);
    const [otpCountdown, setOtpCountdown] = useState("");
    const [lastEmailUsed, setLastEmailUsed] = useState("");
    const [otpSent, setOtpSent] = useState(false);
    const [isRegistering, setIsRegistering] = useState(false); // Thêm flag để tránh call API nhiều lần
    
    const otpRefs = useRef([...Array(6)].map(() => React.createRef()));

    useEffect(() => {
        let timer;
        if (resendCooldown > 0) {
            timer = setInterval(() => {
                setResendCooldown(prev => {
                    if (prev <= 1) {
                        clearInterval(timer);
                        return 0;
                    }
                    return prev - 1;
                });
            }, 1000);
        }
        return () => clearInterval(timer);
    }, [resendCooldown]);

    useEffect(() => {
        if (resendCooldown > 0) {
            const minutes = Math.floor(resendCooldown / 60);
            const seconds = resendCooldown % 60;
            setOtpCountdown(`${minutes}:${seconds < 10 ? '0' : ''}${seconds}`);
        } else {
            setOtpCountdown("");
        }
    }, [resendCooldown]);

    useEffect(() => {
        if (showOtpModal) {
            const normalizedEmail = registerData?.email.trim().toLowerCase();
            if (normalizedEmail !== lastEmailUsed || !otpSent) {
                handleSendOtp();
            }
        }
    }, [showOtpModal]);

    // XÓA useEffect cũ và thay thế bằng function riêng biệt
    // useEffect(() => {
    //     const register = async () => {
    //         if (!isValidOtp || !showOtpModal) return;
            
    //         try {
    //             setLoading(true);
    //             const otp = otpInputs.join("");
    //             const payload = { ...registerData, otp };
    //             await axios.post("/auth/register", payload);
                
    //             setShowOtpModal(false);
                
    //             // Gọi callback để truyền dữ liệu sang SignIn
    //             if (onSuccessfulSignup) {
    //                 onSuccessfulSignup(registerData);
    //             } else {
    //                 setIsLogin(true);
    //             }
    //             setIsValidOtp(false);
    //             setOtpInputs(["", "", "", "", "", ""]);
    //         } catch (error) {
    //             console.log(error)
    //         } finally {
    //             setLoading(false);
    //         }
    //     };
    //     register();
    // }, [isValidOtp, showOtpModal, registerData, onSuccessfulSignup, setIsLogin, otpInputs]);

    useEffect(() => {
        if (showOtpModal) {
            setTimeout(() => {
                otpRefs.current[0]?.current?.focus();
            }, 100);
        }
    }, [showOtpModal]);

    const passwordsMatch = () => {
        return registerData.signup_password === registerData.passwordConfirmed && registerData.signup_password !== "";
    };

    const handleRegister = async (e) => {
        e.preventDefault();
        const errors = validateForm(registerData);
        if (errors > 0) return;

        if (!passwordsMatch()) {
            return;
        }

        setOtpInputs(["", "", "", "", "", ""]);
        setOtpSent(false);
        setIsRegistering(false); // Reset flag

        const normalizedEmail = registerData.email.trim().toLowerCase();
        if (normalizedEmail !== lastEmailUsed) {
            setResendCooldown(0);
        }

        setShowOtpModal(true);
    };

    const handleSendOtp = async () => {
        try {
            const normalizedEmail = registerData.email.trim().toLowerCase();
            const emailChanged = normalizedEmail !== lastEmailUsed;

            if (emailChanged || resendCooldown === 0) {
                setLoading(true);
                await axios.post("/auth/send-otp", { email: normalizedEmail });
                setResendCooldown(60);
                setLastEmailUsed(normalizedEmail);
                setOtpSent(true);
                // alert("Mã OTP đã được gửi đến email của bạn!");
            } else {
                // alert(`Mã OTP đã được gửi. Vui lòng đợi ${otpCountdown} để gửi lại.`);
            }
        } catch (error) {
            console.log(error)
        } finally {
            setLoading(false);
        }
    };

    // Tạo function riêng để handle việc đăng ký
    const handleRegisterWithOtp = async () => {
        if (isRegistering) return; // Tránh call API nhiều lần
        
        try {
            setIsRegistering(true);
            setLoading(true);
            const otp = otpInputs.join("");
            const payload = { ...registerData, otp };
            await axios.post("/auth/register", payload);
            
            setShowOtpModal(false);
            
            // Gọi callback để truyền dữ liệu sang SignIn
            if (onSuccessfulSignup) {
                onSuccessfulSignup(registerData);
            } else {
                setIsLogin(true);
            }
            
            // Reset tất cả state
            setOtpInputs(["", "", "", "", "", ""]);
            setIsRegistering(false);
        } catch (error) {
            console.log(error);
            setIsRegistering(false);
            // Có thể thêm thông báo lỗi cho user ở đây
        } finally {
            setLoading(false);
        }
    };

    const handleConfirmOtp = () => {
        const otp = otpInputs.join("");
        if (otp.length !== 6) {
            alert("Vui lòng nhập đủ 6 chữ số OTP.");
            return;
        }
        
        // Gọi function đăng ký thay vì set state
        handleRegisterWithOtp();
    };

    const handleOtpInput = (e, index) => {
        e.preventDefault();
        const inputValue = e.nativeEvent.data;

        if (inputValue && /^[0-9]$/.test(inputValue)) {
            const newOtp = [...otpInputs];
            newOtp[index] = inputValue;
            setOtpInputs(newOtp);

            if (index < 5) {
                otpRefs.current[index + 1]?.current?.focus();
            }
        }
    };

    const handleOtpKeyDown = (e, index) => {
        if (e.key === "Backspace") {
            if (otpInputs[index]) {
                const newOtp = [...otpInputs];
                newOtp[index] = "";
                setOtpInputs(newOtp);
            } else if (index > 0) {
                otpRefs.current[index - 1]?.current?.focus();
            }
        } else if (e.key === "ArrowLeft" && index > 0) {
            otpRefs.current[index - 1]?.current?.focus();
        } else if (e.key === "ArrowRight" && index < 5) {
            otpRefs.current[index + 1]?.current?.focus();
        }
    };

    const handleOtpPaste = (e) => {
        e.preventDefault();
        const pastedData = e.clipboardData.getData('text');
        const pastedOtp = pastedData.replace(/\D/g, '').slice(0, 6);

        if (pastedOtp) {
            const newOtp = [...otpInputs];
            for (let i = 0; i < pastedOtp.length; i++) {
                if (i < 6) newOtp[i] = pastedOtp[i];
            }
            setOtpInputs(newOtp);

            if (pastedOtp.length < 6) {
                otpRefs.current[Math.min(pastedOtp.length, 5)]?.current?.focus();
            }
        }
    };

    const handleOtpClick = (index) => {
        otpRefs.current[index]?.current?.focus();
    };

    const handleEmailChange = (e) => {
        setRegisterData({ ...registerData, email: e.target.value });
    };

    const handleCloseOtpModal = () => {
        setShowOtpModal(false);
        setOtpInputs(["", "", "", "", "", ""]);
        setIsRegistering(false); // Reset flag khi đóng modal
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

                <Button variant="primary" type="submit" disabled={loading}>
                    {loading ? "Đang xử lý..." : "Đăng Ký"}
                </Button>
            </Form>

            <p>
                Đã có tài khoản?{" "}
                <Button variant="link" onClick={() => {
                    setIsLogin(true);
                    setShowOtpModal(false);
                    setOtpInputs(["", "", "", "", "", ""]);
                    setIsRegistering(false); // Reset flag
                }}>
                    Đăng nhập
                </Button>
            </p>

            <Modal show={showOtpModal} onHide={handleCloseOtpModal} centered>
                <Modal.Header closeButton>
                    <Modal.Title>Nhập Mã OTP</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <p className="text-center mb-3">
                        Mã OTP đã được gửi đến email: <strong>{registerData.email}</strong>
                    </p>
                    <Form.Group>
                        <div className="d-flex justify-content-between">
                        {otpInputs.map((value, index) => (
                            <Form.Control
                                key={index}
                                ref={otpRefs.current[index]}
                                type="tel"
                                inputMode="numeric"
                                pattern="[0-9]*"
                                maxLength="1"
                                className="otp-input"
                                value={value}
                                onInput={(e) => handleOtpInput(e, index)}
                                onKeyDown={(e) => handleOtpKeyDown(e, index)}
                                onPaste={index === 0 ? handleOtpPaste : undefined}
                                onClick={() => handleOtpClick(index)}
                                autoComplete="off"
                                style={{ width: "40px", textAlign: "center", fontSize: "1.5rem" }}
                                disabled={isRegistering} // Disable input khi đang xử lý
                            />
                        ))}
                        </div>
                    </Form.Group>
                </Modal.Body>
                <Modal.Footer className="flex-column align-items-start gap-2">
                    <div className="w-100 d-flex justify-content-between">
                        <Button variant="link" onClick={handleSendOtp} disabled={resendCooldown > 0 || isRegistering}>
                            {resendCooldown > 0
                                ? `Gửi lại OTP sau ${otpCountdown}`
                                : "Gửi lại mã OTP"}
                        </Button>
                        <div>
                            <Button variant="secondary" onClick={handleCloseOtpModal} className="me-2" disabled={isRegistering}>
                                Hủy
                            </Button>
                            <Button variant="primary" onClick={handleConfirmOtp} disabled={loading || isRegistering}>
                                {loading || isRegistering ? "Đang xác thực..." : "Xác Nhận"}
                            </Button>
                        </div>
                    </div>
                </Modal.Footer>
            </Modal>
        </>
    );
}

export default SignUp;