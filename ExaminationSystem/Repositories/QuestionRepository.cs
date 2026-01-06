using ExaminationSystem.Data;
using ExaminationSystem.Models;

namespace ExaminationSystem.Repositories
{
    public class QuestionRepository:GeneralRepository<Question>
    {
        private readonly Context _context;
        public QuestionRepository()
        {
            _context = new Context();
        }
        
    }
}
