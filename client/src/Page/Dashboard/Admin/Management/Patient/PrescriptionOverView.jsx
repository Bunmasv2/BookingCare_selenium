import React, { useState, useEffect } from 'react';
import { Container, Spinner, Form, Row, Col, Button } from 'react-bootstrap';
import axios from '../../../../../Util/AxiosConfig';
import PatientPrescriptions from './PatientPrescription';
import List from '../General/List';

const PrescriptionOverView = ({ tabActive }) => {
  const [prescriptions, setPrescriptions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selectedPatient, setSelectedPatient] = useState(null);
  const [searchKeyword, setSearchKeyword] = useState('');
  const [hasSearched, setHasSearched] = useState(false);

  useEffect(() => {
    if (tabActive === 'prescriptions') {
      fetchPrescriptions();
    }
  }, [tabActive, hasSearched]);

  const fetchPrescriptions = async () => {
    if (tabActive !== 'prescriptions') return;

    setLoading(true);
    try {
      const response = searchKeyword.trim()
        ? await axios.get(`users/search/patient/byAdmin`, {
            params: { keyword: searchKeyword.trim() }
          })
        : await axios.get(`users/patient`);

      setPrescriptions(response.data);
    } catch (err) {
      console.error('Lỗi khi lấy đơn thuốc:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleSearchSubmit = (e) => {
    e.preventDefault();
    if (searchKeyword.trim() !== '') {
      setHasSearched(true);
      fetchPrescriptions();
    } else {
      setHasSearched(false);
      fetchPrescriptions();
    }
  };

  const handleBackToOverview = () => {
    setSelectedPatient(null);
  };

  const handleClearSearch = () => {
    setSearchKeyword('');
    setHasSearched(false);
    fetchPrescriptions();
  };

  return (
    <Container fluid className='py-4'>
      {selectedPatient ? (
        <PatientPrescriptions
          patientId={selectedPatient.patientId}
          patientName={selectedPatient.fullName}
          goBack={handleBackToOverview}
        />
      ) : (
        <>
          <Form onSubmit={handleSearchSubmit} className="mb-3">
            <Row>
              <Col md={6}>
                <Form.Control
                  type="text"
                  placeholder="Nhập tên, sđt hoặc email bệnh nhân"
                  value={searchKeyword}
                  onChange={(e) => setSearchKeyword(e.target.value)}
                  spellCheck={false}
                />
              </Col>
              <Col md="auto">
                <Button type="submit" variant="primary">Tìm kiếm</Button>
              </Col>
              {hasSearched && (
                <Col md="auto">
                  <Button variant="secondary" onClick={handleClearSearch}>Xóa tìm kiếm</Button>
                </Col>
              )}
            </Row>
          </Form>

          {loading ? (
            <div className="text-center py-3">
              <Spinner animation="border" />
            </div>
          ) : (
            <List users={prescriptions} role="patient" setselected={setSelectedPatient} />
          )}
        </>
      )}
      </Container>
  );
};

export default PrescriptionOverView;
