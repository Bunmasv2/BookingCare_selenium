import React, { useState, useEffect, useRef } from 'react'
import { Button, Form, Table, Modal, Container, Row, Col, Card, Badge, InputGroup, FormControl, Spinner, Alert, Pagination, Image} from 'react-bootstrap'
import { Search, PlusCircle, Pencil, Trash, XCircle, ArrowClockwise, Upload, Image as ImageIcon, XSquare} from 'react-bootstrap-icons'
import axios from '../../../../Util/AxiosConfig'

function ServiceAdmin({ tabActive }) {
    const [services, setServices] = useState([])
    const [filteredServices, setFilteredServices] = useState([])
    const [showModal, setShowModal] = useState(false)
    const [editing, setEditing] = useState(false)
    const [formData, setFormData] = useState({ name: '', description: '' })
    const [selectedId, setSelectedId] = useState(null)
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)
    const [searchTerm, setSearchTerm] = useState('')
    const [confirmDelete, setConfirmDelete] = useState({ show: false, id: null })
    const [uploadStatus, setUploadStatus] = useState('')
    const [previewImage, setPreviewImage] = useState(null)
    const [imageFile, setImageFile] = useState(null)
    const fileInputRef = useRef(null)
    
    // Thêm state cho icon
    const [previewIcon, setPreviewIcon] = useState(null)
    const [iconFile, setIconFile] = useState(null)
    const iconInputRef = useRef(null)
    const [iconUploadStatus, setIconUploadStatus] = useState('')
    
    const [currentPage, setCurrentPage] = useState(1)
    const [itemsPerPage] = useState(5)

    useEffect(() => {
        if (tabActive !== "services") return
        fetchServices()
    }, [tabActive])

    useEffect(() => {
        const results = services.filter(service => 
            service.serviceName.toLowerCase().includes(searchTerm.toLowerCase()) ||
            (service.description && service.description.toLowerCase().includes(searchTerm.toLowerCase()))
        )
        setFilteredServices(results)
        setCurrentPage(1)
    }, [searchTerm, services])

    const indexOfLastItem = currentPage * itemsPerPage
    const indexOfFirstItem = indexOfLastItem - itemsPerPage
    const currentServices = filteredServices.slice(indexOfFirstItem, indexOfLastItem)

    const fetchServices = async () => {
        if (tabActive !== "services") return

        setLoading(true)
        setError(null)

        try {
            const res = await axios.get('/services')
            setServices(res.data)
            setFilteredServices(res.data)
        } catch (err) {
            console.error("Failed to fetch services:", err)
            setError("Không thể tải dữ liệu dịch vụ. Vui lòng thử lại sau.");
        } finally {
            setLoading(false)
        }
    }

    const handleShowCreate = () => {
        setFormData({ serviceName: '', description: '', price: '' })
        setEditing(false)
        setShowModal(true)
        setPreviewImage(null)
        setImageFile(null)
        setUploadStatus('')
        setPreviewIcon(null)
        setIconFile(null)
        setIconUploadStatus('')
    }

    const handleEdit = (service) => {
        setFormData({ 
            serviceName: service.serviceName || '', 
            description: service.description || '',
            price: service.price
        })
        setSelectedId(service.serviceId)
        setEditing(true)
        setShowModal(true)
        setPreviewImage(null)
        setImageFile(null)
        setUploadStatus('')
        setPreviewIcon(null)
        setIconFile(null)
        setIconUploadStatus('')
        
        if (service.serviceImage) {
            const imageUrl = service.serviceImage
            setPreviewImage(imageUrl)
        }
        
        if (service.serviceIcon) {
            const iconUrl = service.serviceIcon
            setPreviewIcon(iconUrl)
        }
    }

    const handleShowDeleteConfirm = (id) => {
        setConfirmDelete({ show: true, id })
    }

    const handleDeleteConfirm = async () => {
        if (tabActive !== "services") return

        try {
            await axios.delete(`/services/${confirmDelete.id}`)
            fetchServices()
            setConfirmDelete({ show: false, id: null })
        } catch (err) {
            console.error("Failed to delete service:", err)
            setError("Không thể xóa dịch vụ. Vui lòng thử lại sau.")
        }
    }

    const handleSubmit = async (e) => {
        if (tabActive !== "services") return
        
        e.preventDefault()
        setLoading(true)

        try {
            if (editing) {
                await axios.put(`/services/${selectedId}`, formData)
            } else {
                await axios.post("/services", formData)
            }
            
            // Xử lý upload ảnh và icon khi đã lưu dịch vụ thành công
            let serviceId = selectedId;
            
            if (!editing) {
                const res = await axios.get('/services')
                const newService = res.data.find(s => s.serviceName === formData.serviceName)
                if (newService) {
                    serviceId = newService.serviceId
                }
            }
            
            // Upload ảnh nếu có
            if (imageFile && serviceId) {
                await handleImageUpload(serviceId)
            }
            
            // Upload icon nếu có
            if (iconFile && serviceId) {
                await handleIconUpload(serviceId)
            }
        
            setShowModal(false)
            fetchServices()
        } catch (err) {
            console.error("Failed to save service:", err)
            setError(editing 
                ? "Không thể cập nhật dịch vụ. Vui lòng thử lại sau."
                : "Không thể thêm dịch vụ mới. Vui lòng thử lại sau."
            )
        } finally {
            setLoading(false)
        }
    }

    const handleImageChange = (e) => {
        if (e.target.files && e.target.files[0]) {
            const file = e.target.files[0]
            setImageFile(file)
            
            const reader = new FileReader()
            reader.onload = (e) => {
                setPreviewImage(e.target.result)
            }
            reader.readAsDataURL(file)
        }
    }

    // Xử lý thay đổi icon
    const handleIconChange = (e) => {
        if (e.target.files && e.target.files[0]) {
            const file = e.target.files[0]
            setIconFile(file)
            
            const reader = new FileReader()
            reader.onload = (e) => {
                setPreviewIcon(e.target.result)
            }
            reader.readAsDataURL(file)
        }
    }

    const handleClearImage = () => {
        setPreviewImage(null)
        setImageFile(null)
        if (fileInputRef.current) {
            fileInputRef.current.value = ""
        }
    }
    
    // Xử lý xóa icon
    const handleClearIcon = () => {
        setPreviewIcon(null)
        setIconFile(null)
        if (iconInputRef.current) {
            iconInputRef.current.value = ""
        }
    }
    
    const handleImageUpload = async (serviceId) => {
        if (tabActive !== "services") return
        if (!imageFile) return
        
        setUploadStatus('uploading')
        const formData = new FormData()
        formData.append('file', imageFile)
        formData.append('serviceId', serviceId)
        
        try {
            await axios.post('/services/upload', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            })
            setUploadStatus('success')
        } catch (error) {
            console.error('Error uploading image:', error)
            setUploadStatus('error')
        }
    }
    
    // Xử lý upload icon
    const handleIconUpload = async (serviceId) => {
        if (tabActive !== "services") return
        if (!iconFile) return
        
        setIconUploadStatus('uploading')
        const formData = new FormData()
        formData.append('file', iconFile)
        formData.append('serviceId', serviceId)
        
        try {
            await axios.post('/services/upload-icon', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            })
            setIconUploadStatus('success')
        } catch (error) {
            console.error('Error uploading icon:', error)
            setIconUploadStatus('error')
        }
    }
    
    const paginate = (pageNumber) => setCurrentPage(pageNumber)
    const totalPages = Math.ceil(filteredServices.length / itemsPerPage)

    const renderPagination = () => {
        if (totalPages <= 1) return null

        let items = [];
        for (let number = 1; number <= totalPages; number++) {
            items.push(
                <Pagination.Item 
                    key={number} 
                    active={number === currentPage}
                    onClick={() => paginate(number)}
                >
                    {number}
                </Pagination.Item>
            )
        }

        return (
            <div className="d-flex justify-content-center mt-4">
                <Pagination>
                    <Pagination.Prev 
                        onClick={() => setCurrentPage(prev => Math.max(prev - 1, 1))}
                        disabled={currentPage === 1}
                    />
                    {items}
                    <Pagination.Next 
                        onClick={() => setCurrentPage(prev => Math.min(prev + 1, totalPages))}
                        disabled={currentPage === totalPages}
                    />
                </Pagination>
            </div>
        )
    }

    const renderServiceImage = (service) => {
        if (service.serviceImage) {
            return (
                <Image 
                    src={service.serviceImage}
                    alt={service.serviceName}
                    thumbnail
                    style={{ height: '50px', width: '80px', objectFit: 'cover' }}
                />
            )
        }
        
        return (
            <div 
                className="d-flex align-items-center justify-content-center bg-light border" 
                style={{ height: '50px', width: '80px' }}
            >
                <ImageIcon size={25} className="text-muted" />
            </div>
        )
    }
    
    // Render icon của dịch vụ trong bảng
    const renderServiceIcon = (service) => {
        if (service.serviceIcon) {
            return (
                <Image 
                    src={service.serviceIcon}
                    alt={`Icon ${service.serviceName}`}
                    thumbnail
                    style={{ height: '40px', width: '40px', objectFit: 'contain' }}
                />
            )
        }
        
        return (
            <div 
                className="d-flex align-items-center justify-content-center bg-light border rounded-circle" 
                style={{ height: '40px', width: '40px' }}
            >
                <ImageIcon size={16} className="text-muted" />
            </div>
        )
    }

    return (
        <Container fluid className="py-4">
            <Card className="shadow-sm mb-4">
                <Card.Body>
                    <Row className="align-items-center">
                        <Col xs={12} md={6} className="mb-3 mb-md-0">
                            <h4 className="mb-0">
                                <Badge bg="primary" className="me-2">
                                    {filteredServices.length}
                                </Badge>
                                Quản lý dịch vụ
                            </h4>
                        </Col>
                        <Col xs={12} md={6}>
                            <div className="d-flex gap-2 justify-content-md-end">
                                <Button 
                                    variant="outline-secondary" 
                                    onClick={fetchServices}
                                    disabled={loading}
                                    title="Làm mới dữ liệu"
                                >
                                    <ArrowClockwise />
                                </Button>
                                <Button 
                                    variant="primary" 
                                    onClick={handleShowCreate}
                                    disabled={loading}
                                    className="d-flex align-items-center gap-1"
                                >
                                    <PlusCircle /> Thêm dịch vụ
                                </Button>
                            </div>
                        </Col>
                    </Row>
                </Card.Body>
            </Card>

            <Card className="shadow-sm mb-4">
                <Card.Body>
                    <Row>
                        <Col>
                            <InputGroup>
                                <InputGroup.Text>
                                    <Search />
                                </InputGroup.Text>
                                <FormControl
                                    placeholder="Tìm kiếm dịch vụ..."
                                    value={searchTerm}
                                    onChange={(e) => setSearchTerm(e.target.value)}
                                    spellCheck={false}
                                />
                                {searchTerm && (
                                    <Button 
                                        variant="outline-secondary" 
                                        onClick={() => setSearchTerm('')}
                                    >
                                        <XCircle />
                                    </Button>
                                )}
                            </InputGroup>
                        </Col>
                    </Row>
                </Card.Body>
            </Card>

            {error && (
                <Alert variant="danger" onClose={() => setError(null)} dismissible>
                    {error}
                </Alert>
            )}

            <Card className="shadow-sm">
                <Card.Body>
                    {loading ? (
                        <div className="text-center py-5">
                            <Spinner animation="border" variant="primary" />
                            <p className="mt-2">Đang tải dữ liệu...</p>
                        </div>
                    ) : filteredServices.length === 0 ? (
                        <div className="text-center py-5">
                            <p className="text-muted">
                                {searchTerm 
                                    ? "Không tìm thấy dịch vụ nào phù hợp" 
                                    : "Chưa có dịch vụ nào trong hệ thống"}
                            </p>
                            <Button 
                                variant="primary" 
                                onClick={handleShowCreate}
                                className="mt-2"
                            >
                                <PlusCircle className="me-1" /> Thêm dịch vụ mới
                            </Button>
                        </div>
                    ) : (
                        <>
                            <Table responsive hover className="align-middle mb-0">
                                <thead className="table-light">
                                    <tr>
                                        <th style={{ width: '80px' }} className="text-center">ID</th>
                                        <th style={{ width: '100px' }} className="text-center">Hình ảnh</th>
                                        <th style={{ width: '60px' }} className="text-center">Icon</th>
                                        <th>Tên dịch vụ</th>
                                        <th>Mô tả</th>
                                        <th>Giá</th>
                                        <th style={{ width: '150px' }} className="text-center">Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {currentServices.map((service) => (
                                        <tr key={service.serviceId}>
                                            <td className="text-center">
                                                <Badge bg="secondary">
                                                    {service.serviceId}
                                                </Badge>
                                            </td>
                                            <td className="text-center">
                                                {renderServiceImage(service)}
                                            </td>
                                            <td className="text-center">
                                                {renderServiceIcon(service)}
                                            </td>
                                            <td className="fw-medium">{service.serviceName}</td>
                                            <td>
                                                {service.description 
                                                    ? service.description 
                                                    : <span className="text-muted fst-italic">Chưa có mô tả</span>}
                                            </td>
                                            <td className="fw-medium">{service.price}</td>
                                            <td>
                                                <div className="d-flex gap-2 justify-content-center">
                                                    <Button 
                                                        variant="outline-warning" 
                                                        size="sm" 
                                                        onClick={() => handleEdit(service)}
                                                        title="Sửa"
                                                    >
                                                        <Pencil />
                                                    </Button>
                                                    <Button 
                                                        variant="outline-danger" 
                                                        size="sm" 
                                                        onClick={() => handleShowDeleteConfirm(service.serviceId)}
                                                        title="Xóa"
                                                    >
                                                        <Trash />
                                                    </Button>
                                                </div>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </Table>
                            {renderPagination()}
                        </>
                    )}
                </Card.Body>
            </Card>

            <Modal 
                show={showModal} 
                onHide={() => setShowModal(false)} 
                centered
                backdrop="static"
                size="lg"
            >
                <Modal.Header closeButton>
                    <Modal.Title>
                        {editing ? "Cập nhật dịch vụ" : "Thêm dịch vụ mới"}
                    </Modal.Title>
                </Modal.Header>
                <Form onSubmit={handleSubmit}>
                    <Modal.Body>
                        <Row>
                            <Col md={8}>
                                <Form.Group className="mb-3">
                                    <Form.Label>Tên dịch vụ <span className="text-danger">*</span></Form.Label>
                                    <Form.Control
                                        type="text"
                                        required
                                        placeholder="Nhập tên dịch vụ"
                                        value={formData.serviceName}
                                        onChange={(e) => setFormData({ ...formData, serviceName: e.target.value })}
                                    />
                                </Form.Group>
                                <Form.Group className="mb-3">
                                    <Form.Label>Mô tả</Form.Label>
                                    <Form.Control
                                        as="textarea"
                                        rows={5}
                                        placeholder="Nhập mô tả chi tiết về dịch vụ (không bắt buộc)"
                                        value={formData.description || ''}
                                        onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                                    />
                                </Form.Group>
                                <Form.Group className="mb-3">
                                    <Form.Label>Giá</Form.Label>
                                    <Form.Control
                                        type="text"
                                        placeholder="Nhập giá dịch vụ (không bắt buộc)"
                                        value={formData.price || ''}
                                        onChange={(e) => setFormData({ ...formData, price: e.target.value })}
                                    />
                                </Form.Group>
                            </Col>
                            <Col md={4}>
                                {/* Upload hình ảnh */}
                                <Form.Group className="mb-3">
                                    <Form.Label>Hình ảnh dịch vụ</Form.Label>
                                    <div className="mb-3">
                                        {previewImage ? (
                                            <div className="position-relative mb-3">
                                                <Image 
                                                    src={previewImage} 
                                                    alt="Preview" 
                                                    fluid 
                                                    thumbnail
                                                    style={{ height: '150px', width: '100%', objectFit: 'cover' }}
                                                />
                                                <Button 
                                                    variant="light" 
                                                    size="sm" 
                                                    className="position-absolute top-0 end-0"
                                                    onClick={handleClearImage}
                                                    title="Xóa hình ảnh"
                                                >
                                                    <XSquare />
                                                </Button>
                                            </div>
                                        ) : (
                                            <div 
                                                className="d-flex flex-column align-items-center justify-content-center bg-light border rounded p-4 mb-3"
                                                style={{ height: '150px' }}
                                            >
                                                <ImageIcon size={40} className="text-muted mb-2" />
                                                <p className="text-muted small text-center mb-0">
                                                    Chưa có hình ảnh
                                                </p>
                                            </div>
                                        )}
                                        
                                        <div className="d-grid">
                                            <Button 
                                                variant="outline-primary" 
                                                onClick={() => fileInputRef.current?.click()}
                                                className="d-flex align-items-center justify-content-center gap-1"
                                            >
                                                <Upload /> Chọn hình ảnh
                                            </Button>
                                            <Form.Control
                                                ref={fileInputRef}
                                                type="file"
                                                accept="image/*"
                                                onChange={handleImageChange}
                                                className="d-none"
                                            />
                                        </div>
                                        
                                        <div className="mt-2 mb-3">
                                            <small className="text-muted">
                                                Định dạng hỗ trợ: JPG, PNG, GIF.
                                                <br />Kích thước tối đa: 2MB.
                                            </small>
                                        </div>
                                        
                                        {uploadStatus === 'uploading' && (
                                            <div className="mt-2 text-primary">
                                                <Spinner as="span" animation="border" size="sm" /> Đang tải lên...
                                            </div>
                                        )}
                                        {uploadStatus === 'success' && (
                                            <div className="mt-2 text-success">
                                                Tải lên thành công!
                                            </div>
                                        )}
                                        {uploadStatus === 'error' && (
                                            <div className="mt-2 text-danger">
                                                Lỗi khi tải lên. Vui lòng thử lại.
                                            </div>
                                        )}
                                    </div>
                                </Form.Group>
                                
                                {/* Phần upload icon */}
                                <Form.Group className="mb-3">
                                    <Form.Label>Icon dịch vụ</Form.Label>
                                    <div className="mb-3">
                                        {previewIcon ? (
                                            <div className="position-relative mb-3 d-flex justify-content-center">
                                                <Image 
                                                    src={previewIcon} 
                                                    alt="Icon Preview" 
                                                    thumbnail
                                                    style={{ height: '80px', width: '80px', objectFit: 'contain' }}
                                                />
                                                <Button 
                                                    variant="light" 
                                                    size="sm" 
                                                    className="position-absolute top-0 end-0"
                                                    onClick={handleClearIcon}
                                                    title="Xóa icon"
                                                >
                                                    <XSquare />
                                                </Button>
                                            </div>
                                        ) : (
                                            <div 
                                                className="d-flex flex-column align-items-center justify-content-center bg-light border rounded-circle p-2 mb-3 mx-auto"
                                                style={{ height: '80px', width: '80px' }}
                                            >
                                                <ImageIcon size={30} className="text-muted" />
                                                <p className="text-muted small text-center mb-0" style={{ fontSize: '0.7rem' }}>
                                                    Chưa có icon
                                                </p>
                                            </div>
                                        )}
                                        
                                        <div className="d-grid">
                                            <Button 
                                                variant="outline-primary" 
                                                onClick={() => iconInputRef.current?.click()}
                                                className="d-flex align-items-center justify-content-center gap-1"
                                            >
                                                <Upload /> Chọn icon
                                            </Button>
                                            <Form.Control
                                                ref={iconInputRef}
                                                type="file"
                                                accept="image/*"
                                                onChange={handleIconChange}
                                                className="d-none"
                                            />
                                        </div>
                                        
                                        <div className="mt-2">
                                            <small className="text-muted">
                                                Nên sử dụng icon vuông hoặc tròn.
                                                <br />Định dạng: SVG, PNG (khuyên dùng).
                                            </small>
                                        </div>
                                        
                                        {iconUploadStatus === 'uploading' && (
                                            <div className="mt-2 text-primary">
                                                <Spinner as="span" animation="border" size="sm" /> Đang tải icon lên...
                                            </div>
                                        )}
                                        {iconUploadStatus === 'success' && (
                                            <div className="mt-2 text-success">
                                                Tải icon lên thành công!
                                            </div>
                                        )}
                                        {iconUploadStatus === 'error' && (
                                            <div className="mt-2 text-danger">
                                                Lỗi khi tải icon lên. Vui lòng thử lại.
                                            </div>
                                        )}
                                    </div>
                                </Form.Group>
                            </Col>
                        </Row>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="outline-secondary" onClick={() => setShowModal(false)}>
                            Hủy
                        </Button>
                        <Button type="submit" variant="success" disabled={loading}>
                            {loading ? (
                                <>
                                    <Spinner as="span" animation="border" size="sm" role="status" className="me-1" />
                                    Đang xử lý...
                                </>
                            ) : editing ? "Cập nhật" : "Thêm mới"}
                        </Button>
                    </Modal.Footer>
                </Form>
            </Modal>

            {/* Delete Confirmation Modal */}
            <Modal 
                show={confirmDelete.show} 
                onHide={() => setConfirmDelete({ show: false, id: null })}
                centered
                size="sm"
            >
                <Modal.Header closeButton>
                    <Modal.Title className="text-danger">Xác nhận xóa</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <p>Bạn có chắc chắn muốn xóa dịch vụ này?</p>
                    <p className="text-muted small mb-0">Lưu ý: Thao tác này không thể hoàn tác.</p>
                </Modal.Body>
                <Modal.Footer>
                    <Button 
                        variant="outline-secondary" 
                        onClick={() => setConfirmDelete({ show: false, id: null })}
                    >
                        Hủy
                    </Button>
                    <Button 
                        variant="danger" 
                        onClick={handleDeleteConfirm}
                        disabled={loading}
                    >
                        {loading ? (
                            <>
                                <Spinner as="span" animation="border" size="sm" role="status" className="me-1" />
                                Đang xóa...
                            </>
                        ) : "Xóa"}
                    </Button>
                </Modal.Footer>
            </Modal>
        </Container>
    )
}

export default ServiceAdmin