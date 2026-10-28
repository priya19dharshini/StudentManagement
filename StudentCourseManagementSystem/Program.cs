
using StudentCourseManagementSystem.Models;
using StudentCourseManagementSystem.Repositories;
using StudentCourseManagementSystem.Services;

namespace StudentCourseManagementSystem;

internal class Program
{
    static async Task Main(string[] args)
    {
        // Create repositories
        var departmentRepository = new DepartmentRepository();
        var studentRepository = new StudentRepository();
        var courseRepository = new CourseRepository();
        var enrollmentRepository = new EnrollmentRepository();
        var auditLogRepository = new AuditLogRepository();

        // Create services
        var departmentService =
            new DepartmentService(departmentRepository);

        var studentService = new StudentService(
            studentRepository,
            departmentRepository);

        var courseService = new CourseService(
            courseRepository,
            departmentRepository);

        var enrollmentService = new EnrollmentService(
            enrollmentRepository,
            studentRepository,
            courseRepository);

        var auditLogService =
            new AuditLogService(auditLogRepository);

        // Main menu
        while (true)
        {
            Console.WriteLine("\n==================================");
            Console.WriteLine(" STUDENT COURSE MANAGEMENT SYSTEM");
            Console.WriteLine("==================================");
            Console.WriteLine("1. Department Management");
            Console.WriteLine("2. Student Management");
            Console.WriteLine("3. Course Management");
            Console.WriteLine("4. Enrollment Management");
            Console.WriteLine("5. View Audit Logs");
            Console.WriteLine("0. Exit");
            Console.WriteLine("==================================");

            string choice = ReadText("Enter your choice: ");

            try
            {
                switch (choice)
                {
                    case "1":
                        await DepartmentMenu(departmentService);
                        break;

                    case "2":
                        await StudentMenu(studentService);
                        break;

                    case "3":
                        await CourseMenu(courseService);
                        break;

                    case "4":
                        await EnrollmentMenu(enrollmentService);
                        break;

                    case "5":
                        await ViewAuditLogs(auditLogService);
                        break;

                    case "0":
                        Console.WriteLine("Application closed.");
                        return;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Operation failed: {ex.Message}");
            }
        }
    }

    // ==========================================
    // DEPARTMENT MENU
    // ==========================================

    static async Task DepartmentMenu(DepartmentService service)
    {
        while (true)
        {
            Console.WriteLine("\n--- DEPARTMENT MANAGEMENT ---");
            Console.WriteLine("1. Add Department");
            Console.WriteLine("2. List Departments");
            Console.WriteLine("3. Search Department");
            Console.WriteLine("4. Update Department");
            Console.WriteLine("5. Deactivate Department");
            Console.WriteLine("0. Back to Main Menu");

            string choice = ReadText("Enter choice: ");

            try
            {
                switch (choice)
                {
                    case "1":
                        {
                            var department = new Department
                            {
                                DepartmentName =
                                    ReadText("Department name: "),
                                Location =
                                    ReadText("Location (optional): ")
                            };

                            int id = await service.AddAsync(department);
                            Console.WriteLine(
                                $"Department added successfully. ID: {id}");
                            break;
                        }

                    case "2":
                        {
                            var departments = await service.GetAllAsync();

                            foreach (var d in departments)
                            {
                                Console.WriteLine(
                                    $"{d.DepartmentId} | {d.DepartmentName} | " +
                                    $"{d.Location ?? "-"} | Active: {d.IsActive}");
                            }

                            if (departments.Count == 0)
                                Console.WriteLine("No departments found.");

                            break;
                        }

                    case "3":
                        {
                            int id = ReadInt("Enter department ID: ");
                            var d = await service.GetByIdAsync(id);

                            if (d is null)
                                Console.WriteLine("Department not found.");
                            else
                                Console.WriteLine(
                                    $"{d.DepartmentId} | {d.DepartmentName} | " +
                                    $"{d.Location ?? "-"} | Active: {d.IsActive}");

                            break;
                        }

                    case "4":
                        {
                            int id = ReadInt("Enter department ID: ");
                            var d = await service.GetByIdAsync(id);

                            if (d is null)
                            {
                                Console.WriteLine("Department not found.");
                                break;
                            }

                            d.DepartmentName =
                                ReadText("New department name: ");

                            d.Location =
                                ReadText("New location (optional): ");

                            bool updated = await service.UpdateAsync(d);

                            Console.WriteLine(
                                updated ? "Department updated."
                                        : "No changes made.");

                            break;
                        }

                    case "5":
                        {
                            int id = ReadInt("Enter department ID: ");
                            bool result = await service.DeactivateAsync(id);

                            Console.WriteLine(
                                result ? "Department deactivated."
                                       : "Department not found.");

                            break;
                        }

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    // ==========================================
    // STUDENT MENU
    // ==========================================

    static async Task StudentMenu(StudentService service)
    {
        while (true)
        {
            Console.WriteLine("\n--- STUDENT MANAGEMENT ---");
            Console.WriteLine("1. Register Student");
            Console.WriteLine("2. List Students");
            Console.WriteLine("3. Search Student by ID");
            Console.WriteLine("4. Search Student by Name");
            Console.WriteLine("5. Filter by Department");
            Console.WriteLine("6. Update Student");
            Console.WriteLine("7. Deactivate Student");
            Console.WriteLine("0. Back to Main Menu");

            string choice = ReadText("Enter choice: ");

            try
            {
                switch (choice)
                {
                    case "1":
                        {
                            var student = new Student
                            {
                                Name = ReadText("Student name: "),
                                Email = ReadText("Email: "),
                                Age = ReadInt("Age: "),
                                DepartmentId =
                                    ReadInt("Department ID: ")
                            };

                            int id = await service.RegisterAsync(student);

                            Console.WriteLine(
                                $"Student registered successfully. ID: {id}");

                            break;
                        }

                    case "2":
                        {
                            var students = await service.GetAllAsync();

                            foreach (var s in students)
                            {
                                Console.WriteLine(
                                    $"{s.StudentId} | {s.Name} | {s.Email} | " +
                                    $"Age: {s.Age} | Department: {s.DepartmentId} | " +
                                    $"Active: {s.IsActive}");
                            }

                            if (students.Count == 0)
                                Console.WriteLine("No students found.");

                            break;
                        }

                    case "3":
                        {
                            int id = ReadInt("Enter student ID: ");
                            var s = await service.GetByIdAsync(id);

                            if (s is null)
                                Console.WriteLine("Student not found.");
                            else
                                PrintStudent(s);

                            break;
                        }

                    case "4":
                        {
                            string name = ReadText("Enter student name: ");
                            var students = await service.SearchByNameAsync(name);

                            foreach (var s in students)
                                PrintStudent(s);

                            if (students.Count == 0)
                                Console.WriteLine("No matching students.");

                            break;
                        }

                    case "5":
                        {
                            int departmentId =
                                ReadInt("Enter department ID: ");

                            var students =
                                await service.GetByDepartmentAsync(departmentId);

                            foreach (var s in students)
                                PrintStudent(s);

                            if (students.Count == 0)
                                Console.WriteLine("No students found.");

                            break;
                        }

                    case "6":
                        {
                            int id = ReadInt("Enter student ID: ");
                            var s = await service.GetByIdAsync(id);

                            if (s is null)
                            {
                                Console.WriteLine("Student not found.");
                                break;
                            }

                            s.Name = ReadText("New name: ");
                            s.Email = ReadText("New email: ");
                            s.Age = ReadInt("New age: ");
                            s.DepartmentId =
                                ReadInt("New department ID: ");

                            bool updated = await service.UpdateAsync(s);

                            Console.WriteLine(
                                updated ? "Student updated."
                                        : "No changes made.");

                            break;
                        }

                    case "7":
                        {
                            int id = ReadInt("Enter student ID: ");
                            bool result = await service.DeactivateAsync(id);

                            Console.WriteLine(
                                result ? "Student deactivated."
                                       : "Student not found.");

                            break;
                        }

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    // ==========================================
    // COURSE MENU
    // ==========================================

    static async Task CourseMenu(CourseService service)
    {
        while (true)
        {
            Console.WriteLine("\n--- COURSE MANAGEMENT ---");
            Console.WriteLine("1. Add Course");
            Console.WriteLine("2. List Courses");
            Console.WriteLine("3. Search Course by ID");
            Console.WriteLine("4. Filter by Department");
            Console.WriteLine("5. Update Course");
            Console.WriteLine("6. Deactivate Course");
            Console.WriteLine("0. Back to Main Menu");

            string choice = ReadText("Enter choice: ");

            try
            {
                switch (choice)
                {
                    case "1":
                        {
                            var course = new Course
                            {
                                CourseName = ReadText("Course name: "),
                                DurationMonths =
                                    ReadInt("Duration in months: "),
                                Fee = ReadDecimal("Course fee: "),
                                DepartmentId =
                                    ReadInt("Department ID: ")
                            };

                            int id = await service.AddAsync(course);

                            Console.WriteLine(
                                $"Course added successfully. ID: {id}");

                            break;
                        }

                    case "2":
                        {
                            var courses = await service.GetAllAsync();

                            foreach (var c in courses)
                                PrintCourse(c);

                            if (courses.Count == 0)
                                Console.WriteLine("No courses found.");

                            break;
                        }

                    case "3":
                        {
                            int id = ReadInt("Enter course ID: ");
                            var c = await service.GetByIdAsync(id);

                            if (c is null)
                                Console.WriteLine("Course not found.");
                            else
                                PrintCourse(c);

                            break;
                        }

                    case "4":
                        {
                            int departmentId =
                                ReadInt("Enter department ID: ");

                            var courses =
                                await service.GetByDepartmentAsync(departmentId);

                            foreach (var c in courses)
                                PrintCourse(c);

                            if (courses.Count == 0)
                                Console.WriteLine("No courses found.");

                            break;
                        }

                    case "5":
                        {
                            int id = ReadInt("Enter course ID: ");
                            var c = await service.GetByIdAsync(id);

                            if (c is null)
                            {
                                Console.WriteLine("Course not found.");
                                break;
                            }

                            c.CourseName = ReadText("New course name: ");
                            c.DurationMonths =
                                ReadInt("New duration in months: ");
                            c.Fee = ReadDecimal("New course fee: ");
                            c.DepartmentId =
                                ReadInt("New department ID: ");

                            bool updated = await service.UpdateAsync(c);

                            Console.WriteLine(
                                updated ? "Course updated."
                                        : "No changes made.");

                            break;
                        }

                    case "6":
                        {
                            int id = ReadInt("Enter course ID: ");
                            bool result = await service.DeactivateAsync(id);

                            Console.WriteLine(
                                result ? "Course deactivated."
                                       : "Course not found.");

                            break;
                        }

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    // ==========================================
    // ENROLLMENT MENU
    // ==========================================

    static async Task EnrollmentMenu(EnrollmentService service)
    {
        while (true)
        {
            Console.WriteLine("\n--- ENROLLMENT MANAGEMENT ---");
            Console.WriteLine("1. Enroll Student");
            Console.WriteLine("2. List Enrollments");
            Console.WriteLine("3. Complete Enrollment");
            Console.WriteLine("4. Cancel Enrollment");
            Console.WriteLine("0. Back to Main Menu");

            string choice = ReadText("Enter choice: ");

            try
            {
                switch (choice)
                {
                    case "1":
                        {
                            int studentId =
                                ReadInt("Enter student ID: ");

                            int courseId =
                                ReadInt("Enter course ID: ");

                            int id = await service.EnrollAsync(
                                studentId, courseId);

                            Console.WriteLine(
                                $"Enrollment successful. ID: {id}");

                            break;
                        }

                    case "2":
                        {
                            var enrollments = await service.GetAllAsync();

                            foreach (var e in enrollments)
                            {
                                Console.WriteLine(
                                    $"{e.EnrollmentId} | Student: {e.StudentId} | " +
                                    $"Course: {e.CourseId} | {e.EnrolledAt} | " +
                                    $"Status: {e.Status}");
                            }

                            if (enrollments.Count == 0)
                                Console.WriteLine("No enrollments found.");

                            break;
                        }

                    case "3":
                        {
                            int id = ReadInt("Enter enrollment ID: ");
                            bool result = await service.CompleteAsync(id);

                            Console.WriteLine(
                                result ? "Enrollment completed."
                                       : "Enrollment not found.");

                            break;
                        }

                    case "4":
                        {
                            int id = ReadInt("Enter enrollment ID: ");
                            bool result = await service.CancelAsync(id);

                            Console.WriteLine(
                                result ? "Enrollment cancelled."
                                       : "Enrollment not found.");

                            break;
                        }

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    // ==========================================
    // AUDIT LOGS
    // ==========================================

    static async Task ViewAuditLogs(AuditLogService service)
    {
        int count = ReadInt("How many recent logs? (default 10): ");

        if (count == 0)
            count = 10;

        var logs = await service.GetRecentAsync(count);

        Console.WriteLine("\n--- RECENT AUDIT LOGS ---");

        foreach (var log in logs)
        {
            Console.WriteLine(
                $"{log.AuditId} | {log.EntityName} | " +
                $"{log.EntityId} | {log.Action} | " +
                $"{log.Details ?? "-"} | {log.CreatedAt}");
        }

        if (logs.Count == 0)
            Console.WriteLine("No audit logs found.");
    }

    // ==========================================
    // DISPLAY HELPERS
    // ==========================================

    static void PrintStudent(Student s)
    {
        Console.WriteLine(
            $"{s.StudentId} | {s.Name} | {s.Email} | " +
            $"Age: {s.Age} | Department: {s.DepartmentId} | " +
            $"Admission: {s.AdmissionDate} | Active: {s.IsActive}");
    }

    static void PrintCourse(Course c)
    {
        Console.WriteLine(
            $"{c.CourseId} | {c.CourseName} | " +
            $"Duration: {c.DurationMonths} months | " +
            $"Fee: {c.Fee:F2} | Department: {c.DepartmentId} | " +
            $"Active: {c.IsActive}");
    }

    // ==========================================
    // INPUT HELPERS
    // ==========================================

    static string ReadText(string message)
    {
        Console.Write(message);
        return Console.ReadLine()?.Trim() ?? string.Empty;
    }

    static int ReadInt(string message)
    {
        while (true)
        {
            Console.Write(message);

            if (int.TryParse(Console.ReadLine(), out int value))
                return value;

            Console.WriteLine("Enter a valid whole number.");
        }
    }

    static decimal ReadDecimal(string message)
    {
        while (true)
        {
            Console.Write(message);

            if (decimal.TryParse(Console.ReadLine(), out decimal value))
                return value;

            Console.WriteLine("Enter a valid amount.");
        }
    }
}
