import React, { useEffect, useState } from 'react';
import { Table, Spinner, Alert, Form, Modal, Button } from 'react-bootstrap';
import axios from '../../../../Util/AxiosConfig';
import { Bar } from 'react-chartjs-2';
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend } from 'chart.js';

ChartJS.register( CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend );


function DoctorSalaryTable({ tabActive }) {
    const [salaries, setSalaries] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const getCurrentMonth = () => {
        const now = new Date();
        const month = (now.getMonth() + 1).toString().padStart(2, "0"); // Thêm số 0 nếu nhỏ hơn 10
        return `${now.getFullYear()}-${month}`;
    };
    const [month, setMonth] = useState(getCurrentMonth());
    const [showModal, setShowModal] = useState(false);
    const [detailData, setDetailData] = useState([]);
    const [selectedDoctor, setSelectedDoctor] = useState(null);
    const [revenueStats, setRevenueStats] = useState({
        totalCommission: 0,
        grossRevenue: 0,
        totalSalary: 0,
        netRevenue: 0,
    });

    const formatMonth = (month) => {
        if (month) {
            const [year, monthNumber] = month.split("-");
            return new Date(`${year}-${monthNumber}-01T00:00:00`);
        }
        return null;
    };

    const fetchSalaries = async () => {
        if (tabActive !== "salary") return

        setLoading(true);
        setError(null);
        try {
            const formattedMonth = formatMonth(month);
            const params = formattedMonth ? { month: formattedMonth.toISOString() } : {};
            const response = await axios.get("/doctors/salary/monthly", { params });

            // Ưu tiên lấy totalSalary từ backend nếu có, nếu không thì tính thủ công
            const mapped = response.data.map(item => ({
                ...item,
                totalSalary: item.totalSalary ?? (item.baseSalary + item.bonus + item.commission)
            }));
            setSalaries(mapped);
        } catch (err) {
            setError("Lỗi khi tải dữ liệu bảng lương.");
        } finally {
            setLoading(false);
        }
    };

    const fetchSalarySummary = async () => {
        try {
            const formattedMonth = formatMonth(month);
            const params = formattedMonth ? { month: formattedMonth.toISOString() } : {};
            const response = await axios.get("/doctors/salary-summary", { params });

            setRevenueStats(response.data); // Gán trực tiếp từ backend
        } catch (err) {
            console.error("Lỗi khi tải tổng kết lương từ backend:", err);
        }
    };


    const fetchSalaryDetails = async (doctorId) => {
        if (tabActive !== "salary") return
        
        try {
            const formattedMonth = formatMonth(month);
            const params = {
                doctorId,
                month: formattedMonth ? formattedMonth.toISOString() : null
            };
            const response = await axios.get('/doctors/salary/details', { params });

            // Nếu backend trả đúng fullName, totalSalary, baseSalary, bonus => giữ nguyên
            setSelectedDoctor({
                ...selectedDoctor,           // Giữ fullName từ bảng chính
                ...response.data             // Ghi đè bằng dữ liệu chi tiết
            });

            if (Array.isArray(response.data.details)) {
                setDetailData(response.data.details);
            } else {
                setDetailData([]);
            }

            setShowModal(true);
        } catch (err) {
            alert("Lỗi khi tải dữ liệu chi tiết.");
        }
    };

    useEffect(() => {
        if (tabActive === 'salary') {
            fetchSalaries();
            fetchSalarySummary();
        }
    }, [tabActive, month]);

    const handleClose = () => {
        setShowModal(false);
        setDetailData([]);
    };

    return (
        <div className='py-4'>
            <h5 className="mb-4">Bảng Lương Bác Sĩ Theo Tháng</h5>

            <Form.Group className="mb-3" controlId="monthPicker">
                <Form.Label>Chọn tháng:</Form.Label>
                <Form.Control
                    type="month"
                    value={month}
                    onChange={(e) => setMonth(e.target.value)}
                />
            </Form.Group>

            {loading && <Spinner animation="border" />}
            {error && <Alert variant="danger">{error}</Alert>}

            {!loading && salaries.length > 0 && (
            <>
                {revenueStats && (
                    <div className="row mb-4">
                        {/* Tổng kết tháng */}
                        <div className="col-md-5 mb-3 mb-md-0">
                            <div className="card h-100">
                                <div className="card-body p-3">
                                    <h6 className="card-title mb-3">📊 <strong>Tổng kết tháng:</strong></h6>
                                    <p className="mb-1"><strong>Tổng hoa hồng:</strong> {revenueStats.totalCommission.toLocaleString()} đ</p>
                                    <p className="mb-1"><strong>Tổng lương bác sĩ:</strong> {revenueStats.totalSalary.toLocaleString()} đ</p>
                                    <p className="mb-1"><strong>Tổng doanh thu (ước tính):</strong> {revenueStats.grossRevenue.toLocaleString()} đ</p>
                                    <p className="mb-0"><strong>Doanh thu ròng:</strong> {revenueStats.netRevenue.toLocaleString()} đ</p>
                                </div>
                            </div>
                        </div>

                        {/* Biểu đồ */}
                        <div className="col-md-7">
                            <div className="card h-100">
                                <div className="card-body p-3" style={{ height: '250px' }}>
                                    <Bar
                                        data={{
                                            labels: ['Tổng hoa hồng', 'Tổng lương bác sĩ', 'Tổng doanh thu', 'Doanh thu ròng'],
                                            datasets: [
                                                {
                                                    label: 'VNĐ',
                                                    data: [
                                                        revenueStats.totalCommission,
                                                        revenueStats.totalSalary,
                                                        revenueStats.grossRevenue,
                                                        revenueStats.netRevenue
                                                    ],
                                                    backgroundColor: ['#36a2eb', '#ff6384', '#4bc0c0', '#9966ff']
                                                }
                                            ]
                                        }}
                                        options={{
                                            responsive: true,
                                            maintainAspectRatio: false,
                                            plugins: {
                                                legend: { display: false },
                                                title: {
                                                    display: true,
                                                    text: 'Biểu đồ doanh thu tháng'
                                                }
                                            },
                                            scales: {
                                                y: {
                                                    ticks: {
                                                        callback: function (value) {
                                                            return value.toLocaleString() + ' đ';
                                                        }
                                                    }
                                                }
                                            }
                                        }}
                                    />
                                </div>
                            </div>
                        </div>
                    </div>
                )}

                <Table striped bordered hover responsive>
                    <thead>
                        <tr>
                            <th>Họ tên</th>
                            <th>Chuyên khoa</th>
                            <th>Lương cứng</th>
                            <th>Hoa hồng</th>
                            <th>Thưởng</th>
                            <th>Tổng lương</th>
                            <th>Hành động</th>
                        </tr>
                    </thead>
                    <tbody>
                        {salaries.map((item, idx) => (
                            <tr key={idx}>
                                <td>{item.doctorName}</td>
                                <td>{item.specialty}</td>
                                <td>{item.baseSalary?.toLocaleString()} đ</td>
                                <td>{item.commission?.toLocaleString()} đ</td>
                                <td>{item.bonus?.toLocaleString()} đ</td>
                                <td>{item.totalSalary?.toLocaleString()} đ</td>
                                <td>
                                    <Button
                                        variant="info"
                                        size="sm"
                                        onClick={() => {
                                            setSelectedDoctor(item); // Lưu fullName để hiển thị trước
                                            fetchSalaryDetails(item.doctorId);
                                        }}
                                    >
                                        Chi tiết
                                    </Button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </Table>
            </>
        )}

            {/* Modal Chi tiết */}
            <Modal show={showModal} onHide={handleClose} size="lg">
                <Modal.Header closeButton>
                    <Modal.Title>
                        Chi tiết lương bác sĩ: {selectedDoctor?.doctorName || selectedDoctor?.fullName}
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {Array.isArray(detailData) && detailData.length > 0 ? (
                        <>
                            <Table striped bordered hover responsive>
                                <thead>
                                    <tr>
                                        <th>Bệnh nhân</th>
                                        <th>Dịch vụ</th>
                                        <th>Ngày khám</th>
                                        <th>Giá dịch vụ</th>
                                        <th>Hoa hồng</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {detailData.map((item, idx) => (
                                        <tr key={idx}>
                                            <td>{item.patientName}</td>
                                            <td>{item.serviceName}</td>
                                            <td>{new Date(item.appointmentDate).toLocaleDateString()}</td>
                                            <td>{item.servicePrice?.toLocaleString()} đ</td>
                                            <td>{item.commission?.toLocaleString()} đ</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </Table>

                            {/* Hiển thị bonus và tổng lương */}
                            <div className="mt-3">
                                <h6>Tổng kết:</h6>
                                <p><strong>Lương cứng:</strong> {selectedDoctor?.baseSalary?.toLocaleString()} đ</p>
                                <p><strong>Thưởng:</strong> {selectedDoctor?.bonus?.toLocaleString()} đ</p>
                                <p><strong>lương khám dịch vụ:</strong> {selectedDoctor?.commissionTotal?.toLocaleString()} đ</p>
                                <p><strong>Tổng lương:</strong> {selectedDoctor?.totalSalary?.toLocaleString()} đ</p>
                            </div>
                        </>
                    ) : (
                        <p>Không có dữ liệu chi tiết.</p>
                    )}
                </Modal.Body>

                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Đóng
                    </Button>
                </Modal.Footer>
            </Modal>
        </div>
    );
}

export default DoctorSalaryTable;