using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Madrasa.Models
{
    public class Student
    {
        public int id       { get; set; }
        public int userId   { get; set; }
        public int schoolId { get; set; }
        public int classId  { get; set; }
    }
    public class StudentDbContext : DbContext
    {
        public DbSet<Student> dbSet { get; set; }
    }
}