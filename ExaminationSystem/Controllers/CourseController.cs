using AutoMapper;
using ExaminationSystem.DTOs.Course;
using ExaminationSystem.DTOs.Student;
using ExaminationSystem.Services;
using ExaminationSystem.ViewModels.Course;
using ExaminationSystem.ViewModels.Instructor;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ExaminationSystem.Controllers
{
    [Route("[controller]/[Action]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly CourseService _courseService;
        private readonly IMapper _mapper;
        public CourseController(IMapper mapper)
        {
            _mapper= mapper;
            _courseService = new CourseService(mapper);
        }
        
        [HttpGet]
        public IEnumerable<GetAllCoursesViewModel> GetAll()  
        {
            var Courses = _courseService.GetAll();
            return _mapper.Map<IEnumerable<GetAllCoursesViewModel>>(Courses);
            /*
            List<GetAllCoursesViewModel> result = new List<GetAllCoursesViewModel>();
            foreach (var course in Courses)
            {
                    var courseViewModel = new GetAllCoursesViewModel
                    {
                        ID = course.ID,
                        Name = course.Name,
                        Description = course.Description,
                        Instructor = new GetInstuctorInfoViewModel
                            {
                               ID = course.Instructor.ID,
                               Name = course.Instructor.Name
                            }
                        
                    };
                    result.Add(courseViewModel);
                
            }
            return result.ToList();
            */
        }

        [HttpGet]
        public async Task<GetByIdCourseViewModel> GetByID(int id)
        {
            var course =_mapper.Map<GetByIdCourseViewModel>(await _courseService.GetByID(id));
            return course;
            /*
            var courseViewModel = new GetByIdCourseViewModel
            {
                ID = res.ID,
                Name = res.Name,
                Description = res.Description,
                Hours = res.Hours,
                Instructor = new DTOs.Instructor.GetInstructorInfoDTO
                {
                    ID = res.Instructor.ID,
                    Name = res.Instructor.Name,
                }
            };
            return courseViewModel;
            */
        }

        [HttpGet]
        public IEnumerable<GetAllCoursesViewModel> Get(int? courseID, string? courseName, int? courseHours) 
        {
            var res = _courseService.Get(courseID, courseName, courseHours);
            return _mapper.Map<IEnumerable<GetAllCoursesViewModel>>(res);
            /*
            List<GetAllCoursesViewModel> getAllCoursesViewModel = new();
            foreach (var item in res)
            {
                getAllCoursesViewModel.Add(new GetAllCoursesViewModel
                {
                    ID = item.ID,
                    Name = item.Name,
                    Description = item.Description,
                    Instructor = new GetInstuctorInfoViewModel
                    {
                        ID = item.Instructor.ID,
                        Name = item.Instructor.Name
                    }
                });
            }
            return getAllCoursesViewModel;
            */
        }

        [HttpPost]
        public async Task Create(CreateCourseViewModel course)
        {
            await _courseService.Create(_mapper.Map<CreateCourseDTO>(course));
            /*
            var dto = new CreateCourseDTO
            {
               Name=course.Name,
               Description=course.Description,
               Hours=course.Hours,
               InstructorID=course.InstructorID
            };
            await _courseService.Create(dto);*/
        }

        [HttpPut]
        public async Task Update(int courseid,UpdateCourseViewModel viewModel)
        {
            await _courseService.Update(courseid,_mapper.Map<UpdateCourseDTO>(viewModel));
            /*
            var dto = new UpdateCourseDTO
            {
                
                Name = viewModel.Name,
                Description = viewModel.Description,
                Hours = viewModel.Hours,
                InstructorId = viewModel.InstructorId
            };
            await _courseService.Update(dto);
            */
        }
        
        [HttpDelete]
        public async Task<bool> SoftDelete(int id)
        {
            await _courseService.SoftDelete(id);
            return true;
        }

        [HttpDelete]
        public async Task DeleteCourse(int id)
        {            
             await _courseService.HardDelete(id);
        }

        [HttpGet]
        public IEnumerable<GetAllStudentsDTO> GetStudents(int courseId)
        {
            return _courseService.GetStudents(courseId);
        }
        
        [HttpPost]
        public async Task<bool> AssignExamToCourse(int courseID, int examID)
        {
            var res =_courseService.AssignExamToCourse(courseID, examID);
            return res.Result;
        }

        [HttpPut]
        public async Task SoftDeleteAllExamsFromCourse(int courseID)
        {
            await _courseService.SoftDeleteAllExamsFromCourse(courseID);
        }
    }
}
