import { Badge, Calendar, Clock } from 'lucide-react'
import { useEffect, useState } from 'react'
import { Card, Col, Row } from 'react-bootstrap'
import axios from '../../../Util/AxiosConfig'
import PrescriptionCard from '../../../Component/Card/PrescriptionCard'
import { formatDateToLocale } from "../../../Util/DateUtils"
import AppointmentStatus from '../../../Component/AppointmentSatus'

function Overview({ tabActive, setTabActive }) {
    const [appointment, setAppointment] = useState()
    const [medicalRecords, setMedicalRecords] = useState([])

    useEffect(() => {
        if (tabActive !== "overview") return

        const fetchAppointment = async () => {
            try {
                const response = await axios.get("/appointments/recently")

                setAppointment(response.data)
            } catch (error) {
                console.log(error)
            }
        }

        fetchAppointment()
    }, [tabActive])

    // useEffect(() => {
    //     if (appointment === null || appointment === undefined || tabActive !== 'overview') return

    //     const fetchPrescriptions = async () => {
    //         try {
    //             const response = await axios.get("/medicalRecords/prescriptions/recently")

    //             setMedicalRecords(response.data)
    //         } catch (error) {
    //             console.log(error)
    //         }
    //     }

    //     fetchPrescriptions()
    // }, [tabActive, appointment])

    return (
        <Row>
            <Col xs={12} >
                <AppointmentStatus appointment={appointment} />
            </Col>
            {/* <Col xs={12} className='px-4'>
                <Card className="shadow border-0" style={{ borderRadius: "15px", overflow: "hidden" }}>
                    <Card.Header className="header-gradient text-white border-0" style={{ padding: "25px" }}>
                        <Row className="align-items-center">
                            <Col md={8}>
                                <div className="d-flex align-items-center">
                                    <div
                                        className="me-3 d-flex align-items-center justify-content-center"
                                        style={{
                                            width: "45px",
                                            height: "45px",
                                            backgroundColor: "rgba(255,255,255,0.2)",
                                            borderRadius: "10px"
                                        }}
                                    >
                                        <Calendar size={24} />
                                    </div>
                                    <div>
                                        <h4 className="mb-1 fw-bold">Trạng thái lịch khám</h4>
                                        <p className="mb-0 opacity-75">
                                            Mã đặt lịch: <span className="fw-semibold">{ }</span>
                                        </p>
                                    </div>
                                </div>
                            </Col>
                        </Row>
                    </Card.Header>
                    <Card.Body>
                        <h5 className="mb-4 d-flex align-items-center gap-2">
                            <Clock size={20} className="text-primary" />
                            Lịch Hẹn Sắp Tới
                        </h5>

                        <div
                            className="border rounded-4 p-3 d-flex align-items-start gap-3 bg-light-subtle hover-shadow transition"
                            style={{ cursor: "pointer" }}
                        >
                            <div className="bg-white rounded-circle p-3 shadow-sm d-flex align-items-center justify-content-center">
                                <Clock size={28} className="text-success" />
                            </div>

                            <div className="flex-grow-1">
                                <p className="mb-1 fw-semibold text-dark">
                                    {formatDateToLocale(appointment?.appointmentDate)} • {appointment?.appointmentTime}
                                </p>
                                <p className="mb-1 text-muted">
                                    <strong>Bác sĩ:</strong> {appointment?.doctorName}
                                </p>
                                <p className="mb-0 text-muted">
                                    <strong>Dịch vụ:</strong> {appointment?.serviceName}
                                </p>
                            </div>

                            <div className="d-none d-md-block text-end">
                                <Badge bg="success" pill>
                                    Sắp tới
                                </Badge>
                            </div>
                        </div>
                    </Card.Body>
                </Card>
            </Col>

            <Col xs={12} >
                <Card>
                    <Card.Body>
                        <h5 className='mb-3'>Đơn Thuốc Gần Đây</h5>

                        {medicalRecords.map((record, index) => (
                            <div key={index}>
                                <PrescriptionCard
                                    key={record.recordId}
                                    record={record}
                                    tabActive={tabActive}
                                    setTabActive={setTabActive}
                                    isSelected={true}
                                />
                            </div>
                        ))}
                    </Card.Body>
                </Card>
            </Col> */}
        </Row>
    )
}

export default Overview