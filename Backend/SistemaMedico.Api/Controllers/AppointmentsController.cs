using Microsoft.AspNetCore.Mvc;
using SistemaMedico.Application.DTO;
using SistemaMedico.Application.Interfaces;
using SistemaMedico.Application.Validators;

namespace SistemaMedico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(IAppointmentService appointmentService, ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        [HttpGet]
        [Route("List")]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetAll()
        {
            _logger.LogInformation("Consultando lista completa de citas.");
            var appointments = await _appointmentService.GetAllAsync();
            return Ok(appointments);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<ActionResult<AppointmentDTO>> GetById(int id)
        {
            _logger.LogInformation("Consultando cita con ID: {Id}", id);
            var appointment = await _appointmentService.GetByIdAsync(id);
            if (appointment == null)
            {
                _logger.LogWarning("Cita con ID: {Id} no encontrada.", id);
                return NotFound(new { message = "La cita solicitada no existe." });
            }
            return Ok(appointment);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult<int>> Create(AppointmentDTO appointmentDto)
        {
            var validation = AppointmentValidator.Validate(appointmentDto);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Intento de creación rechazado por validación: {Msg}", validation.Message);
                return BadRequest(new { message = validation.Message });
            }

            try
            {
                _logger.LogInformation("Iniciando creación de cita para el doctor {Doctor}", appointmentDto.DoctorName);
                var id = await _appointmentService.CreateAsync(appointmentDto);
                _logger.LogInformation("Cita creada con éxito. ID: {Id}", id);

                return CreatedAtAction(nameof(GetById), new { id = id }, id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la creación de la cita");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("Update/{id}")]
        public async Task<IActionResult> Update(int id, AppointmentDTO appointmentDto)
        {
            if (id != appointmentDto.Id)
            {
                _logger.LogWarning("IDs discrepantes en Update. Ruta: {Id}, Body: {DtoId}", id, appointmentDto.Id);
                return BadRequest(new { message = "El ID de la ruta no coincide con el ID del cuerpo." });
            }

            var validation = AppointmentValidator.Validate(appointmentDto);
            if (!validation.IsValid)
            {
                return BadRequest(new { message = validation.Message });
            }

            try
            {
                _logger.LogInformation("Actualizando cita ID: {Id}", id);
                var result = await _appointmentService.UpdateAsync(appointmentDto);
                if (!result)
                {
                    _logger.LogWarning("No se pudo actualizar. Cita ID: {Id} no encontrada.", id);
                    return NotFound(new { message = "No se encontró la cita para actualizar." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la cita ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("Attend/{id}")]
        public async Task<IActionResult> Attend(int id, [FromBody] AttendAppointmentDTO dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "El ID de la ruta no coincide con el del cuerpo." });

            var validation = AppointmentValidator.ValidateAttend(dto);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Validación de atención fallida para ID {Id}: {Msg}", id, validation.Message);
                return BadRequest(new { message = validation.Message });
            }

            try
            {
                _logger.LogInformation("Marcando como atendida la cita ID: {Id}", id);
                var result = await _appointmentService.AttendAsync(dto);

                if (!result)
                {
                    _logger.LogWarning("No se pudo marcar como atendida. Cita ID: {Id} no encontrada.", id);
                    return NotFound(new { message = "La cita no existe o ya fue procesada." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al atender la cita ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Iniciando cancelación de cita ID: {Id}", id);
                var result = await _appointmentService.DeleteAsync(id);

                if (!result)
                {
                    _logger.LogWarning("Cancelación fallida. Cita ID: {Id} no encontrada.", id);
                    return NotFound(new { message = "La cita no existe." });
                }

                _logger.LogInformation("Cita ID: {Id} cancelada correctamente.", id);
                return Ok(new { success = true, message = "Cita cancelada correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la cancelación de la cita ID: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}