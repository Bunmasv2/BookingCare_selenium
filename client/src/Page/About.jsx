import { Container, Row, Col, Card, Button, Badge } from "react-bootstrap"
import { useNavigate } from "react-router-dom"
import { Building, CheckCircle, Users, Heart, Award, Calendar, Clock, Target, ChevronRight, MapPin, Phone, Mail } from "lucide-react"
import image from '../Image/About/Anh_gioi_thieu.jpg'

function About() {
  const navigate = useNavigate()

  const handleAppointment = () => {
    navigate("/đặt lịch khám")
  }

  const statistics = [
    { number: "10+", label: "Bác sĩ chuyên khoa", icon: <Users size={24} /> },
    { number: "5000+", label: "Bệnh nhân hài lòng", icon: <Heart size={24} /> },
    { number: "15+", label: "Dịch vụ y tế", icon: <CheckCircle size={24} /> },
    { number: "24/7", label: "Hỗ trợ khẩn cấp", icon: <Clock size={24} /> },
  ]

  const timeline = [
    {
      year: "2025",
      title: "Thành lập phòng khám",
      description: "Phòng Khám Đa Khoa X chính thức đi vào hoạt động tại TP.HCM",
    },
    {
      year: "2026",
      title: "Mở rộng chuyên khoa",
      description: "Bổ sung thêm nhiều chuyên khoa mới và trang thiết bị hiện đại",
    },
    {
      year: "2027",
      title: "Hợp tác quốc tế",
      description: "Ký kết hợp tác với các đối tác y tế hàng đầu trong khu vực",
    },
    {
      year: "2028",
      title: "Mở rộng chi nhánh",
      description: "Khai trương chi nhánh mới tại các quận trung tâm TP.HCM",
    },
  ]

  return (
    <Container fluid className="py-5 bg-light">
      <Container className="w-75">
        <Row className="mb-3">
          <Col lg={12}>
            <Card className="border-0 shadow-sm overflow-hidden">
              <Row className="g-0">
                <Col md={6} className="bg-primary text-white p-5 d-flex flex-column justify-content-center">
                  <h1 className="fw-bold mb-3">Phòng Khám Đa Khoa X</h1>
                  {/* <p className="fs-5 mb-4">Địa Chỉ Tin Cậy Cho Sức Khỏe Của Bạn</p> */}
                  <p className="mb-3">
                    Phòng Khám Đa Khoa X là một cơ sở y tế tư nhân, chính thức đi vào hoạt động từ năm 2025, với sứ mệnh
                    mang đến dịch vụ khám chữa bệnh chất lượng cao, đáp ứng nhu cầu chăm sóc sức khỏe cho mọi đối tượng.
                  </p>
                  <div>
                    <Button
                      variant="light"
                      className="text-primary fw-bold d-inline-flex align-items-center gap-2"
                      onClick={handleAppointment}
                    >
                      Đặt lịch khám ngay
                      <ChevronRight size={18} />
                    </Button>
                  </div>
                </Col>
                <Col md={6}>
                  <div className="h-100">
                    <img
                      src={image}
                      alt="Phòng Khám Đa Khoa X"
                      className="img-fluid h-90 w-100"
                      style={{ objectFit: "cover" }}
                    />
                  </div>
                </Col>
              </Row>
            </Card>
          </Col>
        </Row>

        <Row className="mb-4">
          <Col lg={12}>
            <Card className="border-0 shadow-sm">
              <Card.Body className="p-4">
                <Row>
                  {statistics.map((stat, index) => (
                    <Col md={3} sm={6} key={index} className="text-center mb-4 mb-md-0">
                      <div className="d-flex flex-column align-items-center">
                        <div className="bg-light p-3 rounded-circle mb-3 text-primary">{stat.icon}</div>
                        <h2 className="fw-bold text-primary mb-1">{stat.number}</h2>
                        <p className="text-muted mb-0">{stat.label}</p>
                      </div>
                    </Col>
                  ))}
                </Row>
              </Card.Body>
            </Card>
          </Col>
        </Row>

        <Row className="mb-4">
          <Col lg={8} className="mb-4 mb-lg-0">
            <Card className="border-0 shadow-sm h-100">
              <Card.Header className="bg-white p-4 border-bottom">
                <h3 className="fw-bold text-primary mb-0">Về Chúng Tôi</h3>
              </Card.Header>
              <Card.Body className="p-4">
                <div className="mb-4">
                  <div className="d-flex align-items-center mb-3">
                    <div className="bg-primary p-2 rounded text-white me-3">
                      <Building size={20} />
                    </div>
                    <h4 className="text-primary mb-0">Cơ Sở Hiện Đại</h4>
                  </div>
                  <p className="text-justify">
                    Phòng khám được thiết kế hiện đại, không gian rộng rãi, thoáng mát với nhiều cây xanh, tạo cảm giác
                    thoải mái cho người bệnh. Chúng tôi đầu tư vào hệ thống trang thiết bị y tế tiên tiến, đồng bộ nhằm
                    hỗ trợ tối ưu cho quá trình chẩn đoán và điều trị. Mục tiêu của chúng tôi là xây dựng Phòng Khám Đa
                    Khoa X trở thành một địa chỉ uy tín, chất lượng, nằm trong top các phòng khám tư nhân hàng đầu tại
                    TP.HCM.
                  </p>
                </div>

                <div className="mb-4">
                  <div className="d-flex align-items-center mb-3">
                    <div className="bg-primary p-2 rounded text-white me-3">
                      <CheckCircle size={20} />
                    </div>
                    <h4 className="text-primary mb-0">Tiêu Chuẩn Chất Lượng</h4>
                  </div>
                  <p className="text-justify">
                    Chúng tôi luôn tuân thủ nghiêm ngặt các tiêu chuẩn về vệ sinh môi trường, kiểm soát nhiễm khuẩn chặt
                    chẽ và không ngừng cập nhật, ứng dụng các kỹ thuật y khoa tiên tiến nhằm nâng cao hiệu quả điều trị.
                    Đặc biệt, quy trình khám bệnh được thực hiện cẩn trọng, khoa học để đảm bảo tính chính xác, hạn chế
                    tối đa sai sót trong chẩn đoán.
                  </p>
                </div>

                <div className="mb-4">
                  <div className="d-flex align-items-center mb-3">
                    <div className="bg-primary p-2 rounded text-white me-3">
                      <Users size={20} />
                    </div>
                    <h4 className="text-primary mb-0">Đội Ngũ Chuyên Gia</h4>
                  </div>
                  <p className="text-justify">
                    Đội ngũ y bác sĩ tại Phòng Khám Đa Khoa X là những chuyên gia tận tâm, giàu kinh nghiệm, được đào
                    tạo bài bản. Bên cạnh đó, chúng tôi còn hợp tác chặt chẽ với các bác sĩ đầu ngành đến từ những bệnh
                    viện lớn tại TP.HCM như Bệnh viện Chợ Rẫy, Bệnh viện Đại học Y Dược TP.HCM... để mang đến chất lượng
                    khám chữa bệnh tốt nhất cho khách hàng.
                  </p>
                </div>

                <div className="mb-4">
                  <div className="d-flex align-items-center mb-3">
                    <div className="bg-primary p-2 rounded text-white me-3">
                      <Heart size={20} />
                    </div>
                    <h4 className="text-primary mb-0">Sứ Mệnh</h4>
                  </div>
                  <p className="text-justify">
                    Sứ mệnh của chúng tôi là cung cấp dịch vụ y tế phù hợp cho tất cả mọi người, kể cả những đối tượng
                    có thu nhập trung bình và thấp, giúp mọi người tiếp cận được dịch vụ chăm sóc sức khỏe chất lượng
                    cao.
                  </p>
                </div>

                <div className="bg-light p-4 rounded text-center mb-4">
                  <h4 className="text-primary fw-bold mb-0">Tận tâm vì sức khỏe cộng đồng!</h4>
                </div>

                <p className="text-justify mb-0">
                  Chúng tôi hiểu rằng chất lượng khám chữa bệnh chính là yếu tố quyết định sự phát triển của phòng khám.
                  Vì vậy, toàn thể nhân viên y tế tại Phòng Khám Đa Khoa X luôn đặt người bệnh lên hàng đầu, làm việc
                  với tinh thần trách nhiệm cao, cùng nhau xây dựng một thương hiệu uy tín, đáng tin cậy trong lòng
                  khách hàng.
                </p>
              </Card.Body>
            </Card>
          </Col>

          <Col lg={4}>
            <Card className="border-0 shadow-sm mb-4">
              <Card.Header className="bg-white p-4 border-bottom">
                <h4 className="fw-bold text-primary mb-0">Giá Trị Cốt Lõi</h4>
              </Card.Header>
              <Card.Body className="p-4">
                <div className="d-flex align-items-start gap-3 mb-4">
                  <div className="bg-light p-2 rounded text-primary">
                    <Target size={20} />
                  </div>
                  <div>
                    <h5 className="fw-bold mb-2">Chuyên Nghiệp</h5>
                    <p className="mb-0 text-muted">
                      Đội ngũ y bác sĩ và nhân viên được đào tạo bài bản, làm việc với tinh thần trách nhiệm cao.
                    </p>
                  </div>
                </div>

                <div className="d-flex align-items-start gap-3 mb-4">
                  <div className="bg-light p-2 rounded text-primary">
                    <Heart size={20} />
                  </div>
                  <div>
                    <h5 className="fw-bold mb-2">Tận Tâm</h5>
                    <p className="mb-0 text-muted">
                      Luôn đặt sức khỏe và quyền lợi của người bệnh lên hàng đầu, phục vụ với tất cả tâm huyết.
                    </p>
                  </div>
                </div>

                <div className="d-flex align-items-start gap-3 mb-4">
                  <div className="bg-light p-2 rounded text-primary">
                    <Award size={20} />
                  </div>
                  <div>
                    <h5 className="fw-bold mb-2">Chất Lượng</h5>
                    <p className="mb-0 text-muted">
                      Cam kết mang đến dịch vụ y tế chất lượng cao với trang thiết bị hiện đại và quy trình chuẩn quốc
                      tế.
                    </p>
                  </div>
                </div>

                <div className="d-flex align-items-start gap-3">
                  <div className="bg-light p-2 rounded text-primary">
                    <CheckCircle size={20} />
                  </div>
                  <div>
                    <h5 className="fw-bold mb-2">Tin Cậy</h5>
                    <p className="mb-0 text-muted">
                      Xây dựng niềm tin với khách hàng thông qua sự minh bạch, trung thực và kết quả điều trị hiệu quả.
                    </p>
                  </div>
                </div>
              </Card.Body>
            </Card>

            <Card className="border-0 shadow-sm">
              <Card.Header className="bg-white p-4 border-bottom">
                <h4 className="fw-bold text-primary mb-0">Thông Tin Liên Hệ</h4>
              </Card.Header>
              <Card.Body className="p-4">
                <div className="d-flex align-items-start gap-3 mb-4">
                  <div className="bg-light p-2 rounded text-primary">
                    <MapPin size={20} />
                  </div>
                  <div>
                    <h5 className="fw-bold mb-2">Địa Chỉ</h5>
                    <p className="mb-0 text-muted">475A Đ. Điện Biên Phủ, Phường 25, Bình Thạnh, Hồ Chí Minh</p>
                  </div>
                </div>

                <div className="d-flex align-items-start gap-3 mb-4">
                  <div className="bg-light p-2 rounded text-primary">
                    <Phone size={20} />
                  </div>
                  <div>
                    <h5 className="fw-bold mb-2">Điện Thoại</h5>
                    <p className="mb-0 text-muted">Hotline: 1900 1234</p>
                  </div>
                </div>

                <div className="d-flex align-items-start gap-3 mb-4">
                  <div className="bg-light p-2 rounded text-primary">
                    <Mail size={20} />
                  </div>
                  <div>
                    <h5 className="fw-bold mb-2">Email</h5>
                    <p className="mb-0 text-muted">datteo192004@gmail.com</p>
                  </div>
                </div>

                <div className="d-flex align-items-start gap-3">
                  <div className="bg-light p-2 rounded text-primary">
                    <Clock size={20} />
                  </div>
                  <div>
                    <h5 className="fw-bold mb-2">Giờ Làm Việc</h5>
                    <p className="mb-1 text-muted">Thứ 2 - Thứ 6: 08:00 - 17:00</p>
                    <p className="mb-1 text-muted">Thứ 7: 08:00 - 12:00</p>
                    <p className="mb-0 text-muted">Chủ nhật: Nghỉ</p>
                  </div>
                </div>
              </Card.Body>
            </Card>
          </Col>
        </Row>

        <Row className="mb-4">
          <Col lg={12}>
            <Card className="border-0 shadow-sm">
              <Card.Header className="bg-white p-4 border-bottom">
                <h3 className="fw-bold text-primary mb-0">Lịch Sử Phát Triển</h3>
              </Card.Header>
              <Card.Body className="p-4">
                <div className="position-relative">
                  <div
                    className="position-absolute h-100"
                    style={{ width: "2px", backgroundColor: "#e9ecef", left: "20px", top: 0 }}
                  ></div>

                  <Row>
                    {timeline.map((item, index) => (
                      <Col md={6} key={index} className="mb-4">
                        <div className="d-flex">
                          <div
                            className="bg-primary rounded-circle d-flex align-items-center justify-content-center text-white"
                            style={{ width: "40px", height: "40px", zIndex: 1 }}
                          >
                            <Calendar size={20} />
                          </div>
                          <div className="ms-4">
                            <Badge bg="primary" className="mb-2 px-3 py-2">
                              {item.year}
                            </Badge>
                            <h5 className="fw-bold mb-2">{item.title}</h5>
                            <p className="mb-0 text-muted">{item.description}</p>
                          </div>
                        </div>
                      </Col>
                    ))}
                  </Row>
                </div>
              </Card.Body>
            </Card>
          </Col>
        </Row>

        <Row>
          <Col lg={12}>
            <Card className="border-0 shadow-sm bg-primary text-white">
              <Card.Body className="p-5 text-center">
                <h3 className="fw-bold mb-3">Sẵn sàng trải nghiệm dịch vụ của chúng tôi?</h3>
                <p className="mb-4">
                  Đặt lịch hẹn ngay hôm nay để được tư vấn và chăm sóc sức khỏe bởi đội ngũ y bác sĩ chuyên nghiệp
                </p>
                <Button
                  variant="light"
                  size="lg"
                  className="text-primary fw-bold px-4 d-inline-flex align-items-center gap-2"
                  onClick={handleAppointment}
                >
                  Đặt lịch khám ngay
                  <ChevronRight size={18} />
                </Button>
              </Card.Body>
            </Card>
          </Col>
        </Row>
      </Container>
    </Container>
  )
}

export default About