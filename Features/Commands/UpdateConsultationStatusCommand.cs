using MediatR;
using PatientRecoverySystem.DoctorService.DTOs;
using PatientRecoverySystem.DoctorService.Models;

namespace PatientRecoverySystem.DoctorService.Features.Commands
{
    public class UpdateConsultationStatusCommand : IRequest<ConsultationDto>
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public ConsultationStatus Status { get; set; }
    }
}