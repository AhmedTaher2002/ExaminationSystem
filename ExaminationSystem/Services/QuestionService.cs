using AutoMapper;
using ExaminationSystem.DTOs.Question;
using ExaminationSystem.Models;
using ExaminationSystem.Models.Enums;
using ExaminationSystem.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Services
{
    public class QuestionService
    {
        private readonly QuestionRepository _questionRepository;
        private readonly IMapper _mapper;
        public QuestionService(IMapper  mapper)
        {
            _questionRepository = new QuestionRepository();
            _mapper = mapper;
        }

        public List<GetAllQuestionsDTO> GetAll()
        {
            var questions = _questionRepository.GetAll().AsNoTracking().ToList();
            return _mapper.Map<List<GetAllQuestionsDTO>>(questions);
            /*
            return _questionRepository.GetAll()
                .Select(q => new GetAllQuestionsDTO
                {
                    ID = q.ID,
                    Text = q.Text,
                    Level = q.Level
                }).AsNoTracking().ToList();*/

        }
        public async Task<GetQuestionByIdDTO> GetByID(int id)
        {
            if (!_questionRepository.IsExist(id))
                throw new Exception("Question Not Found");

            var question = await _questionRepository.Get(q => q.ID == id).Include(q => q.Choices).FirstAsync();
            return _mapper.Map<GetQuestionByIdDTO>(question);
            /*
            return new GetQuestionByIdDTO
            {
                ID = question.ID,
                Text = question.Text,
                Level = question.Level
            };*/
        }
        public async Task<GetAllQuestionsDTO> Get(int? id, string? text, QuestionLevel? level)
        {
            var query = _questionRepository.GetAll().AsQueryable();
            if (id.HasValue)
            {
                query = query.Where(q => q.ID == id.Value);
            }
            if (!string.IsNullOrEmpty(text))
            {
                query = query.Where(q => q.Text.Contains(text));
            }
            if (level.HasValue)
            {
                query = query.Where(q => q.Level == level.Value);
            }
            var question = await query.AsNoTracking().FirstOrDefaultAsync()??throw new Exception("Question Not Found");
            return _mapper.Map<GetAllQuestionsDTO>(question);
            /*
            return new GetAllQuestionsDTO
            {
                ID = question.ID,
                Text = question.Text,
                Level = question.Level
            };
            */
        }
        public async Task Create(CreateQuestionDTO dto)
        {
            /*
            Question question = new Question
            {
                Text = dto.Text,
                Level = dto.Level,
                InstructorId = dto.InstructorId
            };
            */
            var question = _mapper.Map<Question>(dto);
            await _questionRepository.Add(question);
        }
        public async Task Update(int id, UpdateQuestionDTO questionDto)
        {
            if (!_questionRepository.IsExist(id))
                throw new Exception("Question Not Found");
            /*
            Question question = new Question
            {
                ID = id,
                Text = dto.Text,
                Level = dto.Level
            };
            */
            var question = await _questionRepository.GetByIDWithTracking(id);
            questionDto = new()
            {
                Level=questionDto.Level==default?question.Level:questionDto.Level,
                Text=questionDto.Text=="string"?question.Text:questionDto.Text
            };
            await _questionRepository.Update(_mapper.Map<Question>(questionDto));
        }
        public async Task<bool> SoftDelete(int questionId)
        {
            if (!_questionRepository.IsExist(questionId))
                throw new Exception("Question Not Found");

            await _questionRepository.SoftDelete(questionId);
            return true;
        }
        public async Task<bool>HardDelete(int questionId)
        {
            if (!_questionRepository.IsExist(questionId))
                throw new Exception("Question Not Found");

            await _questionRepository.HardDelete(questionId);
            return true;
        }
        public IEnumerable<GetAllQuestionsDTO> GetInstructorQuestions(int instructorId)
        {
            var questions = _questionRepository.Get(q => q.InstructorId == instructorId).ToList();
            return _mapper.Map<IEnumerable<GetAllQuestionsDTO>>(questions);
        }
        public IEnumerable<GetAllQuestionsDTO> GetByLevel(QuestionLevel level)
        { 
            return _mapper.Map<IEnumerable<GetAllQuestionsDTO>>(_questionRepository.Get(q => q.Level == level)); 
        }
    }
}
