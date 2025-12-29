import { useEffect, useState } from "react"
import { Modal, Button, Form, Row, Col } from "react-bootstrap"
import { Star } from "lucide-react"
import axios from "../../Util/AxiosConfig"

function RatingModal({ show, onHide, recordId, isExist }) {
  const [ratingType, setRatingType] = useState("doctor")
  const [comment, setComment] = useState("")
  const [submitting, setSubmitting] = useState(false)
  const [overallRating, setOverallRating] = useState(0)
  const [hoverRating, setHoverRating] = useState(0)
  const [detailRatings, setDetailRatings] = useState({
    // Bác sĩ
    knowledge: 0,
    attitude: 0,
    dedication: 0,
    communicationSkill: 0,
    // Dịch vụ
    effectiveness: 0,
    price: 0,
    serviceSpeed: 0,
    convenience: 0,
  })

  useEffect(() => {
    if (typeof isExist === 'object') {
      setComment(isExist.comment)
      setOverallRating(isExist.overallRating)
      setDetailRatings({
        knowledge: isExist?.knowledge || 0,
        attitude: isExist?.attitude || 0,
        dedication: isExist?.dedication || 0,
        communicationSkill: isExist?.communicationSkill || 0,
        effectiveness: isExist?.effectiveness || 0,
        price: isExist?.price || 0,
        serviceSpeed: isExist?.serviceSpeed || 0,
        convenience: isExist?.convenience || 0,
      })
    }
  }, [isExist])
  
  const resetForm = () => {
    setComment("")
    setOverallRating(0)
    setHoverRating(0)
    setDetailRatings({
      knowledge: 0,
      attitude: 0,
      dedication: 0,
      communicationSkill: 0,
      effectiveness: 0,
      price: 0,
      serviceSpeed: 0,
      convenience: 0,
    })
  }
  
  const convertRatingForm = () => {
    const ratingData = {
      recordId,
      overallRating,
      comment,
      doctorRatings: {
        knowledge: detailRatings.knowledge,
        attitude: detailRatings.attitude,
        dedication: detailRatings.dedication,
        communicationSkill: detailRatings.communicationSkill,
      },
      serviceRatings: {
        effectiveness: detailRatings.effectiveness,
        price: detailRatings.price,
        serviceSpeed: detailRatings.serviceSpeed,
        convenience: detailRatings.convenience,
      }
    }
    return ratingData
  }  

  const handleSubmit = async (e) => {
    e.preventDefault()
    if (isExist !== false) return
    setSubmitting(true)

    try {
      const reviewForm = convertRatingForm()
      console.log("aaaaaaaaa", reviewForm)
      const response = await axios.post(`/reviews`, reviewForm)
      console.log(response.data)
      // resetForm()
    } catch (error) {
      console.error("Lỗi khi gửi đánh giá:", error)
    } finally {
      setSubmitting(false)
    }
  }

  const handleDetailRatingChange = (category, value) => {
    console.log(category, value)
    setDetailRatings((prev) => ({
      ...prev,
      [category]: value,
    }))
  }

  const StarRating = ({ value, onChange }) => {
    const [localHover, setLocalHover] = useState(0)
  
    return (
      <div className="d-flex">
        {[1, 2, 3, 4, 5].map((star) => (
          <Star
            key={star}
            size={24}
            fill={star <= (localHover || value) ? "#FFD700" : "none"}
            color={star <= (localHover || value) ? "#FFD700" : "#D3D3D3"}
            style={{ cursor: "pointer", marginRight: "4px" }}
            onMouseEnter={() => setLocalHover(star)}
            onMouseLeave={() => setLocalHover(0)}
            onClick={() => onChange(star)}
          />
        ))}
      </div>
    )
  }  

  const DetailRating = ({ category, label }) => {
    return (
      <div className="d-flex align-items-center mb-2">
        <Form.Label className="mb-0 me-3" style={{ minWidth: '200px' }}>{label}</Form.Label>
          <StarRating
            value={detailRatings[category]}
            onChange={(value) => handleDetailRatingChange(category, value)}
          />
      </div>
    )
  }

  return (
    <Modal show={show} onHide={onHide} centered>
      <Modal.Header closeButton>
        <Modal.Title>Đánh giá {ratingType === "doctor" ? "Bác sĩ" : "Dịch vụ"}</Modal.Title>
      </Modal.Header>

      <Modal.Body>
        <Form>
          <p className="text-muted mb-3 p-2 rounded"
              style={{
                backgroundColor: "#CFF4FC",
                color: "#055160",
                textAlign: "center",
                fontStyle: "italic",
                boxShadow: "0 2px 4px rgba(0,0,0,0.1)"
              }}>
            Câu chuyện của bạn có thể là ánh sáng dẫn đường, giúp người khác tìm được bác sĩ tận tâm hay dịch vụ y tế chất lượng nhất. Hãy chia sẻ trải nghiệm của bạn để lan tỏa điều tốt đẹp!
          </p>

          <div className="mb-3">
            <div className="d-flex">
              <Form.Check
                type="radio"
                id="doctor-rating"
                label="Đánh giá Bác sĩ"
                name="ratingType"
                checked={ratingType === "doctor"}
                onChange={() => setRatingType("doctor")}
                className="me-4"
              />
              <Form.Check
                type="radio"
                id="service-rating"
                label="Đánh giá Dịch vụ"
                name="ratingType"
                checked={ratingType === "service"}
                onChange={() => setRatingType("service")}
              />
            </div>
          </div>

          <div className="mb-4 d-flex align-items-center">
            <Form.Label className="mb-0 me-3" style={{ minWidth: '200px' }}>
              <b>Đánh giá tổng thể </b><span className="text-danger">*</span>
            </Form.Label>
            <StarRating value={overallRating} onChange={setOverallRating} />
          </div>

          <div className="mb-4">
            <h6 className="mb-3"><b>Đánh giá chi tiết:</b></h6>

            {ratingType === "doctor" ? (
              <div className="mb-4 ">
                <div className="mb-3 ">
                  <DetailRating category="knowledge" label="- Kiến thức chuyên môn" />
                </div>
                <div className="mb-3">
                  <DetailRating category="attitude" label="- Thái độ phục vụ" />
                </div>
                <div className="mb-3">
                  <DetailRating category="dedication" label="- Sự tận tâm" />
                </div>
                <div className="mb-3">
                  <DetailRating category="communicationSkill" label="- Kỹ năng giao tiếp" />
                </div>
              </div>

            ) : (
              <div className="mb-4">
                <div className="mb-3">
                  <DetailRating category="effectiveness" label="Hiệu quả dịch vụ" />
                </div>
                <div className="mb-3">
                  <DetailRating category="price" label="Giá cả hợp lý" />
                </div>
                <div className="mb-3">
                  <DetailRating category="serviceSpeed" label="Tốc độ phục vụ" />
                </div>
                <div className="mb-3">
                  <DetailRating category="convenience" label="Sự thuận tiện" />
                </div>
              </div>

            )}
          </div>

          <div
            className="d-flex flex-wrap gap-2 p-3"
            style={{
              borderRadius: "12px",
              boxShadow: "0 2px 8px rgba(0, 0, 0, 0.05)",
            }}
          >
            {[
              "Bác sĩ nhiệt tình. Dịch vụ xuất sắc.",
              "Gói dịch vụ OK.",
              "Chức năng đặt online thuận tiện, nhanh chóng.",
              "Thời gian khám nhanh.",
              "Dễ dàng sử dụng.",
            ].map((text, index) => (
              <div
                key={index}
                className="px-3 py-2 rounded-pill border text-muted small"
                style={{
                  backgroundColor: "#fff",
                  borderColor: "#b6e0ea",
                  cursor: "pointer",
                  transition: "all 0.2s",
                }}
                onClick={() => setComment(text)}
                onMouseOver={(e) => {
                  e.currentTarget.style.backgroundColor = "#d1f3fa";
                  e.currentTarget.style.boxShadow = "0 2px 6px rgba(0,0,0,0.1)";
                }}
                onMouseOut={(e) => {
                  e.currentTarget.style.backgroundColor = "#fff";
                  e.currentTarget.style.boxShadow = "none";
                }}
              >
                {text}
              </div>
            ))}
          </div>

          <div className="mb-3 mt-3">
            <Form.Label>Nhận xét chi tiết</Form.Label>
            <Form.Control
              as="textarea"
              rows={3}
              placeholder="Chia sẻ trải nghiệm của bạn..."
              value={comment}
              onChange={(e) => setComment(e.target.value)}
              spellCheck={false}
            />
          </div>

        </Form>
      </Modal.Body>

      <Modal.Footer>
        <Button variant="light" onClick={onHide} disabled={submitting}>
          {!isExist ? "Hủy" : "Ẩn"}
        </Button>
        {!isExist && 
          <Button variant="primary" onClick={handleSubmit} disabled={submitting}>
            {submitting ? "Đang gửi..." : "Gửi đánh giá"}
          </Button>
        }
      </Modal.Footer>
    </Modal>
  )
}

export default RatingModal