using AutoMapper;
using ExaminationSystem.DTOs.Instructor;
using ExaminationSystem.DTOs.Other;
using ExaminationSystem.Services;
using ExaminationSystem.ViewModels.Course;
using ExaminationSystem.ViewModels.Instructor;
using ExaminationSystem.ViewModels.Other;
using ExaminationSystem.ViewModels.Question;
using ExaminationSystem.ViewModels.Response;
using ExaminationSystem.ViewModels.Student;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstructorController : ControllerBase
    {
        private readonly InstructorService _instructorService;
        private readonly IMapper _mapper;

        public InstructorController(InstructorService instructorService, IMapper mapper)
        {
            _instructorService = instructorService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ResponseViewModel<IEnumerable<GetAllInstructorsViewModel>>> GetAll()
        {
            var result = await _instructorService.GetAll();
            return _mapper.Map<ResponseViewModel<IEnumerable<GetAllInstructorsViewModel>>>(result);
        }

        [HttpGet("{id}")]
        public async Task<ResponseViewModel<GetInstructorByIdViewModel>> GetByID(int id)
        {
            var result = await _instructorService.GetByID(id);
            return _mapper.Map<ResponseViewModel<GetInstructorByIdViewModel>>(result);
        }

        [HttpGet("filter")]
        public async Task<ResponseViewModel<IEnumerable<GetAllInstructorsViewModel>>> Get(int? id, string? name, string? username, string? email)
        {
            var result = await _instructorService.Get(id, name, username, email);
            return _mapper.Map<ResponseViewModel<IEnumerable<GetAllInstructorsViewModel>>>(result);
        }

        [HttpPost]
        public async Task<ResponseViewModel<bool>> Create([FromBody] CreateInstructorDTO dto)
        {
            var result = await _instructorService.Create(dto);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpPut("{id}")]
        public async Task<ResponseViewModel<bool>> Update(int id, [FromBody] UpdateInstructorDTO dto)
        {
            var result = await _instructorService.Update(id, dto);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpDelete("{id}")]
        public async Task<ResponseViewModel<bool>> SoftDelete(int id)
        {
            var result = await _instructorService.SoftDelete(id);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpGet("{instructorId}/courses")]
        public async Task<ResponseViewModel<IEnumerable<GetAllCoursesViewModel>>> GetCoursesForInstructor(int instructorId)
        {
            var result = await _instructorService.GeTCoursesForInstructor(instructorId);
            return _mapper.Map<ResponseViewModel<IEnumerable<GetAllCoursesViewModel>>>(result);
        }

        [HttpGet("course/{courseId}/students")]
        public async Task<ResponseViewModel<IEnumerable<GetAllStudentsViewModel>>> GetStudentsInCourse(int courseId)
        {
            var result = await _instructorService.GetStudentsInCourse(courseId);
            return _mapper.Map<ResponseViewModel<IEnumerable<GetAllStudentsViewModel>>>(result);
        }

        [HttpGet("{instructorId}/questions")]
        public async Task<ResponseViewModel<IEnumerable<GetAllQuestionsViewModel>>> GetQuestionsForInstructor(int instructorId)
        {
            var result = await _instructorService.GetQuestionsForInstructor(instructorId);
            return _mapper.Map<ResponseViewModel<IEnumerable<GetAllQuestionsViewModel>>>(result);
        }

        [HttpGet("exam/{examId}/questions")]
        public async Task<ResponseViewModel<IEnumerable<GetAllQuestionsViewModel>>> GetQuestionsByExam(int examId)
        {
            var result = await _instructorService.GetQuestionsByExam(examId);
            return _mapper.Map<ResponseViewModel<IEnumerable<GetAllQuestionsViewModel>>>(result);
        }

        [HttpGet("student/answers")]
        public async Task<ResponseViewModel<IEnumerable<GetStudentAnswersViewModel>>> GetStudentAnswers([FromBody] StudentExamDTO dto)
        {
            var result = await _instructorService.GetStudentAnswers(dto);
            return _mapper.Map<ResponseViewModel<IEnumerable<GetStudentAnswersViewModel>>>(result);
        }

        [HttpPost("assign/student")]
        public async Task<ResponseViewModel<bool>> AssignStudentToCourse([FromBody] StudentCourseDTO dto)
        {
            var result = await _instructorService.AssignStudentToCourse(dto);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpPost("assign/exam")]
        public async Task<ResponseViewModel<bool>> AssignExamToCourse([FromBody] CourseExamDTO dto)
        {
            var result = await _instructorService.AssignExamToCourse(dto);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpPost("assign/question")]
        public async Task<ResponseViewModel<bool>> AddQuestionToExam([FromBody] ExamQuestionDTO dto)
        {
            var result = await _instructorService.AddQuestionToExam(dto);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpDelete("remove/question")]
        public async Task<ResponseViewModel<bool>> RemoveQuestionFromExam([FromBody] ExamQuestionDTO dto)
        {
            var result = await _instructorService.RemoveQuestionFromExam(dto);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }
    }
}
