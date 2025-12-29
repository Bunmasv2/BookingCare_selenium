import { Container, Row, Col } from "react-bootstrap"
import { Link } from "react-router-dom"
import { FaPhoneAlt, FaEnvelope, FaMapMarkerAlt } from "react-icons/fa"

const Footer = () => {
  return (
    <footer className="bg-light text-dark py-4 text-start mx-3">
      <Container className="w-75">
        <Row>
          <Col md={4} className="mb-3">
            <h5 className="text-primary fw-bold">Phòng Khám Đa Khoa XYZ</h5>
            <p className="small w-75">
              Chúng tôi cam kết cung cấp dịch vụ y tế chất lượng cao với đội ngũ bác sĩ tận tâm và giàu kinh nghiệm.
            </p>
          </Col>

          <Col md={4} className="mb-3">
            <h5 className="text-primary fw-bold">Liên kết nhanh</h5>
            <ul className="list-unstyled">
              <li className="my-2"><Link to="/" className="text-dark text-decoration-none hover:text-primary">Trang chủ</Link></li>
              <li className="my-2"><Link to="/về chúng tôi" className="text-dark text-decoration-none hover:text-primary">Giới thiệu</Link></li>
              <li className="my-2"><Link to="/bác sĩ" className="text-dark text-decoration-none hover:text-primary">Đội ngũ bác sĩ</Link></li>
              <li className="my-2"><Link to="/chuyên khoa/Khoa Nội tổng quát" className="text-dark text-decoration-none hover:text-primary">Chuyên khoa</Link></li>
              <li className="my-2"><Link to="/dịch vụ/Khám tổng quát" className="text-dark text-decoration-none hover:text-primary">Dịch vụ</Link></li>
              {/* <li className="my-2"><Link to="/tin tức" className="text-dark text-decoration-none hover:text-primary">Tin tức</Link></li> */}
              <li className="my-2"><Link to="/đặt lịch khám" className="text-dark text-decoration-none hover:text-primary">Đặt lịch khám</Link></li>
              <li className="my-2"><Link to="/liên hệ" className="text-dark text-decoration-none hover:text-primary">Liên hệ</Link></li>
            </ul>
          </Col>

          <Col md={4} className="mb-3">
            <h5 className="text-primary fw-bold">Liên hệ</h5>
            <p><FaMapMarkerAlt className="text-primary me-2" /> 475A Đ. Điện Biên Phủ, Phường 25, Bình Thạnh, Hồ Chí Minh</p>
            <p><FaPhoneAlt className="text-primary me-2" /> 0123 456 789</p>
            <p><FaEnvelope className="text-primary me-2" /> datteo192004@gmail.com</p>
          </Col>
        </Row>

        <hr />
        <div className="small">
          &copy; 2025 Phòng Khám Đa Khoa XYZ. All rights reserved.
        </div>
      </Container>
    </footer>
  )
}

export default Footer