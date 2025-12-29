import React, { useState, useEffect } from 'react'
import { Container, Table, Button, Spinner, Form, Row, Col } from 'react-bootstrap'
import axios from '../../../../../Util/AxiosConfig'
import List from '../General/List'
import Profile from '../General/Profile'

const DoctorList = ({ tabActive }) => {
  const [doctors, setDoctors] = useState()
  const [selectedDoctor, setSelectedDoctor] = useState()

  useEffect(() => {
    if (tabActive !== "admins") return

    const fetchAdmins = async () => {
        try {
            const response = await axios.get(`/users/${"doctor"}`)
            console.log(response.data)
            setDoctors(response.data)
        } catch (error) {
            console.log(error)
        }
    }

    fetchAdmins()
  }, [tabActive])

  return (
    <Container fluid className='py-4'>
      {selectedDoctor ? (
        <Profile 
          userId={selectedDoctor?.userId}
        />
      ) : (
        <List users={doctors} role={"doctor"} setselected={setSelectedDoctor} />
      )}
    </Container>
  )
}

export default DoctorList