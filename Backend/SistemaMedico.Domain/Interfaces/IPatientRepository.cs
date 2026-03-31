using SistemaMedico.Domain.Entities;

namespace SistemaMedico.Domain.Interfaces
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task<int> CreateAsync(Patient patient);
        Task<bool> UpdateAsync(Patient patient);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Patient>> SearchAsync(string searchTerm);
    }
}
