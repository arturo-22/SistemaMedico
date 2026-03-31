using SistemaMedico.Application.DTO;
using System;

namespace SistemaMedico.Application.Validators
{
    public static class PatientValidator
    {
        public static (bool IsValid, string Message) Validate(PatientDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName))
                return (false, "El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.LastName))
                return (false, "El apellido es obligatorio.");

            if (dto.BirthDate == default || dto.BirthDate > DateTime.Now)
                return (false, "La fecha de nacimiento no es válida.");

            if (string.IsNullOrWhiteSpace(dto.Gender))
                return (false, "El género es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.Address))
                return (false, "La dirección es obligatoria.");

            if (string.IsNullOrWhiteSpace(dto.Phone))
                return (false, "El número de teléfono es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains("@"))
                return (false, "Debe proporcionar un correo electrónico válido.");

            return (true, string.Empty);
        }
    }
}