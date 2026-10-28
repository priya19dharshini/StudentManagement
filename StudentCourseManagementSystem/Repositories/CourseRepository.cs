using Microsoft.Data.SqlClient;
using StudentCourseManagementSystem.Helpers;
using StudentCourseManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StudentCourseManagementSystem.Repositories
{
    public class CourseRepository
    {
        public async Task<int> AddAsync(Course course)
        {
            const string sql = """
            INSERT INTO Courses
                (CourseName, DurationMonths, Fee, DepartmentId)
            VALUES
                (@Name, @Duration, @Fee, @DepartmentId);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

            await using SqlConnection connection =
                DatabaseHelper.CreateConnection();
            await using SqlCommand command = new(sql, connection);

            command.Parameters.Add("@Name", SqlDbType.NVarChar, 100)
                .Value = course.CourseName;
            command.Parameters.Add("@Duration", SqlDbType.Int)
                .Value = course.DurationMonths;
            command.Parameters.Add("@Fee", SqlDbType.Decimal)
                .Value = course.Fee;
            command.Parameters["@Fee"].Precision = 10;
            command.Parameters["@Fee"].Scale = 2;
            command.Parameters.Add("@DepartmentId", SqlDbType.Int)
                .Value = course.DepartmentId;

            await connection.OpenAsync();

            return Convert.ToInt32(await command.ExecuteScalarAsync());
        }

        public async Task<List<Course>> GetAllAsync()
        {
            const string sql = """
            SELECT CourseId, CourseName, DurationMonths,
                   Fee, DepartmentId, IsActive
            FROM Courses
            ORDER BY CourseName;
            """;

            var courses = new List<Course>();

            await using SqlConnection connection =
                DatabaseHelper.CreateConnection();
            await using SqlCommand command = new(sql, connection);

            await connection.OpenAsync();

            await using SqlDataReader reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                courses.Add(new Course
                {
                    CourseId = reader.GetInt32(0),
                    CourseName = reader.GetString(1),
                    DurationMonths = reader.GetInt32(2),
                    Fee = reader.GetDecimal(3),
                    DepartmentId = reader.GetInt32(4),
                    IsActive = reader.GetBoolean(5)
                });
            }

            return courses;
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            const string sql = """
            SELECT CourseId, CourseName, DurationMonths,
                   Fee, DepartmentId, IsActive
            FROM Courses
            WHERE CourseId = @Id;
            """;

            await using SqlConnection connection =
                DatabaseHelper.CreateConnection();
            await using SqlCommand command = new(sql, connection);

            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();

            await using SqlDataReader reader =
                await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new Course
            {
                CourseId = reader.GetInt32(0),
                CourseName = reader.GetString(1),
                DurationMonths = reader.GetInt32(2),
                Fee = reader.GetDecimal(3),
                DepartmentId = reader.GetInt32(4),
                IsActive = reader.GetBoolean(5)
            };
        }

        public async Task<List<Course>> GetByDepartmentAsync(int departmentId)
        {
            const string sql = """
            SELECT CourseId, CourseName, DurationMonths,
                   Fee, DepartmentId, IsActive
            FROM Courses
            WHERE DepartmentId = @DepartmentId
            ORDER BY CourseName;
            """;

            var courses = new List<Course>();

            await using SqlConnection connection =
                DatabaseHelper.CreateConnection();
            await using SqlCommand command = new(sql, connection);

            command.Parameters.Add("@DepartmentId", SqlDbType.Int)
                .Value = departmentId;

            await connection.OpenAsync();

            await using SqlDataReader reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                courses.Add(new Course
                {
                    CourseId = reader.GetInt32(0),
                    CourseName = reader.GetString(1),
                    DurationMonths = reader.GetInt32(2),
                    Fee = reader.GetDecimal(3),
                    DepartmentId = reader.GetInt32(4),
                    IsActive = reader.GetBoolean(5)
                });
            }

            return courses;
        }

        public async Task<bool> UpdateAsync(Course course)
        {
            const string sql = """
            UPDATE Courses
            SET CourseName = @Name,
                DurationMonths = @Duration,
                Fee = @Fee,
                DepartmentId = @DepartmentId
            WHERE CourseId = @Id;
            """;

            await using SqlConnection connection =
                DatabaseHelper.CreateConnection();
            await using SqlCommand command = new(sql, connection);

            command.Parameters.Add("@Id", SqlDbType.Int)
                .Value = course.CourseId;
            command.Parameters.Add("@Name", SqlDbType.NVarChar, 100)
                .Value = course.CourseName;
            command.Parameters.Add("@Duration", SqlDbType.Int)
                .Value = course.DurationMonths;
            command.Parameters.Add("@Fee", SqlDbType.Decimal)
                .Value = course.Fee;
            command.Parameters["@Fee"].Precision = 10;
            command.Parameters["@Fee"].Scale = 2;
            command.Parameters.Add("@DepartmentId", SqlDbType.Int)
                .Value = course.DepartmentId;

            await connection.OpenAsync();

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            const string sql = """
            UPDATE Courses
            SET IsActive = 0
            WHERE CourseId = @Id;
            """;

            await using SqlConnection connection =
                DatabaseHelper.CreateConnection();
            await using SqlCommand command = new(sql, connection);

            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            await connection.OpenAsync();

            return await command.ExecuteNonQueryAsync() > 0;
        }
    }
}
