import React, { useEffect, useState } from 'react'
import { Bar } from 'react-chartjs-2'

function BarChart({ data, total, label, labels }) {
    const [key, setKey] = useState()
    const [barData, setBarData] = useState()

    useEffect(() => {
        if (data?.length > 0) {
            const keys = Object.keys(data[0])
            setKey(keys[1])
        }
    }, [data])

    useEffect(() => {
        if (!key || !data || !Array.isArray(data) || data.length === 0 || !labels) return

        const newBarData = {
            labels: labels,
            datasets: [
                {
                    label: label || 'Data',
                    data: data.map(item => item[key] || 0),
                    backgroundColor: ['#0dcaf0', '#FF9800', '#198754', '#0d6efd', '#dc3545'],
                },
            ],
        }

        setBarData(newBarData)
    }, [key, data, label, labels])
    
    const barOptions = {
        responsive: true,
            plugins: {
            legend: {
                display: false,
                position: "top",
            },
        },
        scales: {
            y: {
                beginAtZero: true,
                suggestedMax: total, 
                ticks: {
                stepSize: 1,
                },
            },
        },
    }

    return barData ? <Bar data={barData} options={barOptions} /> : <div>Loading chart...</div>

}

export default BarChart