using MediatR;

namespace PatientRecoverySystem.DoctorService.Features.Commands
{
    public class DeleteDoctorScheduleCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
    }
}