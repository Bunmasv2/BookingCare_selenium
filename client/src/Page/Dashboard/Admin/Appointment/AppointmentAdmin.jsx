import React, { useState, useEffect } from 'react'
import { Container, Table, Button, Badge, Modal, Spinner, Pagination, Row, Col, Form } from 'react-bootstrap'
import axios from '../../../../Util/AxiosConfig'
import { extractDateOnly } from '../../../../Util/DateUtils'
import { PencilSquare } from 'react-bootstrap-icons'

const statusOptions = ['Đã xác nhận', 'Đã hủy', 'Chờ xác nhận']

const statusColors = {
  'Chờ xác nhận': 'warning',
  'Đã xác nhận': 'info',
  'Đã hoàn thành': 'success',
  'Đã hủy': 'danger'
}

const AppointmentAdmin = ({ month, year }) => {
  const [appointments, setAppointments] = useState([])
  const [loading, setLoading] = useState(true)
  const [updating, setUpdating] = useState(false)
  const [showModal, setShowModal] = useState(false)
  const [selected, setSelected] = useState(null)
  const [newStatus, setNewStatus] = useState('')
  const [currentPage, setCurrentPage] = useState(0)

  const itemsPerPage = 10
  const totalPages = Math.ceil(appointments.length / itemsPerPage)

  useEffect(() => {
    const fetchAppointments = async () => {
      setLoading(true)
      try {
        const { data } = await axios.get(`/appointments/${month}/${year}`)
        setAppointments(data)
      } catch (err) {
        console.error('Error fetching appointments:', err)
      } finally {
        setLoading(false)
      }
    }

    fetchAppointments()
  }, [month, year])

  const openModal = (appointment) => {
    setSelected(appointment)
    setNewStatus(appointment.status)
    setShowModal(true)
  }

  const closeModal = () => {
    setShowModal(false)
    setSelected(null)
    setNewStatus('')
  }

  const getAvailableStatusOptions = (currentStatus) => {
    if (currentStatus === 'Đã hoàn thành' || currentStatus === 'Đã hủy') {
      return statusOptions.filter(status => status !== 'Chờ xác nhận')
    }
    if (currentStatus === 'Đã xác nhận') {
      return statusOptions.filter(status => status !== 'Chờ xác nhận')
    }
    return statusOptions
  }

  const updateStatus = async () => {
    if (!selected || newStatus === selected.status) return

    setUpdating(true)
    try {
      await axios.put(`/appointments/status/${selected.appointmentId}`, { status: newStatus })
      setAppointments(prev => prev.map(a =>
        a.appointmentId === selected.appointmentId ? { ...a, status: newStatus } : a
      ))
      closeModal()
    } catch (err) {
      console.error('Error updating status:', err)
    } finally {
      setUpdating(false)
    }
  }

  const currentAppointments = appointments.slice(currentPage * itemsPerPage, (currentPage + 1) * itemsPerPage)

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
                  <th>Bệnh nhân</th>
                  <th>Bác sĩ</th>
                  <th>Dịch vụ</th>
                  <th>Ngày hẹn</th>
                  <th>Trạng thái</th>
                  <th>Hành động</th>
                </tr>
              </thead>
              <tbody>
                {currentAppointments.length === 0 ? (
                  <tr>
                    <td colSpan="7" className="text-center">Không có lịch hẹn</td>
                  </tr>
                ) : (
                  currentAppointments.map((item, index) => (
                    <tr key={item.appointmentId}>
                      <td className='text-center'>{index + 1 + currentPage * itemsPerPage}</td>
                      <td>{item.patientName}</td>
                      <td>{item.doctorName}</td>
                      <td>{item.serviceName}</td>
                      <td>{extractDateOnly(item.appointmentDate)}</td>
                      <td>
                        <Badge bg={statusColors[item.status] || 'secondary'}>{item.status}</Badge>
                      </td>
                      <td>
                        <Button size="sm" variant="outline-primary" onClick={() => openModal(item)}>
                          <PencilSquare size={16} className="me-1" /> Sửa
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

      <Modal show={showModal} onHide={closeModal}>
        <Modal.Header closeButton>
          <Modal.Title>Cập nhật trạng thái</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {selected && (
            <>
              <p><strong>Bệnh nhân:</strong> {selected.patientName}</p>
              <p><strong>Bác sĩ:</strong> {selected.doctorName}</p>
              <p><strong>Ngày hẹn:</strong> {extractDateOnly(selected.appointmentDate)}</p>
              <Form.Select value={newStatus} onChange={(e) => setNewStatus(e.target.value)}>
                {getAvailableStatusOptions(selected.status).map(status => (
                  <option key={status} value={status}>{status}</option>
                ))}
              </Form.Select>
            </>
          )}
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={closeModal}>Đóng</Button>
          <Button
            variant="primary"
            onClick={updateStatus}
            // disabled={updating || newStatus === selected?.status}
          >
            {updating ? 'Đang cập nhật...' : 'Cập nhật'}
          </Button>
        </Modal.Footer>
      </Modal>
    </Container>
  )
}

export default AppointmentAdmin