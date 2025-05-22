using PatientRecoverySystem.DoctorService.DTOs;

namespace PatientRecoverySystem.DoctorService.Services
{
    public interface IDoctorService
    {
        Task<DoctorDto> CreateDoctorAsync(CreateDoctorDto dto);
        Task<LoginResponseDto> LoginAsync(LoginDoctorDto dto);
        Task<DoctorDto> GetDoctorByIdAsync(Guid id);
        Task<IEnumerable<DoctorDto>> GetDoctorsAsync();
        Task<DoctorDto> UpdateDoctorAsync(Guid id, CreateDoctorDto dto);
        Task DeleteDoctorAsync(Guid id);
    }
}