
using Microsoft.Data.SqlClient;
using StudentCourseManagementSystem.Helpers;
using StudentCourseManagementSystem.Models;
using System.Data;

namespace StudentCourseManagementSystem.Repositories;

public class DepartmentRepository
{
    public async Task<int> AddAsync(Department department)
    {
        const string sql = """
            INSERT INTO Departments (DepartmentName, Location)
            VALUES (@Name, @Location);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();
        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100)
            .Value = department.DepartmentName;

        command.Parameters.Add("@Location", SqlDbType.NVarChar, 100)
            .Value = (object?)department.Location ?? DBNull.Value;

        await connection.OpenAsync();

        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task<List<Department>> GetAllAsync()
    {
        const string sql = """
            SELECT DepartmentId, DepartmentName, Location, IsActive
            FROM Departments
            ORDER BY DepartmentName;
            """;

        var departments = new List<Department>();

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();
        await using SqlCommand command = new(sql, connection);

        await connection.OpenAsync();

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            departments.Add(new Department
            {
                DepartmentId = reader.GetInt32(0),
                DepartmentName = reader.GetString(1),
                Location = reader.IsDBNull(2)
                    ? null : reader.GetString(2),
                IsActive = reader.GetBoolean(3)
            });
        }

        return departments;
    }

    public async Task<Department?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT DepartmentId, DepartmentName, Location, IsActive
            FROM Departments
            WHERE DepartmentId = @Id;
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

        return new Department
        {
            DepartmentId = reader.GetInt32(0),
            DepartmentName = reader.GetString(1),
            Location = reader.IsDBNull(2)
                ? null : reader.GetString(2),
            IsActive = reader.GetBoolean(3)
        };
    }

    public async Task<bool> UpdateAsync(Department department)
    {
        const string sql = """
            UPDATE Departments
            SET DepartmentName = @Name,
                Location = @Location
            WHERE DepartmentId = @Id;
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();
        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@Id", SqlDbType.Int)
            .Value = department.DepartmentId;
        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100)
            .Value = department.DepartmentName;
        command.Parameters.Add("@Location", SqlDbType.NVarChar, 100)
            .Value = (object?)department.Location ?? DBNull.Value;

        await connection.OpenAsync();

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        const string sql = """
            UPDATE Departments
            SET IsActive = 0
            WHERE DepartmentId = @Id;
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();
        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await connection.OpenAsync();

        return await command.ExecuteNonQueryAsync() > 0;
    }
}
