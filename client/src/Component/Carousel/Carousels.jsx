import Carousel from 'react-bootstrap/Carousel'
import ExampleCarouselImage from './ExampleCarouselImage'

function UncontrolledExample({ list, role }) {
  const items = []

    for (let i = 0; i < list.length; i += 4) {
        items.push(
        <Carousel.Item key={i}>
            <div style={{ display: 'flex', justifyContent: 'center', gap: '10px' }}>
            {list.slice(i, i + 4).map((item, index) => (
                <ExampleCarouselImage key={index} item={item} role={role} />
            ))}
            </div>
        </Carousel.Item>
        )
    }

  return <Carousel>{items}</Carousel>
}

export default UncontrolledExample
