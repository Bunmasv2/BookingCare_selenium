import React from 'react'
import { Link } from 'react-router-dom'
import { formatDateToLocale } from '../../Util/DateUtils'

function Recomend({ item, type }) {
  const shortDescription = item?.description?.length > 80 ? item.description.slice(0, 80) + '...' : item?.description

  if (type === "specialty") {
    return (
      <Link to={`/chuyên khoa/${encodeURIComponent(item?.name)}`} className="text-decoration-none">
        <div
          className="d-flex flex-column gap-2 p-3 rounded border hover-shadow"
          style={{ minHeight: '150px' }}
        >
          <h6 className="text-primary fw-bold text-wrap mb-1" style={{ wordBreak: 'break-word' }}>
            {item?.name}
          </h6>
          <p className="small text-secondary mb-1" style={{ whiteSpace: 'pre-wrap', wordBreak: 'break-word' }}>
            {item?.name} {shortDescription}
          </p>
          <p className="small text-muted mb-0">
            Ngày thành lập: {formatDateToLocale(item?.createdAt)}
          </p>
        </div>
      </Link>
    )    
  }

  return (
    <Link to={`/dịch vụ/${encodeURIComponent(item?.serviceName)}`} className="text-decoration-none">
      <div
        className="d-flex flex-column gap-2 p-3 rounded border hover-shadow"
        style={{ minHeight: '150px' }}
      >
        <h6 className="text-primary fw-bold text-wrap mb-1" style={{ wordBreak: 'break-word' }}>
          {item?.serviceName}
        </h6>
        <p className="small text-muted mb-1">
          {item?.price ? `${item.price.toLocaleString()} VNĐ` : 'Giá liên hệ'}
        </p>
        <p className="small text-secondary mb-1" style={{ whiteSpace: 'pre-wrap', wordBreak: 'break-word' }}>
          {shortDescription}
        </p>
        <p className="small text-muted mb-0">
          Ngày tạo: {formatDateToLocale(item?.createdAt)}
        </p>
      </div>
    </Link>
  )
}

export default Recomend