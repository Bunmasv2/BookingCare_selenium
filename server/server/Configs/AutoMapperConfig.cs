    using AutoMapper;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using server.DTO;
    using server.Models;

    namespace server.Configs
    {
        public class AutoMapperConfig : Profile
        {
        public AutoMapperConfig()
        {
            CreateMap<ApplicationUser, UserDTO.UserBasic>()
                .ForMember(dest => dest.Address, m => m.MapFrom(source => source.Address))
                .ForMember(dest => dest.PhoneNumber, m => m.MapFrom(source => source.PhoneNumber))
                .ForMember(dest => dest.DateOfBirth, m => m.MapFrom(source => source.DateOfBirth))
                .ForMember(dest => dest.UserName, m => m.MapFrom(source => source.FullName));


            CreateMap<ApplicationUser, UserDTO.Doctor>()
                .ForMember(dest => dest.UserId, m => m.MapFrom(src => src.Doctor.UserId))
                .ForMember(dest => dest.DoctorId, m => m.MapFrom(src => src.Doctor.DoctorId))
                .ForMember(dest => dest.SpecialtyName, m => m.MapFrom(src => src.Doctor.Specialty.Name))
                .ForMember(dest => dest.ExperienceYears, m => m.MapFrom(src => src.Doctor.ExperienceYears))
                .ForMember(dest => dest.Position, m => m.MapFrom(src => src.Doctor.Position))
                .ForMember(dest => dest.Biography, m => m.MapFrom(src => src.Doctor.Biography))
                .ForMember(dest => dest.Qualifications, m => m.MapFrom(src => src.Doctor.Qualifications))
                .ForMember(dest => dest.WorkExperience, m => m.MapFrom(src => src.Doctor.WorkExperience))
                .ForMember(dest => dest.Degree, m => m.MapFrom(src => src.Doctor.Degree))
                .ForMember(dest => dest.DoctorImage, m => m.MapFrom(src =>
                    src.Doctor.DoctorImage != null
                        ? $"data:image/png;base64,{Convert.ToBase64String(src.Doctor.DoctorImage)}"
                        : null
                ));

            CreateMap<ApplicationUser, UserDTO.Admin>()
                .ForMember(dest => dest.UserId, m => m.MapFrom(src => src.Id));

            CreateMap<ApplicationUser, UserDTO.Patient>()
                .ForMember(dest => dest.UserId, m => m.MapFrom(src => src.Id))
                .ForMember(dest => dest.PatientId, m => m.MapFrom(src => src.Patient.PatientId))
                .ForMember(dest => dest.DateOfBirth, m => m.MapFrom(src => src.Patient.DateOfBirth))
                .ForMember(dest => dest.Address, m => m.MapFrom(src => src.Patient.Address));

            CreateMap<Doctor, DoctorDTO.DoctorBasic>()
                .ForMember(dest => dest.UserName, m => m.MapFrom(source => source.User.FullName))
                .ForMember(dest => dest.DoctorImage, m => m.MapFrom(source =>
                    source.DoctorImage != null
                        ? $"data:image/png;base64,{Convert.ToBase64String(source.DoctorImage)}"
                        : null
                ));

            CreateMap<Doctor, DoctorDTO.DoctorDetail>()
                .ForMember(dest => dest.UserName, m => m.MapFrom(source => source.User.FullName))
                .ForMember(dest => dest.Email, m => m.MapFrom(source => source.User.Email))
                .ForMember(dest => dest.DoctorImage, m => m.MapFrom(source =>
                    source.DoctorImage != null
                        ? $"data:image/png;base64,{Convert.ToBase64String(source.DoctorImage)}"
                        : null
                ));

            CreateMap<Service, ServiceDTO.ServiceDetail>()
                .ForMember(dest => dest.ServiceName, m => m.MapFrom(source => source.ServiceName))
                .ForMember(dest => dest.ServiceIcon, m => m.MapFrom(source =>
                    source.ServiceIcon != null
                        ? $"data:image/png;base64,{Convert.ToBase64String(source.ServiceIcon)}"
                        : null))
                .ForMember(dest => dest.ServiceImage, m => m.MapFrom(source =>
                    source.ServiceImage != null
                        ? $"data:image/png;base64,{Convert.ToBase64String(source.ServiceImage)}"
                        : null));

            CreateMap<Specialty, SpecialtyDTO>()
                .ForMember(dest => dest.SpecialtyImage, m => m.MapFrom(source =>
                    source.SpecialtyImage != null
                        ? $"data:image/png;base64,{Convert.ToBase64String(source.SpecialtyImage)}"
                        : null));

            CreateMap<Patient, PatientDTO.PatientBasic>()
                .ForMember(dest => dest.UserName, m => m.MapFrom(source => source.User.FullName))
                .ForMember(dest => dest.Email, m => m.MapFrom(source => source.User.Email));

            CreateMap<Patient, PatientDTO.PatientDetail>()
                .ForMember(dest => dest.UserName, m => m.MapFrom(source => source.User.FullName))
                .ForMember(dest => dest.Email, m => m.MapFrom(source => source.User.Email))
                .ForMember(dest => dest.PhoneNumber, m => m.MapFrom(source => source.User.PhoneNumber));

            CreateMap<Appointment, AppointmentDTO.AppointmentDetail>()
                .ForMember(dest => dest.PatientName, m => m.MapFrom(source => source.Patient.User.FullName))
                .ForMember(dest => dest.DoctorName, m => m.MapFrom(source => source.Doctor.User.FullName))
                .ForMember(dest => dest.ServiceName, m => m.MapFrom(source => source.Service.ServiceName))
                .ForMember(dest => dest.AppointmentDate, m => m.MapFrom(source =>
                    source.AppointmentDate.HasValue ? source.AppointmentDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null))
                .ForMember(dest => dest.Status, m => m.MapFrom(source => source.Status));

            CreateMap<IGrouping<AppointmentDTO.DoctorScheduleDTO, Appointment>, AppointmentDTO.DoctorScheduleDTO>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Key.Date))
                .ForMember(dest => dest.AppointmentTime, opt => opt.MapFrom(src => src.Key.AppointmentTime))
                .ForMember(dest => dest.PatientCount, opt => opt.MapFrom(src => src.Count()));

            CreateMap<Medicine, MedicineDTO.MedicineBasic>();

            CreateMap<MedicalRecord, MedicalRecordDTO.MedicalRecordBasic>()
                .ForMember(dest => dest.AppointmentDate, m => m.MapFrom(source => source.Appointment.AppointmentDate))
                .ForMember(dest => dest.AppointmentTime, m => m.MapFrom(source => source.Appointment.AppointmentTime))
                .ForMember(dest => dest.DoctorName, m => m.MapFrom(source => source.Appointment.Doctor.User.FullName))
                .ForMember(dest => dest.ServiceName, m => m.MapFrom(source => source.Appointment.Service.ServiceName))
                .ForMember(dest => dest.PatientName, m => m.MapFrom(source => source.Appointment.Patient.User.FullName))
                .ForMember(dest => dest.PatientId, m => m.MapFrom(source => source.Appointment.PatientId))
                .ForMember(dest => dest.SpecialtyName, m => m.MapFrom(source => source.Appointment.Doctor.Specialty.Name))
                .ForMember(dest => dest.Status, m => m.MapFrom(source => source.Appointment.Status));

            CreateMap<MedicalRecordDetail, MedicalRecordDTO.MedicineDto>()
                .ForMember(dest => dest.MedicineName, m => m.MapFrom(source => source.Medicine.MedicalName))
                .ForMember(dest => dest.Unit, m => m.MapFrom(source => source.Medicine.Unit))
                .ForMember(dest => dest.Price, m => m.MapFrom(source => source.Medicine.Price));

            CreateMap<Review, ReviewDTO>()
                .ForMember(dest => dest.Knowledge, m => m.MapFrom(source => source.DoctorReviewDetail.Knowledge))
                .ForMember(dest => dest.Attitude, m => m.MapFrom(source => source.DoctorReviewDetail.Attitude))
                .ForMember(dest => dest.Dedication, m => m.MapFrom(source => source.DoctorReviewDetail.Dedication))
                .ForMember(dest => dest.CommunicationSkill, m => m.MapFrom(source => source.DoctorReviewDetail.CommunicationSkill))

                .ForMember(dest => dest.Convenience, m => m.MapFrom(source => source.ServiceReviewDetail.Convenience))
                .ForMember(dest => dest.Effectiveness, m => m.MapFrom(source => source.ServiceReviewDetail.Effectiveness))
                .ForMember(dest => dest.Price, m => m.MapFrom(source => source.ServiceReviewDetail.Price))
                .ForMember(dest => dest.ServiceSpeed, m => m.MapFrom(source => source.ServiceReviewDetail.ServiceSpeed));

            CreateMap<Review, ServiceReview>()
                .ForMember(dest => dest.PatientName, m => m.MapFrom(src => src.MedicalRecord.Appointment.Patient.User.FullName))
                .ForMember(dest => dest.ServiceName, m => m.MapFrom(src => src.MedicalRecord.Appointment.Service.ServiceName));

            CreateMap<Review, DoctorReviewDetailDTO>()
                .ForMember(dest => dest.MedicalRecordId, m => m.MapFrom(src => src.MedicalRecord.RecordId))
                .ForMember(dest => dest.Attitude, m => m.MapFrom(src => src.DoctorReviewDetail.Attitude))
                .ForMember(dest => dest.Knowledge, m => m.MapFrom(src => src.DoctorReviewDetail.Knowledge))
                .ForMember(dest => dest.CommunicationSkill, m => m.MapFrom(src => src.DoctorReviewDetail.CommunicationSkill))
                .ForMember(dest => dest.Dedication, m => m.MapFrom(src => src.DoctorReviewDetail.Dedication))
                .ForMember(dest => dest.PatientId, m => m.MapFrom(src => src.MedicalRecord.Appointment.Patient.PatientId))
                .ForMember(dest => dest.PatientName, m => m.MapFrom(src => src.MedicalRecord.Appointment.Patient.User.FullName))
                .ForMember(dest => dest.ServiceId, m => m.MapFrom(src => src.MedicalRecord.Appointment.Service.ServiceId))
                .ForMember(dest => dest.ServiceName, m => m.MapFrom(src => src.MedicalRecord.Appointment.Service.ServiceName));

            CreateMap<Review, ServiceReviewDetailDTO>()
                .ForMember(dest => dest.MedicalRecordId, m => m.MapFrom(src => src.MedicalRecord.RecordId))
                .ForMember(dest => dest.Effectiveness, m => m.MapFrom(src => src.ServiceReviewDetail.Effectiveness))
                .ForMember(dest => dest.Price, m => m.MapFrom(src => src.ServiceReviewDetail.Price))
                .ForMember(dest => dest.ServiceSpeed, m => m.MapFrom(src => src.ServiceReviewDetail.ServiceSpeed))
                .ForMember(dest => dest.Convenience, m => m.MapFrom(src => src.ServiceReviewDetail.Convenience))
                .ForMember(dest => dest.PatientId, m => m.MapFrom(src => src.MedicalRecord.Appointment.Patient.PatientId))
                .ForMember(dest => dest.PatientName, m => m.MapFrom(src => src.MedicalRecord.Appointment.Patient.User.FullName))
                .ForMember(dest => dest.DoctorId, m => m.MapFrom(src => src.MedicalRecord.Appointment.Doctor.DoctorId))
                .ForMember(dest => dest.DoctorName, m => m.MapFrom(src => src.MedicalRecord.Appointment.Doctor.User.FullName));

            CreateMap<ContactMessages, ContactMessagesDTO.ContactMessages>()
                .ForMember(dest => dest.PatientName, opt => 
                    opt.MapFrom(src => src.Patient != null && src.Patient.User != null 
                        ? src.Patient.User.FullName 
                        : string.Empty));            
            }
        }
    }