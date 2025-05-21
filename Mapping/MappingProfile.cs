using AutoMapper;
using PatientRecoverySystem.DoctorService.Models;
using PatientRecoverySystem.DoctorService.DTOs;
using PatientRecoverySystem.DoctorService.Features.Commands;

namespace PatientRecoverySystem.DoctorService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Doctor mappings
            CreateMap<Doctor, DoctorDto>();
            CreateMap<CreateDoctorCommand, Doctor>();
            CreateMap<UpdateDoctorCommand, Doctor>();

            // Schedule mappings
            CreateMap<DoctorSchedule, DoctorScheduleDto>();
            CreateMap<CreateDoctorScheduleCommand, DoctorSchedule>();
            CreateMap<UpdateDoctorScheduleCommand, DoctorSchedule>();

            // Consultation mappings
            CreateMap<Consultation, ConsultationDto>();
            CreateMap<CreateConsultationCommand, Consultation>();
            CreateMap<UpdateConsultationStatusCommand, Consultation>();
        }
    }
}