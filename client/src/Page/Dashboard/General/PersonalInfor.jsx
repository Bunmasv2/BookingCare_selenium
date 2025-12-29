import React, { useState } from "react"
import { Card, Row, Col, Form, Button, Alert } from "react-bootstrap"
import axios from "../../../Util/AxiosConfig"

const PersonalInfor = ({ user, setUser }) => {
  const [address, setAddress] = useState(user?.address || "")
  const [dateOfBirth, setDateOfBirth] = useState(user?.dateOfBirth ? new Date(user.dateOfBirth).toISOString().split('T')[0] : "")
  const [isEditing, setIsEditing] = useState(false)
  const [loading, setLoading] = useState(false)
  const [message, setMessage] = useState({ text: "", type: "" })

  if (!user) {
    return <div>Không có dữ liệu người dùng.</div>
  }

  const handleEditToggle = () => {
    setIsEditing(!isEditing)
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setLoading(true)
    setMessage({ text: "", type: "" })

    try {
      // Chuyển đổi chuỗi ngày thành đối tượng Date hoặc null
      const formattedDate = dateOfBirth ? new Date(dateOfBirth).toISOString().split("T")[0] : null;

      await axios.put("/users/update-info", {
        address,
        dateOfBirth: formattedDate
      });


      // Cập nhật thông tin người dùng tại client
      setUser({
        ...user,
        address: address,
        dateOfBirth: dateOfBirth
      })

      setMessage({ text: "Cập nhật thông tin thành công!", type: "success" })
      setIsEditing(false)
    } catch (error) {
      console.error("Lỗi khi cập nhật thông tin:", error)
      setMessage({ 
        text: error.response?.data?.message || "Đã xảy ra lỗi khi cập nhật thông tin.", 
        type: "danger" 
      })
    } finally {
      setLoading(false)
    }
  }

  return (
    <Card>
      <Card.Header className="d-flex justify-content-between align-items-center">
        <h4>🧑‍💼 Thông tin cá nhân</h4>
        {!isEditing ? (
          <Button variant="primary" size="sm" onClick={handleEditToggle}>
            Chỉnh sửa
          </Button>
        ) : null}
      </Card.Header>
      <Card.Body>
        {message.text && (
          <Alert variant={message.type} dismissible onClose={() => setMessage({ text: "", type: "" })}>
            {message.text}
          </Alert>
        )}

        {!isEditing ? (
          <>
            <Row className="mb-3">
              <Col md={4}>Họ và Tên:</Col>
              <Col md={8}>{user.userName || "Chưa cập nhật"}</Col>
            </Row>
            <Row className="mb-3">
              <Col md={4}>Email:</Col>
              <Col md={8}>{user.email || "Chưa cập nhật"}</Col>
            </Row>
            <Row className="mb-3">
              <Col md={4}>Số điện thoại:</Col>
              <Col md={8}>{user.phoneNumber || "Chưa cập nhật"}</Col>
            </Row>
            <Row className="mb-3">
              <Col md={4}>Địa chỉ:</Col>
              <Col md={8}>{user.address || "Chưa cập nhật"}</Col>
            </Row>
            <Row className="mb-3">
              <Col md={4}>Ngày sinh:</Col>
              <Col md={8}>
                {user.dateOfBirth 
                  ? new Date(user.dateOfBirth).toLocaleDateString('vi-VN')
                  : "Chưa cập nhật"}
              </Col>
            </Row>
          </>
        ) : (
          <Form onSubmit={handleSubmit}>
            <Form.Group as={Row} className="mb-3">
              <Form.Label column md={4}>Họ và Tên:</Form.Label>
              <Col md={8}>
                <Form.Control plaintext readOnly defaultValue={user.userName || "Chưa cập nhật"} />
              </Col>
            </Form.Group>
            
            <Form.Group as={Row} className="mb-3">
              <Form.Label column md={4}>Email:</Form.Label>
              <Col md={8}>
                <Form.Control plaintext readOnly defaultValue={user.email || "Chưa cập nhật"} />
              </Col>
            </Form.Group>
            
            <Form.Group as={Row} className="mb-3">
              <Form.Label column md={4}>Số điện thoại:</Form.Label>
              <Col md={8}>
                <Form.Control plaintext readOnly defaultValue={user.phoneNumber || "Chưa cập nhật"} />
              </Col>
            </Form.Group>
            
            <Form.Group as={Row} className="mb-3">
              <Form.Label column md={4}>Địa chỉ:</Form.Label>
              <Col md={8}>
                <Form.Control 
                  type="text" 
                  value={address} 
                  onChange={(e) => setAddress(e.target.value)}
                  placeholder="Nhập địa chỉ của bạn"
                />
              </Col>
            </Form.Group>
            
            <Form.Group as={Row} className="mb-3">
              <Form.Label column md={4}>Ngày sinh:</Form.Label>
              <Col md={8}>
                <Form.Control 
                  type="date" 
                  value={dateOfBirth} 
                  onChange={(e) => setDateOfBirth(e.target.value)}
                />
              </Col>
            </Form.Group>
            
            <div className="d-flex justify-content-end">
              <Button variant="secondary" className="me-2" onClick={handleEditToggle}>
                Hủy
              </Button>
              <Button variant="primary" type="submit" disabled={loading}>
                {loading ? "Đang cập nhật..." : "Lưu thông tin"}
              </Button>
            </div>
          </Form>
        )}
      </Card.Body>
    </Card>
  )
}

export default PersonalInfor