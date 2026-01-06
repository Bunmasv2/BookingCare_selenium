import { useContext, useEffect, useState } from "react"
import { Button, Col, Container, Form, Row, Modal } from "react-bootstrap"
import { NavContext } from "../Context/NavContext"
import axios from "../Util/AxiosConfig"

function Appointment() {
    const [appointmentForm, setFormData] = useState({
        department: "",
        doctor: "",
        service: "",
        appointmentDate: "",
        appointmentTime: "",
        symptoms: ""
    })

    const { specialties } = useContext(NavContext)
    const [specialty, setSpecialty] = useState("")
    const [doctors, setDoctors] = useState([])
    const [services, setServices] = useState([])

    // const [suggestedAppointments, setSuggestedAppointments] = useState([])
    const [showModal, setShowModal] = useState(false)

    const handleCloseModal = () => setShowModal(false)

    useEffect(() => {
        const fetchDoctors = async () => {
            try {
                const response = await axios.get(`/doctors/${specialty}`)
                setDoctors(response.data)
            } catch (error) {
                console.log(error)
            }
        }
        if (specialty) fetchDoctors()
    }, [specialty])

    useEffect(() => {
        const fetchServices = async () => {
            try {
                const response = await axios.get(`/services/${specialty}/services`)
                setServices(response.data)
            } catch (error) {
                console.log(error)
            }
        }
        if (specialty) fetchServices()
    }, [specialty])

    const handleChange = (event) => {
        const { name, value } = event.target

        if (name === "department") {
            setSpecialty(value)
        }

        setFormData(prev => ({
            ...prev,
            [name]: value
        }))
    }

    const submit = async (e) => {
        try {
            e.preventDefault()
            e.stopPropagation()
            console.log(appointmentForm)

            const formData = new FormData()
            Object.entries(appointmentForm).forEach(([key, value]) => {
                formData.append(key, value)
            })

            const response = await axios.post("/appointments", formData)

            // if (response.data?.availableAppointments) {
            //     setSuggestedAppointments(response.data.availableAppointments)
            //     setShowModal(true)
            // } else {
            //     setSuggestedAppointments([])
            // }

        } catch (error) {
            console.log(error)
            // alert("Đã có lỗi xảy ra. Vui lòng thử lại.")
        }
    }

    return (
        <Container className="p-0 w-75 my-4">
            <div className="text-center mb-4">
                <h1 className="fw-bold" style={{ color: "#0056b3" }}>ĐĂNG KÝ KHÁM</h1>
            </div>

            <div className="border rounded overflow-hidden">
                <Row className="m-0 p-0">
                    <Col md={4} className="p-4 text-white" style={{ backgroundColor: "#0091ea" }}>
                        <h5>Lưu ý:</h5>
                        <p className="text-white">Form này KHÔNG kiểm tra dữ liệu phía UI.</p>
                        <p className="text-white">Dùng cho Selenium / E2E test BE.</p>
                    </Col>

                    <Col md={8} className="p-4 bg-white">
                        <Form>
                            <Form.Group className="mb-3">
                                <Form.Select
                                    id="department"
                                    name="department"
                                    onChange={handleChange}
                                >
                                    <option value="">Chọn chuyên khoa</option>
                                    {specialties.map((s, index) => (
                                        <option key={index} value={s.name}>
                                            {s.name}
                                        </option>
                                    ))}
                                </Form.Select>
                            </Form.Group>

                            <Form.Group className="mb-3">
                                <Form.Select
                                    id="doctor"
                                    name="doctor"
                                    onChange={handleChange}
                                >
                                    <option value="">Chọn bác sĩ</option>
                                    {doctors.map((doctor, index) => (
                                        <option key={index} value={doctor.userName}>
                                            {doctor.userName}
                                        </option>
                                    ))}
                                </Form.Select>
                            </Form.Group>

                            <Form.Group className="mb-3">
                                <Form.Select
                                    id="service"
                                    name="service"
                                    onChange={handleChange}
                                >
                                    <option value="">Chọn dịch vụ</option>
                                    {services.map((service, index) => (
                                        <option
                                            key={index}
                                            value={service.serviceName}
                                        >
                                            {service.serviceName}
                                        </option>
                                    ))}
                                </Form.Select>
                            </Form.Group>

                            <Row className="mb-3">
                                <Col md={6}>
                                    <Form.Control
                                        id="appointmentDate"
                                        type="date"
                                        name="appointmentDate"
                                        onChange={handleChange}
                                    />
                                </Col>

                                <Col md={6}>
                                    <Form.Select
                                        id="appointmentTime"
                                        name="appointmentTime"
                                        onChange={handleChange}
                                    >
                                        <option value="">Buổi khám</option>
                                        <option value="Sáng">Sáng</option>
                                        <option value="Chiều">Chiều</option>
                                    </Form.Select>
                                </Col>
                            </Row>

                            <Form.Group className="mb-4">
                                <Form.Control
                                    id="symptoms"
                                    as="textarea"
                                    rows={4}
                                    placeholder="Triệu chứng"
                                    name="symptoms"
                                    onChange={handleChange}
                                />
                            </Form.Group>

                            <div className="text-end">
                                <Button
                                    id="btn-submit-appointment"
                                    onClick={submit}
                                    className="px-5 py-2 border-0 rounded-5"
                                    style={{ backgroundColor: "#4dabf7" }}
                                >
                                    GỬI
                                </Button>
                            </div>
                        </Form>
                    </Col>
                </Row>
            </div>

            {/* <Modal
                id="suggested-appointments-modal"
                show={showModal}
                onHide={handleCloseModal}
            >
                <Modal.Header closeButton>
                    <h4 className="text-danger">Khung giờ đã đầy</h4>
                </Modal.Header>
                <Modal.Body>
                    <ul>
                        {suggestedAppointments.map((a, index) => (
                            <li key={index}>
                                {new Date(a.date).toLocaleDateString("vi-VN")} - {a.time}
                            </li>
                        ))}
                    </ul>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleCloseModal}>
                        Đóng
                    </Button>
                </Modal.Footer>
            </Modal> */}
        </Container>
    )
}

export default Appointment
