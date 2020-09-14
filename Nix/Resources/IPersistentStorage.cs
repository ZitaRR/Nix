using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nix.Resources
{
    interface IPersistentStorage
    {
        IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate);
        void Store<T>(T entity);
        void Update<T>(T entity);
        void Delete<T>(Expression<Func<T, bool>> predicate);
    }
}
