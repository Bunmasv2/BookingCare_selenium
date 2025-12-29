import React, { useState, useEffect } from 'react'
import { Container, Table, Button, Modal, Spinner, Pagination, Row, Col, Form, Badge } from 'react-bootstrap'
import axios from '../../../../Util/AxiosConfig'
import { PencilSquare } from 'react-bootstrap-icons'
import { extractDateOnly } from '../../../../Util/DateUtils'

const statusColors = {
  'Chưa phản hồi': 'warning',
  'Đã phản hồi': 'success'
}

const ContactMessagesAdmin = () => {
  const [messages, setMessages] = useState([])
  const [loading, setLoading] = useState(true)
  const [currentPage, setCurrentPage] = useState(0)
  const [selected, setSelected] = useState(null)
  const [replyMessage, setReplyMessage] = useState('')
  const [sending, setSending] = useState(false)
  const [showModal, setShowModal] = useState(false)

  const itemsPerPage = 10
  const totalPages = Math.ceil(messages.length / itemsPerPage)

  useEffect(() => {
    const fetchMessages = async () => {
      setLoading(true)
      try {
        const reponse = await axios.get('/contactmessages')
        console.log(reponse.data)
        setMessages(reponse.data)
      } catch (err) {
        console.error('Lỗi khi tải tin nhắn:', err)
      } finally {
        setLoading(false)
      }
    }

    fetchMessages()
  }, [])

  const openModal = (msg) => {
    setSelected(msg)
    setReplyMessage('')
    setShowModal(true)
  }

  const closeModal = () => {
    setSelected(null)
    setReplyMessage('')
    setShowModal(false)
  }

  const handleSendReply = async () => {
    if (!replyMessage || !selected) return
    setSending(true)
    try {
    await axios.post(`/contactmessages/reponse-message/${selected.id}`, {
                        Message: replyMessage
                        })
      setMessages(prev => prev.map(m => m.id === selected.id ? { ...m, status: 'Đã phản hồi' } : m))
      closeModal()
    } catch (err) {
      console.error('Gửi phản hồi thất bại:', err)
    } finally {
      setSending(false)
    }
  }

  const currentMessages = messages.slice(currentPage * itemsPerPage, (currentPage + 1) * itemsPerPage)

  if (loading) {
    return (
      <Container className="text-center py-5">
        <Spinner animation="border" variant="primary" />
        <div>Đang tải dữ liệu...</div>
      </Container>
    )
  }

  return (
    <Container fluid>
      <Row>
        <Col>
          <div className="table-responsive">
            <Table striped bordered hover>
              <thead className='table-light'>
                <tr>
                  <th className='text-center'>STT</th>
                  <th>Tên bệnh nhân</th>
                  <th>Nội dung</th>
                  <th>Ngày gửi</th>
                  <th>Trạng thái</th>
                  <th>Hành động</th>
                </tr>
              </thead>
              <tbody>
                {currentMessages.length === 0 ? (
                  <tr><td colSpan="6" className="text-center">Không có tin nhắn</td></tr>
                ) : (
                  currentMessages.map((msg, index) => (
                    <tr key={msg.id}>
                      <td className='text-center'>{index + 1 + currentPage * itemsPerPage}</td>
                      <td>{msg.patientName}</td>
                      <td>{msg.messages}</td>
                      <td>{extractDateOnly(msg.createdAt)}</td>
                      <td><Badge bg={statusColors[msg.status] || 'secondary'}>{msg.status}</Badge></td>
                      <td>
                        <Button size="sm" variant="outline-primary" onClick={() => openModal(msg)}>
                          <PencilSquare size={16} className="me-1" /> Phản hồi
                        </Button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </Table>
          </div>

          <div className="d-flex justify-content-center mt-3">
            <Pagination>
              <Pagination.First disabled={currentPage === 0} onClick={() => setCurrentPage(0)} />
              <Pagination.Prev disabled={currentPage === 0} onClick={() => setCurrentPage(p => Math.max(p - 1, 0))} />
              {Array.from({ length: totalPages }).map((_, i) => (
                <Pagination.Item key={i} active={i === currentPage} onClick={() => setCurrentPage(i)}>
                  {i + 1}
                </Pagination.Item>
              ))}
              <Pagination.Next disabled={currentPage === totalPages - 1} onClick={() => setCurrentPage(p => p + 1)} />
              <Pagination.Last disabled={currentPage === totalPages - 1} onClick={() => setCurrentPage(totalPages - 1)} />
            </Pagination>
          </div>
        </Col>
      </Row>

      <Modal show={showModal} onHide={closeModal} centered>
        <Modal.Header closeButton>
          <Modal.Title>Chi tiết tin nhắn</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {selected && (
            <>
              <p><strong>Mã bệnh nhân:</strong> {selected.patientID}</p>
              <p><strong>Nội dung:</strong> {selected.messages}</p>
              <p><strong>Ngày gửi:</strong> {extractDateOnly(selected.createdAt)}</p>
              <Form.Group className="mt-3">
                <Form.Label>Phản hồi</Form.Label>
                <Form.Control
                  as="textarea"
                  rows={4}
                  value={replyMessage}
                  onChange={(e) => setReplyMessage(e.target.value)}
                  spellCheck={false}
                />
              </Form.Group>
            </>
          )}
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={closeModal}>Đóng</Button>
          <Button variant="primary" onClick={handleSendReply} disabled={sending || !replyMessage}>
            {sending ? 'Đang gửi...' : 'Gửi phản hồi'}
          </Button>
        </Modal.Footer>
      </Modal>
    </Container>
  )
}

export default ContactMessagesAdmin
