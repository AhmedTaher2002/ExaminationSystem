using ExaminationSystem.Data;
using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExaminationSystem.Repositories
{
    public class StudentAnswerRepository
    {
        private readonly Context _context;

        public StudentAnswerRepository()
        {
            _context = new Context();
        }

        public IQueryable<StudentAnswer> GetAll()
        {
            var res = _context.StudentAnswers;
            return res;
        }


        internal IQueryable<StudentAnswer> Get(int studentId, int examId)
        {
            var query = _context.StudentAnswers.Where(a => a.StudentId == studentId && a.ExamId == examId && !a.IsDeleted);
            return query;
        }
        public async Task<StudentAnswer?> GetWithTracking(int studentId, int examId)
        {
            return await _context.StudentAnswers.AsTracking().FirstOrDefaultAsync(sa => sa.StudentId == studentId && sa.ExamId == examId&&!sa.IsDeleted);
        }

        
        public async Task Add(StudentAnswer answer)
        {   
            _context.StudentAnswers.Add(answer);
            await _context.SaveChangesAsync();
        }

        public async Task Update(StudentAnswer answer)
        {
            _context.StudentAnswers.Update(answer);
            _context.SaveChanges();
        }

        public IEnumerable<StudentAnswer> GetAnswersByStudentExam(int studentId,int examId)
        {
            var res= _context.StudentAnswers.Where(sa => sa.StudentId == studentId && sa.ExamId == examId).AsNoTracking().ToList();
            return res;
        }

        public int CountCorrectAnswers(int studentId,int examId)
        {
            return _context.StudentAnswers.Count(sa => sa.StudentId == studentId&&sa.ExamId==examId && sa.SelectedChoice.IsCorrect);
        }

        internal bool IsAnswered(int studentId, int examId, int questionId)
        {
            return _context.StudentAnswers.Any(sa => sa.StudentId==studentId&&sa.ExamId==examId&&sa.QuestionId==questionId);
        }

        
    }
}

