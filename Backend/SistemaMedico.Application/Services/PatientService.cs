using SistemaMedico.Application.DTO;
using SistemaMedico.Application.Interfaces;
using SistemaMedico.Domain.Entities;
using SistemaMedico.Domain.Interfaces;

namespace SistemaMedico.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;

        public PatientService(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<IEnumerable<PatientDTO>> GetAllAsync()
        {
            var patients = await _patientRepository.GetAllAsync();
            return patients.Select(MapToDto);
        }

        public async Task<PatientDTO?> GetByIdAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null) return null;

            return MapToDto(patient);
        }

        public async Task<int> CreateAsync(PatientDTO dto)
        {
            var patient = new Patient
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BirthDate = dto.BirthDate,
                Gender = dto.Gender,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email
            };

            return await _patientRepository.CreateAsync(patient);
        }

        public async Task<bool> UpdateAsync(PatientDTO dto)
        {
            var patient = new Patient
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BirthDate = dto.BirthDate,
                Gender = dto.Gender,
                Address = dto.Address,
                Phone = dto.Phone,
                Email = dto.Email
            };

            return await _patientRepository.UpdateAsync(patient);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _patientRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<PatientDTO>> SearchAsync(string term)
        {
            var patients = await _patientRepository.SearchAsync(term);
            return patients.Select(MapToDto);
        }

        private static PatientDTO MapToDto(Patient p) => new PatientDTO
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            BirthDate = p.BirthDate,
            Gender = p.Gender,
            Address = p.Address,
            Phone = p.Phone,
            Email = p.Email
        };
    }
}