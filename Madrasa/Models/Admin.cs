using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Madrasa.Models
{
    public class Admin
    {
        public int id       { get; set; }
        public int userId   { get; set; }   //TODO: why would an admin both need an id and a userId? userId is enough.
        public int schoolId { get; set; }
    }

    public class AdminDbContext : DbContext
    {
        public DbSet<Admin> dbSet { get; set; }
    }
}