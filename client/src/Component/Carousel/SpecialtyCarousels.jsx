import { useState } from "react"
import { Button } from "react-bootstrap"
import { SpecialtyCard } from "../Card/Index"

function SpecialtyCarousel({ specialties }) {
    const [activeIndex, setActiveIndex] = useState(0)

    if (!specialties || specialties.length === 0) {
        return <p>Không có chuyên khoa nào để hiển thị.</p>
    }

    const totalSpecialties = specialties.length

    // Xử lý chỉ số theo cơ chế vòng lặp vô tận
    const getLoopedIndex = (index) => {
        if (index < 0) return index + totalSpecialties;
        if (index >= totalSpecialties) return index - totalSpecialties
        return index
    }

    const handlePrev = () => {
        setActiveIndex((prev) => getLoopedIndex(prev - 1))
    }

    const handleNext = () => {
        setActiveIndex((prev) => getLoopedIndex(prev + 1))
    }

    // Lấy danh sách 4 chuyên khoa theo chỉ mục vòng lặp
    const visibleSpecialties = [
        specialties[getLoopedIndex(activeIndex)],
        specialties[getLoopedIndex(activeIndex + 1)],
        specialties[getLoopedIndex(activeIndex + 2)],
        specialties[getLoopedIndex(activeIndex + 3)],
    ]

    return (
        <div>
            {/* Carousel hiển thị nhóm chuyên khoa */}
            <div style={{ display: "flex", justifyContent: "center", alignItems: "center", gap: "10px" }}>
                <Button variant="light" onClick={handlePrev}>
                    ❮
                </Button>

                <div style={{ display: "flex", gap: "10px" }}>
                    {visibleSpecialties.map((specialty, index) => (
                        <SpecialtyCard key={index} specialty={specialty} />
                    ))}
                </div>

                <Button variant="light" onClick={handleNext}>
                    ❯
                </Button>
            </div>
        </div>
    )
}

export default SpecialtyCarousel