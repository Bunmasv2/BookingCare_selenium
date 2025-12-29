import { Award, Star, StarHalf, User } from 'lucide-react'
import { Badge, Button, Card, Col, Row } from 'react-bootstrap'
// import ServiceImages from "../../Image/Service/Index"
import { useNavigate } from 'react-router-dom'

function DoctorServiceCard({ specialty, item, review, type }) {
    const navigate = useNavigate()
    const renderStars = (rating) => {
        const stars = []
        const fullStars = Math.floor(rating)
        const hasHalfStar = rating % 1 >= 0.5

        for (let i = 0; i < fullStars; i++) {
            stars.push(<Star key={`star-${i}`} className="text-warning" fill="#ffc107" size={18} />)
        }

        if (hasHalfStar) {
            stars.push(<StarHalf key="half-star" className="text-warning" fill="#ffc107" size={18} />)
        }

        const emptyStars = 5 - fullStars - (hasHalfStar ? 1 : 0)
            for (let i = 0; i < emptyStars; i++) {
            stars.push(<Star key={`empty-star-${i}`} className="text-muted" size={18} />)
        }

        return stars
    }

    if (type === "dịch vụ") {
        return (
            <Card className="border-0 shadow-sm mb-4 overflow-hidden">
                <Card.Header className="bg-primary text-white py-3">
                    <h4 className="mb-0">Thông tin {type}</h4>
                </Card.Header>
                
                <Card.Body className="p-3">
                    <Row className="align-items-center">
                        <Col lg={7} xs={12} className="mb-4 mb-lg-0">
                            <Row className="align-items-center">
                                <Col md={4} className="text-center text-md-start pe-0">
                                <Card.Img
                                    src={item?.serviceIcon || "/placeholder.svg?height=150&width=150"}
                                    alt={item?.serviceName}
                                    style={{ width: "100%", height: "100%", objectFit: "cover" }}
                                />
                                </Col>

                                <Col md={8} className="ps-0">
                                <div className="mt-3 mt-md-0">
                                    <h3 className="fw-bold text-primary mb-2">{item?.serviceName || "Chưa cập nhật"}</h3>
                                        <div className="d-flex align-items-center mb-2">
                                        <Award size={18} className="text-primary me-2" />
                                        <span className="fw-semibold">{specialty || "Chưa cập nhật"}</span>
                                    </div>
                                    <div className="d-flex align-items-center mb-3">
                                        <User size={18} className="text-primary me-2" />
                                        {/* <span>{item?.position || "Vị trí"}</span> */}
                                    </div>
                                        <div className="d-flex align-items-center">
                                        <Button onClick={() => navigate(`/dich-vu/${item?.name}`)}>Chi tiết</Button>
                                    </div>
                                </div>
                                </Col>
                            </Row>
                        </Col>

                        <Col lg={5} xs={12}>
                            <Card className="border-0 bg-light shadow-sm">
                                <Card.Body className="p-4">
                                    <div className="d-flex align-items-center mb-3">
                                        <h4 className="mb-0 me-2">Đánh giá trung bình:</h4>
                                        <span className="fs-4 fw-bold text-primary">{review?.avgScore || "N/A"}</span>
                                        <span className="ms-2 text-muted">/5</span>
                                    </div>

                                    <div className="mb-3 d-flex align-items-center">{renderStars(review?.avgScore || 0)}</div>
                                    <Badge bg="primary" className="px-3 py-2 rounded-pill">
                                        {review?.reviewCount || 0} lượt đánh giá
                                    </Badge>
                                </Card.Body>
                            </Card>
                        </Col>
                    </Row>
                </Card.Body>
            </Card>
        )
    }

    return (
        <Card className="border-0 shadow-sm mb-4 overflow-hidden">
            <Card.Header className="bg-primary text-white py-3">
                <h4 className="mb-0">Thông tin {type}</h4>
            </Card.Header>
            
            <Card.Body className="p-3">
                <Row className="align-items-center">
                    <Col lg={7} xs={12} className="mb-4 mb-lg-0">
                        <Row className="align-items-center">
                            <Col md={4} className="text-center text-md-start pe-0">
                            <Card.Img
                                src={item?.doctorImage || "/placeholder.svg?height=150&width=150"}
                                alt={item?.userName}
                                style={{ width: "100%", height: "100%", objectFit: "cover" }}
                            />
                            </Col>

                            <Col md={8} className="ps-0">
                            <div className="mt-3 mt-md-0">
                                <h3 className="fw-bold text-primary mb-2">{item?.userName || "Chưa cập nhật"}</h3>
                                    <div className="d-flex align-items-center mb-2">
                                    <Award size={18} className="text-primary me-2" />
                                    <span className="fw-semibold">{specialty || "Chưa cập nhật"}</span>
                                </div>
                                <div className="d-flex align-items-center mb-3">
                                    <User size={18} className="text-primary me-2" />
                                    <span>{item?.position || "Vị trí"}</span>
                                </div>
                                    <div className="d-flex align-items-center">
                                    <Button onClick={() => navigate(`/bac-si/${item?.userName}`)}>Chi tiết</Button>
                                </div>
                            </div>
                            </Col>
                        </Row>
                    </Col>

                    <Col lg={5} xs={12}>
                        <Card className="border-0 bg-light shadow-sm">
                            <Card.Body className="p-4">
                                <div className="d-flex align-items-center mb-3">
                                    <h4 className="mb-0 me-2">Đánh giá trung bình:</h4>
                                    <span className="fs-4 fw-bold text-primary">{review?.avgScore || "N/A"}</span>
                                    <span className="ms-2 text-muted">/5</span>
                                </div>

                                <div className="mb-3 d-flex align-items-center">{renderStars(review?.avgScore || 0)}</div>
                                <Badge bg="primary" className="px-3 py-2 rounded-pill">
                                    {review?.reviewCount || 0} lượt đánh giá
                                </Badge>
                            </Card.Body>
                        </Card>
                    </Col>
                </Row>
            </Card.Body>
        </Card>
    )
}

export default DoctorServiceCard