import { useEffect, useState } from "react"
import { Link, useNavigate, useParams } from "react-router-dom"
import { Container, Row, Col, Card, Spinner, Button, Badge } from "react-bootstrap"
import { Calendar, ChevronLeft, ExternalLink, Info, DollarSign } from "lucide-react"
import ReviewCard from "../Component/Card/ReviewCard"
import Recomend from "../Component/Card/Recomend"
import ServiceSteps from "../Component/Card/ServiceSteps"
import axios from "../Util/AxiosConfig"

const ServiceDetail = () => {
  const { serviceName } = useParams()
  const [service, setService] = useState(null)
  const [loading, setLoading] = useState(true)
  const [reviews, setReviews] = useState()
  const [recommendServices, setRecommendServices] = useState()
  const navigate = useNavigate()

  const handleAppointment = () => {
    navigate("/đặt lịch khám")
  }

  useEffect(() => {
    const fetchServiceDetail = async () => {
      try {
        const response = await axios.get(`/services/detail/${serviceName}`)
        setService(response.data)
      } catch (error) {
        console.error("Lỗi khi lấy chi tiết dịch vụ:", error)
      } finally {
        setLoading(false)
      }
    }

    fetchServiceDetail()
  }, [serviceName])

  useEffect(() => {
    if (service === null) return

    const fetchServiceReviews = async () => {
      try {
        const type = "service"
        const response = await axios.get(`/reviews/${type}/${service?.serviceId}`)
        setReviews(response.data)
      } catch (error) {
        console.log(error)
      }
    }

    fetchServiceReviews()
  }, [service])

  useEffect(() => {
    const fetchRadomServices = async () => {
      try {
        const response = await axios.get(`/services/random`)
        setRecommendServices(response.data)
      } catch (error) {
        console.log(error)
      }
    }

    fetchRadomServices()
  }, [serviceName])

  if (loading) {
    return (
      <Container className="d-flex flex-column align-items-center justify-content-center" style={{ minHeight: "60vh" }}>
        <Spinner animation="border" variant="primary" className="mb-3" />
        <p className="text-muted">Đang tải thông tin dịch vụ...</p>
      </Container>
    )
  }

  if (!service) {
    return (
      <Container className="d-flex flex-column align-items-center justify-content-center" style={{ minHeight: "60vh" }}>
        <div className="text-center">
          <h4 className="mb-3">Không tìm thấy thông tin dịch vụ</h4>
          <p className="text-muted mb-4">Thông tin dịch vụ bạn đang tìm kiếm không tồn tại hoặc đã bị xóa</p>
          <Button
            variant="outline-primary"
            onClick={() => window.history.back()}
            className="d-flex align-items-center gap-2"
          >
            <ChevronLeft size={18} />
            Quay lại
          </Button>
        </div>
      </Container>
    )
  }

  const formattedPrice = typeof service.price === "number" ? service.price.toLocaleString() + " VNĐ" : service.price

  return (
    <Container fluid className="py-4 bg-light">
      <Container className="w-75">
        <div className="d-flex align-items-center mb-4">
          <Button
            variant="link"
            className="p-0 text-decoration-none d-flex align-items-center text-muted"
            onClick={() => window.history.back()}
          >
            <ChevronLeft size={18} />
            <span>Quay lại trang trước</span>
          </Button>
        </div>

        <Row>
          <Col lg={8} className="mb-4 mb-lg-0">
            <Card className="border-0 shadow-sm mb-4 overflow-hidden">
              <Card.Body className="p-4">
                <h2 className="text-primary fw-bold mb-3">{service.serviceName}</h2>
                <div className="d-flex flex-wrap gap-2 mb-3">
                  <Badge bg="light" text="dark" className="d-flex align-items-center gap-1 py-2 px-3">
                    <DollarSign size={14} />
                    <span>{formattedPrice}</span>
                  </Badge>
                  <Badge bg="light" text="dark" className="d-flex align-items-center gap-1 py-2 px-3">
                    <Calendar size={14} />
                    <span>Thời gian: 30-60 phút</span>
                  </Badge>
                </div>
              </Card.Body>
            </Card>

            <Card className="border-0 shadow-sm mb-4 overflow-hidden">
              <Card.Img
                variant="top"
                src={service.serviceImage}
                alt={`Hình ảnh của ${service.serviceName}`}
                className="img-fluid"
                style={{ objectFit: "cover", maxHeight: "400px", width: "100%" }}
              />
            </Card>

            <Card className="border-0 shadow-sm mb-4">
              <Card.Header className="bg-white p-4 border-bottom">
                <h4 className="text-primary mb-0 d-flex align-items-center gap-2">
                  <Info size={20} />
                  Giới thiệu dịch vụ
                </h4>
              </Card.Header>
              <Card.Body className="p-4">
                <div className="bg-light p-4 rounded">
                  {service.description ? (
                    <div dangerouslySetInnerHTML={{ __html: service.description.replace(/\n/g, "<br/>") }} />
                  ) : (
                    <p className="text-muted fst-italic">Mô tả dịch vụ đang được cập nhật...</p>
                  )}
                </div>
              </Card.Body>
            </Card>

            <Card className="border-0 shadow-sm">
              <ServiceSteps />
            </Card>
          </Col>

          <Col lg={4}>
            <Card className="border-0 shadow-sm mb-4">
              <Card.Header className="bg-white p-3 border-bottom">
                <h5 className="mb-0 fw-bold">Thông tin dịch vụ</h5>
              </Card.Header>
              <Card.Body className="p-3">
                <div className="mb-4">
                  <div className="d-flex justify-content-between mb-2">
                    <span>Giá dịch vụ:</span>
                    <span className="fw-medium text-primary">{formattedPrice}</span>
                  </div>
                  <div className="d-flex justify-content-between mb-2">
                    <span>Thời gian thực hiện:</span>
                    <span className="fw-medium">30-60 phút</span>
                  </div>
                </div>

                <Button
                  variant="primary"
                  size="lg"
                  className="w-100 d-flex align-items-center justify-content-center gap-2 fw-bold"
                  onClick={handleAppointment}
                >
                  <Calendar size={18} />
                  Đặt lịch ngay
                </Button>
              </Card.Body>
            </Card>

            <Card className="border-0 shadow-sm mb-4">
              <Card.Header className="bg-white p-3 border-bottom">
                <h5 className="mb-0 fw-bold">Thông tin phòng khám</h5>
              </Card.Header>
              <Card.Body className="p-3">
                <div className="d-flex align-items-start gap-2 mb-3">
                  <div>
                    <p className="mb-1 fw-medium">Địa chỉ phòng khám:</p>
                    <p className="text-muted mb-1">475A Đ. Điện Biên Phủ, Phường 25, Bình Thạnh, Hồ Chí Minh</p>
                    <Link
                      to="https://www.google.com/maps/dir//HUTECH,+7+Nguy%E1%BB%85n+Gia+Tr%C3%AD,+Ph%C6%B0%E1%BB%9Dng+25,+B%C3%ACnh+Th%E1%BA%A1nh,+H%E1%BB%93+Ch%C3%AD+Minh,+Vi%E1%BB%87t+Nam/@10.8018525,106.6740191,13z/data=!3m1!4b1!4m9!4m8!1m0!1m5!1m1!1s0x31752953ade9f9c9:0x6ad5d15cd48a4f4e!2m2!1d106.7152576!2d10.8018439!3e0?hl=vi-VN&entry=ttu&g_ep=EgoyMDI1MDMwNC4wIKXMDSoASAFQAw%3D%3D"
                      target="_blank"
                      className="d-flex align-items-center gap-1 text-primary"
                    >
                      <span>Xem bản đồ</span>
                      <ExternalLink size={14} />
                    </Link>
                  </div>
                </div>

                <hr className="my-3" />

                <h6 className="fw-bold mb-2">Giờ làm việc</h6>
                <div className="d-flex justify-content-between mb-1">
                  <span>Thứ 2 - Thứ 6:</span>
                  <span className="fw-medium">08:00 - 17:00</span>
                </div>
                <div className="d-flex justify-content-between mb-1">
                  <span>Thứ 7:</span>
                  <span className="fw-medium">08:00 - 12:00</span>
                </div>
                <div className="d-flex justify-content-between">
                  <span>Chủ nhật:</span>
                  <span className="fw-medium">Nghỉ</span>
                </div>
              </Card.Body>
            </Card>

            <Card className="border-0 shadow-sm mb-4 bg-white">
              <Card.Header className="bg-white p-3 border-bottom">
                <h5 className="mb-0 fw-bold">Dịch vụ khác</h5>
              </Card.Header>
              <Card.Body className="p-0">
                <div className="d-flex flex-column">
                  {recommendServices?.map((recommend, index) => (
                    <Recomend item={recommend} type={"service"} />
                  ))}
                </div>
              </Card.Body>
            </Card>

            <Card className="border-0 shadow-sm">
              <Card.Header className="bg-white p-3 border-bottom">
                <h5 className="mb-0 fw-bold">Đánh giá từ bệnh nhân</h5>
              </Card.Header>
              <Card.Body className="pb-1">
                <div className="d-flex flex-column gap-3">
                  {reviews?.length > 0 ?
                    reviews.map(review => 
                      <ReviewCard review={review} />
                    ) :
                    <div className="py-auto">
                      <p>Không có đánh giá</p>
                    </div>
                  }
                </div>
              </Card.Body>
            </Card>
          </Col>
        </Row>
      </Container>
    </Container>
  )
}

export default ServiceDetail