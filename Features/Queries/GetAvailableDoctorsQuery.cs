using MediatR;
using PatientRecoverySystem.DoctorService.DTOs;

namespace PatientRecoverySystem.DoctorService.Features.Queries
{
    public class GetAvailableDoctorsQuery : IRequest<IEnumerable<DoctorDto>>
    {
        public string Specialization { get; set; }
    }
}