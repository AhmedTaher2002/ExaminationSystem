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
    }
}
