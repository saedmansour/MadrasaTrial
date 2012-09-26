using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

using Madrasa.Global;


namespace Madrasa.Models
{
   public class Question
    {
        public int      id                   { get; set; }
        public int      subjectId            { get; set; }
        public string   questionHtml             { get; set; }
        public string   answerJsonArray      { get; set; }
        public string   correctAnswerNum     { get; set; }
        public string   solution             { get; set; }
        public string   hint                 { get; set; }
        public int      level                { get; set; }
    }
   
    public class QuestionDbContext : DbContext
    {
        public DbSet<Question> dbSet         { get; set; }
    }
}