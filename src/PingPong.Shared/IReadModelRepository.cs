using System.Linq;

namespace PingPong.Shared
{
    public interface IReadModelRepository<T>
    {
        void Create(T obj);
        T Retrieve(object id);
        IQueryable<T> Query();
        void Update(T obj);
        void Delete(T obj);
    }
}