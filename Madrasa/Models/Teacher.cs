using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Madrasa.Models
{
    public class Teacher
    {
        public int Id           { get; set; }
        public int UserId       { get; set; }
        public int SchoolId     { get; set; }
        public int SubjectId    { get; set; }
    }
    public class TeacherDbContext : DbContext
    {
        public DbSet<Teacher> DbSet { get; set; }
    }
}