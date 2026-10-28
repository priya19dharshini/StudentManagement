using Microsoft.Data.SqlClient;
using StudentCourseManagementSystem.Helpers;
using StudentCourseManagementSystem.Models;
using System.Data;

namespace StudentCourseManagementSystem.Repositories;

public class StudentRepository
{
    // 1. Register a student
    public async Task<int> AddAsync(Student student)
    {
        const string sql = """
            INSERT INTO Students (Name, Email, Age, DepartmentId)
            VALUES (@Name, @Email, @Age, @DepartmentId);

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100)
            .Value = student.Name;

        command.Parameters.Add("@Email", SqlDbType.NVarChar, 255)
            .Value = student.Email;

        command.Parameters.Add("@Age", SqlDbType.Int)
            .Value = student.Age;

        command.Parameters.Add("@DepartmentId", SqlDbType.Int)
            .Value = student.DepartmentId;

        await connection.OpenAsync();

        object? result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    // 2. Get all students
    public async Task<List<Student>> GetAllAsync()
    {
        const string sql = """
            SELECT StudentId, Name, Email, Age,
                   DepartmentId, AdmissionDate, IsActive
            FROM Students
            ORDER BY StudentId;
            """;

        var students = new List<Student>();

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        await connection.OpenAsync();

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            students.Add(MapStudent(reader));
        }

        return students;
    }

    // 3. Search student by ID
    public async Task<Student?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT StudentId, Name, Email, Age,
                   DepartmentId, AdmissionDate, IsActive
            FROM Students
            WHERE StudentId = @Id;
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await connection.OpenAsync();

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return MapStudent(reader);
        }

        return null;
    }

    // 4. Search students by partial name
    public async Task<List<Student>> SearchByNameAsync(string name)
    {
        const string sql = """
            SELECT StudentId, Name, Email, Age,
                   DepartmentId, AdmissionDate, IsActive
            FROM Students
            WHERE Name LIKE @Name
            ORDER BY Name;
            """;

        var students = new List<Student>();

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@Name", SqlDbType.NVarChar, 202)
            .Value = $"%{name}%";

        await connection.OpenAsync();

        await using SqlDataReader reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            students.Add(MapStudent(reader));
        }

        return students;
    }

    // 5. Get students belonging to a department
    public async Task<List<Student>> GetByDepartmentAsync(
        int departmentId)
    {
        const string sql = """
            SELECT StudentId, Name, Email, Age,
                   DepartmentId, AdmissionDate, IsActive
            FROM Students
            WHERE DepartmentId = @DepartmentId
            ORDER BY Name;
            """;

        var students = new List<Student>();

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
            students.Add(MapStudent(reader));
        }

        return students;
    }

    // 6. Update student details
    public async Task<bool> UpdateAsync(Student student)
    {
        const string sql = """
            UPDATE Students
            SET Name = @Name,
                Email = @Email,
                Age = @Age,
                DepartmentId = @DepartmentId
            WHERE StudentId = @Id;
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@Id", SqlDbType.Int)
            .Value = student.StudentId;

        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100)
            .Value = student.Name;

        command.Parameters.Add("@Email", SqlDbType.NVarChar, 255)
            .Value = student.Email;

        command.Parameters.Add("@Age", SqlDbType.Int)
            .Value = student.Age;

        command.Parameters.Add("@DepartmentId", SqlDbType.Int)
            .Value = student.DepartmentId;

        await connection.OpenAsync();

        int rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }

    // 7. Deactivate student instead of deleting
    public async Task<bool> DeactivateAsync(int id)
    {
        const string sql = """
            UPDATE Students
            SET IsActive = 0
            WHERE StudentId = @Id
              AND IsActive = 1;
            """;

        await using SqlConnection connection =
            DatabaseHelper.CreateConnection();

        await using SqlCommand command = new(sql, connection);

        command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

        await connection.OpenAsync();

        int rowsAffected = await command.ExecuteNonQueryAsync();

        return rowsAffected > 0;
    }

    // Shared method to convert a database row into a Student object
    private static Student MapStudent(SqlDataReader reader)
    {
        return new Student
        {
            StudentId = reader.GetInt32(0),
            Name = reader.GetString(1),
            Email = reader.GetString(2),
            Age = reader.GetInt32(3),
            DepartmentId = reader.GetInt32(4),
            AdmissionDate = reader.GetDateTime(5),
            IsActive = reader.GetBoolean(6)
        };
    }
}
