using Microsoft.Data.SqlClient;
using StudentCourseManagementSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace StudentCourseManagementSystem.Repositories
{
    public class ReportsRepository
    {
        public async Task<DataTable> GetEnrollmentCountsAsync()
        {
            const string sql = """
            SELECT c.CourseId, c.CourseName,
                   COUNT(e.EnrollmentId) AS EnrollmentCount
            FROM Courses AS c
            LEFT JOIN Enrollments AS e
                ON e.CourseId = c.CourseId
            GROUP BY c.CourseId, c.CourseName
            ORDER BY EnrollmentCount DESC;
            """;

            var table = new DataTable();

            await using SqlConnection connection =
                DatabaseHelper.CreateConnection();
            await using SqlCommand command = new(sql, connection);

            await connection.OpenAsync();

            await using SqlDataReader reader =
                await command.ExecuteReaderAsync();

            table.Load(reader);

            return table;
        }
    }
}
