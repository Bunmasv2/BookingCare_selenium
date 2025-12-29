import axios from '../../../../../Util/AxiosConfig'
import React, { useEffect, useState } from 'react'
import { Card, Table } from 'react-bootstrap'

function formatLabel(key) {
    return key
        .replace(/([A-Z])/g, ' $1')
        .replace(/_/g, ' ')
        .replace(/^./, str => str.toUpperCase())
}

function Profile({ userId }) {
    const [keys, setKeys] = useState([])
    const [user, setUser] = useState()

    useEffect(() => {
        const fetchUser = async () => {
            try {
                console.log(userId)
                const response = await axios.get(`/users/detail/${userId}`)
                console.log(response.data)
                setUser(response.data)
            } catch (error) {
                console.log(error)        
            }
        }

        fetchUser()
    }, [userId])

    useEffect(() => {
        if (!user) return

        const fields = Object.keys(user)
        const filteredFields = fields.filter(
            key =>
                key !== 'doctorImage' &&
                key !== 'password' &&    
                !key.toLowerCase().includes('image') &&
                !key.endsWith("Id")
        )

        setKeys(filteredFields)
    }, [user])


    return (
        <Card className="p-4 shadow-sm">
            <h3 className="mb-4">Thông tin người dùng</h3>
            <div className="table-responsive">
                <Table bordered>
                    <tbody>
                        {keys.map((key, index) => (
                            <tr key={index}>
                                <th style={{ width: '30%' }}>{formatLabel(key)}</th>
                                <td>{user[key]}</td>
                            </tr>
                        ))}
                    </tbody>
                </Table>
            </div>
        </Card>
    )
}

export default Profile