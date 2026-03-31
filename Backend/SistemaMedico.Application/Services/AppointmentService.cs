using SistemaMedico.Application.DTO;
using SistemaMedico.Application.Interfaces;
using SistemaMedico.Domain.Entities;
using SistemaMedico.Domain.Enums;
using SistemaMedico.Domain.Interfaces;

namespace SistemaMedico.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository, IPatientRepository patientRepository)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAllAsync()
        {
            var appointments = await _appointmentRepository.GetAllAsync();
            return appointments.Select(a => MapToDto(a));
        }

        public async Task<int> CreateAsync(AppointmentDTO dto)
        {
            var patientExists = await _patientRepository.GetByIdAsync(dto.PatientId);
            if (patientExists == null)
                throw new Exception("El paciente especificado no existe en el sistema.");

            var isAvailable = await _appointmentRepository.CheckAvailabilityAsync(dto.DoctorName, dto.AppointmentDate);
            if (!isAvailable)
                throw new Exception($"El Dr. {dto.DoctorName} ya tiene una cita en ese horario.");

            var appointment = new Appointment
            {
                PatientId = dto.PatientId,
                DoctorName = dto.DoctorName,
                AppointmentDate = dto.AppointmentDate,
                Status = AppointmentStatus.Pending,
                Reason = dto.Reason,
            };

            return await _appointmentRepository.CreateAsync(appointment);
        }

        public async Task<bool> UpdateAsync(AppointmentDTO dto)
        {
            var existingAppt = await _appointmentRepository.GetByIdAsync(dto.Id);
            if (existingAppt == null) return false;

            if (existingAppt.Status != AppointmentStatus.Pending)
                throw new Exception("Solo se pueden modificar citas en estado Pendiente.");

            if (existingAppt.DoctorName != dto.DoctorName || existingAppt.AppointmentDate != dto.AppointmentDate)
            {
                var isAvailable = await _appointmentRepository.CheckAvailabilityAsync(dto.DoctorName, dto.AppointmentDate);
                if (!isAvailable) throw new Exception("El nuevo horario no está disponible.");
            }

            existingAppt.PatientId = dto.PatientId;
            existingAppt.DoctorName = dto.DoctorName;
            existingAppt.AppointmentDate = dto.AppointmentDate;
            existingAppt.Reason = dto.Reason;
            existingAppt.Status = (AppointmentStatus)dto.Status;

            return await _appointmentRepository.UpdateFullAsync(existingAppt);
        }

        public async Task<bool> AttendAsync(AttendAppointmentDTO dto)
        {
            var existing = await _appointmentRepository.GetByIdAsync(dto.Id);
            if (existing == null) throw new Exception("La cita no existe.");

            if (existing.Status == AppointmentStatus.Cancelled)
                throw new Exception("No se puede atender una cita cancelada.");

            if (existing.Status == AppointmentStatus.Attended)
                throw new Exception("La cita ya fue atendida previamente.");

            existing.Diagnosis = dto.Diagnosis;
            existing.Treatment = dto.Treatment;
            existing.Status = AppointmentStatus.Attended;

            return await _appointmentRepository.AttendAsync(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _appointmentRepository.GetByIdAsync(id);
            if (existing == null) return false;

            if (existing.Status == AppointmentStatus.Attended)
                throw new Exception("No se puede eliminar una cita que ya ha sido atendida.");

            return await _appointmentRepository.DeleteAsync(id);
        }

        public async Task<AppointmentDTO?> GetByIdAsync(int id)
        {
            var a = await _appointmentRepository.GetByIdAsync(id);
            return a != null ? MapToDto(a) : null;
        }

        private static AppointmentDTO MapToDto(Appointment a) => new AppointmentDTO
        {
            Id = a.Id,
            PatientId = a.PatientId,
            DoctorName = a.DoctorName,
            AppointmentDate = a.AppointmentDate,
            Status = (int)a.Status,
            Reason = a.Reason,
            Diagnosis = a.Diagnosis,
            Treatment = a.Treatment,
            PatientFullName = a.Patient != null ? $"{a.Patient.FirstName} {a.Patient.LastName}" : null
        };
    }
}