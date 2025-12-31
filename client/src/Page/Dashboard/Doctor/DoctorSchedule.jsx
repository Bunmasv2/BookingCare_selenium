import React, { useEffect, useState } from "react"
import { Container, Card } from "react-bootstrap"
import { Calendar, momentLocalizer } from "react-big-calendar"
import moment from "moment"
import "moment/locale/vi"
import "react-big-calendar/lib/css/react-big-calendar.css"
import axios from "../../../Util/AxiosConfig"
import DoctorShiftDetail from "./DoctorShiftDetail"

moment.locale("vi")
const localizer = momentLocalizer(moment)

const DoctorSchedule = ({ tabActive }) => {
    const [schedules, setSchedules] = useState([])
    const [events, setEvents] = useState([])
    const [selectedDate, setSelectedDate] = useState(null)
    const [selectedShift, setSelectedShift] = useState(null)

    const syncStateFromHash = () => {
        const params = new URLSearchParams(
            window.location.hash.split("?")[1]
        )

        setSelectedDate(params.get("date"))
        setSelectedShift(params.get("shift"))
    }

    useEffect(() => {
        syncStateFromHash()
        window.addEventListener("hashchange", syncStateFromHash)

        return () =>
            window.removeEventListener("hashchange", syncStateFromHash)
    }, [])

    useEffect(() => {
        if (tabActive !== "doctorSchedule") return

        const fetchDoctorSchedule = async () => {
            try {
                const res = await axios.get("/appointments/schedule")
                setSchedules(res.data || [])
            } catch (err) {
                console.error(err)
            }
        }

        fetchDoctorSchedule()
    }, [tabActive])

    useEffect(() => {
        if (!schedules.length) return

        const formatted = schedules.map((group, index) => {
            const date = new Date(group.date)
            const start = new Date(date)
            const end = new Date(date)

            if (group.appointmentTime === "Sáng") {
                start.setHours(7, 0, 0)
                end.setHours(11, 0, 0)
            } else {
                start.setHours(13, 0, 0)
                end.setHours(17, 0, 0)
            }

            return {
                id: index,
                title:
                    group.appointmentTime === "Sáng"
                        ? `Sáng ${group.patientCount} bệnh nhân`
                        : `Chiều ${group.patientCount} bệnh nhân`,
                start,
                end,
                time: group.appointmentTime,
                testDate: moment(start).format("YYYY-MM-DD"),
            }
        })

        setEvents(formatted)
    }, [schedules])

    const eventStyleGetter = (event) => {
        let backgroundColor =
            event.time === "Sáng" ? "#27ae60" : "#f39c12"

        if (event.end < new Date()) backgroundColor = "#e74c3c"

        return {
            style: {
                backgroundColor,
                color: "white",
                borderRadius: "5px",
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
            },
        }
    }

    const handleSelectEvent = (event) => {
        const date = moment(event.start).format("YYYY-MM-DD")
        const shift = event.time === "Sáng" ? "Sáng" : "Chiều"

        window.location.hash = `#appointments?date=${date}&shift=${shift}`
    }

    const handleBack = () => {
        window.location.hash = "#appointments"
    }

    if (selectedDate && selectedShift) {
        return (
            <DoctorShiftDetail onBack={handleBack} />
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
                        views={["month", "week", "day"]}
                        selectable
                        onSelectEvent={handleSelectEvent}
                        eventPropGetter={eventStyleGetter}
                        components={{
                            event: ({ event }) => (
                                <div
                                    data-testid={`shift-${event.testDate}-${event.time}`}
                                    style={{ width: "100%", height: "100%" }}
                                >
                                    {event.title}
                                </div>
                            )
                        }}
                    />
                </div>
            </Card>
        </Container>
    )
}

export default DoctorSchedule
