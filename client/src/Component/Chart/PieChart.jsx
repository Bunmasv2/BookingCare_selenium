import { Pie } from 'react-chartjs-2'

function PieChart({ appointments }) {
    const pieData = {
        labels: ['Chờ xác nhận', 'Đã xác nhận', 'Đã khám', 'Đã hoàn thành', 'Đã hủy'],
        datasets: [
            {
                data: [
                    appointments[0]?.appointments || 0,
                    appointments[1]?.appointments || 0,
                    appointments[2]?.appointments || 0,
                    appointments[3]?.appointments || 0,
                    appointments[4]?.appointments || 0,
                ],
                backgroundColor: ['#0dcaf0', '#FF9800', '#198754', '#0d6efd', '#dc3545'],
                borderWidth: 1,
            },
        ],
    }

    const pieOptions = {
        plugins: {
            legend: {
                display: false,
            },
        },
    }

    return <Pie data={pieData} options={pieOptions} />
}

export default PieChart