using ExaminationSystem.Data;
using ExaminationSystem.Models;

namespace ExaminationSystem.Repositories
{
    public class StudentRepository:GeneralRepository<Student>
    {
        private readonly Context _context;
        public StudentRepository()
        {
            _context = new Context();
        }

        internal bool IsExists(string email)
        {
            return _context.Students.Any(s => s.Email == email);
        }
    }
}
