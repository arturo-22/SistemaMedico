using Microsoft.AspNetCore.Mvc;
using SistemaMedico.Application.DTO;
using SistemaMedico.Application.Interfaces;
using Microsoft.Extensions.Logging;
using SistemaMedico.Application.Validators;

namespace SistemaMedico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(IPatientService patientService, ILogger<PatientsController> logger)
        {
            _patientService = patientService;
            _logger = logger;
        }

        [HttpGet]
        [Route("List")]
        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetAll()
        {
            _logger.LogInformation("Obteniendo listado de pacientes.");
            var patients = await _patientService.GetAllAsync();
            return Ok(patients);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<ActionResult<PatientDTO>> GetById(int id)
        {
            _logger.LogInformation("Buscando paciente ID: {Id}", id);
            var patient = await _patientService.GetByIdAsync(id);
            if (patient == null)
            {
                _logger.LogWarning("Paciente ID: {Id} no encontrado.", id);
                return NotFound();
            }
            return Ok(patient);
        }

        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult<IEnumerable<PatientDTO>>> Search([FromQuery] string term)
        {
            _logger.LogInformation("Búsqueda de pacientes con término: {Term}", term);
            if (string.IsNullOrEmpty(term)) return BadRequest("El término de búsqueda es obligatorio");
            var results = await _patientService.SearchAsync(term);
            return Ok(results);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult<int>> Create(PatientDTO patientDto)
        {
            var validation = PatientValidator.Validate(patientDto);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Validación de creación fallida: {Msg}", validation.Message);
                return BadRequest(new { message = validation.Message });
            }

            try
            {
                _logger.LogInformation("Creando paciente: {Nombre}", patientDto.FirstName);
                var id = await _patientService.CreateAsync(patientDto);
                return CreatedAtAction(nameof(GetById), new { id = id }, id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el servidor al crear paciente.");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(int id, PatientDTO patientDto)
        {
            if (id != patientDto.Id)
                return BadRequest(new { message = "El ID de la URL no coincide con el del objeto." });

            var validation = PatientValidator.Validate(patientDto);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Validación de actualización fallida para ID {Id}: {Msg}", id, validation.Message);
                return BadRequest(new { message = validation.Message });
            }

            try
            {
                _logger.LogInformation("Actualizando paciente ID: {Id}", id);
                var result = await _patientService.UpdateAsync(patientDto);
                if (!result) return NotFound(new { message = "Paciente no encontrado." });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar paciente ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Intentando eliminar paciente ID: {Id}", id);

                var result = await _patientService.DeleteAsync(id);

                if (!result)
                {
                    _logger.LogWarning("No se pudo eliminar. Paciente ID: {Id} no encontrado.", id);
                    return NotFound(new { message = "El paciente no existe." });
                }

                _logger.LogInformation("Paciente ID: {Id} eliminado satisfactoriamente.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar paciente ID: {Id}", id);

                if (ex.Message.Contains("REFERENCE constraint") || ex.Message.Contains("conflicted with the REFERENCE constraint"))
                {
                    return BadRequest(new { message = "No se puede eliminar el paciente porque tiene citas médicas registradas. Primero debe eliminar o reasignar sus citas." });
                }

                return BadRequest(new { message = ex.Message });
            }
        }
    }
}