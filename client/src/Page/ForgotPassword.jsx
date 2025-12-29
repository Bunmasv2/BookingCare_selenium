import React, { useState } from "react";
import { Container, Col, Form, Button, Alert } from "react-bootstrap";
import "bootstrap/dist/css/bootstrap.min.css";
import "../Style/Forgot.css";
import axios from '../Util/AxiosConfig';
import { useNavigate } from "react-router-dom";

const ForgotPassword = () => {
  const [email, setEmail] = useState("");
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState(null);
  const [error, setError] = useState(null);
  const [verificationSent, setVerificationSent] = useState(false);
  const [code, setCode] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [showPasswords, setShowPasswords] = useState(false);
  const [otpSending, setOtpSending] = useState(false);
  const navigate = useNavigate();

  // Gửi yêu cầu mã xác thực
  const handleForgotPassword = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setMessage(null);

    try {
      const response = await axios.post("/auth/forgot-password", { Email: email.trim().toLowerCase() });
      setMessage(response.data.message);
      setVerificationSent(true);
    } catch (err) {
      setError(err.response?.data?.message || "Có lỗi xảy ra khi gửi mã xác thực");
    }

    setLoading(false);
  };

  // Gửi lại mã OTP
  const handleSendOTP = async () => {
    if (!email) {
      setError("Vui lòng nhập email trước khi lấy mã OTP");
      return;
    }

    setOtpSending(true);
    setError(null);
    setMessage(null);

    try {
      const response = await axios.post("/auth/forgot-password", { Email: email.trim().toLowerCase() });
      setMessage(response.data.message);
    } catch (err) {
      setError(err.response?.data?.message || "Không thể gửi lại mã OTP");
    }

    setOtpSending(false);
  };

  // Xác thực mã và đặt lại mật khẩu
  const handleResetPassword = async (e) => {
    e.preventDefault();

    if (newPassword !== confirmPassword) {
      setError("Mật khẩu không khớp");
      return;
    }

    setLoading(true);
    setError(null);
    setMessage(null);

    try {
      const response = await axios.post("/auth/verify-reset-code", {
        Email: email.trim().toLowerCase(),
        Code: code,
        NewPassword: newPassword
      });
      setMessage(response.data.message);
      setTimeout(() => navigate("/Đăng nhập"), 2000);
    } catch (err) {
      setError(err.response?.data?.message || "Mã xác thực không hợp lệ hoặc đã hết hạn");
    }

    setLoading(false);
  };

  return (
    <Container className="auth-container2 mx-auto d-flex justify-content-center align-items-center mt-5 w-50">
      <Col md={6} className="auth-section2 show">
        {!verificationSent ? (
          <>
            <h2>Quên Mật Khẩu</h2>
            {error && <Alert variant="danger">{error}</Alert>}
            {message && <Alert variant="success">{message}</Alert>}

            <Form className="auth-form" onSubmit={handleForgotPassword}>
              <Form.Control
                type="email"
                placeholder="Nhập email của bạn"
                className="mb-3"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />

              <Button variant="primary" type="submit" disabled={loading} className="w-100">
                {loading ? "Đang xử lý..." : "Gửi mã xác thực"}
              </Button>
            </Form>

            <p className="mt-3 text-center">
              <Button variant="link" onClick={() => navigate("/Đăng nhập")}>Quay lại đăng nhập</Button>
            </p>
          </>
        ) : (
          <>
            <h2>Đặt Lại Mật Khẩu</h2>
            {error && <Alert variant="danger">{error}</Alert>}
            {message && <Alert variant="success">{message}</Alert>}
            
            <Form className="auth-form" onSubmit={handleResetPassword}>
              <p className="text-muted mb-3">
                Mã xác thực đã được gửi tới: {email}
              </p>
              
              <div className="otp-group">
                <Form.Control
                  type="text"
                  placeholder="Nhập mã OTP"
                  value={code}
                  onChange={(e) => setCode(e.target.value)}
                  required
                  className="mb-3"
                />
                <Button
                  variant="secondary"
                  onClick={handleSendOTP}
                  disabled={otpSending || !email}
                  className="mb-3"
                >
                  {otpSending ? "Đang gửi..." : "Gửi lại mã OTP"}
                </Button>
              </div>
              
              <Form.Control
                type={showPasswords ? "text" : "password"}
                placeholder="Mật khẩu mới"
                className="mb-3"
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
                required
              />
              
              <Form.Control
                type={showPasswords ? "text" : "password"}
                placeholder="Xác nhận mật khẩu mới"
                className="mb-3"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                required
              />
              
              <div className="checkbox-container mb-3">
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
              
              <Button variant="primary" type="submit" className="w-100" disabled={loading}>
                {loading ? "Đang xử lý..." : "Đặt lại mật khẩu"}
              </Button>
            </Form>
            
            <p className="mt-3 text-center">
              <Button variant="link" onClick={() => navigate("/")}>
                Quay lại đăng nhập
              </Button>
            </p>
          </>
        )}
      </Col>
    </Container>
  );
};

export default ForgotPassword;