import { useEffect, useState } from "react"
import { Link, useNavigate, useParams } from "react-router-dom"
import { Container, Row, Col, Card, Spinner, Button, Badge, ListGroup, Tab, Nav } from "react-bootstrap"
import { ChevronLeft, ExternalLink, Stethoscope, Activity, FileText } from "lucide-react"
import axios from "../Util/AxiosConfig"
// import images from "../Image/SpecialtyIntroduce/Index"
import DoctorCard from "../Component/Card/DoctorCard"
import ServiceCard from "../Component/Card/ServiceCard"
import Recomend from "../Component/Card/Recomend"

const DepartmentDetail = () => {
    const { specialty }  = useParams()
    const [loading, setLoading] = useState(true)
    const [activeTab, setActiveTab] = useState("overview")
    const [infor, setInfor] = useState(activeTab === "overview" ? "" : [])
    const [inforSpecialty, setInforspecialty] = useState(activeTab === "overview" ? "" : [])
    const [recommendSpecialties, setRecommendSpecialties] = useState()

    useEffect(() => {
        const fetchData = async () => {
            try {
                let response

                switch (activeTab) {
                    case "overview":
                        response = await axios.get(`/specialties/${specialty}/description`)
                        console.log(response)
                        setInforspecialty(response.data)
                        break
        
                    case "doctors":
                        response = await axios.get(`/doctors/${specialty}`)
                        
                        setInfor(Array.isArray(response.data) ? response.data : [])
                        break
        
                    case "services":
                        response = await axios.get(`/services/${specialty}/services`)

                        setInfor(Array.isArray(response.data) ? response.data : [])
                        break
        
                    default:
                        setInfor(null)
                        break
                }

            } catch (error) {
                console.error("Lỗi lấy dữ liệu:", error)
            }
        }
        
        fetchData()
    }, [activeTab, specialty])

    useEffect(() => {
        const fetchRadomSpecialty = async () => {
          try {
            const response = await axios.get(`/specialties/random`)
            setRecommendSpecialties(response.data)
          } catch (error) {
            console.log(error)
          }
        }
    
        fetchRadomSpecialty()
      }, [specialty])
    
    // if (loading) {
    //     return (
    //     <Container className="d-flex flex-column align-items-center justify-content-center" style={{ minHeight: "60vh" }}>
    //         <Spinner animation="border" variant="primary" className="mb-3" />
    //         <p className="text-muted">Đang tải thông tin khoa bệnh...</p>
    //     </Container>
    //     )
    // }

    if (!specialty) {
        return (
            <Container className="d-flex flex-column align-items-center justify-content-center" style={{ minHeight: "60vh" }}>
                <div className="text-center">
                    <h4 className="mb-3">Không tìm thấy thông tin khoa bệnh</h4>
                    <p className="text-muted mb-4">Thông tin khoa bệnh bạn đang tìm kiếm không tồn tại hoặc đã bị xóa</p>
                    <Button
                        variant="outline-primary"
                        onClick={() => window.history.back()}
                        className="d-flex align-items-center gap-2"
                    >
                        <ChevronLeft size={18} />
                        Quay lại
                    </Button>
                </div>
            </Container>
        )
    }

    return (
        <Container fluid className="py-4 bg-light">
            <Container className="w-75">
                <div className="d-flex align-items-center mb-4">
                    <Button
                        variant="link"
                        className="p-0 text-decoration-none d-flex align-items-center text-muted"
                        onClick={() => window.history.back()}
                    >
                        <ChevronLeft size={18} />
                        <span>Quay lại trang trước</span>
                    </Button>
                </div>
    
                <Row>
                    <Col lg={8} className="mb-4 mb-lg-0">
                        <Card className="border-0 shadow-sm mb-4 overflow-hidden">
                            <Card.Img
                            variant="top"
                            src={inforSpecialty.specialtyImage}
                            alt={`Hình ảnh của ${inforSpecialty.name}`}
                            className="img-fluid"
                            style={{ objectFit: "cover", maxHeight: "400px", width: "100%" }}
                            />
                        </Card>

                        <Tab.Container id="dashboard-tabs" activeKey={activeTab} onSelect={(k) => setActiveTab(k)}>
                            <Nav variant="tabs" className="py-0">
                                <Nav.Item>
                                    <Nav.Link eventKey="overview" className={`d-flex align-items-center ${activeTab === 'overview' ? 'text-primary' : 'text-dark'}`}>
                                        <span className="me-2">
                                        </span>
                                        Tổng Quan
                                    </Nav.Link>
                                </Nav.Item>
                                <Nav.Item>
                                    <Nav.Link eventKey="services" className={`d-flex align-items-center ${activeTab === 'services' ? 'text-primary' : 'text-dark'}`}>
                                        <span className="me-2">
                                        </span>
                                        Dịch vụ
                                    </Nav.Link>
                                </Nav.Item>
                                <Nav.Item>
                                    <Nav.Link eventKey="doctors" className={`d-flex align-items-center ${activeTab === 'doctors' ? 'text-primary' : 'text-dark'}`}>
                                        <span className="me-2">
                                        </span>
                                        Bác sĩ
                                    </Nav.Link>
                                </Nav.Item>
                            </Nav>
                            
                            <Tab.Content>
                                <Tab.Pane eventKey="overview">
                                        <Card className="border-0 shadow-sm">
                                            <Card.Body className="p-4">
                                                <h4 className="text-primary mb-4">Giới thiệu về {inforSpecialty?.name}</h4>
                                                <p className="mb-4">{inforSpecialty?.name} {inforSpecialty?.description}</p>

                                                <h5 className="mb-3">Lịch sử phát triển</h5>
                                                <p className="mb-4">
                                                    Khoa được thành lập vào năm 2030 với đội ngũ y bác sĩ giàu kinh nghiệm.
                                                    Trải qua nhiều năm phát triển, khoa đã không ngừng nâng cao chất lượng khám chữa bệnh, đầu tư trang
                                                    thiết bị hiện đại và đào tạo nhân lực chuyên sâu.
                                                </p>

                                                <h5 className="mb-3">Sứ mệnh</h5>
                                                <p className="mb-4">
                                                    Cung cấp dịch vụ chăm sóc sức khỏe chất lượng cao, an toàn và hiệu quả cho người bệnh. Không ngừng
                                                    nâng cao trình độ chuyên môn, ứng dụng các kỹ thuật tiên tiến trong chẩn đoán và điều trị.
                                                </p>
                                            </Card.Body>
                                        </Card>
                                </Tab.Pane>
                                
                                <Tab.Pane eventKey="services">
                                    <Card className="border-0 shadow-sm">
                                        <Card.Body className="p-4">
                                            <h4 className="text-primary mb-4">Dịch vụ của {inforSpecialty?.name}</h4>

                                            {Array.isArray(infor) && infor.length > 0 ? (
                                                <div className="mx-auto">
                                                    <div className="service-list">
                                                        <Row>
                                                            {infor.map(service => (
                                                                <Col xs={12} md={6} xl={4} >
                                                                    <ServiceCard key={service.id} service={service} />
                                                                </Col>
                                                            ))}
                                                        </Row>
                                                    </div>
                                                </div>
                                            ) : (
                                                <p>Hiện chưa có dịch vụ nào được hiển thị.</p>
                                            )}
                                        </Card.Body>
                                    </Card>
                                </Tab.Pane>
                                
                                <Tab.Pane eventKey="doctors"> 
                                    <Card className="border-0 shadow-sm">
                                        <Card.Body className="p-4">
                                            <h4 className="text-primary mb-4">Đội ngũ bác sĩ</h4>
                                            <p className="mb-4">
                                                Đội ngũ y bác sĩ của khoa là những chuyên gia có trình độ chuyên môn cao, giàu kinh nghiệm và tận
                                                tâm với người bệnh.
                                            </p>
                                            <Row>
                                                {Array.isArray(infor) && infor.length > 0 ? (
                                                    <div className="doctor-list d-flex justify-content-center" style={{ display: 'flex', flexWrap: 'wrap', gap: '20px' }}>
                                                        <Row>
                                                            {infor.map(doctor => (
                                                                <Col xs={12} md={6} xl={4} >
                                                                    <DoctorCard key={doctor.id} doctor={doctor} />
                                                                </Col>
                                                            ))}
                                                        </Row>
                                                    </div>
                                                ) : (
                                                    <p>Hiện chưa có bác sĩ nào được hiển thị.</p>
                                                )}
                                            </Row>
                                        </Card.Body>
                                    </Card>
                                </Tab.Pane>
                            </Tab.Content>
                        </Tab.Container>
                    </Col>
        
                    <Col lg={4}>
                        <Card className="border-0 shadow-sm mb-4">
                            <Card.Header className="bg-white p-3 border-bottom">
                            <h5 className="mb-0 fw-bold">Thông tin phòng khám</h5>
                            </Card.Header>
                            <Card.Body className="p-3">
                            <div className="d-flex align-items-start gap-2 mb-3">
                                <div>
                                    <p className="mb-1 fw-medium">Địa chỉ phòng khám:</p>
                                    <p className="text-muted mb-1">475A Đ. Điện Biên Phủ, Phường 25, Bình Thạnh, Hồ Chí Minh</p>
                                    <Link
                                        to="https://www.google.com/maps/dir//HUTECH,+7+Nguy%E1%BB%85n+Gia+Tr%C3%AD,+Ph%C6%B0%E1%BB%9Dng+25,+B%C3%ACnh+Th%E1%BA%A1nh,+H%E1%BB%93+Ch%C3%AD+Minh,+Vi%E1%BB%87t+Nam/@10.8018525,106.6740191,13z/data=!3m1!4b1!4m9!4m8!1m0!1m5!1m1!1s0x31752953ade9f9c9:0x6ad5d15cd48a4f4e!2m2!1d106.7152576!2d10.8018439!3e0?hl=vi-VN&entry=ttu&g_ep=EgoyMDI1MDMwNC4wIKXMDSoASAFQAw%3D%3D"
                                        target="_blank"
                                        className="d-flex align-items-center gap-1 text-primary"
                                    >
                                        <span>Xem bản đồ</span>
                                        <ExternalLink size={14} />
                                    </Link>
                                </div>
                            </div>
            
                            <hr className="my-3" />
            
                            <h6 className="fw-bold mb-2">Giờ làm việc</h6>
                            <div className="d-flex justify-content-between mb-1">
                                <span>Thứ 2 - Thứ 6:</span>
                                <span className="fw-medium">08:00 - 17:00</span>
                            </div>
                            <div className="d-flex justify-content-between mb-1">
                                <span>Thứ 7:</span>
                                <span className="fw-medium">08:00 - 12:00</span>
                            </div>
                            <div className="d-flex justify-content-between">
                                <span>Chủ nhật:</span>
                                <span className="fw-medium">Nghỉ</span>
                            </div>
                            </Card.Body>
                        </Card>
        
                        <Card className="border-0 shadow-sm mb-4 bg-white">
                            <Card.Header className="bg-white p-3 border-bottom">
                                <h5 className="mb-0 fw-bold">Chuyên khoa khác</h5>
                            </Card.Header>
                            <Card.Body className="p-0">
                                <div className="d-flex flex-column">
                                {recommendSpecialties?.map((recommend, index) => (
                                    <Recomend item={recommend} type={"specialty"} />
                                ))}
                                </div>
                            </Card.Body>
                        </Card>
                    </Col>
                </Row>
            </Container>
        </Container>
    )
}

export default DepartmentDetail