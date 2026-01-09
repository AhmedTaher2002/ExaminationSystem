using AutoMapper;
using ExaminationSystem.DTOs.Question;
using ExaminationSystem.Services;
using ExaminationSystem.ViewModels.Question;
using ExaminationSystem.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly QuestionService _questionService;
        private readonly IMapper _mapper;

        public QuestionController(QuestionService questionService, IMapper mapper)
        {
            _questionService = questionService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ResponseViewModel<IEnumerable<GetAllQuestionsViewModel>>> GetAll()
        {
            var result = await _questionService.GetAll();
            return _mapper.Map<ResponseViewModel<IEnumerable<GetAllQuestionsViewModel>>>(result);
        }

        [HttpGet("{id}")]
        public async Task<ResponseViewModel<GetQuestionByIdViewModel>> GetByID(int id)
        {
            var result = await _questionService.GetByID(id);
            return _mapper.Map<ResponseViewModel<GetQuestionByIdViewModel>>(result);
        }

        [HttpGet("filter")]
        public async Task<ResponseViewModel<IEnumerable<GetAllQuestionsViewModel>>> Get(int? id, string? text, int? level)
        {
            var result = await _questionService.Get(id, text, (Models.Enums.QuestionLevel?)level);
            return _mapper.Map<ResponseViewModel<IEnumerable<GetAllQuestionsViewModel>>>(result);
        }

        [HttpPost]
        public async Task<ResponseViewModel<bool>> Create([FromBody] CreateQuestionDTO dto)
        {
            var result = await _questionService.Create(dto);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpPut("{id}")]
        public async Task<ResponseViewModel<bool>> Update(int id, [FromBody] UpdateQuestionDTO dto)
        {
            var result = await _questionService.Update(id, dto);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpDelete("{id}")]
        public async Task<ResponseViewModel<bool>> SoftDelete(int id)
        {
            var result = await _questionService.SoftDelete(id);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpGet("instructor/{instructorId}")]
        public async Task<ResponseViewModel<IEnumerable<GetAllQuestionsViewModel>>> GetInstructorQuestions(int instructorId)
        {
            var result = await _questionService.GetInstructorQuestions(instructorId);
            return _mapper.Map<ResponseViewModel<IEnumerable<GetAllQuestionsViewModel>>>(result);
        }

        [HttpGet("level/{level}")]
        public async Task<ResponseViewModel<IEnumerable<GetAllQuestionsViewModel>>> GetByLevel(int level)
        {
            var result = await _questionService.GetByLevel((Models.Enums.QuestionLevel)level);
            return _mapper.Map<ResponseViewModel<IEnumerable<GetAllQuestionsViewModel>>>(result);
        }
    }
}
