using System;
using System.Collections.Generic;
using System.Text;

namespace StudentCourseManagementSystem.Models
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public string Status { get; set; } = "Enrolled";
    }

}
