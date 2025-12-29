import { Card, Col, Container, Nav, Row, Tab } from 'react-bootstrap'
import DoctorServiceCard from '../../../../Component/Card/DoctorServiceCard'
import { useEffect, useState } from 'react'
import axios from '../../../../Util/AxiosConfig'
import Barchart from "../../../../Component/Chart/BarChart"
import BarChart from '../../../../Component/Chart/BarChart'
import ReviewDetailCard from '../../../../Component/Card/ReviewDetailCard'

function ReviewDetail({ specialty, service, review }) {
    const [reviews, setReviews] = useState()
    const [reviewRating, setReviewRating] = useState()
    const [monthlyRating, setMonthlyRating] = useState()
    const [total, setTotal] = useState()
    const [monthlyTolal, setMonthlyTotal] = useState()
    const [month, setMonth] = useState(new Date().getMonth() + 1)
    const [year, setYear] = useState(new Date().getFullYear())
    const [tabActive, setTabActive] = useState('all')

    useEffect(() => {
        const fetchReview = async () => {
            try {
                const response = await axios.get(`/reviews/detail/${tabActive}/${"service"}/${service?.serviceId}`)
                setReviews(response.data)
            } catch (error) {
                console.log(error)
            }
        }

        fetchReview()
    }, [tabActive, service])

    useEffect(() => {
        const fetchRatingReview = async () => {
            try {
                const response = await axios.get(`/reviews/rating/service/${service?.serviceId}`)
                setReviewRating(response.data)
            } catch (error) {
                console.log(error)
            }
        }

        fetchRatingReview()
    }, [service])

    useEffect(() => {
        const fetchMonthlyRatingReview = async () => {
            try {
                const response = await axios.get(`/reviews/rating/service/${month}/${year}/${service?.serviceId}`)
                setMonthlyRating(response.data)
            } catch (error) {
                console.log(error)
            }
        }

        fetchMonthlyRatingReview()
    }, [service, month, year])

    useEffect(() => {
        if (reviewRating === null) return
        setTotal(reviewRating?.reduce((sum, review) => sum + (review?.reviewCount || 0), 0))

        if (monthlyRating === null) return
        setMonthlyTotal(monthlyRating?.reduce((sum, review) => sum + (review?.reviewCount || 0, 0)))
    }, [reviewRating, monthlyRating])

    return (
        <Container className='mt-4 mb-5'>
            <DoctorServiceCard specialty={specialty} item={service} review={review} type={"dịch vụ"} />

            <Row className="g-4">
                <Col md={6} xs={12} >
                    <Card className="border-0 shadow-sm h-100">
                        <Card.Body className="p-4">
                            <div className="d-flex justify-content-between align-items-center mb-4 pb-1 flex-wrap">
                                <h5 className="mb-5 text-center w-100">Phân bố đánh giá tổng quát</h5>
                            </div>
                            <Barchart data={reviewRating} total={total} label={"Phân bố đánh giá tổng quát"} labels={["1⭐", "2⭐", "3⭐", "4⭐", "5⭐"]} />
                        </Card.Body>
                    </Card>
                </Col>

                <Col md={6} xs={12} >
                    <Card className="border-0 shadow-sm h-100">
                        <Card.Body className="p-4">
                            <div className="d-flex justify-content-between align-items-center mb-3 flex-wrap">
                                <h5 className="mb-2 text-center w-100">Phân bố đánh giá theo tháng</h5>
                                <div className="d-flex gap-2 justify-content-center w-100 mb-3">
                                    <select
                                        value={month}
                                        onChange={(e) => setMonth(Number(e.target.value))}
                                        className="form-select w-auto"
                                    >
                                        {[...Array(12)].map((_, idx) => (
                                        <option key={idx} value={idx + 1}>
                                            Tháng {idx + 1}
                                        </option>
                                        ))}
                                    </select>

                                    <select
                                        value={year}
                                        onChange={(e) => setYear(Number(e.target.value))}
                                        className="form-select w-auto"
                                    >
                                        {[...Array(5)].map((_, idx) => {
                                        const y = new Date().getFullYear() - idx
                                        return (
                                            <option key={y} value={y}>
                                            Năm {y}
                                            </option>
                                        )
                                        })}
                                    </select>
                                </div>
                            </div>
                        <BarChart data={monthlyRating} total={monthlyTolal} label={"Phaan d"} labels={["1⭐", "2⭐", "3⭐", "4⭐", "5⭐"]} />
                        </Card.Body>
                    </Card>
                </Col>
            </Row>

            <Card className="border-0 shadow-sm mb-4">
                <Card.Header className="bg-primary text-white py-3">
                    <h4 className="mb-0">Đánh giá từ bệnh nhân</h4>
                </Card.Header>

                <Card.Body>
                    <Tab.Container variant="tabs" className="mb-3" activeKey={tabActive} onSelect={(k) => setTabActive(k)} defaultActiveKey={"all"}>
                        <Nav className="py-0">
                            <Nav.Item>
                                <Nav.Link eventKey="all" className={`d-flex align-items-center ${tabActive === "all" ? "fw-bold" : "text-dark"}`}>
                                    Tất cả
                                </Nav.Link>
                            </Nav.Item>
                            <Nav.Item>
                                <Nav.Link eventKey="positive" className={`d-flex align-items-center ${tabActive === "positive" ? "fw-bold" : "text-dark"}`}>
                                    Tích cực
                                </Nav.Link>
                            </Nav.Item>
                            <Nav.Item>
                                <Nav.Link eventKey="negative" className={`d-flex align-items-center ${tabActive === "negative" ? "fw-bold" : "text-dark"}`}>
                                    Cần cải thiện
                                </Nav.Link>
                            </Nav.Item>
                        </Nav>

                        <Tab.Content>
                            <Tab.Pane eventKey="all">
                                <Card.Body>
                                    {reviews?.map((review, index) => (
                                        <ReviewDetailCard type={"service"} review={review} />
                                    ))}
                                </Card.Body>
                            </Tab.Pane>

                            <Tab.Pane eventKey="positive" >
                                <Card.Body>
                                    {reviews?.map((review, index) => (
                                        <ReviewDetailCard type={"service"} review={review} />
                                    ))}
                                </Card.Body>
                            </Tab.Pane>
                                
                            <Tab.Pane eventKey="negative" >
                                <Card.Body>
                                    {reviews?.map((review, index) => (
                                        <ReviewDetailCard type={"service"} review={review} />
                                    ))}
                                </Card.Body>
                        </Tab.Pane> 
                        </Tab.Content>
                    </Tab.Container>
                </Card.Body>
            </Card>
        </Container>
    )

}

export default ReviewDetail