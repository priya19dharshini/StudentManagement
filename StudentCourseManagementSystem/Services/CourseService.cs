using StudentCourseManagementSystem.Models;
using StudentCourseManagementSystem.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentCourseManagementSystem.Services
{

    public class CourseService
    {
        private readonly CourseRepository _repository;
        private readonly DepartmentRepository _departments;

        public CourseService(
            CourseRepository repository,
            DepartmentRepository departments)
        {
            _repository = repository;
            _departments = departments;
        }

        public async Task<int> AddAsync(Course course)
        {
            ValidateCourse(course);

            var department =
                await _departments.GetByIdAsync(course.DepartmentId);

            if (department is null || !department.IsActive)
                throw new ArgumentException(
                    "Department does not exist or is inactive.");

            course.CourseName = course.CourseName.Trim();

            return await _repository.AddAsync(course);
        }

        public Task<List<Course>> GetAllAsync()
            => _repository.GetAllAsync();

        public async Task<Course?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid course ID.");

            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<Course>> GetByDepartmentAsync(int departmentId)
        {
            var department =
                await _departments.GetByIdAsync(departmentId);

            if (department is null)
                throw new ArgumentException("Department not found.");

            return await _repository.GetByDepartmentAsync(departmentId);
        }

        public async Task<bool> UpdateAsync(Course course)
        {
            ValidateCourse(course);

            var department =
                await _departments.GetByIdAsync(course.DepartmentId);

            if (department is null || !department.IsActive)
                throw new ArgumentException(
                    "Department does not exist or is inactive.");

            course.CourseName = course.CourseName.Trim();

            return await _repository.UpdateAsync(course);
        }

        public Task<bool> DeactivateAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid course ID.");

            return _repository.DeactivateAsync(id);
        }

        private static void ValidateCourse(Course course)
        {
            if (course.CourseId < 0)
                throw new ArgumentException("Invalid course ID.");

            if (string.IsNullOrWhiteSpace(course.CourseName))
                throw new ArgumentException("Course name is required.");

            if (course.DurationMonths <= 0)
                throw new ArgumentException(
                    "Duration must be greater than zero.");

            if (course.Fee < 0)
                throw new ArgumentException("Fee cannot be negative.");

            if (course.DepartmentId <= 0)
                throw new ArgumentException("Select a valid department.");
        }
    }
}
