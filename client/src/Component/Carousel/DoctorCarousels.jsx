import { useState } from "react"
import { Button, Col, Container, Row } from "react-bootstrap"
import { useNavigate } from "react-router-dom"

function DoctorCarousels({ doctors }) {
  const [activeIndex, setActiveIndex] = useState(0)
  const navigate = useNavigate()

  if (!doctors || doctors.length === 0) {
    return <p>Không có bác sĩ nào để hiển thị.</p>
  }

  const totalDoctors = doctors.length

  const getLoopedIndex = (index) => {
    if (index < 0) return index + totalDoctors
    if (index >= totalDoctors) return index - totalDoctors
    return index
  }

  const handlePrev = () => {
    setActiveIndex((prev) => getLoopedIndex(prev - 1))
  }

  const handleNext = () => {
    setActiveIndex((prev) => getLoopedIndex(prev + 1))
  }

  const handleSelectDoctor = (index) => {
    setActiveIndex(index)
  }

  const visibleDoctors = Array.from({ length: Math.min(5, totalDoctors) }, (_, i) => {
    const index = getLoopedIndex(activeIndex + i)
    return doctors[index]
  })

  const activeDoctor = doctors[activeIndex]

  return (
    <div className="doctor-section py-0 w-75 mx-auto" >
      <Container>
        <div className="mb-5">
          <h2 className="text-center fw-bold" style={{ color: "#1a56a8", fontSize: "2.5rem" }}>
            ĐỘI NGŨ BÁC SĨ
          </h2>
          <div className="d-flex justify-content-center">
            <div style={{ height: "4px", width: "70px", backgroundColor: "#ffa500", marginTop: "8px" }}></div>
          </div>
        </div>
        <Row>
          <Col lg={5} className="text-center">
            <div
              style={{
                width: "100%",
                maxWidth: "450px",
                aspectRatio: "1/1",
                margin: "0 auto",
                position: "relative",
                // borderRadius: "50%",
                overflow: "hidden",
              }}
            >
              <img
                src={activeDoctor?.doctorImage || "/placeholder.svg"}
                alt={activeDoctor?.userName}
                style={{ width: "100%",
                    height: "100%",
                    objectFit: "cover",
                    borderRadius: "50%",}}
              />
            </div>
          </Col>
          <Col lg={7}>
            <div className="mt-4 mt-lg-0">
              <p className="mb-1" style={{ color: "#555", fontSize: "1rem" }}>
                {activeDoctor?.degree}
              </p>
              <h2 className="fw-bold mb-2" style={{ color: "#333", fontSize: "2rem" }}>
                {activeDoctor?.userName}
              </h2>
              <p className="mb-4" style={{ color: "#ffa500", fontWeight: "500", fontSize: "1.1rem" }}>
                {activeDoctor?.position}
              </p>

              <div className="mb-4" style={{ color: "#555", lineHeight: "1.6" }}>
                <p className="mb-1">
                  <strong>
                    {activeDoctor?.degree} {activeDoctor?.userName}
                  </strong>
                  {activeDoctor?.description}
                </p>
                <p>
                  {activeDoctor?.experienceYears &&
                    `Có hơn ${activeDoctor.experienceYears} năm kinh nghiệm trong ngành.`}
                </p>
              </div>

              <Button
                variant="primary"
                onClick={() => navigate(`/bac-si/${activeDoctor?.userName}`)}
                style={{
                  backgroundColor: "#1a56a8",
                  border: "none",
                  padding: "8px 30px",
                  borderRadius: "4px",
                  fontWeight: "500",
                }}
              >
                Chi tiết
              </Button>

        <div className="mt-5 position-relative">
          <button
            className="position-absolute start-0 top-50 translate-middle-y bg-white rounded-circle border-0 shadow-sm"
            style={{ width: "40px", height: "40px", zIndex: 10 }}
            onClick={handlePrev}
          >
            {/* <ChevronLeft /> */}
            <span>&lt;</span>
          </button>

          <button
            className="position-absolute end-0 top-50 translate-middle-y bg-white rounded-circle border-0 shadow-sm"
            style={{ width: "40px", height: "40px", zIndex: 10 }}
            onClick={handleNext}
          >
            {/* <ChevronRight /> */}
            <div className="mx-auto">&gt;</div>
          </button>

          {/* Thumbnails */}
          <div className="d-flex justify-content-center" style={{ padding: "0 50px" }}>
            {visibleDoctors.map((doctor, index) => {
              const isActive = index === 0
              return (
                <div
                  key={index}
                  className="mx-2"
                  style={{
                    width: "120px",
                    cursor: "pointer",
                  }}
                  onClick={() => handleSelectDoctor(getLoopedIndex(activeIndex + index))}
                >
                  <div
                    style={{
                      width: "100%",
                      aspectRatio: "1/1",
                      borderRadius: "50%",
                      overflow: "hidden",
                      border: isActive ? "3px solid #1a56a8" : "1px solid #ddd",
                      padding: "3px",
                      backgroundColor: "#fff",
                    }}
                  >
                    <img
                      src={doctor.doctorImage || "/placeholder.svg"}
                      alt={doctor.userName}
                      style={{
                        width: "100%",
                        height: "100%",
                        objectFit: "cover",
                        borderRadius: "50%",
                      }}
                    />
                  </div>
                </div>
              )
            })}
          </div>

          {/* Progress indicator */}
          <div className="mt-4">
            <div className="progress" style={{ height: "4px" }}>
              <div
                className="progress-bar"
                style={{
                  width: `${(activeIndex / (totalDoctors - 1)) * 100}%`,
                  backgroundColor: "#1a56a8",
                }}
              ></div>
            </div>
          </div>
        </div>
            </div>
          </Col>
        </Row>
      </Container>
    </div>
  )
}

export default DoctorCarousels