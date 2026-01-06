using ExaminationSystem.Data;
using ExaminationSystem.Models;

namespace ExaminationSystem.Repositories
{
    public class InstructorRepository:GeneralRepository<Instructor>
    {
        private readonly Context _context;
        public InstructorRepository() 
        { 
            _context = new Context();
        }
    }
}
