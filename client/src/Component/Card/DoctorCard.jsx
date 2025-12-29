import React from "react";
import { Card, Button } from "react-bootstrap"
import { useNavigate } from 'react-router-dom'
import "../../Style/DoctorCard.css"

const DoctorCard = ({ doctor }) => {
  const navigate = useNavigate()

  return (
    <Card className="doctor-card text-center shadow-sm" style={{ width: "210px", height: "400px" }} >
      <Card.Img variant="top" src={doctor.doctorImage} alt={doctor.userName} className="mx-auto mt-2 card-img"/>
        <Card.Body className="d-flex flex-column justify-content-between pb-3 px-1">
          <div>
            <Card.Title className="fw-bold text-primary mb-3 fs-6 pt-1">
              {doctor.degree} {doctor.userName}
            </Card.Title>
            <Card.Subtitle className="text-muted fst-italic mb-3" style={{ fontSize: "14px" }}>
              {doctor.position}
            </Card.Subtitle>
            <Card.Text className="small text-dark mb-3">
              {doctor.experienceYears} năm kinh nghiệm
            </Card.Text>
          </div>

          <Button
            variant="primary"
            size="sm"
            onClick={() => navigate(`/bac-si/${doctor.userName}`)}
            className="mt-auto"
          >
            Xem chi tiết
          </Button>
        </Card.Body>
    </Card>
  )
}

export default DoctorCard