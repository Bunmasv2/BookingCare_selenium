import React, { useState, useContext, useEffect } from "react"
import { Container, Col, Button } from "react-bootstrap"
import { ValideFormContext } from "../../Context/ValideFormContext"
import "../../Style/Signin.css"
import SignIn from "./SignIn"
import SignUp from "./SignUp"

const Index = () => {
  const [isLogin, setIsLogin] = useState(true)
  const [resetKey, setResetKey] = useState(0)
  const [transferData, setTransferData] = useState(null) // Dữ liệu chuyển từ signup sang signin
  const { setFormErrors } = useContext(ValideFormContext)

  useEffect(() => {
    // Reset form errors khi chuyển đổi (nhưng không reset nếu có transferData)
    if (!transferData) {
      setFormErrors({})
      setResetKey(prev => prev + 1)
    } else {
      // Chỉ reset form errors, không reset component để giữ transferData
      setFormErrors({})
    }
  }, [isLogin, setFormErrors, transferData])

  // Hàm để xử lý việc chuyển dữ liệu từ signup sang signin
  const handleSuccessfulSignup = (signupData) => {
    setTransferData({
      email: signupData.email,
      password: signupData.signup_password
    })
    setIsLogin(true) // Chuyển sang trang đăng nhập
  }

  // Hàm để clear transferData sau khi đã sử dụng
  const clearTransferData = () => {
    setTransferData(null)
  }

  return (
    <Container className="auth-container mx-auto d-flex justify-content-center align-items-center mt-5 w-50">
      <Col md={6} className={`auth-section ${isLogin ? "show" : "hide"}`}>
        <SignIn 
          key={transferData ? `signin-with-data-${resetKey}` : `signin-${resetKey}`}
          setIsLogin={setIsLogin} 
          transferData={transferData}
          // clearTransferData={clearTransferData}
        />
      </Col>

      <Col md={6} className={`auth-section ${isLogin ? "hide" : "show"} my-auto`}>
        <SignUp 
          key={`signup-${resetKey}`} 
          setIsLogin={setIsLogin}
          onSuccessfulSignup={handleSuccessfulSignup}
        />
      </Col>

      <div className={`toggle-section ${isLogin ? "" : "move"}`}>
        <h2>{isLogin ? "Chào Mừng!" : "Chào Mừng Trở Lại!"}</h2>
        <p>{isLogin ? "Bạn chưa có tài khoản?" : "Bạn đã có tài khoản?"}</p>
        <Button className="toggle-btn" onClick={() => {
          // Clear transferData khi user manually switch
          if (transferData) {
            setTransferData(null)
            setResetKey(prev => prev + 1)
          }
          setIsLogin(!isLogin)
        }}>
          {isLogin ? "Đăng ký" : "Đăng nhập"}
        </Button>
      </div>
    </Container>
  )
}

export default Index