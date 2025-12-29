import React, { useState, useEffect } from 'react'
import { Modal, Button, Form } from 'react-bootstrap'
import axios from '../../../../../Util/AxiosConfig'

function formatCurrency(value) {
  if (!value) return ''
  return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, '.')
}

const CashPaymentModal = ({ show, handleClose, record, onPaymentSuccess }) => {
  const [totalAmount, setTotalAmount] = useState(0)
  const [customerCash, setCustomerCash] = useState('')
  const [changeCash, setChangeCash] = useState('0')
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    if (show && record?.recordId) {
      fetchTotalAmount(record.recordId)
    }
  }, [show, record])

  const fetchTotalAmount = async (recordId) => {
    try {
      const res = await axios.get(`/medicalrecords/total-cash${recordId}`)
      setTotalAmount(res.data)
      setCustomerCash('')
      setChangeCash('0')
    } catch (err) {
      console.error('Lỗi khi lấy tổng tiền:', err)
      alert('Không lấy được số tiền cần thanh toán.')
    }
  }

  const handleCustomerCashChange = (e) => {
    const rawValue = e.target.value.replace(/[^0-9]/g, '')
    setCustomerCash(formatCurrency(rawValue))

    const cashGiven = parseInt(rawValue || '0', 10)
    const balance = cashGiven - totalAmount
    setChangeCash(balance > 0 ? formatCurrency(balance) : '0')
  }

  const handleConfirmPayment = async () => {
    const cashGiven = parseInt(customerCash.replace(/\./g, ''), 10) || 0
    const cashBalance = cashGiven - totalAmount

    if (cashGiven < totalAmount) { 
      alert('Tiền khách đưa chưa đủ.')
      return
    }

    const payload = {
      recordId: record.recordId,
      amount: cashGiven,          
      cashBalance: cashBalance     
    }

    try {
      setLoading(true)
      const res = await axios.post('/medicalrecords/pay-by-cash', payload)
      if (res.data === 'success') {
        alert('Thanh toán thành công!')
        onPaymentSuccess()
      } else {
        alert('Thanh toán thất bại.')
      }
    } catch (err) {
      console.error(err)
      const message =
        err.response?.data?.message || 'Đã xảy ra lỗi khi thanh toán.'
      alert(message)
    } finally {
      setLoading(false)
    }
  }

  return (
    <Modal show={show} onHide={handleClose} centered>
      <Modal.Header closeButton>
        <Modal.Title>Thanh toán tiền mặt</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Form.Group className="mb-3">
          <Form.Label>Số tiền phải thanh toán</Form.Label>
          <Form.Control
            type="text"
            value={formatCurrency(totalAmount)}
            disabled
            readOnly
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Tiền khách đưa</Form.Label>
          <Form.Control
            type="text"
            value={customerCash}
            onChange={handleCustomerCashChange}
            placeholder="Nhập số tiền khách đưa"
          />
        </Form.Group>

        <Form.Group>
          <Form.Label>Tiền thối lại</Form.Label>
          <Form.Control
            type="text"
            value={changeCash}
            disabled
            readOnly
          />
        </Form.Group>
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={handleClose} disabled={loading}>
          Huỷ
        </Button>
        <Button
          variant="primary"
          onClick={handleConfirmPayment}
          disabled={loading}
        >
          {loading ? 'Đang xử lý...' : 'Xác nhận thanh toán'}
        </Button>
      </Modal.Footer>
    </Modal>
  )
}

export default CashPaymentModal
