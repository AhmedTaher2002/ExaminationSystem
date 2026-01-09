using ExaminationSystem.Data;
using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Repositories
{
    public class ExamRepository : GeneralRepository<Exam>
    {
        private readonly Context _context;
        public ExamRepository()
        {
            _context = new Context();
        }

        public IQueryable<Exam> GetExamsByCourse(int courseId)
        {
            var exams = _context.Exams
                .Where(e => e.CourseId == courseId && !e.IsDeleted)
                .AsQueryable();
            return exams;
        }
    }
}
