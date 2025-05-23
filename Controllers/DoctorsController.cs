using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientRecovery.Shared.Messaging;
using PatientRecoverySystem.DoctorService.DTOs;
using PatientRecoverySystem.DoctorService.Features.Commands;
using PatientRecoverySystem.DoctorService.Features.Queries;
using PatientRecoverySystem.DoctorService.Repositories;
using PatientRecoverySystem.DoctorService.Services;

namespace PatientRecoverySystem.DoctorService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRabbitMQService _messageBus;
          private readonly IDoctorRepository _repository;
        private readonly IDoctorService _doctorService;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(IMediator mediator, IRabbitMQService messageBus,
            IDoctorService doctorService,
            ILogger<DoctorsController> logger, IDoctorRepository repository)
        {
            _mediator = mediator;
            _messageBus = messageBus;
            _doctorService = doctorService;
            _logger = logger;
            _repository = repository;
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

// Bemorni doktorga biriktirish (userId ni qo'shish)
        [HttpPost("{doctorId}/assign-user")]
        public async Task<IActionResult> AssignUser(Guid doctorId, [FromBody] string userId)
        {
            await _repository.AssignUserAsync(doctorId, userId);
            return Ok();
        }

        // Bemorni doktordan ajratish (userId ni o‘chirish)
        [HttpPost("{doctorId}/remove-user")]
        public async Task<IActionResult> RemoveUser(Guid doctorId, [FromBody] string userId)
        {
            await _repository.RemoveUserAsync(doctorId, userId);
            return Ok();
        }
     // GET: api/doctor/{doctorId}/userids
        // Bitta doktorga biriktirilgan barcha UserId lar ro'yxatini chiqaradi
        [HttpGet("{doctorId}/userids")]
        public async Task<IActionResult> GetUserIds(Guid doctorId)
        {
            var doctor = await _repository.GetByIdAsyncUserId(doctorId);
            if (doctor == null)
                return NotFound("Doctor not found");

            return Ok(doctor.UserIds ?? new System.Collections.Generic.List<string>());
        }

        // GET: api/doctor/{doctorId}
        // Bitta doktorga oid barcha ma'lumotlarni chiqaradi (shu jumladan UserIds ro'yxati ham bor)
        [HttpGet("{doctorId}")]
        public async Task<IActionResult> GetDoctor(Guid doctorId)
        {
            var doctor = await _repository.GetByIdAsyncUserId(doctorId);
            if (doctor == null)
                return NotFound("Doctor not found");
            return Ok(doctor);
        }
   
    }
}