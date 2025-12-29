import { Card, Button } from 'react-bootstrap'
import { FaStar, FaStarHalfAlt, FaRegStar } from 'react-icons/fa'
import { BsCheckCircle } from 'react-icons/bs'
import { useState } from 'react'

export default function ReviewCard({ review }) {
  const [expanded, setExpanded] = useState(false)

  const shouldTruncate = review?.comment?.length > 100
  const truncatedText = expanded
    ? review?.comment
    : review?.comment?.slice(0, 100) + (shouldTruncate ? '...' : '')

  const renderStars = (rating) => {
    const fullStars = Math.floor(rating)
    const halfStar = rating % 1 >= 0.5
    const emptyStars = 5 - fullStars - (halfStar ? 1 : 0)

    const stars = []
    for (let i = 0; i < fullStars; i++) {
      stars.push(<FaStar key={`full-${i}`} className="text-warning me-1" />);
    }
    if (halfStar) {
      stars.push(<FaStarHalfAlt key="half" className="text-warning me-1" />);
    }
    for (let i = 0; i < emptyStars; i++) {
      stars.push(<FaRegStar key={`empty-${i}`} className="text-warning me-1" />);
    }

    return stars
  }

  const formattedDate = review?.createdAt
    ? new Date(review?.createdAt).toLocaleDateString()
    : 'N/A'

  return (
    <Card className="border-0" style={{ maxWidth: '450px' }}>
      <Card.Body className='p-0'>
        <div className="d-flex justify-content-between align-items-start mb-2">
          <div>
            <Card.Title as="h5" className="mb-1 p-0">
              {review?.patientName || 'Anonymous'}
            </Card.Title>
            <Card.Subtitle className="text-muted small">{formattedDate}</Card.Subtitle>
          </div>
          <div className="text-success d-flex align-items-center small fw-semibold">
            <BsCheckCircle className="me-1" />
            Verified
          </div>
        </div>

        <div className="d-flex align-items-center mb-3">
          <div className="d-flex">{renderStars(review?.overallRating)}</div>
          <div className="ms-2 fw-semibold">{review?.overallRating.toFixed(1)}</div>
        </div>

        <Card.Text className="text-muted small mb-4">
          <div className='d-flex justify-content-between'>
            {truncatedText || 'No comment.'}
            {shouldTruncate && (
              <Button
                variant="link"
                size="sm"
                className="p-0 ms-1 fw-semibold text-decoration-none"
                onClick={() => setExpanded(!expanded)}
              >
                {expanded ? 'Read less' : 'Read more'}
              </Button>
            )}
            {/* <Button variant="outline-primary" size="sm">
              Details
            </Button> */}
          </div>
        </Card.Text>
        {/* <div className="text-end mt-3">
          {review.serviceName && (
            <Badge bg="light" text="dark" className="fw-semibold px-3 py-2 rounded-pill">
              {review.serviceName}
            </Badge>
          )}

        </div> */}
      </Card.Body>
    </Card>
  )
}