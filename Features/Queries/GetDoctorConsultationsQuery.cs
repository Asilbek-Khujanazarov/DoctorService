using MediatR;
using PatientRecoverySystem.DoctorService.DTOs;

namespace PatientRecoverySystem.DoctorService.Features.Queries
{
    public class GetDoctorConsultationsQuery : IRequest<IEnumerable<ConsultationDto>>
    {
        public Guid DoctorId { get; set; }
    }
}