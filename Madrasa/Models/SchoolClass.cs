using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Madrasa.Models
{
    public class SchoolClass
    {
        public int      id               { get; set; }
        public string   name             { get; set; }
        public int      grade            { get; set; }
        public int      schoolId         { get; set; }   // MAYBE: school
        public int      studentNumber    { get; set; }   // TODO: huh? studentsNumbers? studentIdList? does it mean size?
    }

    public class SchoolClassDbContext : DbContext
    {
        public DbSet<SchoolClass> dbSet { get; set; } 
    }
}