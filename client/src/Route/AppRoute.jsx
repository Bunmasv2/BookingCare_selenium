import { Route, Routes } from 'react-router-dom'
import Home from '../Page/Home'
import Index from '../Page/Index'
import About from '../Page/About'
import Contact from '../Page/Contact'
import Doctor from '../Page/Doctor'
import DoctorDetail from '../Page/DoctorDetail'
import Appointment from '../Page/Appointment'
import UploadDoctorImage from '../Page/UploadImages/UploadDoctorImage'
import UploadSpecialtyImage from '../Page/UploadImages/UploadSpecialtyImage'
import Login from '../Page/Login'
import Signin from '../Page/SignIn/Index'
import ServiceDetail from '../Page/ServiceDetail'
import Dashboard from '../Page/Dashboard/Dashboard'
import Admin from '../Page/Admin'
import ForgotPassword from "../Page/ForgotPassword"
import DepartmentDetail from '../Page/DepartmentDetail'
import CheckVNPay from '../Page/Dashboard/Admin/Management/Patient/PaymentVNPayResult'

function AppRoute() {
    return (
        <Routes>
            <Route path='/' element={<Index><Home /></Index>} />
            <Route path='/về chúng tôi' element={<Index><About /></Index>} />
            <Route path='/admin' element={<Index><Admin /></Index>} />
            <Route path='/thông tin cá nhân' element={<Index><Dashboard /></Index>} />
            <Route path='/bác sĩ' element={<Index><Doctor /></Index>} />
            <Route path='/bac-si/:doctorName' element={<Index><DoctorDetail /></Index>} />
            <Route path='/bác sĩ/:doctorName' element={<Index></Index>} />
            <Route path='/chuyên khoa/:specialty' element={<Index><DepartmentDetail /></Index>}></Route>
            <Route path='/dịch vụ/:serviceName' element={<Index><ServiceDetail/></Index>}></Route>
            <Route path='/login' element={<Login /> } />
            <Route path='/Đăng nhập' element={<Signin/>} />
            <Route path='/đặt lịch khám' element={<Index><Appointment /></Index>} />
            <Route path='/liên hệ' element={<Index><Contact /></Index>} />
            <Route path='/' element={<Index><Home /></Index>} />
            <Route path='/upload' element={<UploadDoctorImage></UploadDoctorImage>} />
            <Route path='/UploadSpecialtyImage' element={<UploadSpecialtyImage></UploadSpecialtyImage>} />
            <Route path="/auth/forgot-password" element={<ForgotPassword />} />
            <Route path="/kiểm tra trạng thái" element={<CheckVNPay />} />
        </Routes>
    )
}

export default AppRoute