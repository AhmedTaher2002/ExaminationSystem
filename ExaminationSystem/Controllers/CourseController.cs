using AutoMapper;
using ExaminationSystem.DTOs.Course;
using ExaminationSystem.Services;
using ExaminationSystem.ViewModels.Course;
using ExaminationSystem.ViewModels.Exam;
using ExaminationSystem.ViewModels.Other;
using ExaminationSystem.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly CourseService _courseService;
        private readonly IMapper _mapper;

        public CourseController(CourseService courseService, IMapper mapper)
        {
            _courseService = courseService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ResponseViewModel<IEnumerable<GetAllCoursesViewModel>>> GetAll()
        {
            var result = await _courseService.GetAll();
            return _mapper.Map<ResponseViewModel<IEnumerable<GetAllCoursesViewModel>>>(result);
        }

        [HttpGet("{id}")]
        public async Task<ResponseViewModel<GetByIdCourseViewModel>> GetByID(int id)
        {
            var result = await _courseService.GetByID(id);
            return _mapper.Map<ResponseViewModel<GetByIdCourseViewModel>>(result);
        }

        [HttpGet("filter")]
        public async Task<ResponseViewModel<IEnumerable<GetAllCoursesViewModel>>> Get(int? id, string? name,int? hours)
        {
            var result = await _courseService.Get(id, name,hours);
            return _mapper.Map<ResponseViewModel<IEnumerable<GetAllCoursesViewModel>>>(result);
        }

        [HttpPost]
        public async Task<ResponseViewModel<bool>> Create([FromBody] CreateCourseDTO dto)
        {
            var result = await _courseService.Create(dto);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpPut("{id}")]
        public async Task<ResponseViewModel<bool>> Update(int id, [FromBody] UpdateCourseDTO dto)
        {
            var result = await _courseService.Update(id, dto);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpDelete("{id}")]
        public async Task<ResponseViewModel<bool>> SoftDelete(int id)
        {
            var result = await _courseService.SoftDelete(id);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpGet("{courseId}/students")]
        public async Task<ResponseViewModel<IEnumerable<StudentCourseViewModel>>> GetStudentsInCourse(int courseId)
        {
            var result = _courseService.GetStudentsInCourse(courseId);
            return _mapper.Map<ResponseViewModel<IEnumerable<StudentCourseViewModel>>>(result);
        }

        [HttpPost("{courseId}/AssignInstructor/{instructorId}")]
        public async Task<ResponseViewModel<bool>> AssignInstructorToCourse(int courseId, int instructorId)
        {
            var result = await _courseService.AssignInstructorToCourse(courseId, instructorId);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpPost("{courseId}/AssignExam/{examId}")]
        public async Task<ResponseViewModel<bool>> AssignExamToCourse(int courseId, int examId)
        {
            var result = await _courseService.AssignExamToCourse(courseId, examId);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpGet("{courseId}/exams")]
        public async Task<ResponseViewModel<IEnumerable<GetAllExamsViewModel>>> GetExamsForCourse(int courseId)
        {
            var result = await _courseService.GetExamsForCourse(courseId);
            return _mapper.Map<ResponseViewModel<IEnumerable<GetAllExamsViewModel>>>(result);
        }
    }
}
