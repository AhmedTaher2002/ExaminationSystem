using AutoMapper;
using ExaminationSystem.DTOs.Exam;
using ExaminationSystem.DTOs.Other;
using ExaminationSystem.DTOs.Student;
using ExaminationSystem.Models;
using ExaminationSystem.Models.Enums;
using ExaminationSystem.Services;
using ExaminationSystem.ViewModels.Exam;
using ExaminationSystem.ViewModels.Other;
using ExaminationSystem.ViewModels.Student;
using Microsoft.AspNetCore.Mvc;
namespace ExaminationSystem.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentService _studentService;
        private readonly IMapper _mapper;

        public StudentController(IMapper mapper)
        {
            _studentService = new StudentService(mapper);
            _mapper = mapper;
        }


        [HttpGet]
        public IEnumerable<GetAllStudentsViewModel> GetAll()
        {
            return _mapper.Map<IEnumerable<GetAllStudentsViewModel>>(_studentService.GetAll());
            /*
            return _studentService.GetAll()
                .Select(s => new GetAllStudentsViewModel
                {
                    ID = s.ID,
                    Name = s.Name,
                    Email = s.Email
                });
            */
        }

        [HttpGet]
        public async Task<GetStudentByIdViewModel> GetByID(int id)
        {
            return _mapper.Map<GetStudentByIdViewModel>(_studentService.GetByID(id));
        }

        
        [HttpPost]
        public async Task<bool> Create(CreateStudentViewModel createStudentViewModel)
        {
            /*
            await _studentService.Create(new CreateStudentDTO
            {
                Name = vm.Name,
                Email = vm.Email
            });
            */
            await _studentService.Create(_mapper.Map<CreateStudentDTO>(createStudentViewModel));
            return true;
        }

        [HttpPut]
        public async Task<bool> Update(UpdateStudentViewModel vm)
        {
            var dto = _mapper.Map<UpdateStudentDTO>(vm);
            await _studentService.Update(dto);
            return true;
        }

        [HttpDelete]
        public async Task<bool> SoftDelete(int id)
        {
            await _studentService.SoftDelete(id);
            return true;
        }

        [HttpDelete]
        public async Task<bool> HardDelete(int id)
        {
            await _studentService.HardDelete(id);
            return true;
        }

        [HttpPost]
        public async Task HardDeleteStudentFromCourse(StudentCourseViewModel studentCourseVM)
        {
            await _studentService.HardDeleteStudentFromCourse(_mapper.Map<StudentCourseDTO>(studentCourseVM));
        }
        [HttpPost]
        public async Task SoftDeleteStudentFromCourse(StudentCourseViewModel studentCourseVM)
        {
            await _studentService.SoftDeleteStudentFromCourse(_mapper.Map<StudentCourseDTO>(studentCourseVM));
        }

        [HttpGet]
        public IEnumerable<GetExamsForStudentViewModel> GetExamsForStudent(int studentId)
        {
            return _mapper.Map<IEnumerable<GetExamsForStudentViewModel>>(_studentService.GetExamsForStudent(studentId));
        }

    }
}