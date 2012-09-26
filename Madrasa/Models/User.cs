using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Madrasa.Models
{
    public class User
    {
        public int      id                { get; set; }
        public string   userName          { get; set; }
        public string   email             { get; set; }

        [DataType(DataType.Password)]
        public string   password          { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("password", ErrorMessage = "The password and confirmation password do not match.")]
        public string   confirmPassword   { get; set; }
        
    }
    public class UserDbContext : DbContext
    {
        public DbSet<User> dbSet { get; set; }
    }
}