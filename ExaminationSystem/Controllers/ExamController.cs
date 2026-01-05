using AutoMapper;
using ExaminationSystem.DTOs.Exam;
using ExaminationSystem.DTOs.Other;
using ExaminationSystem.DTOs.Question;
using ExaminationSystem.Models.Enums;
using ExaminationSystem.Services;
using ExaminationSystem.ViewModels.Exam;
using ExaminationSystem.ViewModels.Other;
using ExaminationSystem.ViewModels.Question;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly ExamService _service;
        private readonly IMapper _mapper;
        public ExamController(IMapper mapper)
        {
            _service = new ExamService(mapper);
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<GetAllExamsViewModel> GetAll()
        {
            /*var res= _service.GetAll()
                .Select(e => new GetAllExamsViewModel
                {
                    ID = e.ID,
                    Title = e.Title,
                    Type = e.Type,
                    CourseId = e.CourseId
                });
            return res;*/
            return _mapper.Map<IEnumerable<GetAllExamsViewModel>>(_service.GetAll());
        }

        [HttpGet]
        public async Task<GetExamByIdViewModel> GetByID(int id)
        {
            var dto = await _service.GetByID(id);
            /*return new GetExamByIdViewModel
            {
                ID = dto.ID,
                Title = dto.Title,
                Type = dto.Type,
                CourseId = dto.CourseId
            };*/
            return _mapper.Map<GetExamByIdViewModel>(dto);
        }

        [HttpGet]
        public async Task<IEnumerable<GetAllExamsViewModel>> Get(int? id, string? title, ExamType? type)
        {
            return _mapper.Map<IEnumerable<GetAllExamsViewModel>>(await _service.Get(id, title, type));
        }

        [HttpPost]
        public async Task<bool> Create(CreateExamViewModel vm)
        {
            /*await _service.Create(new CreateExamDTO
            {
                Title = vm.Title,
                Type = vm.Type,
                CourseId = vm.CourseId,
                NumberOfQuestions = vm.NumberOfQuestions
            });*/
            await _service.Create(_mapper.Map<CreateExamDTO>(vm));
            return true;
        }

        [HttpPut]
        public async Task<bool> Update(int id, UpdateExamViewModel vm)
        {
            /*await _service.Update(id, new UpdateExamDTO
            {
                Title = vm.Title,
                Type = vm.Type,
                NumberOfQuestions = vm.NumberOfQuestions
            });*/
            await _service.Update(id, _mapper.Map<UpdateExamDTO>(vm));
            return true;
        }

        [HttpDelete]
        public async Task<bool> SoftDelete(int id)
        {
            await _service.SoftDelete(id);
            return true;
        }

        [HttpGet]
        public IEnumerable<GetAllQuestionsViewModel> GetExamQuestions(int examId)
        {
            return _mapper.Map <IEnumerable<GetAllQuestionsViewModel>>(_service.GetExamQuestions(examId));
        }

        [HttpPut]
        public async Task<ExamEvaluationResultViewModel> EvaluateExamforStudent(StudentExamViewModel studentExamVM)
        {
            var result = await _service.EvaluateExamforStudent(_mapper.Map<StudentExamDTO>(studentExamVM));
            return _mapper.Map<ExamEvaluationResultViewModel>(result);
        }

        [HttpPut]
        public async Task<IEnumerable<ExamEvaluationResultViewModel>> EvaluateAllExams(int examId)
        {
            var result = _service.EvaluateAllExams(examId);
            return _mapper.Map<IEnumerable<ExamEvaluationResultViewModel>>(result);
        }

        [HttpPut]
        public async Task AutoGenerateQuestions(int examId, int totalQuestions)
        {
            await _service.AutoGenerateQuestions(examId, totalQuestions);
        }
    }
}
