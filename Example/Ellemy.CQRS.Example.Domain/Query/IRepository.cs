using System.Collections.Generic;

namespace Ellemy.CQRS.Example.Query
{
    public interface IRepository<T>
    {
        T GetById(object id);
        IEnumerable<T> GetAll();
        void Save(T entity);
    }
}