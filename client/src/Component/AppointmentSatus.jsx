import { Container, Row, Col, Card, Badge } from "react-bootstrap"
import { Clock, CheckCircle, Stethoscope, Star, Calendar, UserCheck } from "lucide-react"
import '../Style/AppointmentStatus.css'
import { formatDateToLocale } from '../Util/DateUtils'

const steps = [
  {
    title: "Chờ xác nhận",
    description: "Cuộc hẹn đang chờ xác nhận từ phòng khám",
    icon: Clock
  },
  {
    title: "Đã xác nhận",
    description: "Lịch hẹn đã được xác nhận",
    icon: CheckCircle
  },
  {
    title: "Đã khám",
    description: "Bác sĩ đã thực hiện khám và kê đơn thuốc",
    icon: Stethoscope
  },
  {
    title: "Đã hoàn thành",
    description: "Cuộc hẹn đã được hoàn tất",
    icon: Star
  },
]

const getStepStyle = (status) => {
  switch (status) {
    case "done":
      return {
        backgroundColor: "#d4edda",
        borderColor: "#c3e6cb",
        color: "#155724"
      }
    case "current":
      return {
        backgroundColor: "#cce7ff",
        borderColor: "#80bdff",
        color: "#0056b3",
        boxShadow: "0 0 0 0.2rem rgba(0, 123, 255, 0.25)"
      }
    default:
      return {
        backgroundColor: "#f8f9fa",
        borderColor: "#e9ecef",
        color: "#6c757d"
      }
  }
}

const getIconStyle = (status) => {
  switch (status) {
    case "done":
      return {
        backgroundColor: "#28a745",
        color: "white"
      }
    case "current":
      return {
        backgroundColor: "#007bff",
        color: "white",
        animation: "pulse 2s infinite"
      }
    default:
      return {
        backgroundColor: "#6c757d",
        color: "white"
      }
  }
}

const getStepStatus = (stepTitle, currentTitle) => {
  const statusOrder = steps.map(s => s.title)
  const currentIndex = statusOrder.indexOf(currentTitle)
  const stepIndex = statusOrder.indexOf(stepTitle)

  if (stepIndex < currentIndex) return "done"
  if (stepIndex === currentIndex) {
    return stepTitle === "Đã hoàn thành" ? "done" : "current"
  }
  return "upcoming"
}

const AppointmentStatus = ({ appointment }) => {
  if (!appointment || !appointment.appointmentDate) {
    return (
      <Container className="my-4 mt-0">
        <Card className="shadow border-0 text-center py-5 d-flex flex-column align-items-center" style={{ borderRadius: "15px" }}>
          <Calendar size={48} color="#adb5bd" className="mb-3" />
          <h5 className="text-muted">Không có lịch hẹn sắp tới</h5>
        </Card>
      </Container>
    );
  }

  return (
    <Container className="my-4 mt-0">
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
                  <CheckCircle size={24} />
                </div>
                <div>
                  <h4 className="mb-1 fw-bold">
                    {formatDateToLocale(appointment?.appointmentTime) === formatDateToLocale(new Date())
                      ? 'Cuộc hẹn hiện tại'
                      : 'Cuộc hẹn sắp tới'}
                  </h4>
                  <p className="mb-0">
                    Mã lịch khám: <span className="fw-semibold">#{appointment?.appointmentId || 'N/A'}</span>
                  </p>
                </div>
              </div>
            </Col>
          </Row>
        </Card.Header>

        <div className="progress-container">
          <Row>
            <Col md={6}>
              <div className="info-item">
                <UserCheck className="info-icon" size={16} />
                <span><strong>Bác sĩ:</strong> {appointment?.doctorName || 'Không có thông tin'}</span>
              </div>
              {appointment?.serviceName && (
                <div className="info-item">
                  <Stethoscope className="info-icon" size={16} />
                  <span><strong>Dịch vụ:</strong> {appointment.serviceName}</span>
                </div>
              )}
            </Col>
            <Col md={6}>
              <div className="info-item">
                <Calendar className="info-icon" size={16} />
                <span><strong>Ngày khám:</strong> {formatDateToLocale(appointment?.appointmentDate)}</span>
              </div>
              <div className="info-item">
                <Clock className="info-icon" size={16} />
                <span><strong>Buổi:</strong> {appointment?.appointmentTime || 'Không xác định'}</span>
              </div>
            </Col>
          </Row>
        </div>

        <Card.Body className="p-4 pb-2">
          <Row>
            {steps.map((step, index) => {
              const IconComponent = step.icon;
              const status = getStepStatus(step.title, appointment?.status);
              const isCompleted = status === "done";
              const isCurrent = status === "current";
              const stepStyle = getStepStyle(status);
              const iconStyle = getIconStyle(status);

              return (
                <Col lg={3} md={6} key={index} className="mb-4">
                  <Card 
                    className="step-card border-2 h-100"
                    style={stepStyle}
                  >
                    <div className="step-number" style={{
                      backgroundColor: isCompleted ? '#28a745' : 'white',
                      color: isCompleted ? 'white' : '#6c757d',
                      borderColor: isCompleted ? '#28a745' : '#dee2e6'
                    }}>
                      {index + 1}
                    </div>

                    {isCurrent && step.title !== "Đã hoàn thành" && (
                      <div className="status-indicator current-indicator"></div>
                    )}
                    {isCompleted && (
                      <div className="status-indicator completed-indicator">
                        <CheckCircle size={10} color="white" />
                      </div>
                    )}

                    <Card.Body className="text-center p-4">
                      <div className="icon-container" style={iconStyle}>
                        <IconComponent size={28} />
                      </div>

                      <h6 className="fw-bold mb-2" style={{ color: stepStyle.color }}>
                        {step.title}
                      </h6>

                      <p className="small mb-0 lh-base" style={{ 
                        color: stepStyle.color,
                        opacity: 0.8 
                      }}>
                        {step.description}
                      </p>

                      {isCurrent && step.title !== "Đã hoàn thành" && (
                        <Badge bg="primary" className="mt-2">
                          Đang thực hiện
                        </Badge>
                      )}

                      {(isCompleted || (isCurrent && step.title === "Đã hoàn thành")) && (
                        <Badge bg="success" className="mt-2">
                          Hoàn thành
                        </Badge>
                      )}
                    </Card.Body>
                  </Card>
                </Col>
              )
            })}
          </Row>
        </Card.Body>
      </Card>
    </Container>
  )
}

export default AppointmentStatus
