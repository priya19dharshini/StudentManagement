using Microsoft.Data.SqlClient;
using StudentCourseManagementSystem.Helpers;
using StudentCourseManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StudentCourseManagementSystem.Repositories
{
    public class AuditLogRepository
    {
        public async Task AddAsync(
            string entityName,
            int entityId,
            string action,
            string? details)
        {
            const string sql = """
            INSERT INTO AuditLogs
                (EntityName, EntityId, Action, Details)
            VALUES
                (@EntityName, @EntityId, @Action, @Details);
            """;

            await using SqlConnection connection =
                DatabaseHelper.CreateConnection();
            await using SqlCommand command = new(sql, connection);

            command.Parameters.Add("@EntityName", SqlDbType.NVarChar, 50)
                .Value = entityName;
            command.Parameters.Add("@EntityId", SqlDbType.Int)
                .Value = entityId;
            command.Parameters.Add("@Action", SqlDbType.NVarChar, 20)
                .Value = action;
            command.Parameters.Add("@Details", SqlDbType.NVarChar, 500)
                .Value = (object?)details ?? DBNull.Value;

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<List<AuditLog>> GetRecentAsync(int count = 10)
        {
            const string sql = """
            SELECT TOP (@Count)
                AuditId, EntityName, EntityId, Action, Details, CreatedAt
            FROM AuditLogs
            ORDER BY CreatedAt DESC, AuditId DESC;
            """;

            var logs = new List<AuditLog>();

            await using SqlConnection connection =
                DatabaseHelper.CreateConnection();
            await using SqlCommand command = new(sql, connection);

            command.Parameters.Add("@Count", SqlDbType.Int)
                .Value = Math.Clamp(count, 1, 100);

            await connection.OpenAsync();

            await using SqlDataReader reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                logs.Add(new AuditLog
                {
                    AuditId = reader.GetInt64(0),
                    EntityName = reader.GetString(1),
                    EntityId = reader.GetInt32(2),
                    Action = reader.GetString(3),
                    Details = reader.IsDBNull(4)
                        ? null : reader.GetString(4),
                    CreatedAt = reader.GetDateTime(5)
                });
            }

            return logs;
        }
    }
}
