import { useContext, useState } from "react"
import { Container, Row, Col, Card, Form, Button, Alert } from "react-bootstrap"
import { MapPin, Phone, Mail, Clock, Send, MessageCircle, ChevronDown, ChevronUp } from "lucide-react"
import { ValideFormContext } from "../Context/ValideFormContext"
import axios from "../Util/AxiosConfig"

const Contact = () => {
  const [formData, setFormData] = useState({
    message: "",
  })
  const { validateForm, formErrors } = useContext(ValideFormContext)
  const [expandedFaq, setExpandedFaq] = useState(null)

  const handleChange = (e) => {
    const { name, value } = e.target
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }))
  }

  const toggleFaq = (index) => {
    setExpandedFaq(expandedFaq === index ? null : index)
  }

  const faqs = [
    {
      question: "Làm thế nào để đặt lịch khám?",
      answer:
        "Bạn có thể đặt lịch khám qua website, gọi điện thoại đến số hotline 1900 1234, hoặc trực tiếp đến phòng khám để đăng ký.",
    },
    {
      question: "Thời gian nhận kết quả xét nghiệm là bao lâu?",
      answer:
        "Thời gian nhận kết quả xét nghiệm thông thường từ 1-3 ngày làm việc tùy vào loại xét nghiệm. Đối với các xét nghiệm cơ bản, kết quả có thể có trong ngày.",
    },
    {
      question: "Phòng khám có làm việc vào cuối tuần không?",
      answer:
        "Phòng khám làm việc từ thứ 2 đến thứ 7. Chúng tôi làm việc từ 8:00 - 17:00 các ngày trong tuần và từ 8:00 - 12:00 vào thứ 7. Chúng tôi nghỉ vào Chủ nhật và các ngày lễ.",
    },
    {
      question: "Có cần đặt lịch trước khi đến khám không?",
      answer:
        "Chúng tôi khuyến khích bệnh nhân đặt lịch trước để giảm thời gian chờ đợi và được phục vụ tốt nhất. Tuy nhiên, phòng khám vẫn tiếp nhận bệnh nhân đến trực tiếp mà không cần đặt lịch.",
    },
    {
      question: "Phòng khám có chấp nhận thanh toán bằng thẻ không?",
      answer:
        "Có, phòng khám chấp nhận nhiều hình thức thanh toán bao gồm tiền mặt, thẻ ATM, thẻ tín dụng, và các ví điện tử như MoMo, ZaloPay, VNPay.",
    },
  ]

  const handleSubmit = async (e) => {
    e.preventDefault()
    // const errors = validateForm(formData)   
    // if (errors > 0) return

    try {
      const response = await axios.post(`/contactmessages/${formData.message}`)
      console.log(response.data)
    } catch (error) {
      console.log(error)
    }
  }

  return (
    <Container fluid className="py-5 bg-light">
      <Container className="w-75">
        <Row className="g-4">
          <Col lg={4}>
            <Card className="border-0 shadow-sm h-100">
              <Card.Header className="bg-primary text-white p-4">
                <h4 className="mb-0 fw-bold">Thông Tin Liên Hệ</h4>
              </Card.Header>
              <Card.Body className="p-4">
                <div className="d-flex align-items-start gap-3 mb-4">
                  <div className="bg-light p-3 rounded border border-primary">
                    <MapPin className="text-primary" size={24} />
                  </div>
                  <div>
                    <h5 className="fw-bold mb-2">Địa Chỉ</h5>
                    <p className="text-muted mb-0">475A Đ. Điện Biên Phủ, Phường 25, Bình Thạnh, Hồ Chí Minh</p>
                  </div>
                </div>

                <div className="d-flex align-items-start gap-3 mb-4">
                  <div className="bg-light p-3 rounded border border-primary">
                    <Phone className="text-primary" size={24} />
                  </div>
                  <div>
                    <h5 className="fw-bold mb-2">Điện Thoại</h5>
                    <p className="text-muted mb-1">Hotline: 1900 1234</p>
                    <p className="text-muted mb-0">Hỗ trợ: 028 1234 5678</p>
                  </div>
                </div>

                <div className="d-flex align-items-start gap-3 mb-4">
                  <div className="bg-light p-3 rounded border border-primary">
                    <Mail className="text-primary" size={24} />
                  </div>
                  <div>
                    <h5 className="fw-bold mb-2">Email</h5>
                    <p className="text-muted mb-1">info@phongkham.com</p>
                    <p className="text-muted mb-0">support@phongkham.com</p>
                  </div>
                </div>

                <div className="d-flex align-items-start gap-3">
                  <div className="bg-light p-3 rounded border border-primary">
                    <Clock className="text-primary" size={24} />
                  </div>
                  <div>
                    <h5 className="fw-bold mb-2">Giờ Làm Việc</h5>
                    <p className="text-muted mb-1">Thứ 2 - Thứ 6: 08:00 - 17:00</p>
                    <p className="text-muted mb-1">Thứ 7: 08:00 - 12:00</p>
                    <p className="text-muted mb-0">Chủ nhật: Nghỉ</p>
                  </div>
                </div>
              </Card.Body>
            </Card>
          </Col>

          <Col lg={8}>
            <Card className="border-0 shadow-sm mb-4">
              <Card.Header className="bg-white p-4 border-bottom">
                <h4 className="mb-0 fw-bold text-primary">Gửi Tin Nhắn Cho Chúng Tôi</h4>
              </Card.Header>
              <Card.Body className="p-4">
                <Form onSubmit={handleSubmit}>
                  <Form.Group controlId="message" className="mb-4">
                    <Form.Label className="fw-medium">
                      Nội dung <span className="text-danger">*</span>
                    </Form.Label>
                    <Form.Control
                      as="textarea"
                      name="message"
                      value={formData.message}
                      onChange={handleChange}
                      placeholder="Nhập nội dung tin nhắn"
                      rows={5}
                      isInvalid={!!formErrors.message}
                      spellCheck={false}
                    />
                    <Form.Control.Feedback type="invalid">{formErrors.message}</Form.Control.Feedback>
                  </Form.Group>

                  <Button
                    variant="primary"
                    className="py-2 px-4 d-flex align-items-center gap-2"
                    onClick={(e) => handleSubmit(e)}
                  >
                    <Send size={18} />
                    <span>Gửi tin nhắn</span>
                  </Button>
                </Form>
              </Card.Body>
            </Card>

            <Card className="border-0 shadow-sm">
              <Card.Header className="bg-white p-4 border-bottom">
                <div className="d-flex align-items-center gap-2">
                  <MessageCircle className="text-primary" size={24} />
                  <h4 className="mb-0 fw-bold text-primary">Câu Hỏi Thường Gặp</h4>
                </div>
              </Card.Header>
              <Card.Body className="p-4">
                <div className="accordion">
                  {faqs.map((faq, index) => (
                    <div key={index} className="mb-3 border rounded overflow-hidden">
                      <div
                        className={`p-3 d-flex justify-content-between align-items-center ${ expandedFaq === index ? "bg-light" : "bg-white" }`}
                        onClick={() => toggleFaq(index)} style={{ cursor: "pointer" }}
                      >
                        <h6 className="mb-0 fw-bold">{faq.question}</h6>
                        {expandedFaq === index ? (
                          <ChevronUp size={20} className="text-primary" />
                        ) : (
                          <ChevronDown size={20} className="text-primary" />
                        )}
                      </div>
                      {expandedFaq === index && (
                        <div className="p-3 border-top bg-white">
                          <p className="mb-0">{faq.answer}</p>
                        </div>
                      )}
                    </div>
                  ))}
                </div>
              </Card.Body>
            </Card>
          </Col>
        </Row>

        <Row className="mt-5">
          <Col>
            <Card className="border-0 shadow-sm overflow-hidden">
              <Card.Header className="bg-white p-4 border-bottom">
                <h4 className="mb-0 fw-bold text-primary">Địa chỉ phòng khám</h4>
              </Card.Header>
              <Card.Body className="p-0">
                <div className="ratio ratio-21x9">
                  <iframe
                    src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3919.1258198718204!2d106.71306867486087!3d10.801843089347313!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x31752953ade9f9c9%3A0x6ad5d15cd48a4f4e!2sHUTECH!5e0!3m2!1svi!2s!4v1713275091291!5m2!1svi!2s"
                    width="600"
                    height="450"
                    style={{ border: 0 }}
                    allowFullScreen=""
                    loading="lazy"
                    referrerPolicy="no-referrer-when-downgrade"
                    title="location"
                  ></iframe>
                </div>
              </Card.Body>
            </Card>
          </Col>
        </Row>
      </Container>
    </Container>
  )
}

export default Contact