using ExaminationSystem.Data;
using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task AddOrUpdate(StudentAnswer answer)
        {
            var existing = await _context.StudentAnswers
                .FirstOrDefaultAsync(a =>
                    a.StudentId == answer.StudentId &&
                    a.ExamId == answer.ExamId &&
                    a.QuestionId == answer.QuestionId);

            if (existing != null)
            {
                existing.SelectedChoiceId = answer.SelectedChoiceId;
            }
            else
            {
                _context.StudentAnswers.Add(answer);
            }

            await _context.SaveChangesAsync();
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
        
    }
}

