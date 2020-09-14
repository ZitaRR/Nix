using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace Nix.Resources
{
    internal sealed class PersistentStorage : IPersistentStorage
    {
        private readonly LiteDatabase context;
        private readonly string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "\\Nix.db";
        private readonly ILogger logger;

        public PersistentStorage(ILogger logger)
        {
            this.logger = logger;
            context = new LiteDatabase(path);
            logger.AppendLog("DATABASE", $"Database initialized [{path}]");
        }

        public void Store<T>(T entity)
        {
            try 
            {
                var collection = context.GetCollection<T>();
                collection.Insert(entity);
                logger.AppendLog("DATABASE", $"Entity of type {entity.GetType()} has been stored");
            }
            catch (Exception e)
            {
                logger.AppendLog("DATABASE", e.Message);
            }
        }

        public void Delete<T>(Expression<Func<T, bool>> predicate)
        {
            var collection = context.GetCollection<T>();
            collection.DeleteMany(predicate);
        }

        public IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate)
        {
            var collection = context.GetCollection<T>();
            return collection.Find(predicate);
        }

        public void Update<T>(T entity)
        {
            var collection = context.GetCollection<T>();
            collection.Update(entity);
        }
    }
}
