using MediatR;
using PatientRecoverySystem.DoctorService.DTOs;

namespace PatientRecoverySystem.DoctorService.Features.Queries
{
    public class GetDoctorQuery : IRequest<DoctorDto>
    {
        public Guid Id { get; set; }
    }
}