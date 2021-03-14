﻿using Dapper;
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
            var sql = $"INSERT INTO [dbo].[{entity.GetType().Name}]({cols})VALUES({values})";

            using(connection = new SqlConnection(connectionString))
            {
                connection.Open();
                int rows = await connection.ExecuteAsync(sql, entity);
                connection.Close();
            }
        }

        public async Task UpdateAsync<T>(T entity) where T : IStorable
        {
            PropertyInfo[] props = GetProperties(entity);
            var expressions = string.Join(",", props.Select(x => $"{x.Name} = @{x.Name}"));
            var sql = $"UPDATE [dbo].[{entity.GetType().Name}] SET {expressions} WHERE DiscordId = @DiscordId";

            using (connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    await connection.ExecuteAsync(sql, entity);
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
                        $"DELETE [dbo].[{entity.GetType().Name}]" +
                        $"WHERE Id = @Id",
                        new { entity.Id });
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<T> FindOneAsync<T>(string sql, object obj = null) where T : IStorable
        {
            using(connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    T result = await connection.QuerySingleAsync<T>(sql, obj);
                    return result;
                }
                catch { }
                finally
                {
                    connection.Close();
                }
                return default;
            }
        }

        public async Task<IEnumerable<T>> FindAsync<T>(string sql, object obj = null) where T : IStorable
        {
            using(connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    IEnumerable<T> result = await connection.QueryAsync<T>(sql, obj);
                    return result;
                }
                catch { }
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
                        $"SELECT * FROM [dbo].[{typeof(T).Name}]");
                    return result;
                }
                catch { }
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
                        $"SELECT * FROM [dbo].[{entity.GetType().Name}]" +
                        $"WHERE DiscordId = @DiscordId",
                        new { DiscordId = entity.DiscordId });
                    return true;
                }
                catch { }
                finally
                {
                    connection.Close();
                }
                return false;
            }
        }

        private PropertyInfo[] GetProperties<T>(T entity) where T : IStorable
        {
            Type type = entity.GetType();
            return type.GetProperties().Where(x => x.Name != "Id").ToArray();
        }
    }
}
