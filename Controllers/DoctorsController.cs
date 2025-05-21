using MediatR;
using Microsoft.AspNetCore.Mvc;
using PatientRecovery.Shared.Messaging;
using PatientRecoverySystem.DoctorService.DTOs;
using PatientRecoverySystem.DoctorService.Features.Commands;
using PatientRecoverySystem.DoctorService.Features.Queries;

namespace PatientRecoverySystem.DoctorService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRabbitMQService _messageBus;

        public DoctorsController(IMediator mediator, IRabbitMQService messageBus)
        {
            _mediator = mediator;
            _messageBus = messageBus;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetAll()
        {
            var query = new GetAllDoctorsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDto>> Get(Guid id)
        {
            var query = new GetDoctorQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<DoctorDto>> Create([FromBody] CreateDoctorCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DoctorDto>> Update(Guid id, [FromBody] UpdateDoctorCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            var result = await _mediator.Send(command);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetAvailableDoctors([FromQuery] string specialization)
        {
            var query = new GetAvailableDoctorsQuery { Specialization = specialization };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}