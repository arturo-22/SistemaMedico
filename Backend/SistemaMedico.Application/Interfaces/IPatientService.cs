using SistemaMedico.Application.DTO;

namespace SistemaMedico.Application.Interfaces
{
    public interface IPatientService
    {
        Task<IEnumerable<PatientDTO>> GetAllAsync();
        Task<PatientDTO?> GetByIdAsync(int id);
        Task<int> CreateAsync(PatientDTO patientDto);
        Task<bool> UpdateAsync(PatientDTO patientDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<PatientDTO>> SearchAsync(string term);
    }
}
