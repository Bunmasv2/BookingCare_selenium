import { useEffect, useState } from 'react'
import { Button, Col, Container, Form, ListGroup, Row } from 'react-bootstrap'
import axios from '../../../../Util/AxiosConfig'
import ReviewDetail from './ReviewDetail'

function Review({ tabActive }) {
    const [specialties, setSpecialties] = useState([])
    const [specialty, setSpecialty] = useState('')
    const [services, setServices] = useState([])
    const [service, setService] = useState()
    const [reviews, setReviews] = useState()
    const [review, setReview] = useState()
    const [showReviewDetail, setShowReviewDetail] = useState(false)
        
    useEffect(() => {
        if (tabActive !== "reviewservices") return 

        const fetchSpecialty = async () => {
            try {
                const response = await axios.get('/specialties')
                setSpecialties(response.data)
                setSpecialty(response.data[0])
            } catch (error) {
                console.error(error)
            }
        }

        fetchSpecialty()
    }, [tabActive])

    useEffect(() => {
        if (!specialty) return

        const fetchServices = async () => {
            try {
                const response = await axios.get(`/services/${specialty?.name}/services`)

                setServices(response.data)
                console.log(response)
            } catch (error) {
                console.error(error)
            }
        }

        fetchServices()
    }, [specialty])

    useEffect(() => {
        if (services === null) return

        const fetchReviewDoctor = async () => {
            try {
                const response = await axios.get(`/reviews/services/${specialty?.name}`)  
                setReviews(response.data)
            } catch (error) {
                console.log(error)
            }
        }

        fetchReviewDoctor()
    }, [services, specialty])

    const handleSpecialty = (e) => {
        const selectedId = parseInt(e.target.value)
        const selectedSpecialty = specialties.find(s => s.specialtyId === selectedId)
        console.log(selectedSpecialty)
        setSpecialty(selectedSpecialty)         
    }

    if (showReviewDetail) {
        return (
            <ReviewDetail specialty={specialty?.name} service={service} review={review} />
        )
    }

    return (
        <Container className="mt-4">
            <Form.Group className="mb-3">
                <Form.Select value={specialty?.specialtyId || ''} onChange={handleSpecialty}>
                    {specialties.map((spec) => (
                        <option key={spec.specialtyId} value={spec.specialtyId}>
                            {spec.name}
                        </option>
                    ))}
                </Form.Select>
            </Form.Group>

            <div className="doctors-section">
                <h5 className="mb-4 fw-bold text-primary">Danh sách Dịch vụ</h5>
                {services.length === 0 ? (
                    <p className="text-muted">Không có dịch vụ nào trong chuyên khoa này.</p>
                ) : (
                <Row className="g-3">
                    {services.map((service, index) => {
                    const r = reviews.find((rev) => rev.serviceId === service.serviceId)
                    console.log(services)
                    return (
                        <Col md={6} key={index}>
                            <div className="border p-3 rounded shadow-sm d-flex align-items-center">
                                <Col xs={9} className='d-flex' >
                                    <img
                                        src={service?.serviceImage || 'https://via.placeholder.com/80'}
                                        alt={service.userName}
                                        className="doctor-image rounded-circle me-3"
                                        style={{ width: '80px', height: '80px', objectFit: 'cover' }}
                                    />
                                    <div className="flex-grow-1 mt-auto">
                                        <h6 className="fw-bold mb-1">{service?.serviceName}</h6>
                                        <p className="text-muted mb-0">Giá: {service?.price?.toLocaleString('vi-VN')} VND</p>
                                        <p className="mb-0">
                                            <span className="fw-bold text-warning">
                                                {r?.avgScore ? r.avgScore.toFixed(1) : 'N/A'}
                                            </span>{' '}
                                            / 5 ★ | {r?.reviewCount || 0} lượt đánh giá
                                        </p>
                                    </div>
                                </Col>

                                <Col xs={3} className='mt-auto ms-3'>
                                    <div>
                                        <Button
                                            variant="primary"
                                            onClick={() => {
                                                setService(service)
                                                setReview(r)
                                                setShowReviewDetail(true)
                                            }}
                                        >
                                            Chi tiết
                                        </Button>
                                    </div>
                                </Col>
                            </div>
                        </Col>
                    )
                    })}
                </Row>
                )}
            </div>
        </Container>
    )
}

export default Review