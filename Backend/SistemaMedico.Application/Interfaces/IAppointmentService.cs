using SistemaMedico.Application.DTO;

namespace SistemaMedico.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDTO>> GetAllAsync();
        Task<AppointmentDTO?> GetByIdAsync(int id);
        Task<int> CreateAsync(AppointmentDTO appointmentDto);
        Task<bool> UpdateAsync(AppointmentDTO appointmentDto);
        Task<bool> AttendAsync(AttendAppointmentDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
