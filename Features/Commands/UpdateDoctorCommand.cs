using MediatR;
using PatientRecoverySystem.DoctorService.DTOs;

namespace PatientRecoverySystem.DoctorService.Features.Commands
{
    public class UpdateDoctorCommand : IRequest<DoctorDto>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public bool IsAvailable { get; set; }
    }
}