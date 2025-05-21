using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientRecovery.Shared.Messaging;
using PatientRecoverySystem.DoctorService.DTOs;
using PatientRecoverySystem.DoctorService.Features.Commands;
using PatientRecoverySystem.DoctorService.Features.Queries;



namespace PatientRecoverySystem.DoctorService.Controllers
{
    [ApiController]
    [Route("api/doctors/{doctorId}/consultations")]
    public class ConsultationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRabbitMQService _messageBus;

        public ConsultationsController(IMediator mediator, IRabbitMQService messageBus)
        {
            _mediator = mediator;
            _messageBus = messageBus;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConsultationDto>>> GetConsultations(Guid doctorId)
        {
            var query = new GetDoctorConsultationsQuery { DoctorId = doctorId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ConsultationDto>> CreateConsultation(
            Guid doctorId,
            [FromBody] CreateConsultationCommand command)
        {
            command.DoctorId = doctorId;
            var result = await _mediator.Send(command);

            // Notify about new consultation
            _messageBus.PublishConsultationScheduled(System.Text.Json.JsonSerializer.Serialize(result));

            return CreatedAtAction(nameof(GetConsultations), new { doctorId }, result);
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<ConsultationDto>> UpdateConsultationStatus(
            Guid doctorId,
            Guid id,
            [FromBody] UpdateConsultationStatusCommand command)
        {
            if (id != command.Id || doctorId != command.DoctorId)
                return BadRequest();

            var result = await _mediator.Send(command);

            if (result == null)
                return NotFound();

            // Notify about consultation status change
            _messageBus.PublishConsultationStatusChanged(System.Text.Json.JsonSerializer.Serialize(result));

            return Ok(result);
        }
    }
}