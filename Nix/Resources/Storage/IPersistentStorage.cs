using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nix.Resources
{
    public interface IPersistentStorage
    {
        IEnumerable<T> FindAll<T>() where T : IStorable;
        IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : IStorable;
        T FindOne<T>(Expression<Func<T, bool>> predicate) where T : IStorable;
        void Store<T>(T entity) where T : IStorable;
        void Update<T>(T entity) where T : IStorable;
        void Delete<T>(Expression<Func<T, bool>> predicate) where T : IStorable;
        bool Exists<T>(Expression<Func<T, bool>> predicate) where T : IStorable;
    }
}
