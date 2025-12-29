import React from 'react'

function SpecialtyLogo({ src }) {
    return (
        <span 
            className="logo d-inline-flex align-items-center justify-content-center rounded-circle text-white"
            style={{ width: 40, height: 40, zIndex: 1, backgroundColor: "#0646a3" }}
        >
            <img src={src} alt="icon" style={{ width: "60%", height: "60%" }} />
        </span> 
    )
}

export default SpecialtyLogo
