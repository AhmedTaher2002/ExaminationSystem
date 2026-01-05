using AutoMapper;
using ExaminationSystem.DTOs.Question;
using ExaminationSystem.Models.Enums;
using ExaminationSystem.Services;
using ExaminationSystem.ViewModels.Question;
using Microsoft.AspNetCore.Mvc;
namespace ExaminationSystem.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly QuestionService _questionService;
        private readonly IMapper _mapper;
        public QuestionController(IMapper mapper)
        {
            _questionService = new QuestionService(mapper);
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<GetAllQuestionsViewModel> GetAll()
        {
            return _mapper.Map<IEnumerable<GetAllQuestionsViewModel>>(_questionService.GetAll());
            /*
             return _questionService.GetAll()
                .Select(q => new GetAllQuestionsViewModel
                {
                    ID = q.ID,
                    Text = q.Text,
                    Level = q.Level
                });
            */
        }

        [HttpGet]
        public async Task<GetQuestionByIdViewModel> GetByID(int id)
        {
            var dto = await _questionService.GetByID(id);
            return _mapper.Map<GetQuestionByIdViewModel>(dto);
            /*
            return new GetQuestionByIdViewModel
            {
                ID = dto.ID,
                Text = dto.Text,
                Level = dto.Level
            };
            */
        }

        [HttpPost]
        public async Task Create(CreateQuestionViewModel vm)
        {
            var question = _mapper.Map<CreateQuestionDTO>(vm);
            await _questionService.Create(question);
            /*
            await _questionService.Create(new CreateQuestionDTO
            {
                Text = vm.Text,
                Level = vm.Level,
                InstructorId = vm.InstructorId
            });
            */
        }

        [HttpPut]
        public async Task Update(int id, UpdateQuestionViewModel questionViewModel)
        {
            await _questionService.Update(id, _mapper.Map<UpdateQuestionDTO>(questionViewModel));             
        }

        [HttpDelete]
        public async Task<bool> SoftDelete(int id)
        {
            return await _questionService.SoftDelete(id);
        }
  
        [HttpGet]
        public async Task<IEnumerable<GetAllQuestionsViewModel>> GetInstructorQuestions(int instructorId)
        {
            var res = _questionService.GetInstructorQuestions(instructorId);
            return _mapper.Map<IEnumerable<GetAllQuestionsViewModel>>(res);
        }

        [HttpGet]
        public IEnumerable<GetAllQuestionsViewModel> GetByLevel(QuestionLevel level)
        {
            return _mapper.Map<IEnumerable<GetAllQuestionsViewModel>>(_questionService.GetByLevel(level));
        }
    }
}
