import React, { useEffect, useState } from 'react';
import { Badge, Button, Card, Table, Pagination } from 'react-bootstrap';
import axios from '../../../Util/AxiosConfig';
import { formatDateToLocale } from '../../../Util/DateUtils';

function Appointments({ tabActive }) {
    const [appointments, setAppointments] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [loading, setLoading] = useState(false);
    const [errorMessage, setErrorMessage] = useState("");
    const pageSize = 10; // Số lượng lịch hẹn mỗi trang

    useEffect(() => {
        const fetchAppointments = async () => {
            if (tabActive !== "appointments") return
            setLoading(true)

            try {
                const response = await axios.post(`appointments/by-patient?page=${currentPage}&pageSize=${pageSize}`)
                console.log(response)
                setAppointments(response.data.appointments)
                setTotalPages(response.data.totalPages)
            } catch (error) {
                console.error(error)
            } finally {
                setLoading(false)
            }
        }

        fetchAppointments()
    }, [tabActive, currentPage])

    const handleCancelAppointment = async (appointmentId) => {
        if (window.confirm("Bạn có chắc chắn muốn hủy lịch hẹn này không?")) {
            try {
                setErrorMessage(""); // clear lỗi cũ
                await axios.put(`/appointments/cancel/${appointmentId}`);

                // Tải lại dữ liệu trang hiện tại
                const response = await axios.post(`appointments/by-patient?page=${currentPage}&pageSize=${pageSize}`);
                setAppointments(response.data.appointments);
                setTotalPages(response.data.totalPages);
            } catch (error) {
                console.error(error);
                if (error.response && error.response.data && error.response.data.message) {
                    setErrorMessage(error.response.data.message); // lấy message từ backend
                } else {
                    setErrorMessage("Đã xảy ra lỗi khi hủy lịch hẹn.");
                }
            }
        }
    }


    const handlePageChange = (page) => {
        setCurrentPage(page)
    }

    // Tạo các mục phân trang
    const renderPaginationItems = () => {
        const items = []
        
        // Nút Previous
        items.push(
            <Pagination.Prev 
                key="prev"
                onClick={() => currentPage > 1 && handlePageChange(currentPage - 1)}
                disabled={currentPage === 1 || loading}
            />
        )

        // Luôn hiển thị trang đầu tiên
        items.push(
            <Pagination.Item 
                key={1} 
                active={currentPage === 1}
                onClick={() => handlePageChange(1)}
                disabled={loading}
            >
                1
            </Pagination.Item>
        )

        // Hiển thị dấu ... nếu trang hiện tại > 3
        if (currentPage > 3) {
            items.push(<Pagination.Ellipsis key="ellipsis-1" disabled />);
        }

        // Hiển thị các trang xung quanh trang hiện tại
        for (let page = Math.max(2, currentPage - 1); page <= Math.min(totalPages - 1, currentPage + 1); page++) {
            if (page !== 1 && page !== totalPages) {  // Đã thêm trang đầu và cuối riêng
                items.push(
                    <Pagination.Item 
                        key={page} 
                        active={currentPage === page}
                        onClick={() => handlePageChange(page)}
                        disabled={loading}
                    >
                        {page}
                    </Pagination.Item>
                )
            }
        }

        // Hiển thị dấu ... nếu trang hiện tại < totalPages - 2
        if (currentPage < totalPages - 2) {
            items.push(<Pagination.Ellipsis key="ellipsis-2" disabled />);
        }

        // Luôn hiển thị trang cuối cùng nếu có nhiều hơn 1 trang
        if (totalPages > 1) {
            items.push(
                <Pagination.Item 
                    key={totalPages} 
                    active={currentPage === totalPages}
                    onClick={() => handlePageChange(totalPages)}
                    disabled={loading}
                >
                    {totalPages}
                </Pagination.Item>
            );
        }

        // Nút Next
        items.push(
            <Pagination.Next 
                key="next"
                onClick={() => currentPage < totalPages && handlePageChange(currentPage + 1)}
                disabled={currentPage === totalPages || loading}
            />
        )

        return items
    }

    return (
        <Card>
            <Card.Body>
                <p>Danh sách các lịch hẹn</p>
                {errorMessage && (
                    <div className="alert alert-danger" role="alert">
                        {errorMessage}
                    </div>
                )}

                <div className="table-responsive">
                    {loading ? (
                        <div className="text-center py-4">
                            <div className="spinner-border text-primary" role="status">
                                <span className="visually-hidden">Đang tải...</span>
                            </div>
                        </div>
                    ) : (
                        <>
                            <Table bordered hover>
                                <thead className="table-light text-center">
                                    <tr>
                                        <th>Ngày hẹn</th>
                                        <th>Bác sĩ</th>
                                        <th>Dịch vụ</th>
                                        <th>Trạng thái</th>
                                        <th>Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {appointments.length > 0 ? (
                                        appointments.map((appointment) => (
                                            <tr key={appointment.appointmentId} className="align-middle text-center">
                                                <td>{formatDateToLocale(appointment.appointmentDate.toString())  }</td>
                                                <td>{appointment.doctorName}</td>
                                                <td>{appointment.serviceName}</td>
                                                <td>
                                                    <Badge bg={
                                                        appointment.status === "Chờ xác nhận" ? "warning" :
                                                        appointment.status === "Đã xác nhận" ? "primary" :
                                                        appointment.status === "Đã hủy" ? "danger" :
                                                        appointment.status === "Đã hoàn thành" ? "success" : "secondary"
                                                    }>
                                                        {appointment.status}
                                                    </Badge>
                                                </td>
                                                <td>
                                                    {(appointment.status !== "Đã hủy" && appointment.status !== "Đã hoàn thành"&& appointment.status !== "Đã khám") && (
                                                        <div className="d-flex gap-2 justify-content-center">
                                                            <Button size="sm" variant="danger" onClick={() => handleCancelAppointment(appointment.appointmentId)}>
                                                                Hủy
                                                            </Button>
                                                        </div>
                                                    )}
                                                </td>
                                            </tr>
                                        ))
                                    ) : (
                                        <tr>
                                            <td colSpan="5" className="text-center py-3">
                                                Không có lịch hẹn nào
                                            </td>
                                        </tr>
                                    )}
                                </tbody>
                            </Table>
                            {totalPages > 1 && (
                                <div className="d-flex justify-content-center mt-3">
                                    <Pagination>
                                        {renderPaginationItems()}
                                    </Pagination>
                                </div>
                            )}
                        </>
                    )}
                </div>
            </Card.Body>
        </Card>
    )
}

export default Appointments