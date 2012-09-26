using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Madrasa.Models
{
    public class Material
    {
        public int      id        { get; set; }
        public int      subjectId { get; set; }
        public string   html      { get; set; }
    }

    public class MaterialDbContext : DbContext
    {
        public DbSet<Material> dbSet { get; set; }
    }
}