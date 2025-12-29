import React, { useState, useEffect } from 'react'
import { Container, Table, Button, Spinner, Form, Row, Col } from 'react-bootstrap'
import axios from '../../../../../Util/AxiosConfig'
import List from '../General/List'
import Profile from '../General/Profile'

const AdminList = ({ tabActive }) => {
  const [admins, setAdmins] = useState()
  const [selectedAdmin, setSelectedAdmin] = useState()

  useEffect(() => {
    if (tabActive !== "admins") return

    const fetchAdmins = async () => {
        try {
            const response = await axios.get(`/users/${"admin"}`)
            console.log(response.data)
            setAdmins(response.data)
        } catch (error) {
            console.log(error)
        }
    }

    fetchAdmins()
  }, [tabActive])

  return (
    <Container fluid className='py-4'>
      {selectedAdmin ? (
        <Profile 
          userId={selectedAdmin?.userId}
        />
      ) : (
        <List users={admins} role={"admin"} setselected={setSelectedAdmin} />
      )}
    </Container>
  )
}

export default AdminList