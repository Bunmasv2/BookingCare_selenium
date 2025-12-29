import React from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import images from '../Image/Specialty/Index'
import SpecialtyNav from '../Component/SpecialtyNav'
import SpecialtyLogo from '../Component/SpecialtyLogo'
import "../Style/Specialty.css"
import { Col, Row } from 'react-bootstrap'

function Specialty() {
    const { specialty }  = useParams()
    const navigate = useNavigate()
    const src = images[specialty]

    const HandleAppointment = () => {
        navigate("/đặt lịch khám")
    }

    return (
        <div className='px-3'>
            <div className=''>
                <img className='w-100' src={images.specialty} alt="" />
            </div>
            <div className='mx-auto w-75'>
                <div className='d-flex mt-3'>
                    <SpecialtyLogo src={src} />
                    <h4 className='ms-2 my-auto'>{specialty}</h4>
                </div>
                <Row>
                    <Col xs={10} >
                        <SpecialtyNav src={src} />
                    </Col>
                    <Col xs={2} >
                    <div className="w-100 text-center">
                            {/* Nút đặt lịch hẹn */}
                            <div 
                                className="appointment bg-primary text-white py-2 fw-bold rounded shadow-sm"
                                style={{ cursor: "pointer", transition: "0.3s" }} 
                                onClick={HandleAppointment}
                            >
                                Đặt lịch hẹn
                            </div>

                            {/* Thông tin phòng khám */}
                            <div 
                                className="text-start p-3 mt-3 border rounded shadow-sm" 
                                style={{ backgroundColor: "#f8f9fa" }}
                            >
                                <p className="fw-bold mb-1">📍 Địa chỉ phòng khám</p>
                                <p className="small text-muted">475A Đ. Điện Biên Phủ, Phường 25, Bình Thạnh, Hồ Chí Minh</p>
                                
                                {/* Link Google Maps */}
                                <Link 
                                    to="https://www.google.com/maps/dir//HUTECH,+7+Nguy%E1%BB%85n+Gia+Tr%C3%AD,+Ph%C6%B0%E1%BB%9Dng+25,+B%C3%ACnh+Th%E1%BA%A1nh,+H%E1%BB%93+Ch%C3%AD+Minh,+Vi%E1%BB%87t+Nam/@10.8018525,106.6740191,13z/data=!3m1!4b1!4m9!4m8!1m0!1m5!1m1!1s0x31752953ade9f9c9:0x6ad5d15cd48a4f4e!2m2!1d106.7152576!2d10.8018439!3e0?hl=vi-VN&entry=ttu&g_ep=EgoyMDI1MDMwNC4wIKXMDSoASAFQAw%3D%3D" 
                                    target="_blank" 
                                    className="btn btn-link p-0 text-primary fw-bold"
                                >
                                    Xem bản đồ
                                </Link>
                            </div>
                        </div>
                    </Col>
                </Row>

            </div>
        </div>
    )
}

export default Specialty