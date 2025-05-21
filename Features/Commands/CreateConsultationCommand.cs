using MediatR;
using PatientRecoverySystem.DoctorService.DTOs;

namespace PatientRecoverySystem.DoctorService.Features.Commands
{
    public class CreateConsultationCommand : IRequest<ConsultationDto>
    {
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public DateTime ConsultationDate { get; set; }
        public string Notes { get; set; }
    }
}