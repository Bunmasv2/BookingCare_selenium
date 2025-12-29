import React, { useContext } from "react"
import { AuthContext } from "../../Context/AuthContext"
import { Container, Row, Col } from "react-bootstrap"
import AdminDashboard from "./Admin/Index"
import DoctorDashboard from "./Doctor/Index"
import PatientDashboard from "./Patient/Index"

const PatientProfile = () => {
  const { role } = useContext(AuthContext)

  const dashboards = {
    "admin": <AdminDashboard />,
    "doctor": <DoctorDashboard />,
    "patient": <PatientDashboard />
  }

  return (
    <Container fluid className="">
      <Row>
        <Col md={10} className="p-0 mx-auto">
          {dashboards[role]}
        </Col>
      </Row>
    </Container>
  )
}

export default PatientProfile