import React, { useEffect, useState } from 'react'
import { Container, Row, Col, Card, Nav, Tab } from 'react-bootstrap'
import Overview from './Overview'
import Appointments from './Appointments'
import Prescriptions from './Prescriptions'
import axios from '../../../Util/AxiosConfig'
import UserProfileCard from '../../../Component/UserProfileCard'

function Index() {
    const [tabActive, setTabActive] = useState("overview")
    const [patient, setPatient] = useState()
    const [recordIsChoose, setRecordIschoose] = useState()

    useEffect(() => {
        const fetchPatient = async () => {
            try {
                const response = await axios.get("/users/profile")
                setPatient(response.data)
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
                    <UserProfileCard user={patient} setUser={setPatient} userType="patient" />
                </Col>
                
                <Col md={9}>
                    <Tab.Container id="dashboard-tabs" activeKey={tabActive} onSelect={(k) => setTabActive(k)}>
                        <Nav variant="tabs" className="pt-0">
                            <Nav.Item>
                                <Nav.Link eventKey="overview" className="d-flex align-items-center">
                                    <span className="me-2">
                                    </span>
                                    Tổng Quan
                                </Nav.Link>
                            </Nav.Item>
                            <Nav.Item>
                                <Nav.Link eventKey="appointments" className="d-flex align-items-center">
                                    <span className="me-2">
                                    </span>
                                    Lịch Hẹn
                                </Nav.Link>
                            </Nav.Item>
                            <Nav.Item>
                                <Nav.Link eventKey="prescriptions" className="d-flex align-items-center">
                                    <span className="me-2">
                                    </span>
                                    Đơn Thuốc
                                </Nav.Link>
                            </Nav.Item>
                        </Nav>
                        
                        <Tab.Content>
                            <Tab.Pane eventKey="overview">
                                <Overview tabActive={tabActive} setTabActive={setTabActive} recordIsChoose={recordIsChoose} setRecordIschoose={setRecordIschoose} />
                            </Tab.Pane>
                            
                            <Tab.Pane eventKey="appointments">
                                <Appointments tabActive={tabActive} />
                            </Tab.Pane>
                            
                            <Tab.Pane eventKey="prescriptions"> 
                                <Prescriptions tabActive={tabActive} setTabActive={setTabActive} recordIsChoose={recordIsChoose} setRecordIschoose={setRecordIschoose} />
                            </Tab.Pane>
                        </Tab.Content>
                    </Tab.Container>
                </Col>
            </Row>
        </Container>
    )
}

export default Index