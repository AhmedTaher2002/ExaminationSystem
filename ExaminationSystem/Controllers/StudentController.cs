using AutoMapper;
using ExaminationSystem.DTOs.Course;
using ExaminationSystem.DTOs.Other;
using ExaminationSystem.DTOs.Student;
using ExaminationSystem.Services;
using ExaminationSystem.ViewModels.Course;
using ExaminationSystem.ViewModels.Exam;
using ExaminationSystem.ViewModels.Other;
using ExaminationSystem.ViewModels.Response;
using ExaminationSystem.ViewModels.Student;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ExaminationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : BaseController
    {
        private readonly StudentService _studentService;

        public StudentController(IMapper mapper): base(mapper)
        {
            _studentService = new StudentService(mapper);
        }

        [HttpGet]
        public async Task<ResponseViewModel<IEnumerable<GetAllStudentsViewModel>>> GetAll()
        {
            var result = await _studentService.GetAll();
            return HandleResult<IEnumerable<GetAllStudentsDTO>, IEnumerable<GetAllStudentsViewModel>>(result);
        }

        [HttpGet("{id}")]
        public async Task<ResponseViewModel<GetStudentByIdViewModel>> GetByID(int id)
        {
            var result = await _studentService.GetByID(id);
            return HandleResult<GetStudentByIdDTO, GetStudentByIdViewModel>(result);
        }

        [HttpGet("filter")]
        public async Task<ResponseViewModel<IEnumerable<GetAllStudentsViewModel>>> Get(int? id, string? name, string? email)
        {
            var result = await _studentService.Get(id, name, email);
            return HandleResult<IEnumerable<GetAllStudentsDTO>, IEnumerable<GetAllStudentsViewModel>>(result);
        }

        [HttpPost]
        public async Task<ResponseViewModel<bool>> Create([FromBody] CreateStudentDTO dto)
        {
            var result = await _studentService.Create(dto);
            return HandleResult<bool,bool>(result);
        }

        [HttpPut("{id}")]
        public async Task<ResponseViewModel<bool>> Update(int id, [FromBody] UpdateStudentDTO dto)
        {
            var result = await _studentService.Update(id, dto);
            return HandleResult<bool,bool>(result);
        }

        [HttpDelete("{id}")]
        public async Task<ResponseViewModel<bool>> SoftDelete(int id)
        {
            var result = await _studentService.SoftDelete(id);
            return HandleResult<bool,bool>(result);
        }

        [HttpPost("enroll")]
        public async Task<ResponseViewModel<bool>> EnrollInCourse([FromBody] StudentCourseViewModel vm)
        {
            var result = await _studentService.EnrollInCourse(_mapper.Map<StudentCourseDTO>(vm));
            return HandleResult<bool, bool>(result);
        }

        [HttpPost("exam/start")]
        public async Task<ResponseViewModel<bool>> StartExam([FromBody] StudentExamViewModel vm)
        {
            var result = await _studentService.StartExam(_mapper.Map<StudentExamDTO>(vm));
            return HandleResult<bool, bool>(result);
        }

        [HttpPost("exam/submit")]
        public async Task<ResponseViewModel<bool>> SubmitAnswers([FromBody] List<StudentAnswerViewModel> answers)
        {
            var result = await _studentService.SubmitAnswers(_mapper.Map<List<StudentAnswerDTO>>(answers));
            return HandleResult<bool, bool>(result);
        }

        [HttpGet("{studentId}/courses")]
        public async Task<ResponseViewModel<IEnumerable<GetAllCoursesViewModel>>> GetCoursesForStudent(int studentId)
        {
            var result = await _studentService.GetCoursesForStudent(studentId);
            return HandleResult<IEnumerable<GetAllCoursesDTO>,IEnumerable<GetAllCoursesViewModel>>(result);
        }

        [HttpDelete("course/remove")]
        public async Task<ResponseViewModel<bool>> SoftDeleteStudentFromCourse([FromBody] StudentCourseViewModel vm)
        {
            var result = await _studentService.SoftDeleteStudentFromCourse(_mapper.Map<StudentCourseDTO>(vm));
            return HandleResult<bool, bool>(result);
        }

        [HttpGet("{studentId}/exams")]
        public async Task<ResponseViewModel<IEnumerable<GetExamsForStudentViewModel>>> GetExamsForStudent(int studentId)
        {
            var result = await _studentService.GetExamsForStudent(studentId);
            return HandleResult<IEnumerable<GetExamsForStudentDTO>, IEnumerable<GetExamsForStudentViewModel>>(result);
        }
    }
}
