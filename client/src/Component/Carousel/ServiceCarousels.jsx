import { useState } from "react"
import { Button } from "react-bootstrap"
import { ServiceCard } from "../Card/Index"

function ServiceCarousel({ services }) {
    const [activeIndex, setActiveIndex] = useState(0)

    if (!services || services.length === 0) {
        return <p>Không có dịch vụ nào để hiển thị.</p>
    }

    const totalServices = services.length

    const getLoopedIndex = (index) => {
        if (index < 0) return index + totalServices
        if (index >= totalServices) return index - totalServices
        return index
    }

    const handlePrev = () => {
        setActiveIndex((prev) => getLoopedIndex(prev - 1))
    }

    const handleNext = () => {
        setActiveIndex((prev) => getLoopedIndex(prev + 1))
    }

    // Lấy danh sách 4 dịch vụ theo chỉ mục vòng lặp
    const visibleServices = [
        services[getLoopedIndex(activeIndex)],
        services[getLoopedIndex(activeIndex + 1)],
        services[getLoopedIndex(activeIndex + 2)],
        services[getLoopedIndex(activeIndex + 3)],
    ]

    return (
        <div>
            {/* Carousel hiển thị nhóm dịch vụ */}
            <div style={{ display: "flex", justifyContent: "center", alignItems: "center", gap: "10px" }}>
                <Button variant="light" onClick={handlePrev}>
                    ❮
                </Button>

                <div style={{ display: "flex", gap: "10px" }}>
                    {visibleServices.map((service, index) => (
                        <ServiceCard key={index} service={service} />
                    ))}
                </div>

                <Button variant="light" onClick={handleNext}>
                    ❯
                </Button>
            </div>
        </div>
    )
}

export default ServiceCarousel