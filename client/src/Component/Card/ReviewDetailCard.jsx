import { Star } from "lucide-react"
import { Card, Row, Col, Badge } from "react-bootstrap"
import { formatDateToLocale, formatTimeToLocale } from "../../Util/DateUtils"
import { useEffect, useState } from "react"
import { useNavigate } from "react-router-dom"

const renderStars = (score) => {
  return (
    <div className="d-flex justify-content-start gap-1">
      {Array.from({ length: 5 }, (_, i) => (
        <Star
          key={i}
          size={16}
          className={i < score ? "text-warning" : "text-secondary"}
          fill={i < score ? "#ffc107" : "none"}
        />
      ))}
    </div>
  )
}

const ReviewDetailCard = ({ type, review }) => {
  const [labels, setLabels] = useState()
  const naviagte = useNavigate()
console.log(review)
  const doctorLabels = [
    { label: "Thái độ", score: review?.attitude },
    { label: "Chuyên môn", score: review?.knowledge },
    { label: "Tận tâm", score: review?.dedication },
    { label: "Giao tiếp", score: review?.communicationSkill },
  ]

  const serviceLabels = [
    { label: "Độ hiệu quả", score: review?.effectiveness },
    { label: "Giá", score: review?.price },
    { label: "Tốc độ", score: review?.serviceSpeed },
    { label: "Sự thuận tiện", score: review?.convenience },
  ]

  useEffect(() => {
    if (type === "doctor") setLabels(doctorLabels)
    else setLabels(serviceLabels)
  }, [type])

  return (
    <Card className="border-1 mb-4 p-4 rounded-4">
      <Row className="align-items-center mb-2">
        <Col>
          <div className="d-flex align-items-center">
            <h5 className="mb-0 fw-semibold">{review?.patientName}</h5>
            <small className="text-muted ms-3">
              {formatDateToLocale(review?.createdAt)} {formatTimeToLocale(review?.createdAt)}
            </small>
          </div>
        </Col>
      </Row>

      {(type === "service" && review?.doctorName) && (
        <div className="mb-2" style={{ cursor: "pointer" }} onClick={() => naviagte(`/bac-si/${review?.doctorName}`)} >
          <strong>Bác sĩ:</strong> {review.doctorName}
        </div>
      )}
      {(type === "doctor" && review?.serviceName) && (
        <div className="mb-2" style={{ cursor: "pointer" }} onClick={() => naviagte(`/dịch vụ/${review?.serviceName}`)} >
          <strong>Dịch vụ:</strong> {review.serviceName}
        </div>
      )}

      <div className="mb-3">{renderStars(review?.overallRating)}</div>

      <p className="text-secondary fst-italic mb-4">{review?.comment}</p>

      <Row className="gy-3">
        {labels?.map((item, index) => (
          <Col key={index} lg={3} md={6} xs={12}>
            <Badge
              bg="light"
              className="w-100 py-3 px-3 border shadow-sm rounded-3 d-flex flex-column align-items-center"
            >
              <span className="text-dark fw-medium">{item.label}</span>
              {renderStars(item.score)}
            </Badge>
          </Col>
        ))}
      </Row>
    </Card>
  )
}

export default ReviewDetailCard
