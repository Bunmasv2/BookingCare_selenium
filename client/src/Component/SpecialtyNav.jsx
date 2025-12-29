// import React, { useEffect, useState } from 'react'
// import axios from '../Util/AxiosConfig'
// import { useParams } from 'react-router-dom'
// import { DoctorCard, ServiceCard } from "../Component/Card/Index"

// function SpecialtyNav() {
//     const { specialty } = useParams()
//     const navItems = ["Giới thiệu", "Bác sĩ", "Dịch vụ"]
//     const [activeNavItem, setActiveNavItem] = useState(navItems[0])
//     const [infor, setInfor] = useState(activeNavItem === "Giới thiệu" ? "" : [])

//     useEffect(() => {
//         const fetchData = async () => {
//             try {
//                 let response

//                 switch (activeNavItem) {
//                     case navItems[0]: // Giới thiệu
//                         response = await axios.get(`/specialties/${specialty}/description`)

//                         setInfor(response.data ?? "")
//                         break
        
//                     case navItems[1]: // Bác sĩ
//                         response = await axios.get(`/doctors/${specialty}`)

//                         setInfor(Array.isArray(response.data) ? response.data : [])
//                         break
        
//                     case navItems[2]: // Dịch vụ
//                         response = await axios.get(`/services/${specialty}/services`)

//                         setInfor(Array.isArray(response.data) ? response.data : [])
//                         break
        
//                     default:
//                         setInfor(null)
//                         break
//                 }
//             } catch (error) {
//                 console.error("Lỗi lấy dữ liệu:", error)
//             }
//         }
        
//         fetchData()
//     }, [activeNavItem, specialty])

//     return (
//         <div className="specialty-container">
//             {/* Thanh điều hướng tab */}
//             <div className='d-flex mb-3'>
//                 {navItems.map((item, index) => (
//                     <div
//                         key={index}
//                         className={`specialty-nav mt-3 me-5  ${activeNavItem === item ? 'active' : ''}`}
//                         onClick={() => setActiveNavItem(item)}
//                         style={{ 
//                             fontWeight: activeNavItem === item ? 'bold' : 'normal',
//                             borderBottom: activeNavItem === item ? "4px solid #0d6efd" : "" 
//                         }}
//                     >
//                         <span>{item}</span>
//                     </div>
//                 ))}
//             </div>

//             {/* Nội dung Giới thiệu */}
//             {activeNavItem === navItems[0] && typeof infor === "string" && (
//                 <div className="specialty-description mt-3">
//                     {/* Hiển thị hình ảnh */}
//                     <img 
//                         src={specialty.specialtyImage} 
//                         alt={`Hình ảnh của ${specialty}`} 
//                         style={{ width: "98%", maxHeight: "400px", objectFit: "cover", marginBottom: "20px" }} 
//                     />
//                     <div><span className='text-warning fw-bold'>{specialty} </span>{infor}</div>
//                 </div>
//             )}

//             {/* Nội dung Bác sĩ */}
//             {activeNavItem === navItems[1] && (
//                 <div className="specialty-doctors mt-3">
//                     <h5 className="mb-3">Danh sách {activeNavItem}</h5>
//                     {Array.isArray(infor) && infor.length > 0 ? (
//                         <div className="doctor-list" style={{ display: 'flex', flexWrap: 'wrap', gap: '20px' }}>
//                             {infor.map(doctor => (
//                                 <DoctorCard key={doctor.doctorId} doctor={doctor} />
//                             ))}
//                         </div>
//                     ) : (
//                         <p>Hiện chưa có bác sĩ nào được hiển thị.</p>
//                     )}
//                 </div>
//             )}

//             {/* Nội dung Dịch vụ */}
//             {activeNavItem === navItems[2] && (
//                 <div className="specialty-services mt-3">
//                     <h5 className="mb-3">Danh sách {activeNavItem}</h5>
//                     {Array.isArray(infor) && infor.length > 0 ? (
//                         <div className="service-list" style={{ display: 'flex', flexWrap: 'wrap', gap: '20px' }}>
//                             {infor.map(service => (
//                                 <ServiceCard key={service.id} service={service} />
//                             ))}
//                         </div>
//                     ) : (
//                         <p>Hiện chưa có dịch vụ nào được hiển thị.</p>
//                     )}
//                 </div>
//             )}
//         </div>
//     )
// }

// export default SpecialtyNav