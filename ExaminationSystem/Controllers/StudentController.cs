using AutoMapper;
using ExaminationSystem.DTOs.Other;
using ExaminationSystem.DTOs.Student;
using ExaminationSystem.Services;
using ExaminationSystem.ViewModels.Course;
using ExaminationSystem.ViewModels.Exam;
using ExaminationSystem.ViewModels.Other;
using ExaminationSystem.ViewModels.Response;
using ExaminationSystem.ViewModels.Student;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly StudentService _studentService;
        private readonly IMapper _mapper;

        public StudentController(StudentService studentService, IMapper mapper)
        {
            _studentService = studentService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ResponseViewModel<IEnumerable<GetAllStudentsViewModel>>> GetAll()
        {
            var result = await _studentService.GetAll();
            return _mapper.Map<ResponseViewModel<IEnumerable<GetAllStudentsViewModel>>>(result);
        }

        [HttpGet("{id}")]
        public async Task<ResponseViewModel<GetStudentByIdViewModel>> GetByID(int id)
        {
            var result = await _studentService.GetByID(id);
            return _mapper.Map<ResponseViewModel<GetStudentByIdViewModel>>(result);
        }

        [HttpGet("filter")]
        public async Task<ResponseViewModel<IEnumerable<GetAllStudentsViewModel>>> Get(int? id, string? name, string? email)
        {
            var result = await _studentService.Get(id, name, email);
            return _mapper.Map<ResponseViewModel<IEnumerable<GetAllStudentsViewModel>>>(result);
        }

        [HttpPost]
        public async Task<ResponseViewModel<bool>> Create([FromBody] CreateStudentDTO dto)
        {
            var result = await _studentService.Create(dto);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpPut("{id}")]
        public async Task<ResponseViewModel<bool>> Update(int id, [FromBody] UpdateStudentDTO dto)
        {
            var result = await _studentService.Update(id, dto);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpDelete("{id}")]
        public async Task<ResponseViewModel<bool>> SoftDelete(int id)
        {
            var result = await _studentService.SoftDelete(id);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpPost("enroll")]
        public async Task<ResponseViewModel<bool>> EnrollInCourse([FromBody] StudentCourseDTO dto)
        {
            var result = await _studentService.EnrollInCourse(dto);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpPost("exam/start")]
        public async Task<ResponseViewModel<bool>> StartExam([FromBody] StudentExamDTO dto)
        {
            var result = await _studentService.StartExam(dto);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpPost("exam/submit")]
        public async Task<ResponseViewModel<bool>> SubmitAnswers([FromBody] List<StudentAnswerDTO> answers)
        {
            var result = await _studentService.SubmitAnswers(answers);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpGet("{studentId}/courses")]
        public async Task<ResponseViewModel<IEnumerable<GetAllCoursesViewModel>>> GetCoursesForStudent(int studentId)
        {
            var result = await _studentService.GetCoursesForStudent(studentId);
            return _mapper.Map<ResponseViewModel<IEnumerable<GetAllCoursesViewModel>>>(result);
        }

        [HttpDelete("course/remove")]
        public async Task<ResponseViewModel<bool>> SoftDeleteStudentFromCourse([FromBody] StudentCourseDTO dto)
        {
            var result = await _studentService.SoftDeleteStudentFromCourse(dto);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpGet("{studentId}/exams")]
        public ActionResult<IEnumerable<GetExamsForStudentViewModel>> GetExamsForStudent(int studentId)
        {
            var result = _studentService.GetExamsForStudent(studentId);
            return _mapper.Map<IEnumerable<GetExamsForStudentViewModel>>(result).Any()
                ? Ok(result)
                : NotFound("No exams found");
        }
    }
}
