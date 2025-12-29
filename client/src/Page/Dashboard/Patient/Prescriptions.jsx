import React, { useEffect, useState } from 'react'
import { Card } from 'react-bootstrap'
import axios from '../../../Util/AxiosConfig'
import PrescriptionCard from '../../../Component/Card/PrescriptionCard'

function Prescriptions({ tabActive, setTabActive, isSelected}) {
    const [medicalRecords, setMedicalRecords] = useState([])
    const [selectedPrescriptionId, setSelectedPrescriptionId] = useState(null)

    useEffect(() => {
        const fetchPrescriptions = async () => {
            try {
                const response = await axios.get("/medicalRecords/prescriptions");

                setMedicalRecords(response.data)
    
                const storedPrescriptionId = sessionStorage.getItem('selectedPrescriptionId');
                if (storedPrescriptionId) {
                    setSelectedPrescriptionId(storedPrescriptionId);
    
                    // Delay xóa session để đảm bảo PrescriptionCard nhận props xong
                    setTimeout(() => {
                        sessionStorage.removeItem('selectedPrescriptionId');
                    }, 1000);
                }
            } catch (error) {
                console.log(error);
            }
        }
    
        if (tabActive === "prescriptions" || tabActive === "overview") {
            fetchPrescriptions()
        }
    }, [tabActive])
    
    return (
        <Card>
            <Card.Body>
                <h4>Đơn Thuốc</h4>
                <p>Lịch sử đơn thuốc đã kê</p>
                
                {medicalRecords && medicalRecords.length > 0 ? (
                    medicalRecords.map((record) => (
                        <PrescriptionCard
                            key={record.recordId}
                            record={record}
                            tabActive={tabActive}
                            setTabActive={setTabActive}
                            isSelected={isSelected || selectedPrescriptionId === record.recordId}
                        />
                    ))
                ) : (
                    <div className="text-center p-4">
                        <p>Không có đơn thuốc nào trong hồ sơ</p>
                    </div>
                )}
            </Card.Body>
        </Card>
    )
}

export default Prescriptions