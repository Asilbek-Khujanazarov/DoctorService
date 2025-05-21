using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientRecoverySystem.DoctorService.DTOs;
using PatientRecoverySystem.DoctorService.Features.Commands;
using PatientRecoverySystem.DoctorService.Features.Queries;

namespace PatientRecoverySystem.DoctorService.Controllers
{
    [ApiController]
    [Route("api/doctors/{doctorId}/schedules")]
    public class SchedulesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SchedulesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorScheduleDto>>> GetDoctorSchedules(Guid doctorId)
        {
            var query = new GetDoctorSchedulesQuery { DoctorId = doctorId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<DoctorScheduleDto>> CreateSchedule(
            Guid doctorId, 
            [FromBody] CreateDoctorScheduleCommand command)
        {
            command.DoctorId = doctorId;
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetDoctorSchedules), new { doctorId }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DoctorScheduleDto>> UpdateSchedule(
            Guid doctorId,
            Guid id,
            [FromBody] UpdateDoctorScheduleCommand command)
        {
            if (id != command.Id || doctorId != command.DoctorId)
                return BadRequest();

            var result = await _mediator.Send(command);
            
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(Guid doctorId, Guid id)
        {
            await _mediator.Send(new DeleteDoctorScheduleCommand { DoctorId = doctorId, Id = id });
            return NoContent();
        }
    }
}