using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;

namespace StudentCourseManagementSystem.Helpers
{
    public static class DatabaseHelper
    {
        private const string ConnectionString =
            @"Server=.\SQLEXPRESS;Database=StudentCourseDB;" +
            @"Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection CreateConnection()
            => new SqlConnection(ConnectionString);
    }

}
