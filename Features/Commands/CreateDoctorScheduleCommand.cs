using MediatR;
using PatientRecoverySystem.DoctorService.DTOs;

namespace PatientRecoverySystem.DoctorService.Features.Commands
{
    public class CreateDoctorScheduleCommand : IRequest<DoctorScheduleDto>
    {
        public Guid DoctorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}