
import React, { useEffect, useState } from 'react'
import { Container, Table, Button, Badge, Form, Modal, Spinner, Row, Col, ListGroup, Card } from 'react-bootstrap'
import axios from "../../../Util/AxiosConfig"
import { extractDateOnly } from "../../../Util/DateUtils"

const statusColors = {
    'Đã xác nhận': 'info',
    'Đã khám': 'primary',
    'Đã hoàn thành': 'success'
}

function DoctorShiftDetail({ onBack }) {
    const [datetime, setDatetime] = useState({
        date: "",
        time: ""
    })
    const [schedules, setSchedules] = useState([])
    const [loading, setLoading] = useState(true)
    const [showPrescriptionModal, setShowPrescriptionModal] = useState(false)
    const [currentAppointment, setCurrentAppointment] = useState(null)
    const [newStatus, setNewStatus] = useState('')
    const [prescriptionInfo, setPrescriptionInfo] = useState({
        notes: '',
        diagnosis: '',
        treatment: ''
    })

    const [medicinesList, setMedicinesList] = useState([])
    const [selectedMedicines, setSelectedMedicines] = useState([])
    const [searchTerm, setSearchTerm] = useState('')
    const [medicineSuggestions, setMedicineSuggestions] = useState([])
    const [showSuggestions, setShowSuggestions] = useState(false)

    // Current medicine being edited
    const [currentMedicine, setCurrentMedicine] = useState({
        medicineId: '',
        medicineName: "",
        dosage: 0,
        frequencyPerDay: 0,
        durationInDays: 0,
        usage: '',
        unit: ""
    })

    useEffect(() => {
        const params = new URLSearchParams(
            window.location.hash.split("?")[1]
        )

        setDatetime({
            date: params.get("date"),
            time: decodeURIComponent(params.get("shift"))
        })
    }, [])

    useEffect(() => {
        if (!datetime?.date || !datetime.time) return
        fetchDoctorSchedule()
        fetchMedicines()
    }, [datetime])

    // Add debounce effect for search
    useEffect(() => {
        const timeoutId = setTimeout(() => {
            if (searchTerm.trim()) {
                searchMedicines(searchTerm)
            } else {
                setMedicineSuggestions([])
            }
        }, 300)

        return () => clearTimeout(timeoutId)
    }, [searchTerm])

    const fetchDoctorSchedule = async () => {
        setLoading(true)
        console.log(datetime)
        if (!datetime?.date || !datetime?.time) return

        try {
            const response = await axios.get('/appointments/schedule_detail', {
                params: {
                    date: datetime.date,
                    time: datetime.time
                }
            })
            console.log(response)
            setSchedules(response.data.schedules || [])
        } catch (error) {
            console.log(error)
        } finally {
            setLoading(false)
        }
    }

    const fetchMedicines = async () => {
        try {
            const response = await axios.get('/medicines')
            setMedicinesList(response.data || [])
        } catch (error) {
            console.log(error.response?.data || error.message)
        }
    }

    const searchMedicines = async (query) => {
        if (!query.trim()) {
            setMedicineSuggestions([])
            return
        }

        try {
            const response = await axios.get(`/medicines/search`, {
                params: { query }
            })
            setMedicineSuggestions(response.data || [])
            setShowSuggestions(true)
        } catch (error) {
            console.log(error.response?.data || error.message)
            setMedicineSuggestions([])
        }
    }

    const handlePrescriptionChange = (field, value) => {
        setPrescriptionInfo(prev => ({
            ...prev,
            [field]: value
        }))
    }

    const handleOpenPrescriptionModal = async (appointment) => {
        setCurrentAppointment(appointment)
        setSelectedMedicines([])
        setShowPrescriptionModal(true)
        resetCurrentMedicine()
    }

    const handleClosePrescriptionModal = () => {
        setShowPrescriptionModal(false)
        setCurrentAppointment(null)
        setSelectedMedicines([])
        resetCurrentMedicine()
        setSearchTerm('')
    }

    const resetCurrentMedicine = () => {
        setCurrentMedicine({
            medicineId: "",
            medicineName: "",
            dosage: 0,
            frequencyPerDay: 0,
            durationInDays: 0,
            usage: "",
            unit: ""
        })
        setSearchTerm('')
    }

    const handleSavePrescription = async () => {
        try {
            const payload = {
                ...prescriptionInfo,
                medicines: selectedMedicines
            }
            console.log(currentAppointment.appointmentId)
            const response = await axios.post(`/medicalRecords/${currentAppointment.appointmentId}`, payload)

            console.log(response.data)
            handleClosePrescriptionModal()
            await fetchDoctorSchedule()
        } catch (err) {
            console.error('Error saving prescription:', err)
        }
    }

    const handleAddMedicine = () => {
        if (!currentMedicine.medicineId || !currentMedicine.dosage || !currentMedicine.frequencyPerDay || !currentMedicine.durationInDays) {
            return
        }

        const quantity = parseInt(currentMedicine.dosage)
            * parseInt(currentMedicine.frequencyPerDay)
            * parseInt(currentMedicine.durationInDays)
        const newMedicine = { ...currentMedicine, quantity }

        setSelectedMedicines([...selectedMedicines, newMedicine])
        resetCurrentMedicine()
    }

    const handleRemoveMedicine = (index) => {
        const updated = [...selectedMedicines]
        updated.splice(index, 1)
        setSelectedMedicines(updated)
    }

    const handleMedicineChange = (field, value) => {
        setCurrentMedicine(prev => ({
            ...prev,
            [field]: value
        }))
    }

    const handleSelectMedicine = (medicine) => {
        setCurrentMedicine({
            ...currentMedicine,
            medicineId: medicine.medicineId,
            medicineName: medicine.medicalName,
            unit: medicine.unit
        })
        setSearchTerm(medicine.medicalName)
        setShowSuggestions(false)
        setIsMouseOverSuggestions(false)
    }

    const handleSearchInputChange = (e) => {
        setSearchTerm(e.target.value)
        setShowSuggestions(true)
    }

    const [isMouseOverSuggestions, setIsMouseOverSuggestions] = useState(false)

    const handleSearchInputBlur = () => {
        if (!isMouseOverSuggestions) {
            setShowSuggestions(false)
        }
    }

    if (loading) {
        return (
            <Container className="d-flex justify-content-center align-items-center" style={{ minHeight: '300px' }}>
                <Spinner animation="border" role="status">
                    <span className="visually-hidden">Đang tải...</span>
                </Spinner>
            </Container>
        )
    }

    return (
        <Container fluid className="mt-4">
            <div className='d-flex justify-content-between mb-4'>
                <h2>Chi tiết ca làm việc: {datetime.date} - {datetime.time}</h2>
                <Button variant='warning text-light' onClick={onBack}>Trở về</Button>
            </div>

            <Table responsive striped bordered hover>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Bệnh nhân</th>
                        <th>Dịch vụ</th>
                        <th>Ngày hẹn</th>
                        <th>Trạng thái</th>
                        <th>Thao tác</th>
                    </tr>
                </thead>
                <tbody>
                    {schedules.length === 0 ? (
                        <tr>
                            <td colSpan="7" className="text-center">
                                Không có lịch hẹn nào
                            </td>
                        </tr>
                    ) : (
                        schedules.map((appointment, index) => (
                            <tr
                                key={index}
                                data-testid={`appointment-row-${appointment.patientName}-${appointment.status}`}
                            >
                                <td>{index + 1}</td>
                                <td data-testid="patient-name">
                                    {appointment.patientName}
                                </td>
                                <td>{appointment.serviceName}</td>
                                <td>{extractDateOnly(appointment.appointmentDate)}</td>
                                <td data-testid="appointment-status">
                                    <Badge bg={statusColors[appointment.status] || 'secondary'}>
                                        {appointment.status}
                                    </Badge>
                                </td>
                                <td>
                                    <Button
                                        variant="outline-success"
                                        size="sm"
                                        data-testid="btn-prescribe"
                                        onClick={() => handleOpenPrescriptionModal(appointment)}
                                    >
                                        Kê đơn thuốc
                                    </Button>
                                </td>
                            </tr>
                        ))
                    )}
                </tbody>
            </Table>

            <Modal show={showPrescriptionModal} onHide={handleClosePrescriptionModal} size="lg">
                <Modal.Header closeButton>
                    <Modal.Title>Kê đơn thuốc</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {currentAppointment && (
                        <Form>
                            <Form.Group className="mb-3">
                                <Form.Label>Thông tin lịch hẹn:</Form.Label>
                                <p><strong>Bệnh nhân:</strong> {currentAppointment.patientName}</p>
                                <p><strong>Bác sĩ:</strong> {currentAppointment.doctorName}</p>
                                <p><strong>Dịch vụ:</strong> {currentAppointment.serviceName}</p>
                                <p><strong>Ngày hẹn:</strong> {extractDateOnly(currentAppointment.appointmentDate)}</p>
                            </Form.Group>

                            <Form.Group className="mb-3" controlId="diagnosis">
                                <Form.Label>Chẩn đoán bệnh</Form.Label>
                                <Form.Control
                                    as="textarea"
                                    rows={2}
                                    id="diagnosis"
                                    value={prescriptionInfo.diagnosis}
                                    onChange={(e) => handlePrescriptionChange('diagnosis', e.target.value)}
                                    placeholder="Nhập chẩn đoán"
                                    spellCheck={false}
                                />
                            </Form.Group>

                            <Form.Group className="mb-3" controlId="treatment">
                                <Form.Label>Hướng điều trị</Form.Label>
                                <Form.Control
                                    as="textarea"
                                    rows={2}
                                    id="treatment"
                                    value={prescriptionInfo.treatment}
                                    onChange={(e) => handlePrescriptionChange('treatment', e.target.value)}
                                    placeholder="Nhập hướng điều trị"
                                    spellCheck={false}
                                />
                            </Form.Group>

                            <hr className="my-4" />

                            <h5>Thêm thuốc</h5>
                            <Card className="mb-4">
                                <Card.Body>
                                    <Row className="mb-3 align-items-end">
                                        <Col md={6}>
                                            <Form.Group>
                                                <Form.Label>Tên thuốc</Form.Label>
                                                <div className="position-relative">
                                                    <Form.Control
                                                        type="text"
                                                        id="medicineSearch"
                                                        value={searchTerm}
                                                        onChange={handleSearchInputChange}
                                                        onBlur={handleSearchInputBlur}
                                                        placeholder="Nhập từ khóa để tìm thuốc..."
                                                        autoComplete="off"
                                                        spellCheck={false}
                                                    />
                                                    {showSuggestions && medicineSuggestions.length > 0 && (
                                                        <div
                                                            className="position-absolute w-100 bg-white border rounded-bottom shadow-sm"
                                                            style={{
                                                                zIndex: 1000,
                                                                maxHeight: '200px',
                                                                overflow: 'auto'
                                                            }}
                                                            onMouseEnter={() => setIsMouseOverSuggestions(true)}
                                                            onMouseLeave={() => setIsMouseOverSuggestions(false)}
                                                        >
                                                            {medicineSuggestions.map(medicine => (
                                                                <div
                                                                    key={medicine.medicineId}
                                                                    className="p-2 border-bottom"
                                                                    style={{ cursor: 'pointer' }}
                                                                    onMouseOver={(e) => e.currentTarget.style.backgroundColor = '#f8f9fa'}
                                                                    onMouseOut={(e) => e.currentTarget.style.backgroundColor = ''}
                                                                    onClick={() => handleSelectMedicine(medicine)}
                                                                    onMouseDown={(e) => e.preventDefault()} // Prevent blur before click
                                                                >
                                                                    <strong>{medicine.medicalName}</strong>
                                                                    <span className="text-muted ms-2">({medicine.unit})</span>
                                                                </div>
                                                            ))}
                                                        </div>
                                                    )}
                                                </div>
                                            </Form.Group>
                                        </Col>
                                        <Col md={3}>
                                            <Form.Group>
                                                <Form.Label>Liều dùng (mỗi lần)</Form.Label>
                                                <Form.Control
                                                    type="number"
                                                    id="medicineDosage"
                                                    min="0"
                                                    value={currentMedicine.dosage}
                                                    onChange={(e) => handleMedicineChange('dosage', e.target.value)}
                                                    placeholder="VD: 1"
                                                />
                                            </Form.Group>
                                        </Col>
                                        <Col md={3}>
                                            <Form.Group>
                                                <Form.Label>Đơn vị</Form.Label>
                                                <Form.Control
                                                    type="text"
                                                    id="medicineUnit"
                                                    value={currentMedicine.unit}
                                                />
                                            </Form.Group>
                                        </Col>
                                    </Row>
                                    <Row className="mb-3">
                                        <Col md={4}>
                                            <Form.Group>
                                                <Form.Label>Số lần/ngày</Form.Label>
                                                <Form.Control
                                                    type="number"
                                                    id="medicineFrequency"
                                                    min="0"
                                                    value={currentMedicine.frequencyPerDay}
                                                    onChange={(e) => handleMedicineChange('frequencyPerDay', e.target.value)}
                                                    placeholder="VD: 3"
                                                />
                                            </Form.Group>
                                        </Col>
                                        <Col md={4}>
                                            <Form.Group>
                                                <Form.Label>Số ngày uống</Form.Label>
                                                <Form.Control
                                                    type="number"
                                                    id="medicineDuration"
                                                    min="0"
                                                    value={currentMedicine.durationInDays}
                                                    onChange={(e) => handleMedicineChange('durationInDays', e.target.value)}
                                                    placeholder="VD: 7"
                                                />
                                            </Form.Group>
                                        </Col>
                                        <Col md={4}>
                                            <Form.Group>
                                                <Form.Label>
                                                    Tổng số lượng
                                                    <span className="text-muted ms-2">
                                                        (Tự động tính)
                                                    </span>
                                                </Form.Label>
                                                <Form.Control
                                                    type="number"
                                                    id="medicineQuantity"
                                                    value={
                                                        currentMedicine.dosage *
                                                        currentMedicine.frequencyPerDay *
                                                        currentMedicine.durationInDays || 0
                                                    }
                                                />
                                            </Form.Group>
                                        </Col>
                                    </Row>
                                    <Row>
                                        <Col>
                                            <Form.Group>
                                                <Form.Label>Cách dùng</Form.Label>
                                                <Form.Control
                                                    type="text"
                                                    id="medicineUsage"
                                                    value={currentMedicine.usage}
                                                    onChange={(e) => handleMedicineChange('usage', e.target.value)}
                                                    placeholder="VD: Uống sau khi ăn"
                                                    spellCheck={false}
                                                />
                                            </Form.Group>
                                        </Col>
                                    </Row>
                                    <div className="d-flex justify-content-end mt-3">
                                        <Button
                                            variant="primary"
                                            onClick={handleAddMedicine}
                                            disabled={!currentMedicine.medicineId || (!currentMedicine.dosage && currentMedicine.dosage < 0) || !currentMedicine.frequencyPerDay || !currentMedicine.durationInDays}

                                        >
                                            <i className="bi bi-plus-circle me-1"></i> Thêm thuốc vào đơn
                                        </Button>
                                    </div>
                                </Card.Body>
                            </Card>

                            <h5>Danh sách thuốc đã kê</h5>
                            {selectedMedicines.length === 0 ? (
                                <p className="text-muted">Chưa có thuốc nào được thêm vào đơn</p>
                            ) : (
                                <ListGroup className="mb-4">
                                    {selectedMedicines.map((medicine, idx) => (
                                        <ListGroup.Item key={idx} className="d-flex justify-content-between align-items-center">
                                            <div>
                                                <h6>{medicine.medicineName}</h6>
                                                <div><strong>Liều dùng:</strong> {medicine.dosage} {medicine.unit}/lần, {medicine.frequencyPerDay} lần/ngày, {medicine.durationInDays} ngày</div>
                                                <div><strong>Cách dùng:</strong> {medicine.usage}</div>
                                                <div><strong>Tổng số lượng:</strong> {medicine.quantity} {medicine.unit}</div>
                                            </div>
                                            <Button
                                                variant="outline-danger"
                                                size="sm"
                                                onClick={() => handleRemoveMedicine(idx)}
                                            >
                                                <i className="bi bi-trash"></i>
                                            </Button>
                                        </ListGroup.Item>
                                    ))}
                                </ListGroup>
                            )}

                            <Form.Group className="mb-3">
                                <Form.Label>Lưu ý bổ sung:</Form.Label>
                                <Form.Control
                                    as="textarea"
                                    rows={3}
                                    id="prescriptionNotes"
                                    value={prescriptionInfo.notes}
                                    onChange={(e) => handlePrescriptionChange('notes', e.target.value)}
                                    placeholder="Nhập các lưu ý bổ sung về đơn thuốc..."
                                    spellCheck={false}
                                />
                            </Form.Group>
                        </Form>
                    )}
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClosePrescriptionModal}>
                        Đóng
                    </Button>
                    <Button
                        variant="primary"
                        onClick={handleSavePrescription}
                        data-testid="btn-save"
                        disabled={selectedMedicines.length === 0}
                    >
                        Lưu đơn thuốc
                    </Button>
                </Modal.Footer>
            </Modal>
        </Container>
    )
}

export default DoctorShiftDetail