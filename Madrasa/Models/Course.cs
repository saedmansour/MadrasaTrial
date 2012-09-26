using System.Collections.Generic;
using System.Data.Entity;
using System.Web.Script.Serialization;

namespace Madrasa.Models
{
    // we have two class one for view with list of subjets 
    // and other with json for table
    public class CourseForView
    {
        private readonly JavaScriptSerializer _js = new JavaScriptSerializer();
        public CourseForView(Course course)
        {
            Id = course.Id;
            Name = course.Name;
            SubjectIdList = (List<int>)_js.Deserialize(course.SubjectIdJson, typeof(List<int>));
        }
        public CourseForView() { }
        public int      Id                      { get; set; }
        public string   Name                    { get; set; }
        public List<int> SubjectIdList          { get; set; }
    }
    //
    public class Course
    {
        private readonly JavaScriptSerializer _js = new JavaScriptSerializer();
        public Course(CourseForView course)
        {
            Id = course.Id;
            Name = course.Name;
            SubjectIdJson = _js.Serialize(course.SubjectIdList);
        }
        public Course(){}
        public int Id { get; set; }
        public string Name { get; set; }
        public string SubjectIdJson { get; set; }
    }
    public class CourseDbContext : DbContext
    {
        public DbSet<Course> DbSet { get; set; } 
    }
}