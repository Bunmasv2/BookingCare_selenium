import axios from '../../../../Util/AxiosConfig'
import React, { useEffect, useState } from 'react'
import { Col, Row, Card } from 'react-bootstrap'
import PieChart from "../../../../Component/Chart/PieChart"
import BarChart from '../../../../Component/Chart/BarChart'
import { Chart as ChartJS, ArcElement, Tooltip, Legend, CategoryScale, LinearScale, BarElement } from 'chart.js'
import AppointmentAdmin from './AppointmentAdmin'

ChartJS.register(ArcElement, Tooltip, Legend, CategoryScale, LinearScale, BarElement)

function AppointmentStatistics({ tabActive }) {
  const [appointmentsPerMonth, setAppointmentsMonth] = useState([])
  const [appointmentsPerWeek, setAppointmentsPerWeek] = useState([])
  const [month, setMonth] = useState(new Date().getMonth() + 1)
  const [year, setYear] = useState(new Date().getFullYear())
  const [total, setTotal] = useState(0)
  const [prevTotal, setPrevTotal] = useState(20)
  const labels = ['Chờ xác nhận', 'Đã xác nhận', 'Đã khám', 'Đã hoàn thành', 'Đã hủy']
  const backgroundColor = ['#0dcaf0', '#FF9800', '#198754', '#0d6efd', '#dc3545']

  useEffect(() => {
    if (tabActive !== "appointments") return

    const fetchAppointmentsPerMonth = async () => {
      try {
        const response = await axios.get(`/appointments/statistics/${month}/${year}`)
        setAppointmentsMonth(response.data)
        setTotal(response.data.reduce((sum, item) => sum + item?.appointments, 0))
      } catch (error) {
        console.log(error)
      }
    }

    fetchAppointmentsPerMonth()
  }, [tabActive, month, year])

  useEffect(() => {
    if (tabActive !== "appointments") return

    let prevMonth 
    let preYear
    if (month === 1) {
      prevMonth = 12
      preYear = year - 1 
    } else {
      prevMonth = month - 1
      preYear = year
    }

    const fetchPrevAppointmentsPerMonth = async () => {
      try {
        const response = await axios.get(`/appointments/statistics/${prevMonth}/${preYear}`)
        setPrevTotal(response.data.reduce((sum, item) => sum + item?.appointments, 0))
      } catch (error) {
        console.log(error)
      }
    }

    fetchPrevAppointmentsPerMonth()
  }, [tabActive, month, year])

  useEffect(() => {
    if (tabActive !== "appointments") return
    
    const fetchAppointmentsPerWeek = async () => {
      try {
        const response = await axios.get(`/appointments/statistics/${month}/week`)
        setAppointmentsPerWeek(response.data)
      } catch (error) {
        console.log(error)
      }            
    }

    fetchAppointmentsPerWeek()
    }, [tabActive, month])

  const rateChange = prevTotal === 0 ? 0 : (((total - prevTotal) / prevTotal) * 100).toFixed(1)

  return (
    <div className='container py-4'>
      <Row className='mb-4'>
        <Col>
          <h4>Thống kê lịch hẹn tháng {month}</h4>
          <select value={month} onChange={(e) => setMonth(Number(e.target.value))}>
            {[...Array(12)].map((_, idx) => (
              <option key={idx} value={idx + 1}>
                Tháng {idx + 1}
              </option>
            ))}
          </select>

          <select value={year} onChange={(e) => setYear(Number(e.target.value))}>
            {[...Array(5)].map((_, idx) => {
              const y = new Date().getFullYear() - idx
              return (
                <option key={y} value={y}>
                  Năm {y}
                </option>
              )
            })}
          </select>
        </Col>
      </Row>

      <Row className='mb-4'>
        <Col md={4}>
          <Card className='p-3'>
            <h5>Tổng số lịch hẹn</h5>
            <h3>{total}</h3>
            <p className={rateChange >= 0 ? 'text-success' : 'text-danger'}>So với tháng trước: {rateChange}%</p>
          </Card>

          <Card className='px-3 my-3'>
            <ul style={{ listStyle: 'none', padding: 0, marginTop: '1rem' }}>
              {labels.map((label, idx) => (
                <li key={idx} style={{ marginBottom: '6px', display: 'flex', alignItems: 'center' }}>
                  <span
                    style={{
                      display: 'inline-block',
                      width: 12,
                      height: 12,
                      backgroundColor: backgroundColor[idx],
                      marginRight: 8,
                      borderRadius: '50%',
                    }}
                  ></span>
                  {label}: {appointmentsPerMonth[idx]?.appointments || 0}
                </li>
              ))}
            </ul>
          </Card>
        </Col>

        <Col md={4}>
          <Card className='p-3'>
            <h5 className='text-center mb-3'>Biểu đồ theo tháng</h5>
            <PieChart appointments={appointmentsPerMonth} />
          </Card>
        </Col>

        <Col md={4}>
          <Card className='p-3'>
            <h5 className='text-center mb-3'>Biểu đồ theo tuần</h5>
            <BarChart data={appointmentsPerWeek} total={total} label='Lịch hẹn theo tuần' labels={['Tuần 1', 'Tuần 2', 'Tuần 3', 'Tuần 4']} />
          </Card>
        </Col>
      </Row>

      <Row className='' >
        <AppointmentAdmin month={month} year={year} />
      </Row>
    </div>
  )
}

export default AppointmentStatistics