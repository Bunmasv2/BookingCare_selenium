import { Nav } from 'react-bootstrap'

function Index({ tabActive, systemMenuOpen, setSystemMenuOpen }) {
    
    return (
        <div className="system-management-section mb-2">
            <div 
                className="system-management-header px-3 py-2 mb-1 bg-light rounded d-flex justify-content-between align-items-center"
                onClick={() => setSystemMenuOpen(!systemMenuOpen)}
                style={{ cursor: 'pointer' }}
            >
                <span className="fw-medium text-primary">Quản lý hệ thống</span>
                <i className={`fas ${systemMenuOpen ? 'fa-chevron-up' : 'fa-chevron-down'} small text-secondary`}></i>
            </div>
                                        
            {systemMenuOpen && (
                <>
                    <Nav.Link 
                        eventKey="services" 
                        className={`sidebar-link ms-3 mb-1 ${tabActive === "services" ? "active" : ""}`}
                    >
                        <i className="fas fa-cogs me-2 small"></i>
                        Dịch vụ
                    </Nav.Link>
                                        
                    <Nav.Link 
                        eventKey="specialties" 
                        className={`sidebar-link ms-3 mb-1 ${tabActive === "specialties" ? "active" : ""}`}
                    >
                        <i className="fas fa-stethoscope me-2 small"></i>
                        Chuyên khoa
                    </Nav.Link>
                    
                    <Nav.Link 
                        eventKey="prescriptions"
                        className={`sidebar-link ms-3 mb-1 ${tabActive === "prescriptions" ? "active" : ""}`}
                    >
                        <i className="fas fa-stethoscope me-2 small"></i>
                        Hồ sơ bệnh nhân
                    </Nav.Link>

                    <Nav.Link 
                        eventKey="admins"
                        className={`sidebar-link ms-3 mb-1 ${tabActive === "admins" ? "active" : ""}`}
                    >
                        <i className="fas fa-stethoscope me-2 small"></i>
                        Quản trị viên
                    </Nav.Link>

                    <Nav.Link 
                        eventKey="doctors"
                        className={`sidebar-link ms-3 mb-1 ${tabActive === "admins" ? "active" : ""}`}
                    >
                        <i className="fas fa-stethoscope me-2 small"></i>
                        Bác sĩ
                    </Nav.Link>
                </>
            )}
        </div>
    )
}

export default Index