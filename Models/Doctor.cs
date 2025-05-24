using System;
using PatientRecoverySystem.Shared.Models;

namespace PatientRecoverySystem.DoctorService.Models
{
    public class Doctor : BaseEntity
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!; // Bu yerda hash qilingan parol saqlanadi
        public string Role { get; set; } = "Doctor";
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialization { get; set; }
        public string LicenseNumber { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public bool IsAvailable { get; set; }
        public List<string> UserIds { get; set; } = new List<string>();
        public virtual ICollection<DoctorSchedule> Schedules { get; set; }
        public virtual ICollection<Consultation> Consultations { get; set; }
    }

    public class DoctorSchedule : BaseEntity
    {
        public Guid DoctorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public virtual Doctor Doctor { get; set; }
    }

    public class Consultation : BaseEntity
    {
        public Guid DoctorId { get; set; }
        public Guid PatientId { get; set; }
        public DateTime ConsultationDate { get; set; }
        public string Notes { get; set; }
        public ConsultationStatus Status { get; set; }
        public virtual Doctor Doctor { get; set; }
    }

    public enum ConsultationStatus
    {
        Scheduled,
        InProgress,
        Completed,
        Cancelled
    }
}