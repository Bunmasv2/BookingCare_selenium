import React, { useState, useEffect, useContext } from 'react'
import { Container, Form, InputGroup, Button, Nav, Row, Col } from 'react-bootstrap'
import { FaSearch } from 'react-icons/fa'
import { DoctorCard } from '../Component/Card/Index'
import axios from '../Util/AxiosConfig'
import Loading from '../Component/Loading'
import '../Style/DoctorPage.css'
import { NavContext } from '../Context/NavContext'

const Doctor = () => {
  const { specialties } = useContext(NavContext)
  const [doctors, setDoctors] = useState([])
  const [activeSpecialty, setActiveSpecialty] = useState('all')
  const [searchTerm, setSearchTerm] = useState('')
  const [loading, setLoading] = useState(true)
  
  // Pagination
  const [currentPage, setCurrentPage] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [totalItems, setTotalItems] = useState(0)

  useEffect(() => {
    fetchDoctors(1)
  }, [])

  const fetchDoctors = async (page, specialty = 'all', keyword = '') => {
    try {
      setLoading(true)
      let url = `/doctors/paged?pageNumber=${page}`
      if (specialty !== 'all') {
        url += `&specialty=${specialty}`
      }
      if (keyword.trim()) {
        url += `&keyword=${keyword}`
      }
      
      const response = await axios.get(url, { withCredentials: true })

      console.log(response)
      
      setDoctors(response.data.items)
      setTotalPages(response.data.totalPages)
      setTotalItems(response.data.totalItems)
      setCurrentPage(response.data.pageNumber)
    } catch (error) {
      console.error('Lỗi khi lấy danh sách bác sĩ:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleSpecialtyFilter = async (specialty) => {
    setActiveSpecialty(specialty)
    await fetchDoctors(1, specialty, searchTerm)
  }

  const handleSearch = async (e) => {
    e.preventDefault()
    await fetchDoctors(1, activeSpecialty, searchTerm)
  }

  const handlePageChange = (pageNumber) => {
    setCurrentPage(pageNumber)
    fetchDoctors(pageNumber, activeSpecialty, searchTerm)
    window.scrollTo(0, 0)
  }

  return (
    <Container className="py-5">
      <div className='mx-auto'>
      <h1 className="text-center text-primary mb-5">Đội ngũ bác sĩ</h1>

      <div className="search-container mb-4">
        <Form onSubmit={handleSearch}>
          <InputGroup className="shadow-sm rounded-pill overflow-hidden">
            <InputGroup.Text className="bg-white border-end-0">
              <FaSearch className="text-muted" />
            </InputGroup.Text>
            <Form.Control
              placeholder="Tìm kiếm bác sĩ theo tên, chức vụ..."
              className="border-start-0"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              spellCheck={false}
            />
          </InputGroup>
        </Form>
      </div>

      <Nav className="justify-content-center mb-4 specialty-nav">
        <Nav.Item>
          <Nav.Link
            className={activeSpecialty === 'all' ? 'active' : ''}
            onClick={() => handleSpecialtyFilter('all')}
          >
            Tất cả
          </Nav.Link>
        </Nav.Item>
        {specialties.map((specialty) => (
          <Nav.Item key={specialty.id}>
            <Nav.Link
              className={activeSpecialty === specialty.name ? 'active' : ''}
              onClick={() => handleSpecialtyFilter(specialty.name)}
            >
              {specialty.name}
            </Nav.Link>
          </Nav.Item>
        ))}
      </Nav>

      {loading ? (
        <Loading text="Đang tải danh sách bác sĩ..." />
      ) : (
        <>
          <Row className="d-flex g-1 mx-auto" style={{ width: "90%" }}>
            {doctors.length > 0 ? (
              doctors.map(doctor => (
                <Col
                  key={doctor.doctorId}
                  lg={3} md={4} sm={6}
                  className="mb-4 d-flex justify-content-center"
                  style={{ minHeight: '300px' }}
                >
                  <DoctorCard doctor={doctor} />
                </Col>
              ))
            ) : (
              <div className="text-center my-5 w-100">
                <h5>Không tìm thấy bác sĩ phù hợp!</h5>
              </div>
            )}
          </Row>

          {totalItems > 0 && (
            <div className="d-flex justify-content-center mt-4">
              <nav>
                <ul className="pagination">
                  {[...Array(totalPages).keys()].map(number => (
                    <li key={number + 1} className={`page-item ${currentPage === number + 1 ? 'active' : ''}`}>
                      <button className="page-link" onClick={() => handlePageChange(number + 1)}>
                        {number + 1}
                      </button>
                    </li>
                  ))}
                </ul>
              </nav>
            </div>
          )}
        </>
      )}
      </div>
    </Container>
  )
}

export default Doctor