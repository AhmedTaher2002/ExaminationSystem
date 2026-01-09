using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExaminationSystem.DTOs.Instructor;
using ExaminationSystem.DTOs.Question;
using ExaminationSystem.Models;
using ExaminationSystem.Models.Enums;
using ExaminationSystem.Repositories;
using ExaminationSystem.ViewModels.Response;
using Microsoft.EntityFrameworkCore;
using PredicateExtensions;
using System.Linq.Expressions;

namespace ExaminationSystem.Services
{
    public class QuestionService
    {
        private readonly QuestionRepository _questionRepository;
        private readonly InstructorRepository _instructorRepository;
        private readonly IMapper _mapper;

        public QuestionService(IMapper mapper)
        {
            _questionRepository = new QuestionRepository();
            _instructorRepository = new InstructorRepository();
            _mapper = mapper;
        }

        #region Question CRUD

        // Get all questions
        public async Task<ResponseViewModel<IEnumerable<GetAllQuestionsDTO>>> GetAll()
        {
            var questions = _questionRepository.GetAll();
            var result = await questions.ProjectTo<GetAllQuestionsDTO>(_mapper.ConfigurationProvider).ToListAsync();

            return result != null
                ? new SuccessResponseViewModel<IEnumerable<GetAllQuestionsDTO>>(result)
                : new FailResponseViewModel<IEnumerable<GetAllQuestionsDTO>>("No Questions Found", ErrorCode.QuestionNotFound);
        }

        // Get question by ID
        public async Task<ResponseViewModel<GetQuestionByIdDTO>> GetByID(int id)
        {
            if (!_questionRepository.IsExists(id))
                return new FailResponseViewModel<GetQuestionByIdDTO>("QuestionId Not Found", ErrorCode.InvalidQuestionId);

            var question = _questionRepository.Get(q => q.ID == id).Include(q => q.Choices);
            var result = await question.ProjectTo<GetQuestionByIdDTO>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

            return result != null
                ? new SuccessResponseViewModel<GetQuestionByIdDTO>(result)
                : new FailResponseViewModel<GetQuestionByIdDTO>("Question Not Exist", ErrorCode.QuestionNotFound);
        }

        // Filter questions dynamically
        public async Task<ResponseViewModel<IEnumerable<GetAllQuestionsDTO>>> Get(int? id, string? text, QuestionLevel? level)
        {
            var predicate = QuestionPredicateBuilder(id, text, level);
            var questions = _questionRepository.Get(predicate);
            var result = await questions.ProjectTo<GetAllQuestionsDTO>(_mapper.ConfigurationProvider).ToListAsync();

            return result != null
                ? new SuccessResponseViewModel<IEnumerable<GetAllQuestionsDTO>>(result)
                : new FailResponseViewModel<IEnumerable<GetAllQuestionsDTO>>("No Questions Found", ErrorCode.QuestionNotFound);
        }

        // Create a new question
        public async Task<ResponseViewModel<bool>> Create(CreateQuestionDTO dto)
        {
            var question = _mapper.Map<Question>(dto);
            await _questionRepository.AddAsync(question);

            return question != null
                ? new SuccessResponseViewModel<bool>(true)
                : new FailResponseViewModel<bool>("Question not created", ErrorCode.ChoiceNotCreated);
        }

        // Update an existing question
        public async Task<ResponseViewModel<bool>> Update(int id, UpdateQuestionDTO dto)
        {
            if (!_questionRepository.IsExists(id))
                return new FailResponseViewModel<bool>("QuestionId Not Found", ErrorCode.InvalidQuestionId);

            var question = await _questionRepository.GetByIDWithTracking(id);
            dto = new UpdateQuestionDTO
            {
                QuestionId = question.ID,
                Text = dto.Text == "string" ? question.Text : dto.Text,
                Level = dto.Level == default ? question.Level : dto.Level
            };

            await _questionRepository.UpdateAsync(_mapper.Map<Question>(dto));

            return question != null
                ? new SuccessResponseViewModel<bool>(true)
                : new FailResponseViewModel<bool>("Question not Updated", ErrorCode.QuestionNotUpdated);
        }

        // Soft delete a question
        public async Task<ResponseViewModel<bool>> SoftDelete(int questionId)
        {
            if (!_questionRepository.IsExists(questionId))
                return new FailResponseViewModel<bool>("QuestionId Not Found", ErrorCode.InvalidQuestionId);

            await _questionRepository.SoftDeleteAsync(questionId);
            return new SuccessResponseViewModel<bool>(true);
        }

        #endregion

        #region Instructor Questions

        // Get all questions created by an instructor
        public async Task<ResponseViewModel<IEnumerable<GetAllQuestionsDTO>>> GetInstructorQuestions(int instructorId)
        {
            if (!_instructorRepository.IsExists(instructorId))
                return new FailResponseViewModel<IEnumerable<GetAllQuestionsDTO>>("InstructorId Not Exist", ErrorCode.InvalidInstrutorId);

            var questions = _questionRepository.Get(q => q.InstructorId == instructorId);
            var result = await questions.ProjectTo<GetAllQuestionsDTO>(_mapper.ConfigurationProvider).ToListAsync();

            return result != null
                ? new SuccessResponseViewModel<IEnumerable<GetAllQuestionsDTO>>(result)
                : new FailResponseViewModel<IEnumerable<GetAllQuestionsDTO>>("No Questions Found", ErrorCode.QuestionNotFound);
        }

        // Get questions by difficulty level
        public async Task<ResponseViewModel<IEnumerable<GetAllQuestionsDTO>>> GetByLevel(QuestionLevel level)
        {
            var questions = _questionRepository.Get(q => q.Level == level);
            var result = await questions.ProjectTo<GetAllQuestionsDTO>(_mapper.ConfigurationProvider).ToListAsync();

            return result != null
                ? new SuccessResponseViewModel<IEnumerable<GetAllQuestionsDTO>>(result)
                : new FailResponseViewModel<IEnumerable<GetAllQuestionsDTO>>("No Questions Found", ErrorCode.QuestionNotFound);
        }

        #endregion

        #region Private Helpers

        // Dynamic predicate for filtering questions
        private Expression<Func<Question, bool>> QuestionPredicateBuilder(int? id, string? text, QuestionLevel? level)
        {
            var predicate = PredicateExtensions.PredicateExtensions.Begin<Question>(true);

            if (id.HasValue) predicate = predicate.And(q => q.ID == id.Value);
            if (!string.IsNullOrEmpty(text)) predicate = predicate.And(q => q.Text.Contains(text));
            if (level.HasValue) predicate = predicate.And(q => q.Level == level.Value);

            return predicate;
        }

        #endregion
    }
}
