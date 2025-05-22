using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientRecovery.Shared.Messaging;
using PatientRecoverySystem.DoctorService.DTOs;
using PatientRecoverySystem.DoctorService.Features.Commands;
using PatientRecoverySystem.DoctorService.Features.Queries;
using PatientRecoverySystem.DoctorService.Services;

namespace PatientRecoverySystem.DoctorService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRabbitMQService _messageBus;
        private readonly IDoctorService _doctorService;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(IMediator mediator, IRabbitMQService messageBus,
            IDoctorService doctorService,
            ILogger<DoctorsController> logger)
        {
            _mediator = mediator;
            _messageBus = messageBus;
            _doctorService = doctorService;
            _logger = logger;
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DoctorDto>> Register(CreateDoctorDto dto)
        {
            try
            {
                var result = await _doctorService.CreateDoctorAsync(dto);
                // CreatedAtAction o'rniga Created ishlatamiz
                return Created($"/api/doctors/{result.Id}", result);
                // Yoki shunchaki Ok qaytaramiz
                // return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering doctor");
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<DoctorDto>>> GetAll()
        {
            var doctors = await _doctorService.GetDoctorsAsync();
            return Ok(doctors);
        }

        [HttpGet("details/{id}")]
        [Authorize]
        public async Task<ActionResult<DoctorDto>> GetById(Guid id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound();

            return Ok(doctor);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDoctorDto dto)
        {
            try
            {
                var result = await _doctorService.LoginAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in doctor");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DoctorDto>> Update(Guid id, [FromBody] CreateDoctorDto dto)
        {
            var result = await _doctorService.UpdateDoctorAsync(id, dto);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _doctorService.DeleteDoctorAsync(id);
            return NoContent();
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