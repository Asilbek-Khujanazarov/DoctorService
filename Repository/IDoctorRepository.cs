using PatientRecoverySystem.DoctorService.Models;

namespace PatientRecoverySystem.DoctorService.Repositories
{
    public interface IDoctorRepository
    {
        Task<IEnumerable<Doctor>> GetAllAsync();
        Task<Doctor> GetByIdAsync(Guid id);
        Task<Doctor> CreateAsync(Doctor doctor);
        Task UpdateAsync(Doctor doctor);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<DoctorSchedule>> GetDoctorScheduleAsync(Guid doctorId);
        Task<IEnumerable<Consultation>> GetDoctorConsultationsAsync(Guid doctorId);
        Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync(string specialization);
    }
}