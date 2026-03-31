using SistemaMedico.Domain.Enums;

namespace SistemaMedico.Domain.Entities
{
    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public AppointmentStatus Status { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public Patient? Patient { get; set; }
    }
}
