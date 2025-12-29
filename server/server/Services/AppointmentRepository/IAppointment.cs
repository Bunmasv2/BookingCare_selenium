using server.DTO;
using server.Models;

namespace server.Services
{
    public interface IAppointment
    {
        Task<Appointment> IsExistAppointment(int? patientId, DateTime appointmentDate, string appointmentTime);
        Task<Appointment> Appointment(int? patientId, int? doctorId, int? serviceId, DateTime appointmentDate, string appointmentTime, string status);
        Task<List<AppointmentDTO.AppointmentDetail>> GetAppointments();
        Task<List<AppointmentDTO.AppointmentDetail>> GetAppointmentsByMonthYear(int month, int year);
        Task<List<AppointmentDTO.AppointmentDetail>> GetAppointmentByPatientId(int? patientId, int quantity);
        // Task<Appointment> GetAppointmentById(int appointmentId);
        Task UpdateStatus(Appointment appointment, string newStatus);
        void CancelAppointment(Appointment appointment);
        Task<List<AppointmentDTO.DoctorScheduleDTO>> GetDoctorSchedule(int? doctorId);
        Task<List<AppointmentDTO.AppointmentDetail>> GetDoctorScheduleDetail(int? doctorId, string date, string time);
        Task<List<AppointmentDTO.AppointmentDetail>> GetPatientScheduleDetail(int? doctorId);
        Task<List<int>> GetAppointmentsId(int? patientId);
        Task<List<int>> GetRecentAppointmentsId(int? patientId);
        Task<AppointmentDTO.AppointmentDetail> GetRecentAppointment(int? patientId);
        Task<int> GetExaminedPatientCount(int doctorId);
        Task<object> AppointmentStatistics(int month, int year);
        Task<object> AppointmentStatisticsPerWeek(int month);
        Task<int> CountAppointmentsByPatientId(int patientId);
        Task<List<AppointmentDTO.AppointmentDetail>> GetAppointmentsByPatientIdPaginated(int patientId, int page, int pageSize);
        Task<int> CountAppointsByDate(DateTime date, string time);
        Task<List<AppointmentDTO.AvailableAppointment>> CheckAvailableAppointment(int? doctorId, DateTime date, string time);
        Task<int> CountAppointment(DateTime date, string time);
        Task<Appointment> GetAppointmentById(int appointmentId);
    }
}
