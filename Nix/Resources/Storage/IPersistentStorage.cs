﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nix.Resources
{
    public interface IPersistentStorage
    {
        Task<IEnumerable<T>> FindAllAsync<T>() where T : IStorable;
        Task<IEnumerable<T>> FindAsync<T>(string sql, object obj = null) where T : IStorable;
        Task<T> FindOneAsync<T>(string sql, object obj = null) where T : IStorable;
        Task InsertAsync<T>(T entity) where T : IStorable;
        Task UpdateAsync<T>(T entity) where T : IStorable;
        Task DeleteAsync<T>(T entity) where T : IStorable;
        Task<bool> ExistsAsync<T>(T entity) where T : IStorable;
    }
}
