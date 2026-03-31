using SistemaMedico.Domain.Entities;

namespace SistemaMedico.Domain.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment?> GetByIdAsync(int id);
        Task<int> CreateAsync(Appointment appointment);
        Task<bool> UpdateFullAsync(Appointment appointment);
        Task<bool> AttendAsync(Appointment appointment);
        Task<bool> DeleteAsync(int id);
        Task<bool> CheckAvailabilityAsync(string doctor, DateTime dateTime);
    }
}
