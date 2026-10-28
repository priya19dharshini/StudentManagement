using StudentCourseManagementSystem.Models;
using StudentCourseManagementSystem.Repositories;

namespace StudentCourseManagementSystem.Services;

public class StudentService
{
    private readonly StudentRepository _repository;
    private readonly DepartmentRepository _departmentRepository;

    public StudentService(
        StudentRepository repository,
        DepartmentRepository departmentRepository)
    {
        _repository = repository;
        _departmentRepository = departmentRepository;
    }

    // 1. Register student
    public async Task<int> RegisterAsync(Student student)
    {
        ValidateStudent(student);

        var department =
            await _departmentRepository.GetByIdAsync(
                student.DepartmentId);

        if (department is null || !department.IsActive)
        {
            throw new ArgumentException(
                "Department does not exist or is inactive.");
        }

        student.Name = student.Name.Trim();
        student.Email = student.Email.Trim();

        return await _repository.AddAsync(student);
    }

    // 2. Get all students
    public Task<List<Student>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    // 3. Search student by ID
    public async Task<Student?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid student ID.");
        }

        return await _repository.GetByIdAsync(id);
    }

    // 4. Search students by name
    public async Task<List<Student>> SearchByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Search name cannot be empty.");
        }

        return await _repository.SearchByNameAsync(name.Trim());
    }

    // 5. Filter students by department
    public async Task<List<Student>> GetByDepartmentAsync(
        int departmentId)
    {
        if (departmentId <= 0)
        {
            throw new ArgumentException("Invalid department ID.");
        }

        var department =
            await _departmentRepository.GetByIdAsync(departmentId);

        if (department is null)
        {
            throw new ArgumentException("Department not found.");
        }

        return await _repository.GetByDepartmentAsync(departmentId);
    }

    // 6. Update student
    public async Task<bool> UpdateAsync(Student student)
    {
        if (student.StudentId <= 0)
        {
            throw new ArgumentException("Invalid student ID.");
        }

        ValidateStudent(student);

        var existingStudent =
            await _repository.GetByIdAsync(student.StudentId);

        if (existingStudent is null)
        {
            throw new ArgumentException("Student not found.");
        }

        var department =
            await _departmentRepository.GetByIdAsync(
                student.DepartmentId);

        if (department is null || !department.IsActive)
        {
            throw new ArgumentException(
                "Department does not exist or is inactive.");
        }

        student.Name = student.Name.Trim();
        student.Email = student.Email.Trim();

        return await _repository.UpdateAsync(student);
    }

    // 7. Deactivate student
    public async Task<bool> DeactivateAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid student ID.");
        }

        var student = await _repository.GetByIdAsync(id);

        if (student is null)
        {
            throw new ArgumentException("Student not found.");
        }

        if (!student.IsActive)
        {
            throw new ArgumentException(
                "Student is already inactive.");
        }

        return await _repository.DeactivateAsync(id);
    }

    // Shared validation
    private static void ValidateStudent(Student student)
    {
        if (string.IsNullOrWhiteSpace(student.Name))
        {
            throw new ArgumentException(
                "Student name is required.");
        }

        if (string.IsNullOrWhiteSpace(student.Email))
        {
            throw new ArgumentException(
                "Student email is required.");
        }

        if (student.Age is < 17 or > 60)
        {
            throw new ArgumentException(
                "Age must be between 17 and 60.");
        }

        if (student.DepartmentId <= 0)
        {
            throw new ArgumentException(
                "Select a valid department.");
        }
    }
}
