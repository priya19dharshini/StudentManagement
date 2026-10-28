using Microsoft.Data.SqlClient;
using StudentCourseManagementSystem.Helpers;
using StudentCourseManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StudentCourseManagementSystem.Repositories
{

    public class EnrollmentRepository
    {
        public async Task<int> AddAsync(int studentId, int courseId)
        {
            await using SqlConnection connection =
                DatabaseHelper.CreateConnection();

            await connection.OpenAsync();

            using SqlTransaction transaction =
                (SqlTransaction)await connection.BeginTransactionAsync();

            try
            {
                const string sql = """
                INSERT INTO Enrollments (StudentId, CourseId, Status)
                VALUES (@StudentId, @CourseId, N'Enrolled');
                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """;

                int enrollmentId;

                await using (SqlCommand command =
                    new(sql, connection, transaction))
                {
                    command.Parameters.Add("@StudentId", SqlDbType.Int)
                        .Value = studentId;
                    command.Parameters.Add("@CourseId", SqlDbType.Int)
                        .Value = courseId;

                    enrollmentId = Convert.ToInt32(
                        await command.ExecuteScalarAsync());
                }

                const string auditSql = """
                INSERT INTO AuditLogs
                    (EntityName, EntityId, Action, Details)
                VALUES
                    (@EntityName, @EntityId, @Action, @Details);
                """;

                await using (SqlCommand command =
                    new(auditSql, connection, transaction))
                {
                    command.Parameters.Add("@EntityName",
                        SqlDbType.NVarChar, 50).Value = "Enrollment";
                    command.Parameters.Add("@EntityId",
                        SqlDbType.Int).Value = enrollmentId;
                    command.Parameters.Add("@Action",
                        SqlDbType.NVarChar, 20).Value = "INSERT";
                    command.Parameters.Add("@Details",
                        SqlDbType.NVarChar, 500).Value =
                        $"Student {studentId} enrolled in course {courseId}.";

                    await command.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();

                return enrollmentId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Enrollment>> GetAllAsync()
        {
            const string sql = """
            SELECT EnrollmentId, StudentId, CourseId,
                   EnrolledAt, Status
            FROM Enrollments
            ORDER BY EnrolledAt DESC;
            """;

            var enrollments = new List<Enrollment>();

            await using SqlConnection connection =
                DatabaseHelper.CreateConnection();
            await using SqlCommand command = new(sql, connection);

            await connection.OpenAsync();

            await using SqlDataReader reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                enrollments.Add(new Enrollment
                {
                    EnrollmentId = reader.GetInt32(0),
                    StudentId = reader.GetInt32(1),
                    CourseId = reader.GetInt32(2),
                    EnrolledAt = reader.GetDateTime(3),
                    Status = reader.GetString(4)
                });
            }

            return enrollments;
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            const string sql = """
            UPDATE Enrollments
            SET Status = @Status
            WHERE EnrollmentId = @Id;
            """;

            await using SqlConnection connection =
                DatabaseHelper.CreateConnection();
            await using SqlCommand command = new(sql, connection);

            command.Parameters.Add("@Status", SqlDbType.NVarChar, 20)
                .Value = status;
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();

            return await command.ExecuteNonQueryAsync() > 0;
        }
    }
}
