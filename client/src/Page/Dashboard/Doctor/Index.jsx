import { Clock, FileText } from 'lucide-react'
import { useEffect, useState } from 'react'
import { Card, Col, Container, Nav, Row, Tab } from 'react-bootstrap'
import ReviewDoctor from './ReviewDoctor'
import PatientHistory from './PatientHistory'
import DoctorSchedule from './DoctorSchedule'
import axios from '../../../Util/AxiosConfig'
import UserProfileCard from '../../../Component/UserProfileCard'

function Index() {
    const [tabActive, setTabActive] = useState("doctorSchedule")
    const [doctor, setDoctor] = useState()

    useEffect(() => {
        const fetchPatient = async () => {
            try {
                const response = await axios.get("/users/profile")
                setDoctor(response.data)
            } catch (error) {
                console.log(error)
            }
        }

        fetchPatient()
    }, [])

    return (
        <Container fluid className="p-4">
            <Row>
                <Col md={3}>
                    <UserProfileCard user={doctor} setUser={setDoctor} userType="doctor" />
                </Col>
                
                <Col md={9}>
                    <Tab.Container id="dashboard-tabs" activeKey={tabActive} onSelect={(k) => setTabActive(k)}>
                        <Nav variant="tabs" className='pb-0'>
                            <Nav.Item>
                                <Nav.Link eventKey="doctorSchedule" className="d-flex align-items-center">
                                    Lịch làm việc
                                </Nav.Link>
                            </Nav.Item>

                            <Nav.Item>
                                <Nav.Link eventKey="patientHistory" className="d-flex align-items-center">
                                    Danh sách bệnh nhân
                                </Nav.Link>
                            </Nav.Item>
                        </Nav>
                        
                        <Tab.Content>
                            <Tab.Pane eventKey="doctorSchedule">
                                <Card>
                                    <Card.Body>
                                        <DoctorSchedule tabActive={tabActive} />
                                    </Card.Body>
                                </Card>
                            </Tab.Pane>

                            <Tab.Pane eventKey="patientHistory">
                                <Card>
                                    <Card.Body>
                                        <PatientHistory tabActive={tabActive} />
                                    </Card.Body>
                                </Card>
                            </Tab.Pane>
                            
                            {/* <Tab.Pane eventKey="reviews"> 
                                <Card>
                                    <Card.Body>
                                        <h4>Đánh giá</h4>
                                        <p>Đánh giá từ bệnh nhân</p>
                                        
                                        <ReviewDoctor tabActive={tabActive} />
                                    </Card.Body>
                                </Card>
                            </Tab.Pane> */}
                        </Tab.Content>
                    </Tab.Container>
                </Col>
            </Row>
        </Container>
    )
}

export default Index