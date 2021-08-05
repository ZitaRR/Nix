using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using Nix.MVC; 

namespace Nix.Resources
{
    public sealed class PersistentStorage : IPersistentStorage
    {
        private readonly ILogger logger;
        private readonly string connectionString;
        private IDbConnection connection;

        public PersistentStorage(ILogger logger)
        {
            this.logger = logger;
            connectionString = Utility.ConnectionString;

            logger.AppendLog("DATABASE", $"Database initialized [{connectionString}]");
        }

        public async Task InsertAsync<T>(T entity) where T : IStorable
        {
            entity.StoredAt = DateTime.UtcNow;
            PropertyInfo[] props = GetProperties(entity);
            string cols = string.Join(",", props.Select(x => x.Name));
            string values = string.Join(",", props.Select(x => $"@{x.Name}"));
            string sql = $"INSERT INTO {typeof(T).Name} " +
                $"({cols})VALUES({values})";

            using(connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.ExecuteAsync(sql, entity);
                }
                catch (Exception e)
                {
                    logger.AppendLog(e.Message);
                }
            }
        }

        public async Task UpdateAsync<T>(T entity) where T : IStorable
        {
            PropertyInfo[] props = GetProperties(entity);
            string expressions = string.Join(",", props.Select(x => $"{x.Name} = @{x.Name}"));
            string sql = $"UPDATE {typeof(T).Name} " +
                $"SET {expressions} " +
                $"WHERE Id = @Id";

            if (!(entity is NixGuild))
            {
                sql += " AND GuildId = @GuildId";
            }

            using (connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.ExecuteAsync(sql, entity);
                }
                catch (Exception e)
                {
                    logger.AppendLog(e.Message);
                }
            }
        }

        public async Task DeleteAsync<T>(object param) where T : IStorable
        {
            PropertyInfo[] props = GetProperties(param);
            string conditions = string.Join(" AND ", props.Select(x => $"{x.Name} = @{x.Name}"));
            string sql = $"DELETE * FROM {typeof(T).Name} " +
                $"WHERE {conditions}";

            using (connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.ExecuteAsync(sql, param);
                }
                catch (Exception e)
                {
                    logger.AppendLog(e.Message);
                }
            }
        }

        public async Task<T> FindOneAsync<T>(object param) where T : IStorable
        {
            using(connection = new SqlConnection(connectionString))
            {
                try
                {
                    T result = (await FindAsync<T>(param)).FirstOrDefault();
                    return result;
                }
                catch (Exception e)
                {
                    logger.AppendLog(e.Message);
                }
                return default;
            }
        }

        public async Task<IEnumerable<T>> FindAsync<T>(object param) where T : IStorable
        {
            PropertyInfo[] props = GetProperties(param);
            string conditions = string.Join(" AND ", props.Select(x => $"{x.Name} = @{x.Name}"));
            string sql = $"SELECT * FROM {typeof(T).Name} " +
                $"WHERE {conditions}";

            using(connection = new SqlConnection(connectionString))
            {
                try
                {
                    IEnumerable<T> result = await connection.QueryAsync<T>(sql, param);
                    return result;
                }
                catch (Exception e)
                {
                    logger.AppendLog(e.Message);
                }
                return default;
            }
        }

        public async Task<IEnumerable<T>> FindAllAsync<T>() where T : IStorable
        {
            using (connection = new SqlConnection(connectionString))
            {
                try
                {
                    IEnumerable<T> result = await connection.QueryAsync<T>(
                        $"SELECT * FROM {typeof(T).Name}");
                    return result;
                }
                catch (Exception e)
                {
                    logger.AppendLog(e.Message);
                }
                return default;
            }
        }

        public async Task<bool> ExistsAsync<T>(object param) where T : IStorable
        {
            PropertyInfo[] props = GetProperties(param);
            string conditions = string.Join(" AND ", props.Select(x => $"{x.Name} = @{x.Name}"));
            string sql = $"SELECT * FROM {typeof(T).Name} " +
                $"WHERE {conditions}";

            using (connection = new SqlConnection(connectionString))
            {
                try
                {
                    var result = await connection.QuerySingleOrDefaultAsync<T>(sql, param);
                    return result != null;
                }
                catch (Exception e)
                {
                    logger.AppendLog(e.Message);
                }
                return false;
            }
        }

        private PropertyInfo[] GetProperties(object obj)
        {
            Type type = obj.GetType();
            return type.GetProperties();
        }
    }
}
