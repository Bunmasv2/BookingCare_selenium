import { useEffect, useState } from 'react'
import { Button, Col, Container, Form, Row } from 'react-bootstrap'
import axios from '../../../../Util/AxiosConfig'
import ReviewDetail from './ReviewDetail'

function Review({ tabActive }) {
  const [specialties, setSpecialties] = useState([])
  const [specialty, setSpecialty] = useState(null)
  const [doctors, setDoctors] = useState([])
  const [reviews, setReviews] = useState([])
  const [doctor, setDoctor] = useState(null)
  const [review, setReview] = useState(null)
  const [showReviewDetail, setShowReviewDetail] = useState(false)

  useEffect(() => {
    if (tabActive !== "reviewDoctors") return

    const fetchSpecialty = async () => {
      try {
        const response = await axios.get('/specialties')
        setSpecialties(response.data)
        setSpecialty(response.data[0])
      } catch (error) {
        console.error('Error fetching specialties:', error)
      }
    }

    fetchSpecialty()
  }, [tabActive])

  useEffect(() => {
    if (!specialty?.name) return

    const fetchDoctors = async () => {
      try {
        const response = await axios.get(`/doctors/${specialty.name}`)
        setDoctors(response.data)
      } catch (error) {
        console.error('Error fetching doctors:', error)
      }
    }

    fetchDoctors()
  }, [specialty])

  useEffect(() => {
    if (!specialty?.name) return

    const fetchReviewDoctor = async () => {
      try {
        const response = await axios.get(`/reviews/doctors/${specialty.name}`)
        setReviews(response.data)
      } catch (error) {
        console.error('Error fetching reviews:', error)
      }
    }

    fetchReviewDoctor()
  }, [doctors, specialty])

  const handleSpecialty = (e) => {
    const selectedId = parseInt(e.target.value)
    const selectedSpecialty = specialties.find((s) => s.specialtyId === selectedId)
    setSpecialty(selectedSpecialty)
  }

  if (showReviewDetail) {
    return <ReviewDetail specialty={specialty?.name} doctor={doctor} review={review} />
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
        <h5 className="mb-4 fw-bold text-primary">Danh sách Bác sĩ</h5>
        {doctors.length === 0 ? (
          <p className="text-muted">Không có bác sĩ nào trong chuyên khoa này.</p>
        ) : (
          <Row className="g-3">
            {doctors.map((doctor, index) => {
              const r = reviews.find((rev) => rev.doctorId === doctor.doctorId)

              return (
                <Col md={6} key={index}>
                  <div className="border p-3 rounded shadow-sm d-flex align-items-center">
                    <Col xs={9} className="d-flex">
                      <img
                        src={doctor.doctorImage || 'https://via.placeholder.com/80'}
                        alt={doctor.userName}
                        className="doctor-image rounded-circle me-3"
                        style={{ width: '80px', height: '80px', objectFit: 'cover' }}
                      />
                      <div className="flex-grow-1 mt-auto">
                        <h6 className="fw-bold mb-1">{doctor.userName}</h6>
                        <p className="text-muted mb-1">Chuyên khoa: {specialty?.name}</p>
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
                            setDoctor(doctor)
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
