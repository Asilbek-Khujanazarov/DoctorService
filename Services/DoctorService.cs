using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PatientRecoverySystem.DoctorService.DTOs;
using PatientRecoverySystem.DoctorService.Models;
using PatientRecoverySystem.DoctorService.Data;
using BC = BCrypt.Net.BCrypt;

namespace PatientRecoverySystem.DoctorService.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly DoctorDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(
            DoctorDbContext context,
            IConfiguration configuration,
            ILogger<DoctorService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto dto)
        {
            // Check if username already exists
            var existingDoctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Username == dto.Username);
            if (existingDoctor != null)
            {
                throw new Exception("Username already exists");
            }

            // Hash the password
            string passwordHash = BC.HashPassword(dto.Password);

            var doctor = new Doctor
            {
                Username = dto.Username,
                Password = passwordHash,
                Role = "Doctor", // Default role
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Specialization = dto.Specialization,
                LicenseNumber = dto.LicenseNumber,
                ContactNumber = dto.ContactNumber,
                Email = dto.Email,
                IsAvailable = dto.IsAvailable,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();

            return MapToDto(doctor);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDoctorDto dto)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Username == dto.Username);

            if (doctor == null || !BC.Verify(dto.Password, doctor.Password))
            {
                throw new Exception("Invalid username or password");
            }

            // Generate JWT token
            var token = GenerateJwtToken(doctor);

            return new LoginResponseDto
            {
                Token = token,
                Doctor = MapToDto(doctor)
            };
        }

        // Implement GetDoctorByIdAsync
        public async Task<DoctorDto> GetDoctorByIdAsync(Guid id)
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
                return null;

            return MapToDto(doctor);
        }

        // Implement GetDoctorsAsync
        public async Task<IEnumerable<DoctorDto>> GetDoctorsAsync()
        {
            var doctors = await _context.Doctors
                .Where(d => !d.IsDeleted)
                .ToListAsync();

            return doctors.Select(MapToDto);
        }

        // Implement UpdateDoctorAsync
        public async Task<DoctorDto> UpdateDoctorAsync(Guid id, CreateDoctorDto dto)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                return null;

            // Check if username is being changed and if new username exists
            if (doctor.Username != dto.Username)
            {
                var existingDoctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.Username == dto.Username);
                if (existingDoctor != null)
                {
                    throw new Exception("Username already exists");
                }
            }

            // Update fields
            doctor.Username = dto.Username;
            if (!string.IsNullOrEmpty(dto.Password))
            {
                doctor.Password = BC.HashPassword(dto.Password);
            }
            doctor.FirstName = dto.FirstName;
            doctor.LastName = dto.LastName;
            doctor.Specialization = dto.Specialization;
            doctor.LicenseNumber = dto.LicenseNumber;
            doctor.ContactNumber = dto.ContactNumber;
            doctor.Email = dto.Email;
            doctor.IsAvailable = dto.IsAvailable;
            doctor.UpdatedAt = DateTime.UtcNow;

            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();

            return MapToDto(doctor);
        }

        // Implement DeleteDoctorAsync
        public async Task DeleteDoctorAsync(Guid id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor != null)
            {
                doctor.IsDeleted = true;
                doctor.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        private string GenerateJwtToken(Doctor doctor)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, doctor.Id.ToString()),
                    new Claim(ClaimTypes.Name, doctor.Username),
                    new Claim(ClaimTypes.Role, doctor.Role),
                    new Claim("FirstName", doctor.FirstName),
                    new Claim("LastName", doctor.LastName)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private DoctorDto MapToDto(Doctor doctor)
        {
            return new DoctorDto
            {
                Id = doctor.Id,
                Username = doctor.Username,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Specialization = doctor.Specialization,
                LicenseNumber = doctor.LicenseNumber,
                ContactNumber = doctor.ContactNumber,
                Email = doctor.Email,
                IsAvailable = doctor.IsAvailable,
                CreatedAt = doctor.CreatedAt,
                UpdatedAt = doctor.UpdatedAt
            };
        }
    }
}