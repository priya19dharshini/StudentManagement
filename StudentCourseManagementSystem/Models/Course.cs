using System;
using System.Collections.Generic;
using System.Text;

namespace StudentCourseManagementSystem.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public int DurationMonths { get; set; }
        public decimal Fee { get; set; }
        public int DepartmentId { get; set; }
        public bool IsActive { get; set; }
    }

}
