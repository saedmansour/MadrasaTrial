using System.Data.Entity;

namespace Madrasa.Models
{
    public class CourseRegistration
    {
        public CourseRegistration() {}
        public CourseRegistration(int userId,int courseId,string role)
        {
            UserId = userId;
            CourseId = courseId;
            Role = role;
        }
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public string Role { get; set; }
    }
    public class CourseRegistrationDbContext : DbContext
    {
        public DbSet<CourseRegistration> DbSet { get; set; } 
    }
}