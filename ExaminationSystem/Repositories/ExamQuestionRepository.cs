using ExaminationSystem.Data;
using ExaminationSystem.DTOs.Choice;
using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExaminationSystem.Repositories
{
    public class ExamQuestionRepository
    {
        private readonly Context _context;

        public ExamQuestionRepository()
        {
            _context = new Context();
        }

        public async Task<ExamQuestion?> Get(int examId, int quetionId)
        {
            var res = await _context.ExamQuestions.AsNoTracking().FirstOrDefaultAsync(eq => eq.ExamId == examId && eq.QuestionId == quetionId && !eq.IsDeleted);
            return res;
        }
        public IQueryable<ExamQuestion> GetWithTracking() { 
            return _context.ExamQuestions.AsTracking().AsQueryable();
        }
        public async Task<bool> Add(ExamQuestion examQuestion)
        {
            _context.ExamQuestions.Add(examQuestion);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Update(ExamQuestion examQuestion)
        {
            _context.ExamQuestions.Update(examQuestion);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task SoftDelete(int examId, int questionId)
        {
            var res = GetWithTracking().FirstOrDefault(a=>a.ExamId==examId&&a.QuestionId==questionId&&!a.IsDeleted) ??throw new Exception("exam Question Not Found"); ;

            res.IsDeleted = true;
            _context.ExamQuestions.Update(res);
            await _context.SaveChangesAsync();
        }
        public async Task HardDelete(int examId, int questionId)
        {
            var res = GetWithTracking().FirstOrDefault(a => a.ExamId == examId && a.QuestionId == questionId && !a.IsDeleted);
            if (res == null)
                if (res == null)
                throw new Exception("ExamQuestion Not Found");
             _context.ExamQuestions.Remove(res);
             await _context.SaveChangesAsync();
        }
        //--------------------------------SRS BUSINESS HELPERS


        // Check if question already assigned to exam
        public bool IsQuestionAssigned(int examId, int questionId)
        {
            return _context.ExamQuestions.Any(eq => eq.ExamId == examId && eq.QuestionId == questionId);
        }

        // Get all questions in an exam
        public IEnumerable<ExamQuestion> GetQuestionsByExam(int examId)
        {
            return _context.ExamQuestions.AsNoTracking().ToList();
        }

        // Get exams containing a question (question reuse)
        public IEnumerable<ExamQuestion> GetExamsByQuestion(int questionId)
        {
            return _context.ExamQuestions.AsNoTracking().ToList();
        }

        // Remove question from exam
        public bool RemoveQuestion(int examId, int questionId)
        {
            var eq = _context.ExamQuestions.FirstOrDefault(x => x.ExamId == examId && x.QuestionId == questionId);

            if (eq == null)
                return false;

            _context.ExamQuestions.Remove(eq);
            _context.SaveChanges();
            return true;
        }

        // Remove all questions from exam (used before auto-generation)
        public void RemoveAllQuestionsFromExam(int examId)
        {
            var questions = _context.ExamQuestions.Where(eq => eq.ExamId == examId);

            _context.ExamQuestions.RemoveRange(questions);
            _context.SaveChanges();
        }


        internal bool IsAssigned(int examId, int questionId)
        {
            return _context.ExamQuestions.Any(eq => eq.ExamId == examId && eq.QuestionId == questionId);
        }

        internal IQueryable<ExamQuestion> GetByExam(int examId)
        {
            var query= _context.ExamQuestions.Where(eq => eq.ExamId == examId).AsQueryable();
            return query;

        }
    }
}
