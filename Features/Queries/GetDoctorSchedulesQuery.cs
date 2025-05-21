using MediatR;
using PatientRecoverySystem.DoctorService.DTOs;

namespace PatientRecoverySystem.DoctorService.Features.Queries
{
    public class GetDoctorSchedulesQuery : IRequest<IEnumerable<DoctorScheduleDto>>
    {
        public Guid DoctorId { get; set; }
    }
}