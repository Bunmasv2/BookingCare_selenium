import React, { useContext, useState, useRef, useEffect } from "react"
import { Container, Nav, Navbar, NavDropdown } from "react-bootstrap"
import { useLocation, useNavigate } from "react-router-dom"
import { NavContext } from "../Context/NavContext"
import { AuthContext } from "../Context/AuthContext"
import "../Style/Nav.css"

const Navigation = () => {
  const location = useLocation()
  const navigate = useNavigate()
  const { isAuthenticated, UserName, logout } = useContext(AuthContext)
  const { specialties, services, HandleNavigation } = useContext(NavContext)
  const indicatorRef = useRef(null)
  const navRefs = useRef([])
  const pages = [
    { name: "Trang chủ", link: "/" },
    { name: "Giới Thiệu", link: "/về chúng tôi" },
    { name: "Đội ngũ bác sĩ", link: "/bác sĩ" },
    { name: "Chuyên khoa", link: "/chuyên khoa" },
    { name: "Dịch vụ", link: "/dịch vụ" },
    { name: "Đặt lịch khám", link: "/đặt lịch khám" },
    { name: "Liên hệ", link: "/liên hệ" },
  ]

  const [openDropdown, setOpenDropdown] = useState(null)
  const [activeIndex, setActiveIndex] = useState(null)
  const savedIndexRef = useRef(null)

  const normalizePath = (path) => {
    return decodeURIComponent(path).replace(/\/$/, "")
  }

  const isPageActive = (pageLink) => {
    const currentPath = normalizePath(location.pathname)
    const targetPath = normalizePath(pageLink)
    return currentPath === targetPath 
  }

  const moveIndicatorTo = (index) => {
    if (indicatorRef.current && navRefs.current[index]) {
      const navItem = navRefs.current[index]
      const indicator = indicatorRef.current
      indicator.style.width = `${navItem.offsetWidth}px`
      indicator.style.left = `${navItem.offsetLeft}px`
      indicator.style.opacity = "1"
    }
  }

  const handleMouseEnter = (index) => {
    setOpenDropdown(index)
    moveIndicatorTo(index)
  }

  const handleMouseLeave = () => {
    setOpenDropdown(null)
    if (savedIndexRef.current !== null) {
      moveIndicatorTo(savedIndexRef.current)
    }
  }

  const handleClick = (index, link) => {
    savedIndexRef.current = index
    setActiveIndex(index)
    setOpenDropdown(null) // Đóng dropdown khi click vào một link
    navigate(link)
  }

  // Đóng tất cả dropdown khi URL thay đổi
  useEffect(() => {
    setOpenDropdown(null)
    
    const currentIndex = pages.findIndex((page) => isPageActive(page.link))
    if (currentIndex !== -1) {
      savedIndexRef.current = currentIndex
      setActiveIndex(currentIndex)
      setTimeout(() => {
        moveIndicatorTo(currentIndex)
      }, 300);
    }
  }, [location.pathname])

  const handleLogout = () => {
    logout()
    navigate("/")
  }

  const RenderNav = () => {
    return pages.map((page, index) => {
      const isActive = isPageActive(page.link);

      if (index === 3 || index === 4) {
        return (
          <NavDropdown
            title={page.name}
            key={index}
            id="basic-nav-dropdown"
            show={openDropdown === index}
            onMouseEnter={() => handleMouseEnter(index)}
            onMouseLeave={handleMouseLeave}
            className={`drop-item nav-item ${isActive ? "active" : ""} me-2`}
            ref={(el) => (navRefs.current[index] = el)}
          >
            {index === 3 ? RenderSpecialties() : RenderServices()}
          </NavDropdown>
        )
      }

      return (
        <Nav.Link
          key={index}
          className={`nav ${isActive ? "active" : ""} me-2`}
          onMouseEnter={() => handleMouseEnter(index)}
          onMouseLeave={handleMouseLeave}
          onClick={() => handleClick(index, page.link)}
          ref={(el) => (navRefs.current[index] = el)}
        >
          {page.name}
        </Nav.Link>
      )
    })
  }

  const RenderSpecialties = () => {
    return specialties.map((speciality, index) => (
      <NavDropdown.Item
        key={index}
        onClick={() => {
          // Đóng dropdown trước khi điều hướng
          setOpenDropdown(null);
          HandleNavigation("chuyên khoa", speciality.name);
          savedIndexRef.current = 3;
        }}
      >
        {speciality.name}
      </NavDropdown.Item>
    ));
  };
  
  const RenderServices = () => {
    const itemsPerColumn = 15;
    const numColumns = Math.ceil(services.length / itemsPerColumn);
  
    // Chia đều services thành các mảng con để đổ vào từng cột
    const chunked = Array.from({ length: numColumns }, (_, i) =>
      services.slice(i * itemsPerColumn, (i + 1) * itemsPerColumn)
    );
  
    return (
      <div className="multi-column-dropdown">
        {chunked.map((chunk, index) => (
          <div className="dropdown-column" key={index}>
            {chunk.map((service, i) => (
              <NavDropdown.Item
                key={i}
                onClick={() => {
                  setOpenDropdown(null);
                  HandleNavigation("dịch vụ", service.serviceName);
                  savedIndexRef.current = 4;
                }}
              >
                {service.serviceName}
              </NavDropdown.Item>
            ))}
          </div>
        ))}
      </div>
    );
  };
  
  return (
    <Navbar expand="lg" className="bg-info-subtle py-2">
      <Container className="w-75 mx-auto" style={{ width: "70%" }}>
        <Navbar.Brand href="/">{/* Logo */}</Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
          <Nav className="me-auto position-relative">
            {RenderNav()}
            <div className="nav-indicator" ref={indicatorRef}></div>
          </Nav>
          <Nav>
            {isAuthenticated ? (
              <Nav.Link
                onClick={() => navigate("/thông tin cá nhân/#appointments")}
                className="btn-login"
              >
                {`Xin chào, ${UserName}`}
              </Nav.Link>
            ) : (
              <Nav.Link
                onClick={() => navigate("/Đăng nhập")}
                className="btn-login"
              >
                Đăng nhập / Đăng ký
              </Nav.Link>
            )}
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  )
}

export default Navigation