using MediatR;
using PatientRecoverySystem.DoctorService.DTOs;

namespace PatientRecoverySystem.DoctorService.Features.Queries
{
    public class GetAllDoctorsQuery : IRequest<IEnumerable<DoctorDto>>
    {
    }
}