using SistemaMedico.Application.DTO;
using System;

namespace SistemaMedico.Application.Validators
{
    public static class AppointmentValidator
    {
        public static (bool IsValid, string Message) Validate(AppointmentDTO dto)
        {
            if (dto.PatientId <= 0)
                return (false, "Debe seleccionar un paciente válido de la lista.");

            if (string.IsNullOrWhiteSpace(dto.DoctorName))
                return (false, "El nombre del médico es obligatorio.");

            if (dto.DoctorName.Length < 3)
                return (false, "El nombre del médico debe tener al menos 3 caracteres.");

            if (dto.AppointmentDate == default)
                return (false, "La fecha de la cita no es válida.");

            if (dto.AppointmentDate < DateTime.Now)
                return (false, "No se puede agendar una cita en una fecha o hora que ya pasó.");

            if (string.IsNullOrWhiteSpace(dto.Reason))
                return (false, "El motivo de la cita es obligatorio para el expediente.");

            if (dto.Reason.Length < 5)
                return (false, "Por favor, detalle un poco más el motivo de la cita (mínimo 5 caracteres).");

            return (true, string.Empty);
        }

        public static (bool IsValid, string Message) ValidateAttend(AttendAppointmentDTO dto)
        {
            if (dto.Id <= 0)
                return (false, "ID de cita no válido.");

            if (string.IsNullOrWhiteSpace(dto.Diagnosis))
                return (false, "El diagnóstico es obligatorio para finalizar la atención.");

            if (string.IsNullOrWhiteSpace(dto.Treatment))
                return (false, "El tratamiento es obligatorio para finalizar la atención.");

            return (true, string.Empty);
        }
    }
}