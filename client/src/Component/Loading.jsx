import React from 'react'

const Loading = ({ text = "Đang tải..." }) => {
  return (
    <div className="text-center my-5">
      <div className="spinner-border text-primary" role="status">
        <span className="visually-hidden">{text}</span>
      </div>
    </div>
  )
}

export default Loading
