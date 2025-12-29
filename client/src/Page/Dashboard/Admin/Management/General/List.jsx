import React from 'react'
import { Table } from 'react-bootstrap'

function List({ users, role, setselected }) {
    const handleSelect = (user) => {
        if (role === "patient") {
            setselected({ patientId: user?.patientId, fullName: user?.fullName })
        } else {
            setselected({ userId: user?.userId })
        }
    }

    return (
        <div className="table-responsive">
            <Table bordered hover>
                <thead>
                      <tr>
                        <th>Tên</th>
                        <th>Năm sinh</th>
                        <th>Email</th>
                        <th>Địa Chỉ</th>
                        <th>Số điện thoại</th>
                      </tr>
                </thead>
                <tbody>
                      {users?.map(a => (
                        <tr key={a?.userId} onClick={() => handleSelect(a)} >
                            <td>{a?.fullName}</td>
                            <td>{a?.dateOfBirth}</td>
                            <td>{a?.email}</td>
                            <td>{a?.address}</td>
                            <td>{a?.phoneNumber}</td>
                        </tr>
                      ))}
                </tbody>
            </Table>
        </div>
    )
}

export default List