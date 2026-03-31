using Dapper;
using SistemaMedico.Domain.Entities;
using SistemaMedico.Domain.Interfaces;
using SistemaMedico.Infrastructure.Persistence;
using System.Data;

namespace SistemaMedico.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AppointmentRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Appointment, Patient, Appointment>(
                "usp_Appointment_GetAll",
                (appointment, patient) =>
                {
                    appointment.Patient = patient;
                    return appointment;
                },
                splitOn: "Id",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Appointment?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Appointment>(
                "usp_Appointment_GetById",
                new { Id = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CreateAsync(Appointment appointment)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("PatientId", appointment.PatientId);
            parameters.Add("DoctorName", appointment.DoctorName);
            parameters.Add("AppointmentDate", appointment.AppointmentDate);
            parameters.Add("Status", (int)appointment.Status);
            parameters.Add("Reason", appointment.Reason);
            parameters.Add("Diagnosis", appointment.Diagnosis);
            parameters.Add("Treatment", appointment.Treatment);

            return await connection.ExecuteScalarAsync<int>(
                "usp_Appointment_Insert",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateFullAsync(Appointment appointment)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("Id", appointment.Id);
            parameters.Add("PatientId", appointment.PatientId);
            parameters.Add("DoctorName", appointment.DoctorName);
            parameters.Add("AppointmentDate", appointment.AppointmentDate);
            parameters.Add("Status", (int)appointment.Status);
            parameters.Add("Reason", appointment.Reason);

            var rows = await connection.ExecuteAsync(
                "usp_Appointment_UpdateFull",
                parameters,
                commandType: CommandType.StoredProcedure);

            return rows > 0;
        }

        public async Task<bool> AttendAsync(Appointment appointment)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("Id", appointment.Id);
            parameters.Add("Diagnosis", appointment.Diagnosis);
            parameters.Add("Treatment", appointment.Treatment);
            parameters.Add("Status", (int)appointment.Status);

            var rows = await connection.ExecuteAsync(
                "usp_Appointment_Attend",
                parameters,
                commandType: CommandType.StoredProcedure);

            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var rows = await connection.ExecuteAsync(
                "usp_Appointment_Delete",
                new { Id = id },
                commandType: CommandType.StoredProcedure);
            return rows > 0;
        }

        public async Task<bool> CheckAvailabilityAsync(string doctorName, DateTime dateTime)
        {
            using var connection = _connectionFactory.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(
                "usp_Appointment_CheckAvailability",
                new { DoctorName = doctorName, AppointmentDate = dateTime },
                commandType: CommandType.StoredProcedure);

            return count == 0;
        }
    }
}