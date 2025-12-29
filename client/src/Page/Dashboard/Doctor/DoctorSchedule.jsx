import React, { useEffect, useState } from "react"
import { Container, Card } from "react-bootstrap"
import { Calendar, momentLocalizer } from "react-big-calendar"
import moment from "moment"
import "moment/locale/vi" // Thêm locale tiếng Việt
import "react-big-calendar/lib/css/react-big-calendar.css"
import axios from "../../../Util/AxiosConfig"
import DoctorShiftDetail from "./DoctorShiftDetail"

moment.locale("vi") // Đặt ngôn ngữ là tiếng Việt
const localizer = momentLocalizer(moment)

const DoctorSchedule = ({ tabActive }) => {
  const [events, setEvents] = useState([])
  const [schedules, setSchedules] = useState()
  const [showShiftDetail, setShowShiftDetail] = useState(false)
  const [dateTime, setDateTime] = useState({
    date: null,
    time: null
  })

  useEffect(() => {
    if (tabActive !== "doctorSchedule") return

    const fetchDoctorSchedule = async () => {
      try {
        const response = await axios.get("/appointments/schedule")
        console.log(response.data)
        setSchedules(response.data)
      } catch (error) {
        console.log(error)
      }
    }

    fetchDoctorSchedule()
  }, [tabActive])

  useEffect(() => {
    const formatSchedule = () => {
      if (!schedules || schedules.length === 0) return

      const schedulesTmp = []

      schedules.forEach((group, groupIndex) => {
        const date = new Date(group.date)
        const time = group.appointmentTime

        const start = new Date(date)
        const end = new Date(date)

        if (time === "Sáng") {
          start.setHours(7, 0, 0)
          end.setHours(11, 0, 0)
        } else {
          start.setHours(13, 0, 0)
          end.setHours(17, 0, 0)
        }

        schedulesTmp.push({
          id: groupIndex,
          title: time === "Sáng" ? `Sáng ${group.patientCount} bệnh nhân` : `Chiều ${group.patientCount} bệnh nhân`,
          start,
          end,
          time,
        })
      })

      setEvents(schedulesTmp)
    }

    formatSchedule()
  }, [schedules])

  const eventStyleGetter = (event) => {
    let backgroundColor = "#3174ad"
    let color = "white"

    if (event.time === "Sáng") {
      backgroundColor = "#27ae60"
    } else {
      backgroundColor = "#f39c12"
    }

    if (event.end < new Date()) {
      backgroundColor = "#e74c3c"
    }

    const style = {
      backgroundColor,
      color,
      borderRadius: "5px",
      display: "flex",
      justifyContent: "center",
      alignItems: "center",
      padding: "5px",
    }

    return { style }
  }

  if (showShiftDetail) {
    return (
      <DoctorShiftDetail dateTime={dateTime} setShowShiftDetail={setShowShiftDetail} />
    )
  }

  return (
    <Container className="p-0">
      <Card className="border-0 w-100 mx-auto p-0">
        <div style={{ height: "600px" }}>
          <Calendar
            localizer={localizer}
            events={events}
            startAccessor="start"
            endAccessor="end"
            style={{ height: "100%" }}
            views={["month", "week", "day"]}
            selectable
            onSelectEvent={(event) => {
              const selectedDate = event.start
              const formattedDate = selectedDate.toLocaleDateString("vi-VN")

              setShowShiftDetail(true)
              setDateTime({
                date: formattedDate,
                time: event.time
              })
            }}
            eventPropGetter={eventStyleGetter}
            messages={{
              next: "Tiếp",
              previous: "Trước",
              today: "Hôm nay",
              month: "Tháng",
              week: "Tuần",
              day: "Ngày",
              agenda: "Lịch biểu",
              date: "Ngày",
              time: "Giờ",
              event: "Sự kiện",
              noEventsInRange: "Không có lịch trình trong khoảng thời gian này.",
              showMore: (total) => `+${total} thêm`
            }}
          />
        </div>
      </Card>
    </Container>
  )
}

export default DoctorSchedule