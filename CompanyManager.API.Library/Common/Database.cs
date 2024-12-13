using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyManager.API.Library.Common;

public class Database
{
    
        private readonly string _connectionString;

        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<T>> LoadDataAsync<T, U>(string sql, U parameters)
        {
            string? connectionString = _connectionString;

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var rows = await connection.QueryAsync<T>(sql, parameters);
                return rows.ToList();
            }
        }

        public Task SaveData<T>(string sql, T parameters)
        {
            string? connectionString = _connectionString;

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                return connection.ExecuteAsync(sql, parameters);
            }
        }

        public async Task<List<T>> LoadDataAsyncSP<T, U>(string storedProcedure, U parameters)
        {
            string? connectionString = _connectionString;

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var rows = await connection.QueryAsync<T>(
                    storedProcedure,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return rows.ToList();
            }
        }

        public async Task<int> SaveDataSP<T>(string storedProcedure, T parameters)
        {
            string? connectionString = _connectionString;

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                var dynamicParameters = new DynamicParameters();

                foreach (var property in typeof(T).GetProperties())
                {
                    var value = property.GetValue(parameters);
                    dynamicParameters.Add($"@{property.Name}", value);
                }

                dynamicParameters.Add("ReturnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                // Wykonanie procedury
                await connection.ExecuteAsync(
                    storedProcedure,
                    dynamicParameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 120
                );

                // Pobieramy wartość zwróconą przez RETURN
                return dynamicParameters.Get<int>("ReturnValue");
            }
        }
}
