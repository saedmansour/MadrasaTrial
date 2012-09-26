using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Madrasa.Models;
using Madrasa.Controllers;

namespace Madrasa.Global
{

    public interface IAlgorithm
    {
        void StartHomework();

        //
        //baseSubjectId - is homeWork subject id
        //depth-  is the depth of subject sons.
        //upOrDown- every subject we visit twice when we go down and when we back up.
        //questionNumebr- is the order number of the next question
        Question GetNextQuestion(HomeworkState state, int userId);

        //
        //get the next material
        Material GetMatrial(int subjectId);

        //
        //End home work
        void EndHomework(int examId);

        //check if we need to go down with levels
        bool IsGoDown(ref HomeworkState state);

        // Get user state and question that had been solved and the user answer and update state and mark.
        //
        void UpdateMark(ref HomeworkState state, int questionId, string userAnswer);
    }


    public static class Helper
    {
        public static List<string> SeparateByString(string source, string splitStr)
        {
            if (source == null) {
                return new List<string>();
            }
            List<string> answers = source.Split(new string[] { splitStr }, StringSplitOptions.None).ToList();
            return answers;
        }
    }

    public class GreedyAlgorithm : IAlgorithm
    {
        private readonly MaterialController _materialDb = new MaterialController();
        private readonly QuestionController _questionDb = new QuestionController();
        private readonly HomeWorkQuestionDbContext _homeworkQuestionSolvedDb = new HomeWorkQuestionDbContext();  

        public  int MaxNumberOfStageQuestion = 3;    // TODO: not stage, something else...

        public void StartHomework() {               // TODO: implement
            return;
        }

        public Material GetMatrial(int subjectId)
        {
            Material material = _materialDb.GetMaterialBySubjectId(subjectId);
            return material; 
        }

        public void EndHomework(int examId) {       //TODO: implement
            return;
        }

        public Question GetNextQuestion(HomeworkState state,int userId) {
            List<int> solvedQuestionIdList = GetSolvedQuestionIdList(state.SubjectId,userId);
            List<Question> questionList = _questionDb.GetQuestionListBySubjectId(state.SubjectId);
            List<Question> unSolvedQuestionList  = questionList.Where(question => !solvedQuestionIdList.Contains(question.id)).ToList() ;
            if(!unSolvedQuestionList.Any())
            {
                throw new UnSolvedQuestionNotFoundException();
            }
            return unSolvedQuestionList.First();
        }

        public bool IsGoDown(ref HomeworkState state) {     // TODO: why ref? you're not changing the variable state.
            return (state.Mark < -3) ;
        }

        public void UpdateMark(ref HomeworkState state, int questionId, string userAnswer)
        {
            Question question = _questionDb.GetQuestionById(questionId);
            int questionSign = (question.correctAnswerNum == userAnswer)? 1 : -1;
            // if the answer right add the level number to mark other sub level number from mark
            state.Mark += questionSign*question.level;
        }

        //GetSolvedQuestionIdList
        // get subject Id and return all solved questions id
        public List<int> GetSolvedQuestionIdList(int subjectId,int userId)
        {
            var homeworkList = (from homework in _homeworkQuestionSolvedDb.DbSet select homework);
            //query to get solved question for the user
            var solvedQuestions = homeworkList.Where(
                homework => homework.SubjectId == subjectId && homework.UserId == userId)
                .Select(homework => homework.QuestionId);
            if (!solvedQuestions.Any())
            {
                return new List<int>();
            }
            return solvedQuestions.ToList();
        }
    }

    public class UnSolvedQuestionNotFoundException : Exception
    {
    }
}