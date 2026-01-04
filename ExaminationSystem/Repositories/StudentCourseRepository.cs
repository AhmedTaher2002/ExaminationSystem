using ExaminationSystem.Data;
using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ExaminationSystem.Repositories
{
    public class StudentCourseRepository
    {
        private readonly Context _context;
        public StudentCourseRepository() 
        { 
            
            _context = new Context();
        }
        public IQueryable<StudentCourse> GetAll()
        {
            var res = _context.StudentCourses;
            return res;
        }
        public async Task<StudentCourse?> GetbyID(int studentId, int courseId) { 
            var res = await _context.StudentCourses.AsNoTracking()
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId&&sc.IsDeleted);
            return res;

        }
        public async Task<StudentCourse?> GetBystudentidWithTracking(int studentId)
        {
            var res = await _context.StudentCourses.Where(sc=>sc.StudentId==studentId&&!sc.IsDeleted).AsTracking().FirstOrDefaultAsync();
            return res;
        }
        internal IQueryable<StudentCourse> GetByStudentCourses(int studentId)
        {
            var query = _context.StudentCourses.Where(sc => sc.StudentId == studentId && !sc.IsDeleted).AsQueryable();
            return query;
        }
        public async Task<StudentCourse?> GetWithTracking(StudentCourse studentCourse)
        {
            var res = await _context.StudentCourses.AsTracking().FirstOrDefaultAsync(sc => sc.StudentId== studentCourse.StudentId && sc.CourseId==studentCourse.StudentId&& !sc.IsDeleted);
            return res;
        }
        public async Task<StudentCourse?> Get(Expression<Func<StudentCourse,bool>> filter){
            var res = await _context.StudentCourses.Include(sc => sc.Student).Include(sc => sc.Course).AsNoTracking()
                .FirstOrDefaultAsync(filter);
            return res;
        }
        public  IQueryable<StudentCourse> Get(int? studentId, int? courseId)
        {
            var query = _context.StudentCourses.AsQueryable();
            if (studentId.HasValue)
            {
                query = query.Where(sc => sc.StudentId == studentId.Value);
            }
            if (courseId.HasValue)
            {
                query = query.Where(sc => sc.CourseId == courseId.Value);
            }
            return query;
        }
        public async Task Add(StudentCourse studentCourse)
        {
            _context.StudentCourses.Add(studentCourse);
            await _context.SaveChangesAsync();
        }
        public async Task Update( StudentCourse StudentCourse)
        {

            if (!StudentCourse.IsDeleted)
            {
                await _context.SaveChangesAsync();
            }
            var studentCourse = new StudentCourse
            {
                StudentId = StudentCourse.StudentId,
                CourseId = StudentCourse.CourseId
            };
            _context.StudentCourses.Update(studentCourse);
            await _context.SaveChangesAsync();

        }
        public async Task SoftDelete(StudentCourse studentCourse)
        {
            var res = await GetWithTracking(studentCourse) ?? throw new Exception("StudentCourse Not Found");
            
            res.IsDeleted = true;
            _context.StudentCourses.Update(res);
            await _context.SaveChangesAsync();
        }
        public async Task HardDelete(StudentCourse studentCourse)
        {
            var res = await GetWithTracking(studentCourse) ?? throw new Exception("StudentCourse Not Found");
            res.IsDeleted = true;
            
            _context.StudentCourses.Remove(res);
            await _context.SaveChangesAsync();
        }
        public IEnumerable<StudentCourse> GetCoursesByStudent(int studentId)
        {
            return _context.StudentCourses.Where(sc => sc.StudentId == studentId).AsNoTracking().ToList();
        }

        public IEnumerable<StudentCourse> GetStudentsByCourse(int courseId)
        {
            return _context.StudentCourses.AsNoTracking();
        }

        internal bool IsAssigned(int studentId,int courseId )
        { 
            var res= _context.StudentCourses.Any(sc => sc.StudentId == studentId && sc.CourseId == courseId);
            return res;
        }

        
        

    }
}
