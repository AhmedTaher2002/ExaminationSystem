using ExaminationSystem.Data;
using ExaminationSystem.DTOs.Choice;
using ExaminationSystem.DTOs.Other;
using ExaminationSystem.Models;
using ExaminationSystem.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Runtime.Intrinsics.Arm;

namespace ExaminationSystem.Repositories
{
    public class StudentExamRepository 
    {
        Context _context;

        public StudentExamRepository()
        {
            _context = new Context();
        }

        
        public IQueryable<StudentExam> GetAll()
        {
            var res= _context.StudentExam;
            return res;
        }

        public IQueryable<StudentExam> Get()
        {
            var res=  _context.StudentExam.AsTracking().AsQueryable();
            return res;
        }

        public async Task<StudentExam> GetWithTracking(int studentId, int examId)
        {
            var res = await _context.StudentExam.AsTracking().FirstOrDefaultAsync(se => se.StudentId == studentId && se.ExamId == examId&&!se.IsDeleted)??throw new Exception ("Student Exam Not Found");
            return res;
        }

        public IQueryable<StudentExam> Get(Expression<Func<StudentExam, bool>> filter)
        {
            var res= _context.StudentExam.AsNoTracking()
                     .Where(filter);
            return res;
        }

        public async Task<bool> Add(StudentExam studentExam)
        {
            _context.StudentExam.Add(studentExam);
            await _context.SaveChangesAsync();
            return true;
        }
    
        public async Task Update(StudentExam studentExam)
        {
            _context.StudentExam.Update(studentExam);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDelete(int studentId,int examId)
        {
            var res = await Get().FirstOrDefaultAsync(se => se.StudentId == studentId && se.ExamId == examId && !se.IsDeleted);
            if (res == null)
                return;
            res.IsDeleted = true;
            _context.StudentExam.Update(res);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HardDelete(int studentId, int examId)
        {
            var studentExam = await Get().FirstOrDefaultAsync(se => se.StudentId == studentId && se.ExamId == examId && !se.IsDeleted);

            if (studentExam == null)
                return false;

             _context.StudentExam.Remove(studentExam);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool HasFinalExam(int studentId)
        {
            var res= _context.StudentExam.Include(se => se.Exam)
                .Any(se => se.StudentId == studentId && se.Exam.Type == Models.Enums.ExamType.Final);
            return res;
        }



        public IEnumerable<StudentExam> GetExamResults(int examId)
        {
            return _context.StudentExam.Include(se => se.Student).Where(se => se.ExamId == examId && se.IsSubmitted)
                .AsNoTracking().ToList();
        }

        public bool IsSubmitted(int studentExamId)
        {
            return _context.StudentExam.Any(se => se.ID == studentExamId && se.IsSubmitted);
        }

        public void SubmitExam(int studentExamId, int score)
        {
            var studentExam = _context.StudentExam.Find(studentExamId);

            if (studentExam == null)
                throw new Exception("StudentExam not found");

            studentExam.Score = score;
            studentExam.IsSubmitted = true;

            _context.SaveChanges();
        }

        public StudentExam? GetByStudentAndExam(int studentId, int examId)
        {
            return _context.StudentExam.Include(se => se.Exam).FirstOrDefault(se => se.StudentId == studentId && se.ExamId == examId);
        }

        internal bool IsAssigned(int studentID,int examId)
        {
            var res= _context.StudentExam.AsNoTracking().Any(se=>se.StudentId==studentID&&se.ExamId==examId);
            return res;
        }
        public async Task<StudentExam> StartExam(int studentId, int examId)
        {
            var studentExam = GetByStudentAndExam(studentId, examId)
                ?? throw new Exception("Student is not assigned to this exam.");

            if (studentExam.StartedTime != null)
                throw new Exception("Exam already started");

            studentExam.StartedTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return studentExam;
        }

        public async Task<bool> IsExamTimeExpired(int studentId, int examId)
        {
            var studentExam = await _context.StudentExam.Include(se => se.Exam).AsNoTracking()
                .Where(se => se.StudentId == studentId && se.ExamId == examId && se.StartedTime != null && !se.IsDeleted)
                .Select(st => new { st.StartedTime, st.DurationMinutes }).FirstOrDefaultAsync();

            if (studentExam == null)
                throw new Exception("Exam not started");

            var endTime = studentExam.StartedTime.AddMinutes(studentExam.DurationMinutes);

            return DateTime.UtcNow > endTime;
        }

        public async Task SubmitExam(StudentExam studentExam, List<StudentAnswer> answers)
        {
            if (await IsExamTimeExpired(studentExam.StudentId,studentExam.ExamId))
                throw new Exception("Time is over. Cannot submit exam.");

            _context.StudentAnswers.AddRange(answers);

            int correct = answers.Count(a => a.SelectedChoice.IsCorrect);
            int total = answers.Count;

            studentExam.Score = total == 0 ? 0 : (decimal)correct / total * 100;
            studentExam.IsSubmitted = true;

            await _context.SaveChangesAsync();
        }

        internal IQueryable<StudentExam> GetByStudent(int studentId)
        {
            var query= _context.StudentExam.Where(se => se.StudentId == studentId).AsQueryable();
            return query;
        }
    }
}
