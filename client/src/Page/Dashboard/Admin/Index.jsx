import { useEffect, useState, useContext} from 'react'
import { Container, Row, Col, Card, Nav, Tab,Button } from 'react-bootstrap'
import AppointmentStatistics from './Appointment/AppointmentStatistics'
import PrescriptionOverView from '../Admin/Management/Patient/PrescriptionOverView'
import DoctorReviews from './Doctor/Reviews'
import DoctorSalary from './Salary/DoctorSalary'
import UserAdmin from './UserAdmin'
import Review from './Service/Review'
import SpecialtyAdmin from './Management/SpecialtyAdmin'
import ServiceAdmin from './Management/ServiceAdmin'
import ReviewManagement from "./Review/Index"
import UserManagement from "./Management/Index"
import AdminList from './Management/Admin/AdminList'
import "../../../Style/Admin.css"
import DoctorList from './Management/Doctor/DoctorList'
import { useNavigate } from "react-router-dom";
import { AuthContext } from "../../../Context/AuthContext";
import ContactMessagesAdmin from './Contact/ContactMessagesAdmin'

function Index() {
    const [tabActive, setTabActive] = useState(() => {
        const hash = window.location.hash.replace('#', '')
        window.location.hash = "appointments"
        return hash || 'appointments'
    })
    const [menuOpen, setMenuOpen] = useState(false)
    const [systemMenuOpen, setSystemMenuOpen] = useState(false)
    const navigate = useNavigate();
    const { logout } = useContext(AuthContext);

    useEffect(() => {
        const handleHashChange = () => {
            const hash = window.location.hash.replace('#', '')
            setTabActive(hash || 'appointments')
        }
        window.addEventListener('hashchange', handleHashChange)
        return () => window.removeEventListener('hashchange', handleHashChange)
    }, [])

    const handleLogout = () => {
        logout();
        navigate("/");
    };
    return (
        <Tab.Container 
            activeKey={tabActive} 
            onSelect={(k) => {
                setTabActive(k)
                window.location.hash = k
                
                if (k === "reviewservices" || k === "doctors") {
                    setMenuOpen(true)
                }
                if (k === "services" || k === "specialties") {
                    setSystemMenuOpen(true)
                }
            }
        }>
            <Container fluid className="p-4">
                <Row>
                    <Col md={3}>
                        <Card className="mb-4 sidebar">
                            <Card.Body>
                                <h5 className="text-primary mb-4 text-center">QUẢN TRỊ HỆ THỐNG</h5>
                                
                                <Nav className="flex-column">
                                    <Nav.Link 
                                        eventKey="appointments"
                                        className={`sidebar-link mb-2 ${tabActive === "appointments" ? "active" : ""}`}
                                    >
                                        Lịch hẹn
                                    </Nav.Link>
                                    
                                    <ReviewManagement tabActive={tabActive} menuOpen={menuOpen} setMenuOpen={setMenuOpen} />

                                    <UserManagement tabActive={tabActive} systemMenuOpen={systemMenuOpen} setSystemMenuOpen={setSystemMenuOpen} />

                                    <Nav.Link 
                                        eventKey="salary"
                                        className={`sidebar-link mb-2 ${tabActive === "salary" ? "active" : ""}`}
                                    >
                                        Quản lý doanh thu
                                    </Nav.Link>         

                                    <Nav.Link 
                                        eventKey="contactmessages"
                                        className={`sidebar-link mb-2 ${tabActive === "contactmessages" ? "active" : ""}`}
                                    >
                                        Phản Hồi Thắc Mắc
                                    </Nav.Link>

                                    {/* <Nav.Link 
                                        eventKey="users"
                                        className={`sidebar-link mb-2 ${tabActive === "users" ? "active" : ""}`}
                                    >
                                        Người Dùng
                                    </Nav.Link> */}
                                </Nav>

                                <div className="session-info mt-auto mx-3 mb-4">
                                    <Card className="border-0 bg-light">
                                        <Card.Body className="p-3">
                                            <div className="d-flex align-items-center mb-3">
                                                <div className="admin-avatar rounded-circle bg-primary text-white d-flex align-items-center justify-content-center me-2" style={{ width: "40px", height: "40px" }}>
                                                    <span className="fw-bold">A</span>
                                                </div>
                                                <div>
                                                    <h6 className="mb-0 fw-bold">Admin</h6>
                                                    <small className="text-muted">{new Date().toLocaleString()}</small>
                                                </div>
                                            </div>
                                            <Button 
                                                variant="outline-danger" 
                                                className="w-100 d-flex align-items-center justify-content-center" 
                                                onClick={handleLogout}
                                                size="sm"
                                            >
                                                <i className="bi bi-box-arrow-right me-1"></i>
                                                Đăng xuất
                                            </Button>
                                        </Card.Body>
                                    </Card>
                                </div>
                            </Card.Body>
                        </Card>
                    </Col>
                    
                    <Col md={9} style={{ fontSize: "14px" }} className='p-0'>
                        <Tab.Content>
                            <Tab.Pane eventKey="appointments">
                                <AppointmentStatistics tabActive={tabActive} />
                            </Tab.Pane>
                            
                            <Tab.Pane eventKey="contactmessages">
                                <ContactMessagesAdmin tabActive={tabActive} />
                            </Tab.Pane>

                            <Tab.Pane eventKey="reviewservices">
                                <Review tabActive={tabActive} />
                            </Tab.Pane>

                            <Tab.Pane eventKey="reviewDoctors">
                                <DoctorReviews tabActive={tabActive} />
                            </Tab.Pane>

                            <Tab.Pane eventKey="services">
                                <ServiceAdmin tabActive={tabActive} />
                            </Tab.Pane>

                            <Tab.Pane eventKey="specialties">
                                <SpecialtyAdmin tabActive={tabActive} />
                            </Tab.Pane>

                            <Tab.Pane eventKey="prescriptions">
                                <PrescriptionOverView tabActive={tabActive} />
                            </Tab.Pane>

                            <Tab.Pane eventKey="salary">
                                <DoctorSalary tabActive={tabActive} />
                            </Tab.Pane>

                            <Tab.Pane eventKey="users">
                                <UserAdmin tabActive={tabActive} />
                            </Tab.Pane>

                            <Tab.Pane eventKey="admins">
                                <AdminList tabActive={tabActive} />
                            </Tab.Pane>

                            <Tab.Pane eventKey="doctors">
                                <DoctorList tabActive={tabActive} />
                            </Tab.Pane>
                        </Tab.Content>
                    </Col>
                </Row>
            </Container>
        </Tab.Container>
    )
}

export default Index