using StudentCourseManagementSystem.Models;
using StudentCourseManagementSystem.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentCourseManagementSystem.Services
{

    public class EnrollmentService
    {
        private readonly EnrollmentRepository _repository;
        private readonly StudentRepository _students;
        private readonly CourseRepository _courses;

        public EnrollmentService(
            EnrollmentRepository repository,
            StudentRepository students,
            CourseRepository courses)
        {
            _repository = repository;
            _students = students;
            _courses = courses;
        }

        public async Task<int> EnrollAsync(int studentId, int courseId)
        {
            if (studentId <= 0 || courseId <= 0)
                throw new ArgumentException("Invalid student or course ID.");

            Student? student = await _students.GetByIdAsync(studentId);
            Course? course = await _courses.GetByIdAsync(courseId);

            if (student is null || !student.IsActive)
                throw new ArgumentException(
                    "Student does not exist or is inactive.");

            if (course is null || !course.IsActive)
                throw new ArgumentException(
                    "Course does not exist or is inactive.");

            return await _repository.AddAsync(studentId, courseId);
        }

        public Task<List<Enrollment>> GetAllAsync()
            => _repository.GetAllAsync();

        public async Task<bool> CompleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid enrollment ID.");

            return await _repository.UpdateStatusAsync(id, "Completed");
        }

        public async Task<bool> CancelAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid enrollment ID.");

            return await _repository.UpdateStatusAsync(id, "Cancelled");
        }
    }
}
