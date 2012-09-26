using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Madrasa.Global;

namespace Madrasa.Models
{
    public class Homework
    {
        public int              Id              { get; set; }
        public string           Title           { get; set; }
        public DateTime         Start           { get; set; }
        public DateTime         End             { get; set; }    
        public int              CourseId        { get; set; }
    }

    public class HomeworkDbContext : DbContext
    {
        public DbSet<Homework> DbSet { get; set; }
    }
    
    public class HomeworkQuestion           // TODO: different name to this, unclear.
    {
        public int      Id              { get; set; }
        public int      UserId          { get; set; }
        public int      HomeworkId      { get; set; }
        public int      QuestionId      { get; set; }
        public int      SubjectId       { get; set; }
        public bool     IsCorrect       { get; set; }       
        public string   UserSolution    { get; set; }
        public int      Stage           { get; set; }       //TODO: stage? huh? missing docuemntation
    }

    public class HomeWorkQuestionDbContext : DbContext
    {
        public DbSet<HomeworkQuestion> DbSet { get; set; }

    }
    
    // DELETE: from HomeworkDone to HomeworkSolved
    public class HomeworkSolved
    {
        public int Id           { get; set; }
        public int HomeworkId   { get; set; }
        public int StudentId    { get; set; }
    }
    public class HomeworkSolvedDbContext : DbContext
    {
        public DbSet<HomeworkSolved> DbSet { get; set; }
    }


    //Stores the current state of the homework. This is used for AI algorithm.
    public class HomeworkState
    {
        public int Id                   { get; set; } //the stateId
        public int UserId               { get; set; } //the UserId we test
        public int BaseSubjectId        { get; set; }  //the subject we test
        public int SubjectId            { get; set; }  // the current son of the subject
        public int Direction            { get; set; }  
        public int QuestionNumber       { get; set; }  //number of question we already asked
        public int HomeworkId           { get; set; }   
        public int Mark                 { get; set; }  //save his mark
        public int IndexOfBaseSubject   { get; set; }  //we have array of baseSubject , and we save the index of the base subject

        // TODO: constructor?
        public HomeworkState()
        {
        }

        // TODO: copy contructor in c#? huh?
        public HomeworkState(HomeworkState state)
        {
            IndexOfBaseSubject = state.IndexOfBaseSubject;
            BaseSubjectId = state.BaseSubjectId;
            SubjectId = state.SubjectId;
            Direction = state.Direction;
            QuestionNumber = state.QuestionNumber;
            HomeworkId = state.HomeworkId;
            Mark = state.Mark;
            UserId = state.UserId;
        }

        // TODO: constructor?
        public HomeworkState(int userId,int indexOfBaseSubject, int baseSubjectId,
            int subjectId, int direction, int questionNumber, int homeworkId, int mark)
        {
            this.IndexOfBaseSubject = indexOfBaseSubject;
            this.BaseSubjectId = baseSubjectId;
            this.SubjectId = subjectId;
            this.Direction = direction;
            this.QuestionNumber = questionNumber;
            this.HomeworkId = homeworkId;
            this.Mark = mark;
            this.UserId = userId;
        }

    }
    public class HomeworkStateDbContext : DbContext
    {
        public DbSet<HomeworkState> DbSet { get; set; }
    }
}