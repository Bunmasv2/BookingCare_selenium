import React, { useState, useEffect } from 'react';
import { Container, Button, Spinner, Card } from 'react-bootstrap';
import axios from '../../../../../Util/AxiosConfig';

const PrescriptionDetail = ({ recordId, goBack }) => {
  const [htmlContent, setHtmlContent] = useState('');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (recordId) {
      fetchHtmlPrescription(recordId);
    }
  }, [recordId]);

  const fetchHtmlPrescription = async (recordId) => {
    setLoading(true);
    try {
      const response = await axios.get(`/medicalRecords/details/${recordId}`, {
        headers: {
          Accept: 'text/html'
        }
      });
      setHtmlContent(response.data);
    } catch (err) {
      console.error('Lỗi khi lấy chi tiết đơn thuốc:', err);
      setHtmlContent('');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container fluid>
      <div className="d-flex justify-content-between align-items-center mb-3">
        {/* <h4>Chi tiết đơn thuốc #{recordId}</h4> */}
        <Button variant="outline-secondary" onClick={goBack}>
          Quay lại
        </Button>
      </div>

      {loading ? (
        <div className="text-center py-3">
          <Spinner animation="border" />
        </div>
      ) : htmlContent ? (
        <div
          className="border rounded p-3 bg-white"
          dangerouslySetInnerHTML={{ __html: htmlContent }}
        />
      ) : (
        <Card>
          <Card.Body className="text-center">
            <p className="mb-0">Không tìm thấy thông tin đơn thuốc</p>
          </Card.Body>
        </Card>
      )}
    </Container>
  );
};

export default PrescriptionDetail;
