import React, { useEffect, useState } from 'react'
import { Container, Table, Spinner, Badge, Button, Modal } from 'react-bootstrap'
import { extractDateOnly } from "../../../Util/DateUtils"
import axios from "../../../Util/AxiosConfig"

function PatientHistory({ tabActive }) {
    console.log(tabActive)
    const [patients, setPatients] = useState([])
    const [loading, setLoading] = useState(true)
    const [showDetailsModal, setShowDetailsModal] = useState(false)
    const [selectedPatient, setSelectedPatient] = useState(null)
    const [showMedicalRecordDetail, setShowMedicalRecordDetail] = useState(false)
    const [htmlPrescription, setHtmlPrescription] = useState("")

    useEffect(() => {
        fetchExaminedPatients()
    }, [tabActive])

    const fetchExaminedPatients = async () => {
        if (tabActive !== "patientHistory") return
        setLoading(true)
    
        try {
            const response = await axios.get('/appointments/examined_patients')
            setPatients(response.data.schedules || [])
        } catch (error) {
            console.error('Error fetching examined patients:', error.response?.data || error.message)
        } finally {
            setLoading(false)
        }
    }

    const handleViewDetails = (patient) => {
        setSelectedPatient(patient)
        setShowDetailsModal(true)
    }

    const handleCloseDetailsModal = () => {
        setShowDetailsModal(false)
        setSelectedPatient(null)
    }

    const handleMedicalRecordDetail = async (appointmentId) => {
        try {
            console.log(appointmentId)
            const response = await axios.get(`/medicalrecords/recorddetail/${appointmentId}`, {
  headers: {
    'Accept': 'text/html'
  },
  responseType: 'text'
})

            setHtmlPrescription(response.data)
            setShowMedicalRecordDetail(true)
        } catch (error) {
            console.error('Lỗi khi lấy toa thuốc:', error.response?.data || error.message)
        }
    }

    const handleCloseMedicalRecordDetail = () => {
        setShowMedicalRecordDetail(false)
        setHtmlPrescription("")
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
            <h2 className="mb-4">Danh sách bệnh nhân đã khám</h2>

            <Table responsive striped bordered hover>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Bệnh nhân</th>
                        <th>Dịch vụ</th>
                        <th>Ngày khám</th>
                        <th>Trạng thái</th>
                        <th>Thao tác</th>
                    </tr>
                </thead>
                <tbody>
                    {patients.length === 0 ? (
                        <tr>
                            <td colSpan="6" className="text-center">Không có bệnh nhân nào đã được khám</td>
                        </tr>
                    ) : (
                        patients.map((patient, index) => (
                            <tr key={index}>
                                <td>{index + 1}</td>
                                <td>{patient.patientName}</td>
                                <td>{patient.serviceName}</td>
                                <td>{extractDateOnly(patient.appointmentDate)}</td>
                                <td>
                                    <Badge bg="primary">
                                        {patient.status}
                                    </Badge>
                                </td>
                                <td>
                                    <Button
                                        variant="outline-info"
                                        size="sm"
                                        onClick={() => handleViewDetails(patient)}
                                        className="me-2"
                                    >
                                        Xem chi tiết
                                    </Button>
                                    <Button
                                        variant="outline-success"
                                        size="sm"
                                        onClick={() => handleMedicalRecordDetail(patient.appointmentId)}
                                    >
                                        Toa thuốc đã kê
                                    </Button>
                                </td>
                            </tr>
                        ))
                    )}
                </tbody>
            </Table>

            {/* Patient Details Modal */}
            <Modal show={showDetailsModal} onHide={handleCloseDetailsModal} size="lg">
                <Modal.Header closeButton>
                    <Modal.Title>Chi tiết lịch sử khám bệnh</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {selectedPatient && (
                        <div>
                            <h5>Thông tin lịch khám</h5>
                            <Table bordered>
                                <tbody>
                                    <tr>
                                        <td className="fw-bold" style={{width: "30%"}}>Mã lịch hẹn</td>
                                        <td>{selectedPatient.appointmentId}</td>
                                    </tr>
                                    <tr>
                                        <td className="fw-bold">Bệnh nhân</td>
                                        <td>{selectedPatient.patientName}</td>
                                    </tr>
                                    <tr>
                                        <td className="fw-bold">Bác sĩ</td>
                                        <td>{selectedPatient.doctorName}</td>
                                    </tr>
                                    <tr>
                                        <td className="fw-bold">Dịch vụ</td>
                                        <td>{selectedPatient.serviceName}</td>
                                    </tr>
                                    <tr>
                                        <td className="fw-bold">Ngày khám</td>
                                        <td>{extractDateOnly(selectedPatient.appointmentDate)}</td>
                                    </tr>
                                    <tr>
                                        <td className="fw-bold">Trạng thái</td>
                                        <td>
                                            <Badge bg="primary">{selectedPatient.status}</Badge>
                                        </td>
                                    </tr>
                                </tbody>
                            </Table>
                        </div>
                    )}
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleCloseDetailsModal}>
                        Đóng
                    </Button>
                </Modal.Footer>
            </Modal>

            {/* Modal Chi tiết toa thuốc */}
            <Modal show={showMedicalRecordDetail} onHide={handleCloseMedicalRecordDetail} size="lg">
                <Modal.Header closeButton>
                    <Modal.Title>Chi tiết toa thuốc đã kê</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {htmlPrescription ? (
                        <div dangerouslySetInnerHTML={{ __html: htmlPrescription }} />
                    ) : (
                        <p>Không có toa thuốc nào được kê.</p>
                    )}
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleCloseMedicalRecordDetail}>
                        Đóng
                    </Button>
                </Modal.Footer>
            </Modal>
        </Container>
    )
}

export default PatientHistory
