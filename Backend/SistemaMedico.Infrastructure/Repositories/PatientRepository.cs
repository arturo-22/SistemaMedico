using Dapper;
using SistemaMedico.Domain.Entities;
using SistemaMedico.Domain.Interfaces;
using SistemaMedico.Infrastructure.Persistence;
using System.Data;

namespace SistemaMedico.Infrastructure.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PatientRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Patient>(
                "usp_Patient_GetAll",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Patient>(
                "usp_Patient_GetById",
                new { Id = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CreateAsync(Patient patient)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("FirstName", patient.FirstName);
            parameters.Add("LastName", patient.LastName);
            parameters.Add("BirthDate", patient.BirthDate);
            parameters.Add("Gender", patient.Gender);
            parameters.Add("Address", patient.Address);
            parameters.Add("Phone", patient.Phone);
            parameters.Add("Email", patient.Email);

            return await connection.ExecuteScalarAsync<int>(
                "usp_Patient_Insert",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateAsync(Patient patient)
        {
            using var connection = _connectionFactory.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("Id", patient.Id);
            parameters.Add("FirstName", patient.FirstName);
            parameters.Add("LastName", patient.LastName);
            parameters.Add("BirthDate", patient.BirthDate);
            parameters.Add("Gender", patient.Gender);
            parameters.Add("Address", patient.Address);
            parameters.Add("Phone", patient.Phone);
            parameters.Add("Email", patient.Email);

            var rows = await connection.ExecuteAsync(
                "usp_Patient_Update",
                parameters,
                commandType: CommandType.StoredProcedure);

            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var rows = await connection.ExecuteAsync(
                "usp_Patient_Delete",
                new { Id = id },
                commandType: CommandType.StoredProcedure);

            return rows > 0;
        }

        public async Task<IEnumerable<Patient>> SearchAsync(string term)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Patient>(
                "usp_Patient_Search",
                new { Term = term },
                commandType: CommandType.StoredProcedure);
        }
    }
}