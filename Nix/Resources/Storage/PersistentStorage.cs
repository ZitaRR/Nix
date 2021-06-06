using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace Nix.Resources
{
    internal sealed class PersistentStorage : IPersistentStorage
    {
        private readonly ILogger logger;
        private readonly string connectionString;
        private IDbConnection connection;

        public PersistentStorage(ILogger logger)
        {
            this.logger = logger;
            connectionString = Program.ConnectionString();

            logger.AppendLog("DATABASE", $"Database initialized [{connectionString}]");
        }

        public async Task InsertAsync<T>(T entity) where T : IStorable
        {
            entity.StoredAt = DateTime.UtcNow;
            PropertyInfo[] props = GetProperties(entity);
            var cols = string.Join(",", props.Select(x => x.Name));
            var values = string.Join(",", props.Select(x => $"@{x.Name}"));
            var sql = $"INSERT INTO {entity.GetType().Name} " +
                $"({cols})VALUES({values})";

            using(connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    await connection.ExecuteAsync(sql, entity);
                }
                catch (Exception e)
                {
                    logger.AppendLog(e.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task UpdateAsync<T>(T entity) where T : IStorable
        {
            PropertyInfo[] props = GetProperties(entity);
            var expressions = string.Join(",", props.Select(x => $"{x.Name} = @{x.Name}"));
            var sql = $"UPDATE {entity.GetType().Name} " +
                $"SET {expressions} " +
                $"WHERE Id = @Id";

            using (connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    await connection.ExecuteAsync(sql, entity);
                }
                catch (Exception e)
                {
                    logger.AppendLog(e.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task DeleteAsync<T>(T entity) where T : IStorable
        {
            using (connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    await connection.ExecuteAsync(
                        $"DELETE {entity.GetType().Name} " +
                        $"WHERE Id = @Id", entity);
                }
                catch (Exception e)
                {
                    logger.AppendLog(e.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<T> FindOneAsync<T>(object param) where T : IStorable
        {
            using(connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    T result = (await FindAsync<T>(param)).FirstOrDefault();
                    return result;
                }
                catch (Exception e)
                {
                    logger.AppendLog(e.Message);
                }
                finally
                {
                    connection.Close();
                }
                return default;
            }
        }

        public async Task<IEnumerable<T>> FindAsync<T>(object param) where T : IStorable
        {
            PropertyInfo[] props = GetProperties(param);
            var conditions = string.Join(" AND ", props.Select(x => $"{x.Name} = @{x.Name}"));
            var sql = $"SELECT * FROM {typeof(T).Name} " +
                $"WHERE {conditions}";

            using(connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    IEnumerable<T> result = await connection.QueryAsync<T>(sql, param);
                    return result;
                }
                catch (Exception e)
                {
                    logger.AppendLog(e.Message);
                }
                finally
                {
                    connection.Close();
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
                    connection.Open();
                    IEnumerable<T> result = await connection.QueryAsync<T>(
                        $"SELECT * FROM {typeof(T).Name}");
                    return result;
                }
                catch (Exception e)
                {
                    logger.AppendLog(e.Message);
                }
                finally
                {
                    connection.Close();
                }
                return default;
            }
        }

        public async Task<bool> ExistsAsync<T>(T entity) where T : IStorable
        {
            using (connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {
                    var result = await connection.QuerySingleAsync<T>(
                        $"SELECT * FROM {typeof(T).Name} " +
                        $"WHERE DiscordId = @DiscordId", entity);
                    return true;
                }
                catch (Exception e)
                {
                    logger.AppendLog(e.Message);
                }
                finally
                {
                    connection.Close();
                }
                return false;
            }
        }

        private PropertyInfo[] GetProperties(object obj)
        {
            Type type = obj.GetType();
            return type.GetProperties().Where(x => x.Name != "Id").ToArray();
        }
    }
}
