using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public interface IPersistentStorage
    {
        Task<IEnumerable<T>> FindAllAsync<T>() where T : IStorable;
        Task<IEnumerable<T>> FindAsync<T>(object param) where T : IStorable;
        Task<T> FindOneAsync<T>(object param) where T : IStorable;
        Task InsertAsync<T>(T entity) where T : IStorable;
        Task UpdateAsync<T>(T entity) where T : IStorable;
        Task DeleteAsync<T>(object param) where T : IStorable;
        Task<bool> ExistsAsync<T>(object param) where T : IStorable;
    }
}
