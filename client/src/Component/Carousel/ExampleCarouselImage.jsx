import React from 'react'
import ServiceCard from './ServiceCard'
import { Card } from "react-bootstrap"

const ExampleCarouselImage = ({ item, role }) => {

  if (role === 'doctor') {
    return (
      <Card.Img variant="top" src={item.doctorImage} alt={item.userName} className="mx-auto mt-3 card-img"/>
    )
  }

  return (
    <ServiceCard service={item} />
  )
}

export default ExampleCarouselImage
