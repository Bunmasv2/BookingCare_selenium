import { CheckCircle, Info } from 'lucide-react'
import React from 'react'
import { Card } from 'react-bootstrap'

function ServiceSteps() {
    return (
        <>
            <Card.Header className="bg-white p-4 border-bottom">
                <h4 className="text-primary mb-0 d-flex align-items-center gap-2">
                <CheckCircle size={20} />
                    Quy trình thực hiện
                </h4>
            </Card.Header>
            <Card.Body className="p-4">
                <div className="bg-light p-4 rounded">
                    <div className="position-relative">
                        <div
                            className="position-absolute h-100"
                            style={{ width: "2px", backgroundColor: "#e9ecef", left: "10px", top: 0 }}
                        ></div>

                        <div className="d-flex mb-4 position-relative">
                            <div
                                className="bg-primary rounded-circle d-flex align-items-center justify-content-center"
                                style={{ width: "22px", height: "22px", zIndex: 1 }}
                            >
                                <span className="text-white fw-bold" style={{ fontSize: "12px" }}>1</span>
                            </div>
                            <div className="ms-4">
                                <h6 className="fw-bold mb-2">Đăng ký dịch vụ</h6>
                                <p className="mb-0">
                                    Khách hàng đăng ký dịch vụ tại Phòng Khám Đa khoa XYZ bằng cách đến trực tiếp phòng khám, hoặc
                                    đặt lịch hẹn qua website http://bookingcare.vn, fanpage Phòng Khám Đa Khoa XYZ.
                                </p>
                            </div>
                        </div>

                        <div className="d-flex mb-4 position-relative">
                            <div
                                className="bg-primary rounded-circle d-flex align-items-center justify-content-center"
                                style={{ width: "22px", height: "22px", zIndex: 1 }}
                            >
                                <span className="text-white fw-bold" style={{ fontSize: "12px" }}>2</span>
                            </div>
                            <div className="ms-4">
                                <h6 className="fw-bold mb-2">Tư vấn dịch vụ</h6>
                                <p className="mb-0">Bác sĩ hàng đầu sẽ tư vấn và hướng dẫn dịch vụ phù hợp với quý khách hàng.</p>
                            </div>
                        </div>

                        <div className="d-flex mb-4 position-relative">
                            <div
                                className="bg-primary rounded-circle d-flex align-items-center justify-content-center"
                                style={{ width: "22px", height: "22px", zIndex: 1 }}
                            >
                                <span className="text-white fw-bold" style={{ fontSize: "12px" }}>3</span>
                            </div>
                            <div className="ms-4">
                                <h6 className="fw-bold mb-2">Thực hiện dịch vụ</h6>
                                <p className="mb-0">Khách hàng thực hiện theo hướng dẫn của các chuyên gia hàng đầu tại Phòng Khám Đa Khoa XYZ.</p>
                            </div>
                        </div>

                        <div className="d-flex position-relative">
                            <div
                                className="bg-primary rounded-circle d-flex align-items-center justify-content-center"
                                style={{ width: "22px", height: "22px", zIndex: 1 }}
                            >
                                <span className="text-white fw-bold" style={{ fontSize: "12px" }}>4</span>
                            </div>
                            <div className="ms-4">
                                <h6 className="fw-bold mb-2">Nhận kết quả và tư vấn</h6>
                                <p className="mb-0">Khách hàng nhận kết quả, gặp bác sĩ để được tư vấn và hướng dẫn bước tiếp theo (nếu có).</p>
                            </div>
                        </div>
                    </div>
                </div>
            </Card.Body>
        </>
    )
}

export default ServiceSteps