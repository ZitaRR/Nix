using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public void Store<T>(T entity) where T : IStorable
        {
            if (Exists<T>(x => x.ID == entity.ID))
            {
                logger.AppendLog("DATABASE", $"Entity already exists");
                return;
            }

            var collection = context.GetCollection<T>();
            collection.Insert(entity);
            logger.AppendLog("DATABASE", $"Entity of type {entity.GetType().Name} has been stored");
        }

        public void Delete<T>(Expression<Func<T, bool>> predicate) where T : IStorable
        {
            try
            {
                var collection = context.GetCollection<T>();
                collection.DeleteMany(predicate);
            }
            catch (LiteException e)
            {
                logger.AppendLog(e);
            }
        }

        public IEnumerable<T> Find<T>(Expression<Func<T, bool>> predicate) where T : IStorable
        {
            var collection = context.GetCollection<T>();
            return collection.Find(predicate);
        }

        public IEnumerable<T> FindAll<T>() where T : IStorable
        {
            var collection = context.GetCollection<T>();
            return collection.FindAll();
        }

        public T FindOne<T>(Expression<Func<T, bool>> predicate) where T : IStorable
            => Find(predicate).FirstOrDefault();

        public void Update<T>(T entity) where T : IStorable
        {
            var collection = context.GetCollection<T>();
            collection.Update(entity);
        }

        public bool Exists<T>(Expression<Func<T, bool>> predicate) where T : IStorable
        {
            var collection = context.GetCollection<T>();
            return collection.Exists(predicate);
        }
    }
}
