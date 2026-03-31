namespace SistemaMedico.Application.DTO
{
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public int Status { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public string? PatientFullName { get; set; }
    }
}
