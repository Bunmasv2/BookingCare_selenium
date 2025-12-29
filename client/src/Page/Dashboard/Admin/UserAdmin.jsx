import { useState, useEffect } from "react"
import { Table, Container, Spinner, Button, Modal, Pagination, Row, Col, Form, Card, Badge, Nav, Tab } from "react-bootstrap"
import axios from "../../../Util/AxiosConfig"

const UserAdmin = ({ tabActive }) => {
  const [users, setUsers] = useState([])
  const [loading, setLoading] = useState(true)
  const [currentPage, setCurrentPage] = useState(0)
  const [role, setRole] = useState("admin")
  const [searchTerm, setSearchTerm] = useState("")
  const [showModal, setShowModal] = useState(false)
  const [selectedUser, setSelectedUser] = useState(null)
  const [editableUser, setEditableUser] = useState(null)
  const [editingField, setEditingField] = useState(null)
  const [keys, setKeys] = useState([])
  const [saving, setSaving] = useState(false)
  const [activeTab, setActiveTab] = useState("personal")
  const itemsPerPage = 10
  const totalPages = Math.ceil(users.length / itemsPerPage)

  useEffect(() => {
    if (tabActive !== "users") return
    
    fetchUsers()
  }, [role, tabActive])

  useEffect(() => {
    if (searchTerm === "" || searchTerm === null) {
      fetchUsers()
      return
    }
    
    const searchUser = setTimeout(async () => {
      try {
        const response = await axios.get(`/users/search/${role}/${searchTerm}`)
        setUsers(response.data)
      } catch (error) {
        console.log(error)
      }
    }, 300)

    return () => clearTimeout(searchUser)
  }, [role, searchTerm])

  const fetchUsers = async () => {
    if (tabActive !== "users") return

    try {
      const response = await axios.get(`/users/${role}`)
      setUsers(response.data)
      console.log(response.data)
    } catch (err) {
      console.error("Lỗi khi lấy danh sách người dùng:", err)
    } finally {
      setLoading(false)
    }
  }

  const processUserFields = (user) => {
    if (!user) return
    const fields = Object.keys(user)
    const filteredFields = fields.filter(key => key !== "doctorImage" || key.indexOf("Id") === 0);

    setKeys(filteredFields)
  }

  const handleRowClick = (user) => {
    setEditableUser({...user})
    setEditingField(null)
    setShowModal(true)
    setSelectedUser(user)
    processUserFields(user)
  }

  const handleEditClick = (field) => {
    setEditingField(field)
  }

  const handleFieldChange = (field, value) => {
    setEditableUser(prev => ({...prev, [field]: value}))
  }

  const handleBlur = () => {
    setEditingField(null)
  }

  const saveChanges = async () => {
    if (!editableUser) return
    
    setSaving(true)
    try {
      console.log(editableUser)
      const response = await axios.put(`/users/edit/${role}`, editableUser)
      // setSelectedUser({...editableUser})
      console.log(response.data)
      // const updatedUsers = users.map(user => 
      //   user.id === editableUser.id ? editableUser : user
      // )
      // setUsers(updatedUsers)
      
    } catch (error) {
      console.error("Error saving user:", error)
    } finally {
      setSaving(false)
    }
  }

  const cancelEditing = () => {
    setEditableUser({...selectedUser})
    setEditingField(null)
  }

  const startIndex = currentPage * itemsPerPage
  const endIndex = startIndex + itemsPerPage
  const currentUsers = users.slice(startIndex, endIndex)

  if (loading) {
    return (
      <Container fluid className="d-flex justify-content-center align-items-center" style={{ minHeight: "300px" }}>
        <Spinner animation="border" role="status">
          <span className="visually-hidden">Đang tải...</span>
        </Spinner>
      </Container>
    )
  }

  return (
    <Container fluid className="py-4">
      <h4 className="mb-4">Danh Sách Người Dùng</h4>
      <Row className="mb-4">
        <Col md={3}>
          <Form.Select value={role} onChange={(e) => setRole(e.target.value)}>
            {["admin", "doctor", "patient"].map((r, idx) => (
              <option key={idx} value={r}>
                {r}
              </option>
            ))}
          </Form.Select>
        </Col>
        <Col md={6}>
          <Form.Control
            type="text"
            placeholder="Tìm theo tên..."
            value={searchTerm}
            onChange={(e) => {
              setSearchTerm(e.target.value)
              setCurrentPage(0)
            }}
          />
        </Col>
      </Row>

      <div className="table-responsive">
        <Table bordered hover>
          <thead>
            <tr>
              <th>ID</th>
              <th>Họ và Tên</th>
              <th>Email</th>
              <th>Số điện thoại</th>
            </tr>
          </thead>
          <tbody>
            {currentUsers.length === 0 ? (
              <tr>
                <td colSpan="5" className="text-center">
                  Không có người dùng nào
                </td>
              </tr>
            ) : (
              currentUsers.map((user, index) => (
                <tr key={user.id} onClick={() => handleRowClick(user)} style={{cursor: 'pointer'}}>
                  <td>{startIndex + index + 1}</td>
                  <td>{user.fullName}</td>
                  <td>{user.email}</td>
                  <td>{user.phoneNumber}</td>
                </tr>
              ))
            )}
          </tbody>
        </Table>
      </div>

      <Row>
        <Col className="d-flex justify-content-center mt-3">
          <Pagination>
            <Pagination.First onClick={() => setCurrentPage(0)} disabled={currentPage === 0} />
            <Pagination.Prev
              onClick={() => setCurrentPage((prev) => Math.max(prev - 1, 0))}
              disabled={currentPage === 0}
            />
            {Array.from({ length: totalPages }).map((_, index) => (
              <Pagination.Item key={index} active={index === currentPage} onClick={() => setCurrentPage(index)}>
                {index + 1}
              </Pagination.Item>
            ))}
            <Pagination.Next
              onClick={() => setCurrentPage((prev) => Math.min(prev + 1, totalPages - 1))}
              disabled={currentPage === totalPages - 1}
            />
            <Pagination.Last onClick={() => setCurrentPage(totalPages - 1)} disabled={currentPage === totalPages - 1} />
          </Pagination>
        </Col>
      </Row>

      <Modal show={showModal} onHide={() => setShowModal(false)} centered size="lg" backdrop="static">
        <Modal.Header closeButton>
          <div className="d-flex align-items-center">
            {selectedUser?.doctorImage ? (
              <img src={selectedUser.doctorImage} alt="Avatar" className="rounded-circle me-3" style={{ width: 60, height: 60, objectFit: "cover" }} />
            ) : (
              <div className="rounded-circle bg-secondary text-white d-flex align-items-center justify-content-center me-3" style={{ width: 60, height: 60, fontSize: 24 }}>
                {selectedUser?.fullName?.[0]?.toUpperCase() || "U"}
              </div>
            )}
            <div>
              <h5 className="mb-0">{selectedUser?.fullName}</h5>
              <small className="text-muted">{selectedUser?.email}</small>
            </div>
          </div>
        </Modal.Header>

        <Modal.Body>
          <Row>
            {keys.map((key, index) => (
              <Col md={6} key={index} className="mb-3">
                <Form.Group>
                  <Form.Label className="fw-semibold text-primary" onClick={() => handleEditClick(key)} style={{ cursor: "pointer" }}>
                    {key}
                  </Form.Label>

                  {editingField === key ? (
                    <div className="d-flex">
                      <Form.Control
                        type="text"
                        value={editableUser?.[key] || ""}
                        onChange={(e) => handleFieldChange(key, e.target.value)}
                        autoFocus
                        className="flex-grow-1"
                      />
                      <Button variant="success" size="sm" className="ms-2" onClick={handleBlur}>
                        <i className="bi bi-check"></i>
                      </Button>
                      <Button variant="danger" size="sm" className="ms-1" onClick={cancelEditing}>
                        <i className="bi bi-x"></i>
                      </Button>
                    </div>
                  ) : (
                    <Card className="border p-2 shadow-sm rounded small bg-light">
                      {selectedUser?.[key] || <span className="text-muted">Chưa cập nhật</span>}
                    </Card>
                  )}
                </Form.Group>
              </Col>
            ))}
          </Row>
        </Modal.Body>

        <Modal.Footer className="bg-light d-flex justify-content-between">
          <div className="text-muted small">
            <i className="bi bi-pencil-square me-1"></i> Nhấn vào tên trường để chỉnh sửa
          </div>
          <div>
            <Button variant="secondary" onClick={() => setShowModal(false)}>
              Đóng
            </Button>
            {editableUser && selectedUser && JSON.stringify(editableUser) !== JSON.stringify(selectedUser) && (
              <Button variant="primary" onClick={saveChanges} disabled={saving} className="ms-2">
                {saving ? (
                  <>
                    <Spinner as="span" animation="border" size="sm" role="status" aria-hidden="true" className="me-1" />
                    Đang lưu...
                  </>
                ) : "Lưu thay đổi"}
              </Button>
            )}
          </div>
        </Modal.Footer>
      </Modal>

    </Container>
  )
}

export default UserAdmin