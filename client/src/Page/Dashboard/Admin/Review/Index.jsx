import React from 'react'
import { Nav } from 'react-bootstrap'

function Index({ tabActive, menuOpen, setMenuOpen }) {
    return (
        <div className="system-management-section mb-2">
            <div 
                className="system-management-header px-3 py-2 mb-1 bg-light rounded d-flex justify-content-between align-items-center"
                onClick={() => setMenuOpen(!menuOpen)}
                style={{ cursor: 'pointer' }}
            >
                <span className="fw-medium text-primary">Quản Lý Đánh Giá</span>
                <i className={`fas ${menuOpen ? 'fa-chevron-up' : 'fa-chevron-down'} small text-secondary`}></i>
            </div>
                                    
            {menuOpen && (
                <>
                    <Nav.Link 
                        eventKey="reviewservices" 
                        className={`sidebar-link ms-3 mb-1 ${tabActive === "reviewservices" ? "active" : ""}`}
                    >
                        <i className="fas fa-cogs me-2 small"></i>
                        Dịch vụ
                    </Nav.Link>
                                                
                    <Nav.Link 
                        eventKey="reviewDoctors"
                        className={`sidebar-link ms-3 mb-1 ${tabActive === "reviewDoctors" ? "active" : ""}`}
                    >
                        <i className="fas fa-cogs me-2 small"></i>
                        Bác sĩ
                    </Nav.Link>
                </>
            )}
        </div>
    )
}

export default Index