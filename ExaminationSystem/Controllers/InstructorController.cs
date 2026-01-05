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
using ExaminationSystem.ViewModels.Student;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
namespace ExaminationSystem.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        private readonly InstructorService _instructorService;
        private readonly IMapper _mapper;
        public InstructorController(IMapper mapper)
        {
            _instructorService = new InstructorService(mapper);
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<GetAllInstructorsViewModel> GetAll()
        {
            return _mapper.Map<IEnumerable<GetAllInstructorsViewModel>>(_instructorService.GetAll());
            /*
            return _instructorService.GetAll()
                .Select(i => new GetAllInstructorsViewModel
                {
                    ID = i.ID,
                    Name = i.Name,
                    Email = i.Email
                });
            */
        }

        [HttpGet]
        public IEnumerable<GetAllInstructorsViewModel> Get(int? id, string? name, string? email)
        {
            return _mapper.Map<IEnumerable<GetAllInstructorsViewModel>>(_instructorService.Get(id, name, email));
        }
        
        [HttpGet]
        public async Task<GetInstructorByIdViewModel> GetByID(int id)
        {
            var Instructor= _mapper.Map<GetInstructorByIdViewModel>(_instructorService.GetByID(id));
            return Instructor;
            /*
            var dto = await _instructorService.GetByID(id);
            return new GetInstructorByIdViewModel
            {
                ID = dto.ID,
                Name = dto.Name,
                Email = dto.Email
            };
            */
        }

        [HttpPost]
        public async Task<bool> Create(CreateInstructorViewModel vm)
        {
            var res = _mapper.Map<CreateInstructorDTO>(vm);
            await _instructorService.Create(res);
            /*
            _instructorService.Create(new CreateInstructorDTO
            {
                Name = vm.Name,
                Email = vm.Email
            });
            */
            return true;
        }

        [HttpPut("{id}")]
        public async Task<bool> Update(int id, UpdateInstructorViewModel vm)
        {
            await _instructorService.Update(id, _mapper.Map<UpdateInstructorDTO>(vm));
            /*
            _instructorService.Update(id, new UpdateInstructorDTO
            {
                Name = vm.Name,
                Email = vm.Email
            });
            */
            return true;
        }

        [HttpDelete("{id}")]
        public async Task<bool> SoftDelete(int id)
        {
            await _instructorService.SoftDelete(id);
            return true;
        }

        [HttpGet]
        public IEnumerable<GetAllCoursesViewModel> GeTCoursesForInstructor(int instructorId)
        {
            var courses = _instructorService.GeTCoursesForInstructor(instructorId);
            return _mapper.Map <IEnumerable<GetAllCoursesViewModel>>(courses);
        }
        
        [HttpGet]
        public IEnumerable<GetAllStudentsViewModel> GetStudentsInCourse(int courseId)
        {
            return _mapper.Map<IEnumerable<GetAllStudentsViewModel>>(_instructorService.GetStudentsInCourse(courseId));
        }
        
        [HttpGet]
        public IEnumerable<GetAllQuestionsViewModel> GetQuestionsForInstructor(int instructorId)
        {
            var questionDto=_instructorService.GetQuestionsForInstructor(instructorId);
            return _mapper.Map<IEnumerable<GetAllQuestionsViewModel>>(questionDto);
        }
        
        [HttpGet]
        public IEnumerable<GetAllQuestionsViewModel> GetQuestionsByExam(int examId)
        {
            return _mapper.Map<IEnumerable<GetAllQuestionsViewModel>>(_instructorService.GetQuestionsByExam(examId));
        }

        [HttpGet]
        public IEnumerable<GetStudentAnswersViewModel> GetStudentAnswers(StudentExamViewModel studentExamViewModel)
        {
            return _mapper.Map<IEnumerable<GetStudentAnswersViewModel>>(_instructorService.GetStudentAnswers(_mapper.Map<StudentExamDTO>(studentExamViewModel)));
        }
        
        [HttpPost]
        public async Task<bool> AssignStudentToCourse(StudentCourseDTO studentCourseDTO)
        {
            await _instructorService.AssignStudentToCourse(studentCourseDTO);
            return true;
        }

        [HttpPost]
        public async Task<bool> AssignExamToCourse(CourseExamViewModel courseExamViewModel)
        {
            await _instructorService.AssignExamToCourse(_mapper.Map<CourseExamDTO>(courseExamViewModel));
            return true;    
        }

        [HttpPost]
        public async Task AddQuestionToExam(ExamQuestionViewModel examQuestionViewModel)
        {
            await _instructorService.AddQuestionToExam(_mapper.Map<ExamQuestionDTO>(examQuestionViewModel)); 
        }
        
        [HttpPost]
        public async Task RemoveQuestionFromExam(ExamQuestionViewModel examQuestionViewModel)
        {
            await _instructorService.RemoveQuestionFromExam(_mapper.Map<ExamQuestionDTO>(examQuestionViewModel));
        }

    }
}