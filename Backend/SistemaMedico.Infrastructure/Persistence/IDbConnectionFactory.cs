using System.Data;

namespace SistemaMedico.Infrastructure.Persistence
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
