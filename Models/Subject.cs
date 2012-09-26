using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Madrasa.Global;



namespace Madrasa.Models
{
    public class Subject
    {
        public int      id                      { get; set; }
        public string   ancestorIdSplitStr      { get; set; }      
        public string   title                   { get; set; }
    }

    public class SubjectDbContext : DbContext
    {
        public DbSet<Subject> dbSet { get; set; }
    }

    public class SubjectView
    {
        public int              id              { get; set; }
        public string           title           { get; set; }
        public string           childrenId      { get; set; }
        public List<string>     ancestorIdList  { get; set; }   


        //
        //constructor
        //
        public SubjectView() {
        }

        //
        //constructor
        //
        public SubjectView(Subject subject, string splitStr)
        {
            //get Father id from string to List
            this.ancestorIdList = Helper.SeparateByString(subject.ancestorIdSplitStr, splitStr);
            this.title          = subject.title;
            this.id             = subject.id;

            //the next generation ancestorsIds
            if (subject.ancestorIdSplitStr != null)
            {
                this.childrenId = subject.ancestorIdSplitStr + splitStr;
            }
            this.childrenId += subject.id;
        }
    }
}