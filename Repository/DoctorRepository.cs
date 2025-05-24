using Microsoft.EntityFrameworkCore;
using PatientRecoverySystem.DoctorService.Data;
using PatientRecoverySystem.DoctorService.Models;

namespace PatientRecoverySystem.DoctorService.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly DoctorDbContext _context;

        public DoctorRepository(DoctorDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Doctor>> GetAllAsync()
        {
            return await _context.Doctors
                .Where(d => !d.IsDeleted)
                .Include(d => d.Schedules)
                .ToListAsync();
        }

        public async Task<Doctor> GetByIdAsync(Guid id)
        {
            return await _context.Doctors
                .Include(d => d.Schedules)
                .Include(d => d.Consultations)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        }

        public async Task<Doctor> CreateAsync(Doctor doctor)
        {
            doctor.CreatedAt = DateTime.UtcNow;
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();
            return doctor;
        }

        public async Task UpdateAsync(Doctor doctor)
        {
            doctor.UpdatedAt = DateTime.UtcNow;
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var doctor = await GetByIdAsync(id);
            if (doctor != null)
            {
                doctor.IsDeleted = true;
                doctor.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<DoctorSchedule>> GetDoctorScheduleAsync(Guid doctorId)
        {
            return await _context.DoctorSchedules
                .Where(ds => ds.DoctorId == doctorId && !ds.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Consultation>> GetDoctorConsultationsAsync(Guid doctorId)
        {
            return await _context.Consultations
                .Where(c => c.DoctorId == doctorId && !c.IsDeleted)
                .OrderByDescending(c => c.ConsultationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync(string specialization)
        {
            return await _context.Doctors
                .Where(d => d.IsAvailable &&
                           !d.IsDeleted &&
                           d.Specialization == specialization)
                .Include(d => d.Schedules)
                .ToListAsync();
        }


        public async Task AssignUserAsync(Guid doctorId, string userId)
        {
            var doctor = await GetByIdAsync(doctorId);
            if (doctor == null)
                throw new Exception("Doctor topilmadi");

            if (doctor.UserIds == null)
                doctor.UserIds = new List<string>();

            if (!doctor.UserIds.Contains(userId))
            {
                doctor.UserIds.Add(userId);
                await UpdateAsync(doctor);
            }
        }

        public async Task RemoveUserAsync(Guid doctorId, string userId)
        {
            var doctor = await GetByIdAsync(doctorId);
            if (doctor == null)
                throw new Exception("Doctor topilmadi");

            if (doctor.UserIds == null)
                doctor.UserIds = new List<string>();

            if (doctor.UserIds.Contains(userId))
            {
                doctor.UserIds.Remove(userId);
                await UpdateAsync(doctor);
            }
        }

         public async Task<Doctor> GetByIdAsyncUserId(Guid id)
        {
            return await _context.Doctors.FindAsync(id);
        }

        public async Task<List<Doctor>> GetAllAsyncUserId()
        {
            return await _context.Doctors.ToListAsync();
        }
    }
}