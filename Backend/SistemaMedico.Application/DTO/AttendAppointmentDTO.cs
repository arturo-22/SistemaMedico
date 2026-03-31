namespace SistemaMedico.Application.DTO
{
    public class AttendAppointmentDTO
    {
        public int Id { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty;
    }
}
