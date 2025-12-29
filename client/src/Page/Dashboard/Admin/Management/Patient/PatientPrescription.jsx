import React, { useState, useEffect } from 'react'
import { Container, Table, Button, Spinner, Card, Row, Col, Modal, Form } from 'react-bootstrap'
import axios from '../../../../../Util/AxiosConfig'
import { extractDateOnly } from '../../../../../Util/DateUtils'
import PrescriptionCard from '../../../../../Component/Card/PrescriptionCard'
import PrescriptionDetail from './PrescriptionDetail'
import CashPaymentModal from './CashPaymentModel';

const PatientPrescriptions = ({ patientId, patientName, goBack }) => {
  const [patientPrescriptions, setPatientPrescriptions] = useState([])
  const [loading, setLoading] = useState(true)
  const [viewMode, setViewMode] = useState('table')
  const [selectedPrescriptionId, setSelectedPrescriptionId] = useState(null)
  const [showPaymentModal, setShowPaymentModal] = useState(false)
  const [selectedRecord, setSelectedRecord] = useState(null)
  const [showCashModal, setShowCashModal] = useState(false);
  
  const [startDate, setStartDate] = useState('')
  const [endDate, setEndDate] = useState('')
  const [selectedService, setSelectedService] = useState('')
  const [selectedStatus, setSelectedStatus] = useState('')
  const [uniqueServices, setUniqueServices] = useState([])
  const [uniqueStatuses, setUniqueStatuses] = useState([])
  const [cashGiven, setCashGiven] = useState('')
  const [changeAmount, setChangeAmount] = useState(0)

  useEffect(() => {
    if (patientId) {
      fetchPatientPrescriptions()
    }
  }, [patientId])

  useEffect(() => {
    const savedPrescriptionId = sessionStorage.getItem("selectedPrescriptionId");
    if (savedPrescriptionId) {
      setSelectedPrescriptionId(savedPrescriptionId)
      sessionStorage.removeItem("selectedPrescriptionId")
    }
  }, [])

  useEffect(() => {
    if (patientPrescriptions.length > 0) {
      const services = [...new Set(patientPrescriptions.map(p => p.serviceName))]
      const statuses = [...new Set(patientPrescriptions.map(p => p.status))]
      setUniqueServices(services)
      setUniqueStatuses(statuses)
    }
  }, [patientPrescriptions])

  const fetchPatientPrescriptions = async (filters = {}) => {
    setLoading(true)
    try {
      let url = `/medicalrecords/prescriptions/patient/${patientId}`
      
      const queryParams = [];
      if (filters.startDate) queryParams.push(`startDate=${filters.startDate}`)
      if (filters.endDate) queryParams.push(`endDate=${filters.endDate}`)
      if (filters.serviceName) queryParams.push(`serviceName=${encodeURIComponent(filters.serviceName)}`)
      if (filters.status) queryParams.push(`status=${encodeURIComponent(filters.status)}`)
      
      if (queryParams.length > 0) {
        url += `?${queryParams.join('&')}`
      }
      
      const response = await axios.get(url)

      setPatientPrescriptions(response.data)
    } catch (err) {
      console.error('Lỗi khi lấy đơn thuốc bệnh nhân:', err)
    } finally {
      setLoading(false)
    }
  }

  const applyFilters = () => {
    const filters = {
      startDate: startDate || null,
      endDate: endDate || null,
      serviceName: selectedService || null,
      status: selectedStatus || null
    };
    
    fetchPatientPrescriptions(filters);
  }

  const resetFilters = () => {
    setStartDate('');
    setEndDate('');
    setSelectedService('');
    setSelectedStatus('');
    fetchPatientPrescriptions();
  }

  const handleSelectPrescription = (recordId) => {
    console.log("===> Chọn recordId:", recordId)
    setSelectedPrescriptionId(recordId)
  }

  const handleBackToList = () => {
    setSelectedPrescriptionId(null)
  }

  const handlePaymentClick = (record) => {
    setSelectedRecord(record)
    setShowPaymentModal(true)
  }

  const handleCloseModal = () => {
    setShowPaymentModal(false)
    setSelectedRecord(null)
  }

  const handleMomoPayment = async () => {
    try {
      const response = await axios.post('/medicalrecords/create-payment', {
        orderInfo: "Thanh toán đơn thuốc",
        recordId: selectedRecord.recordId,
      });
  
      if (response.data.payUrl) {
        window.location.href = response.data.payUrl;
      } else {
        alert("Không nhận được URL thanh toán từ MoMo.");
      }
    } catch (error) {
      console.error('Lỗi khi tạo yêu cầu thanh toán MoMo:', error);
      alert("Có lỗi xảy ra khi tạo thanh toán.");
    }
  }

  const handleVnpayPayment = async () => {
    try {
      console.log(selectedRecord.recordId)
      const response = await axios.post(`/medicalrecords/create-vnpay/${selectedRecord.recordId}`);

      if (response.status === 200 && response.data.paymentUrl) {
        window.location.href = response.data.paymentUrl;
      } else {
        alert("Không lấy được URL thanh toán.");
      }
    } catch (error) {
      console.error('Lỗi khi tạo yêu cầu thanh toán VNPay:', error);
      alert("Có lỗi xảy ra khi tạo thanh toán.");
    }
  }

  const openCashPaymentModal = () => {
    setShowPaymentModal(false);
    setShowCashModal(true);
  };

  const renderFilterSection = () => {
    return (
      <Card className="mb-3">
        <Card.Body>
          <Row className="align-items-end">
            <Col md={3}>
              <Form.Group className="mb-md-0 mb-3">
                <Form.Label>Từ ngày</Form.Label>
                <Form.Control 
                  type="date" 
                  value={startDate} 
                  onChange={(e) => setStartDate(e.target.value)}
                />
              </Form.Group>
            </Col>
            <Col md={3}>
              <Form.Group className="mb-md-0 mb-3">
                <Form.Label>Đến ngày</Form.Label>
                <Form.Control 
                  type="date" 
                  value={endDate} 
                  onChange={(e) => setEndDate(e.target.value)}
                />
              </Form.Group>
            </Col>
            <Col md={2}>
              <Form.Group className="mb-md-0 mb-3">
                <Form.Label>Dịch vụ</Form.Label>
                <Form.Select
                  value={selectedService}
                  onChange={(e) => setSelectedService(e.target.value)}
                >
                  <option value="">Tất cả</option>
                  {uniqueServices.map((service, index) => (
                    <option key={index} value={service}>{service}</option>
                  ))}
                </Form.Select>
              </Form.Group>
            </Col>
            <Col md={2}>
              <Form.Group className="mb-md-0 mb-3">
                <Form.Label>Trạng thái</Form.Label>
                <Form.Select
                  value={selectedStatus}
                  onChange={(e) => setSelectedStatus(e.target.value)}
                >
                  <option value="">Tất cả</option>
                  {uniqueStatuses.map((status, index) => (
                    <option key={index} value={status}>{status}</option>
                  ))}
                </Form.Select>
              </Form.Group>
            </Col>
            <Col md={2} className="d-flex">
              <Button variant="primary" className="me-2 w-50" onClick={applyFilters}>
                Lọc
              </Button>
              <Button variant="outline-secondary" className="w-50" onClick={resetFilters}>
                Đặt lại
              </Button>
            </Col>
          </Row>
        </Card.Body>
      </Card>
    );
  }

  return (
    <Container fluid>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h4>
          {selectedPrescriptionId
            ? `Chi tiết toa thuốc #${selectedPrescriptionId}`
            : `Toa thuốc của ${patientName}`}
        </h4>
        <div>
          <Button
            variant="outline-secondary"
            size="sm"
            className="me-2"
            onClick={selectedPrescriptionId ? handleBackToList : goBack}
          >
            {selectedPrescriptionId ? 'Quay lại danh sách' : 'Quay lại'}
          </Button>
          {!selectedPrescriptionId && (
            <>
              <Button
                variant={viewMode === 'table' ? 'primary' : 'outline-primary'}
                size="sm"
                className="me-2"
                onClick={() => setViewMode('table')}
              >
                Dạng bảng
              </Button>
              <Button
                variant={viewMode === 'card' ? 'primary' : 'outline-primary'}
                size="sm"
                onClick={() => setViewMode('card')}
              >
                Dạng thẻ
              </Button>
            </>
          )}
        </div>
      </div>

      {loading ? (
        <div className="text-center py-3">
          <Spinner animation="border" />
        </div>
      ) : selectedPrescriptionId ? (
        <PrescriptionDetail recordId={selectedPrescriptionId} goBack={handleBackToList} />
      ) : (
        <>
          {renderFilterSection()}
          {patientPrescriptions.length > 0 ? (
            viewMode === 'table' ? (
              <div className="table-responsive">
                <Table bordered hover>
                  <thead>
                    <tr>
                      <th>Mã toa thuốc</th>
                      <th>Bác sĩ phụ trách</th>
                      <th>Dịch vụ khám</th>
                      <th>Chẩn đoán</th>
                      <th>Ngày tạo</th>
                      <th>Trạng thái</th>
                      <th>Thao tác</th>
                    </tr>
                  </thead>
                  <tbody>
                    {patientPrescriptions.map(p => (
                      <tr key={p.recordId}>
                        <td>{p.recordId}</td>
                        <td>{p.doctorName}</td>
                        <td>{p.serviceName}</td>
                        <td>{p.diagnosis}</td>
                        <td>{extractDateOnly(p.appointmentDate)}</td>
                        <td>{p.status}</td>
                        <td className="text-nowrap">
                          <div className="d-flex gap-2 flex-nowrap align-items-center">
                            <Button
                              variant="outline-primary"
                              size="sm"
                              className="d-inline-flex align-items-center text-nowrap"
                              onClick={() => handleSelectPrescription(p.recordId)}
                            >
                              Chi tiết
                            </Button>
                            {p.status === "Đã hoàn thành" || p.status === "Đã khám" ? (
                              <Button 
                                size="sm" 
                                variant={p.status === "Đã hoàn thành" ? "secondary" : "success"}
                                className="d-flex justify-content-center align-items-center text-nowrap"
                                onClick={() => handlePaymentClick(p)}
                                disabled={p.status === "Đã hoàn thành"}
                                style={{ minWidth: "120px"  }}
                                mx-auto
                              >
                                {p.status === "Đã hoàn thành" ? "Đã thanh toán" : "Thanh toán"}
                              </Button>
                            ) : (
                              <Button 
                                size="sm" 
                                variant="outline-secondary"
                                className="d-inline-flex align-items-center text-nowrap"
                                disabled
                                style={{ minWidth: "120px" }}
                              >
                                Không khả dụng
                              </Button>
                            )}
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </Table>
              </div>
            ) : (
              <Card.Body>
                <h4>Đơn Thuốc</h4>
                <p>Lịch sử đơn thuốc đã kê</p>
                
                {patientPrescriptions.map(record => (
                  <PrescriptionCard
                    key={record.recordId}
                    record={record}
                    tabActive="prescriptions"
                    isSelected={record.recordId === selectedPrescriptionId}
                    onSelect={() => handleSelectPrescription(record.recordId)}
                    onPayment={() => handlePaymentClick(record)}
                  />
                ))}
              </Card.Body>
            )
          ) : (
            <Card className="text-center p-4">
              <Card.Body>
                <p className="mb-0">Không có đơn thuốc nào phù hợp với bộ lọc</p>
              </Card.Body>
            </Card>
          )}
        </>
      )}

      <Modal show={showPaymentModal} onHide={handleCloseModal} centered>
        <Modal.Header closeButton>
          <Modal.Title>Chọn phương thức thanh toán</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <div className="d-grid gap-3">
            {/* <Button 
              variant="warning" 
              size="lg" 
              onClick={handleMomoPayment}
              className="d-flex align-items-center justify-content-center"
              style={{ backgroundColor: '#ae2070', borderColor: '#ae2070' }}
            >
              <img 
                src="https://cdn.haitrieu.com/wp-content/uploads/2022/10/Logo-MoMo-Square-1024x1024.png" 
                alt="MoMo" 
                style={{ height: '30px', marginRight: '10px' }} 
              />
              Thanh toán qua MoMo
            </Button> */}
            
            <Button 
              variant="primary" 
              size="lg" 
              onClick={handleVnpayPayment}
              className="d-flex align-items-center justify-content-center"
              style={{ backgroundColor: '#0072bc', borderColor: '#0072bc' }}
            >
              <img 
                src="https://vinadesign.vn/uploads/images/2023/05/vnpay-logo-vinadesign-25-12-57-55.jpg" 
                alt="VNPay" 
                style={{ height: '30px', marginRight: '10px' }} 
              />
              Thanh toán qua VNPay
            </Button>

            <Button 
              variant="primary" 
              size="lg" 
              onClick={openCashPaymentModal}
              className="d-flex align-items-center justify-content-center"
              style={{ backgroundColor: '#40649A', borderColor: '#40649A' }}
            >
              <img 
                src="https://cdn-icons-png.freepik.com/512/7630/7630510.png" 
                alt="VNPay" 
                style={{ height: '30px', marginRight: '10px' }} 
              />
              Thanh toán bằng tiền mặt
            </Button>
          </div>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={handleCloseModal}>
            Hủy
          </Button>
        </Modal.Footer>
      </Modal>
      <CashPaymentModal
        show={showCashModal}
        handleClose={() => setShowCashModal(false)}
        record={selectedRecord}
        onPaymentSuccess={() => {
          fetchPatientPrescriptions();
          setShowCashModal(false);
        }}
      />
    </Container>
  );
}

export default PatientPrescriptions