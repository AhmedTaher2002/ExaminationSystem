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


    }
}
