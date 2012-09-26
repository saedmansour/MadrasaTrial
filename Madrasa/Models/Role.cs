using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Madrasa.Models
{
    public class Role
    {
        public int      id   { get; set; }
        public string   role { get; set; }
    }
    public class RoleDbContext : DbContext
    {
        public DbSet<Role> dbSet { get; set; }
    }
}