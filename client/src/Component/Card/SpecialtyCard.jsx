import React from 'react'
import { useNavigate } from "react-router-dom"

function SpecialtyCard({ specialty }) {
    const navigate = useNavigate()
    const truncateDescription = (description, wordLimit = 30) => {
        const words = description.split(' ')
        if (words.length > wordLimit) {
            return words.slice(0, wordLimit).join(' ') + '...'
        }
        return description
    }

    return (
        <div 
            style={{height: "400px", objectFit: "cover" }}
            className="specialty-card"
            onClick={() => navigate(`/chuyên khoa/${specialty.name}`)}
        >
            <img
                src={specialty.specialtyImage}
                alt={`Hình ảnh của ${specialty.name}`}
                style={{ width: "100%", height: "150px", objectFit: "cover" }}
            />
            <div style={{ padding: "10px" }}>
                <h5>{specialty.name}</h5>
                <p>{truncateDescription(specialty.description)}</p>
            </div>
        </div>
    )
}

export default SpecialtyCard