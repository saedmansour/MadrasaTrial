using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Madrasa.Models
{
    public class SchoolAdmin
    {
        public int id       { get; set; }   // TODO: isn't id a redundancy to userId?
        public int userId   { get; set; }
        public int schoolId { get; set; }
    }

    public class SchoolAdminDbContext : DbContext
    {
        public DbSet<SchoolAdmin> dbSet { get; set; }
    }
}