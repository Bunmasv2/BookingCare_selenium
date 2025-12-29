import React, { useState, useEffect, useRef } from 'react';
import { Button, Form, Table, Modal, Container, Row, Col, Card, Badge, InputGroup, FormControl, Spinner, Alert, Pagination, Image} from 'react-bootstrap';
import { Search, PlusCircle, Pencil, Trash, XCircle, ArrowClockwise, Upload, Image as ImageIcon, XSquare} from 'react-bootstrap-icons';
import axios from '../../../../Util/AxiosConfig';

function SpecialtyAdmin({ tabActive }) {
    // State management
    const [specialties, setSpecialties] = useState([]);
    const [filteredSpecialties, setFilteredSpecialties] = useState([]);
    const [showModal, setShowModal] = useState(false);
    const [editing, setEditing] = useState(false);
    const [formData, setFormData] = useState({ name: '', description: '' });
    const [selectedId, setSelectedId] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [confirmDelete, setConfirmDelete] = useState({ show: false, id: null });
    const [uploadStatus, setUploadStatus] = useState('');
    const [previewImage, setPreviewImage] = useState(null);
    const [imageFile, setImageFile] = useState(null);
    const fileInputRef = useRef(null);
    
    // Pagination
    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage] = useState(5);

    // Fetch specialties on component mount
    useEffect(() => {
        if (tabActive !== "specialties") return
        fetchSpecialties();
    }, [tabActive]);

    // Filter specialties based on search term
    useEffect(() => {
        const results = specialties.filter(specialty => 
            specialty.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
            (specialty.description && specialty.description.toLowerCase().includes(searchTerm.toLowerCase()))
        );
        setFilteredSpecialties(results);
        setCurrentPage(1); // Reset to first page when filtering
    }, [searchTerm, specialties]);

    // Get current specialties for pagination
    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    const currentSpecialties = filteredSpecialties.slice(indexOfFirstItem, indexOfLastItem);

    // Fetch specialties from API
    const fetchSpecialties = async () => {
        if (tabActive !== "specialties") return

        setLoading(true);
        setError(null);

        try {
            const res = await axios.get('/specialties');
            console.log(res)
            setSpecialties(res.data);
            setFilteredSpecialties(res.data);
        } catch (err) {
            console.error("Failed to fetch specialties:", err);
            setError("Không thể tải dữ liệu chuyên khoa. Vui lòng thử lại sau.");
        } finally {
            setLoading(false);
        }
    };

    // Modal handlers
    const handleShowCreate = () => {
        setFormData({ name: '', description: '' });
        setEditing(false);
        setShowModal(true);
        setPreviewImage(null);
        setImageFile(null);
        setUploadStatus('');
    };

    const handleEdit = (specialty) => {
        setFormData({ 
            name: specialty.name || '', 
            description: specialty.description || '' 
        });
        setSelectedId(specialty.specialtyId);
        setEditing(true);
        setShowModal(true);
        setPreviewImage(null);
        setImageFile(null);
        setUploadStatus('');
        
        // If specialty has an image, show it in the preview
        if (specialty.specialtyImage) {
            const imageUrl = specialty.specialtyImage;
            setPreviewImage(imageUrl);
        }
    };

    // Delete confirmation
    const handleShowDeleteConfirm = (id) => {
        setConfirmDelete({ show: true, id });
    };

    const handleDeleteConfirm = async () => {
        if (tabActive !== "specialties") return

        try {
            await axios.delete(`/specialties/${confirmDelete.id}`);
            fetchSpecialties();
            setConfirmDelete({ show: false, id: null });
        } catch (err) {
            console.error("Failed to delete specialty:", err);
            setError("Không thể xóa chuyên khoa. Vui lòng thử lại sau.");
        }
    };

    // Form submission
    const handleSubmit = async (e) => {
        if (tabActive !== "specialties") return

        e.preventDefault();
        setLoading(true);
        
        try {
            if (editing) {
                await axios.put(`/specialties/${selectedId}`, formData);
            } else {
                await axios.post("/specialties", formData);
            }
            
            // If there's a new image to upload and we're editing an existing specialty
            if (imageFile && editing) {
                await handleImageUpload(selectedId);
            } else if (imageFile && !editing) {
                // For new specialty, we need to fetch it first to get its ID
                const res = await axios.get('/specialties');
                const newSpecialty = res.data.find(s => s.name === formData.name);
                if (newSpecialty) {
                    await handleImageUpload(newSpecialty.specialtyId);
                }
            }
            
            setShowModal(false);
            fetchSpecialties();
        } catch (err) {
            console.error("Failed to save specialty:", err);
            setError(editing 
                ? "Không thể cập nhật chuyên khoa. Vui lòng thử lại sau."
                : "Không thể thêm chuyên khoa mới. Vui lòng thử lại sau."
            );
        } finally {
            setLoading(false);
        }
    };

    // Handle image selection
    const handleImageChange = (e) => {
        if (e.target.files && e.target.files[0]) {
            const file = e.target.files[0];
            setImageFile(file);
            
            // Create preview
            const reader = new FileReader();
            reader.onload = (e) => {
                setPreviewImage(e.target.result);
            };
            reader.readAsDataURL(file);
        }
    };

    // Clear selected image
    const handleClearImage = () => {
        setPreviewImage(null);
        setImageFile(null);
        if (fileInputRef.current) {
            fileInputRef.current.value = "";
        }
    };
    
    // Handle image upload
    const handleImageUpload = async (specialtyId) => {
        if (tabActive !== "specialties") return
        if (!imageFile) return;
        
        setUploadStatus('uploading');
        const formData = new FormData();
        formData.append('file', imageFile);
        formData.append('specialtyId', specialtyId);
        
        try {
            await axios.post('/specialties/upload', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            });
            setUploadStatus('success');
        } catch (error) {
            console.error('Error uploading image:', error);
            setUploadStatus('error');
        }
    };

    // Pagination handlers
    const paginate = (pageNumber) => setCurrentPage(pageNumber);
    const totalPages = Math.ceil(filteredSpecialties.length / itemsPerPage);

    // Render pagination
    const renderPagination = () => {
        if (totalPages <= 1) return null;

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
            );
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
        );
    };

    // Helper function to render image or placeholder
    const renderSpecialtyImage = (specialty) => {
        if (specialty.specialtyImage) {
            return (
                <Image 
                    src={specialty.specialtyImage}
                    alt={specialty.name}
                    thumbnail
                    style={{ height: '50px', width: '80px', objectFit: 'cover' }}
                />
            );
        }
        
        return (
            <div 
                className="d-flex align-items-center justify-content-center bg-light border" 
                style={{ height: '50px', width: '80px' }}
            >
                <ImageIcon size={25} className="text-muted" />
            </div>
        );
    };

    return (
        <Container fluid className="py-4">
            {/* Header */}
            <Card className="shadow-sm mb-4">
                <Card.Body>
                    <Row className="align-items-center">
                        <Col xs={12} md={6} className="mb-3 mb-md-0">
                            <h4 className="mb-0">
                                <Badge bg="primary" className="me-2">
                                    {filteredSpecialties.length}
                                </Badge>
                                Quản lý chuyên khoa
                            </h4>
                        </Col>
                        <Col xs={12} md={6}>
                            <div className="d-flex gap-2 justify-content-md-end">
                                <Button 
                                    variant="outline-secondary" 
                                    onClick={fetchSpecialties}
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
                                    <PlusCircle /> Thêm chuyên khoa
                                </Button>
                            </div>
                        </Col>
                    </Row>
                </Card.Body>
            </Card>

            {/* Search and filter */}
            <Card className="shadow-sm mb-4">
                <Card.Body>
                    <Row>
                        <Col>
                            <InputGroup>
                                <InputGroup.Text>
                                    <Search />
                                </InputGroup.Text>
                                <FormControl
                                    placeholder="Tìm kiếm chuyên khoa..."
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

            {/* Error message */}
            {error && (
                <Alert variant="danger" onClose={() => setError(null)} dismissible>
                    {error}
                </Alert>
            )}

            {/* Data table */}
            <Card className="shadow-sm">
                <Card.Body>
                    {loading ? (
                        <div className="text-center py-5">
                            <Spinner animation="border" variant="primary" />
                            <p className="mt-2">Đang tải dữ liệu...</p>
                        </div>
                    ) : filteredSpecialties.length === 0 ? (
                        <div className="text-center py-5">
                            <p className="text-muted">
                                {searchTerm 
                                    ? "Không tìm thấy chuyên khoa nào phù hợp" 
                                    : "Chưa có chuyên khoa nào trong hệ thống"}
                            </p>
                            <Button 
                                variant="primary" 
                                onClick={handleShowCreate}
                                className="mt-2"
                            >
                                <PlusCircle className="me-1" /> Thêm chuyên khoa mới
                            </Button>
                        </div>
                    ) : (
                        <>
                            <Table responsive hover className="align-middle mb-0">
                                <thead className="table-light">
                                    <tr>
                                        <th style={{ width: '80px' }} className="text-center">ID</th>
                                        <th style={{ width: '100px' }} className="text-center">Hình ảnh</th>
                                        <th>Khoa</th>
                                        <th>Mô tả</th>
                                        <th style={{ width: '150px' }} className="text-center">Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {currentSpecialties.map((specialty) => (
                                        <tr key={specialty.specialtyId}>
                                            <td className="text-center">
                                                <Badge bg="secondary">
                                                    {specialty.specialtyId}
                                                </Badge>
                                            </td>
                                            <td className="text-center">
                                                {renderSpecialtyImage(specialty)}
                                            </td>
                                            <td className="fw-medium">{specialty.name}</td>
                                            <td>
                                                {specialty.description 
                                                    ? specialty.description 
                                                    : <span className="text-muted fst-italic">Chưa có mô tả</span>}
                                            </td>
                                            <td>
                                                <div className="d-flex gap-2 justify-content-center">
                                                    <Button 
                                                        variant="outline-warning" 
                                                        size="sm" 
                                                        onClick={() => handleEdit(specialty)}
                                                        title="Sửa"
                                                    >
                                                        <Pencil />
                                                    </Button>
                                                    <Button 
                                                        variant="outline-danger" 
                                                        size="sm" 
                                                        onClick={() => handleShowDeleteConfirm(specialty.specialtyId)}
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

            {/* Create/Edit Modal */}
            <Modal 
                show={showModal} 
                onHide={() => setShowModal(false)} 
                centered
                backdrop="static"
                size="lg"
            >
                <Modal.Header closeButton>
                    <Modal.Title>
                        {editing ? "Cập nhật chuyên khoa" : "Thêm chuyên khoa mới"}
                    </Modal.Title>
                </Modal.Header>
                <Form onSubmit={handleSubmit}>
                    <Modal.Body>
                        <Row>
                            <Col md={8}>
                                <Form.Group className="mb-3">
                                    <Form.Label>Tên chuyên khoa <span className="text-danger">*</span></Form.Label>
                                    <Form.Control
                                        type="text"
                                        required
                                        placeholder="Nhập tên chuyên khoa"
                                        value={formData.name}
                                        onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                                    />
                                </Form.Group>
                                <Form.Group className="mb-3">
                                    <Form.Label>Mô tả</Form.Label>
                                    <Form.Control
                                        as="textarea"
                                        rows={5}
                                        placeholder="Nhập mô tả chi tiết về chuyên khoa (không bắt buộc)"
                                        value={formData.description || ''}
                                        onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                                    />
                                </Form.Group>
                            </Col>
                            <Col md={4}>
                                <Form.Group className="mb-3">
                                    <Form.Label>Hình ảnh chuyên khoa</Form.Label>
                                    <div className="mb-3">
                                        {previewImage ? (
                                            <div className="position-relative mb-3">
                                                <Image 
                                                    src={previewImage} 
                                                    alt="Preview" 
                                                    fluid 
                                                    thumbnail
                                                    style={{ height: '200px', width: '100%', objectFit: 'cover' }}
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
                                                style={{ height: '200px' }}
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
                                        
                                        <div className="mt-2">
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
                    <p>Bạn có chắc chắn muốn xóa chuyên khoa này?</p>
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
    );
}

export default SpecialtyAdmin;