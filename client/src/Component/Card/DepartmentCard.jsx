import Card from 'react-bootstrap/Card'

function DepartmentCard({ image, title, description }) {
  return (
    <Card style={{ width: '16rem' }} className="shadow rounded-4">
      <Card.Img variant="top" src={image} style={{ height: '200px', objectFit: 'cover' }} />
      <Card.Body>
        <Card.Title>{title}</Card.Title>
        <Card.Text>{description}</Card.Text>
      </Card.Body>
    </Card>
  )
}

export default DepartmentCard
