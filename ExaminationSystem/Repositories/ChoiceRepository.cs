using ExaminationSystem.Data;
using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Repositories
{
    public class ChoiceRepository:GeneralRepository<Choice>
    {
        private readonly Context _context;
        public ChoiceRepository()
        {
            _context = new Context();
        }

        internal async Task<bool> IsChoiceBelongsToQuestion(int choiceId, int questionId)
        {
            return await _context.Choices.AnyAsync(c=>c.ID ==choiceId && c.QuestionId==questionId);           
        }
    }
}
