using System.Data;

namespace Ping.Web.Services
{
    public interface IConnectionFactory
    {
        IDbConnection Open();
    }
}