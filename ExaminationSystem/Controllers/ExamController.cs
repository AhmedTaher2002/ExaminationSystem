using AutoMapper;
using ExaminationSystem.DTOs.Exam;
using ExaminationSystem.DTOs.Other;
using ExaminationSystem.DTOs.Question;
using ExaminationSystem.Services;
using ExaminationSystem.ViewModels.Exam;
using ExaminationSystem.ViewModels.Other;
using ExaminationSystem.ViewModels.Question;
using ExaminationSystem.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamController : BaseController
    {
        private readonly ExamService _examService;

        public ExamController( IMapper mapper): base(mapper)
        {
            _examService = new ExamService(mapper);
        }

        [HttpGet]
        public async Task<ResponseViewModel<IEnumerable<GetAllExamsViewModel>>> GetAll()
        {
            var result = await _examService.GetAll();
            return HandleResult<IEnumerable<GetAllExamsDTO>, IEnumerable<GetAllExamsViewModel>>(result);
        }

        [HttpGet("{id}")]
        public async Task<ResponseViewModel<GetExamByIdViewModel>> GetByID(int id)
        {
            var result = await _examService.GetByID(id);
            return HandleResult<GetExamByIdDTO, GetExamByIdViewModel>(result);
        }

        [HttpGet("filter")]
        public async Task<ResponseViewModel<IEnumerable<GetAllExamsViewModel>>> Get(int? id, string? title, int? type)
        {
            var result = await _examService.Get(id, title, (Models.Enums.ExamType?)type);
            return HandleResult<IEnumerable<GetAllExamsDTO>, IEnumerable<GetAllExamsViewModel>>(result);
        }

        [HttpPost]
        public async Task<ResponseViewModel<bool>> Create([FromBody] CreateExamViewModel vm)
        {
            var result = await _examService.Create(_mapper.Map<CreateExamDTO>(vm));
            return HandleResult<bool,bool>(result);
        }

        [HttpPut("{id}")]
        public async Task<ResponseViewModel<bool>> Update(int id, [FromBody] UpdateExamViewModel vm)
        {
            var result = await _examService.Update(id, _mapper.Map<UpdateExamDTO>(vm));
            return HandleResult<bool, bool>(result);
        }

        [HttpDelete("{id}")]
        public async Task<ResponseViewModel<bool>> SoftDelete(int id)
        {
            var result = await _examService.SoftDelete(id);
            return HandleResult<bool, bool>(result);
        }

        [HttpGet("{examId}/questions")]
        public async Task<ResponseViewModel<IEnumerable<GetAllQuestionsViewModel>>> GetExamQuestions(int examId)
        {
            var result = _examService.GetExamQuestions(examId);
            return HandleResult<IEnumerable<GetAllQuestionsDTO>, IEnumerable<GetAllQuestionsViewModel>>(result);
        }

        [HttpPost("assign-question")]
        public async Task<ResponseViewModel<bool>> AssignQuestion([FromBody] ExamQuestionViewModel vm)
        {
            var result = await _examService.AssignQuestion(_mapper.Map<ExamQuestionDTO>(vm));
            return HandleResult<bool, bool>(result);
        }

        [HttpPost("{examId}/evaluate")]
        public async Task<ResponseViewModel<bool>> EvaluateAllExams(int examId)
        {
            var result = await _examService.EvaluateAllExams(examId);
            return HandleResult<bool, bool>(result);
        }
    }
}
