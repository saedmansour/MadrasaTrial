using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Madrasa.Models
{
    public class School
    {
        public int      id          { get; set; }
        public string   name        { get; set; }
        public string   country     { get; set; }
        public string   city        { get; set; }
        public string   language    { get; set; }
    }

    public class SchoolDbContext : DbContext
    {
        public DbSet<School> dbSet { get; set; }
    }
}