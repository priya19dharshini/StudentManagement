
using StudentCourseManagementSystem.Models;
using StudentCourseManagementSystem.Repositories;

namespace StudentCourseManagementSystem.Services;

public class DepartmentService
{
    private readonly DepartmentRepository _repository;

    public DepartmentService(DepartmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> AddAsync(Department department)
    {
        if (string.IsNullOrWhiteSpace(department.DepartmentName))
            throw new ArgumentException("Department name is required.");

        department.DepartmentName =
            department.DepartmentName.Trim();

        department.Location =
            string.IsNullOrWhiteSpace(department.Location)
                ? null : department.Location.Trim();

        return await _repository.AddAsync(department);
    }

    public Task<List<Department>> GetAllAsync()
        => _repository.GetAllAsync();

    public Task<Department?> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid department ID.");

        return _repository.GetByIdAsync(id);
    }

    public async Task<bool> UpdateAsync(Department department)
    {
        if (department.DepartmentId <= 0)
            throw new ArgumentException("Invalid department ID.");

        if (string.IsNullOrWhiteSpace(department.DepartmentName))
            throw new ArgumentException("Department name is required.");

        department.DepartmentName =
            department.DepartmentName.Trim();

        return await _repository.UpdateAsync(department);
    }

    public Task<bool> DeactivateAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid department ID.");

        return _repository.DeactivateAsync(id);
    }
}
