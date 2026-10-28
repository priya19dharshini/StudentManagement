using StudentCourseManagementSystem.Models;
using StudentCourseManagementSystem.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentCourseManagementSystem.Services
{
    public class AuditLogService
    {
        private readonly AuditLogRepository _repository;

        public AuditLogService(AuditLogRepository repository)
        {
            _repository = repository;
        }

        public Task<List<AuditLog>> GetRecentAsync(int count = 10)
        {
            if (count <= 0)
                throw new ArgumentException("Count must be positive.");

            return _repository.GetRecentAsync(count);
        }

        public Task LogAsync(
            string entityName,
            int entityId,
            string action,
            string? details = null)
        {
            if (string.IsNullOrWhiteSpace(entityName))
                throw new ArgumentException("Entity name is required.");

            if (entityId <= 0)
                throw new ArgumentException("Invalid entity ID.");

            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException("Action is required.");

            return _repository.AddAsync(
                entityName.Trim(),
                entityId,
                action.Trim().ToUpperInvariant(),
                details);
        }
    }
}
