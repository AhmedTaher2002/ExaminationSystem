using AutoMapper;
using ExaminationSystem.DTOs.Course;
using ExaminationSystem.DTOs.Instructor;
using ExaminationSystem.DTOs.Other;
using ExaminationSystem.DTOs.Question;
using ExaminationSystem.DTOs.Student;
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
    public class InstructorController : BaseController
    {
        private readonly InstructorService _instructorService;

        public InstructorController( IMapper mapper): base(mapper)
        {
            _instructorService = new InstructorService(mapper);
        }

        [HttpGet("Instructors")]
        public async Task<ResponseViewModel<IEnumerable<GetAllInstructorsViewModel>>> GetAll()
        {
            var result = await _instructorService.GetAll();
            return HandleResult<IEnumerable<GetAllInstructorsDTO>, IEnumerable<GetAllInstructorsViewModel>> (result);
        }

        [HttpGet("{id}")]
        public async Task<ResponseViewModel<GetInstructorByIdViewModel>> GetByID(int id)
        {
            var result = await _instructorService.GetByID(id);
            return HandleResult<GetInstructorByIdDTO, GetInstructorByIdViewModel>(result);
        }

        [HttpGet("filter")]
        public async Task<ResponseViewModel<IEnumerable<GetAllInstructorsViewModel>>> Get(int? id, string? name, string? username, string? email)
        {
            var result = await _instructorService.Get(id, name, username, email);
            return HandleResult<IEnumerable<GetAllInstructorsDTO>, IEnumerable<GetAllInstructorsViewModel>>(result);
        }

        [HttpPost]
        public async Task<ResponseViewModel<bool>> Create([FromBody] CreateInstructorViewModel vm)
        {
            var result = await _instructorService.Create(_mapper.Map<CreateInstructorDTO>(vm));
            return HandleResult<bool,bool>(result);
        }

        [HttpPut("{id}")]
        public async Task<ResponseViewModel<bool>> Update(int id, [FromBody] UpdateInstructorViewModel vm)
        {
            var result = await _instructorService.Update(id, _mapper.Map<UpdateInstructorDTO>(vm));
            return HandleResult<bool,bool>(result);
        }

        [HttpDelete("{id}")]
        public async Task<ResponseViewModel<bool>> SoftDelete(int id)
        {
            var result = await _instructorService.SoftDelete(id);
            return HandleResult<bool, bool>(result);
        }

        [HttpGet("{instructorId}/courses")]
        public async Task<ResponseViewModel<IEnumerable<GetAllCoursesViewModel>>> GetCoursesForInstructor(int instructorId)
        {
            var result = await _instructorService.GeTCoursesForInstructor(instructorId);
            return HandleResult<IEnumerable<GetAllCoursesDTO>, IEnumerable<GetAllCoursesViewModel>>(result);
        }

        [HttpGet("course/{courseId}/students")]
        public async Task<ResponseViewModel<IEnumerable<GetAllStudentsViewModel>>> GetStudentsInCourse(int courseId)
        {
            var result = await _instructorService.GetStudentsInCourse(courseId);
            return HandleResult<IEnumerable<GetAllStudentsDTO>,IEnumerable<GetAllStudentsViewModel>>(result);
        }

        [HttpGet("{instructorId}/questions")]
        public async Task<ResponseViewModel<IEnumerable<GetAllQuestionsViewModel>>> GetQuestionsForInstructor(int instructorId)
        {
            var result = await _instructorService.GetQuestionsForInstructor(instructorId);
            return HandleResult<IEnumerable<GetAllQuestionsDTO>, IEnumerable<GetAllQuestionsViewModel>>(result);
        }

        [HttpGet("exam/{examId}/questions")]
        public async Task<ResponseViewModel<IEnumerable<GetAllQuestionsViewModel>>> GetQuestionsByExam(int examId)
        {
            var result = await _instructorService.GetQuestionsByExam(examId);
            return HandleResult<IEnumerable<GetAllQuestionsDTO>, IEnumerable<GetAllQuestionsViewModel>>(result);
        }

        [HttpGet("student/answers")]
        public async Task<ResponseViewModel<IEnumerable<GetStudentAnswersViewModel>>> GetStudentAnswers([FromBody] StudentExamViewModel vm)
        {
            var result = await _instructorService.GetStudentAnswers(_mapper.Map<StudentExamDTO>(vm));
            return HandleResult<IEnumerable<GetStudentAnswersDTO>, IEnumerable<GetStudentAnswersViewModel>>(result);
        }

        [HttpPost("assign/student")]
        public async Task<ResponseViewModel<bool>> AssignStudentToCourse([FromBody] StudentCourseViewModel vm)
        {
            var result = await _instructorService.AssignStudentToCourse(_mapper.Map<StudentCourseDTO>(vm));
            return HandleResult<bool,bool>(result);
        }

        [HttpPost("assign/exam")]
        public async Task<ResponseViewModel<bool>> AssignExamToCourse([FromBody] CourseExamViewModel vm)
        {
            var result = await _instructorService.AssignExamToCourse(_mapper.Map<CourseExamDTO>(vm));
            return HandleResult<bool,bool>(result);
        }

        [HttpPost("assign/question")]
        public async Task<ResponseViewModel<bool>> AddQuestionToExam([FromBody] ExamQuestionDTO dto)
        {
            var result = await _instructorService.AddQuestionToExam(dto);
            return HandleResult<bool, bool>(result);
        }

        [HttpDelete("remove/question")]
        public async Task<ResponseViewModel<bool>> RemoveQuestionFromExam([FromBody] ExamQuestionDTO dto)
        {
            var result = await _instructorService.RemoveQuestionFromExam(dto);
            return HandleResult<bool, bool>(result);
        }
    }
}
