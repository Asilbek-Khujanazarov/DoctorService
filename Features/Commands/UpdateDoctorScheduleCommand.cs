using MediatR;
using PatientRecoverySystem.DoctorService.DTOs;

namespace PatientRecoverySystem.DoctorService.Features.Commands
{
    public class UpdateDoctorScheduleCommand : IRequest<DoctorScheduleDto>
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}