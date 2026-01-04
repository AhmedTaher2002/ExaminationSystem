using Microsoft.AspNetCore.Mvc;
using ExaminationSystem.DTOs.Course;
using ExaminationSystem.Services;
using ExaminationSystem.ViewModels.Course;
using ExaminationSystem.ViewModels.Instructor;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;

namespace ExaminationSystem.Controllers
{
    [Route("[controller]/[Action]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        CourseService _courseService;
        IMapper _mapper;
        public CourseController(IMapper mapper)
        {
            _mapper= mapper;
            _courseService = new CourseService(mapper);
        }


        
        [HttpGet]
        public IEnumerable<GetAllCoursesViewModel> GetAll() // IQueryable and Serilization 
        {

            var Courses = _courseService.GetAll();
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
            return _mapper.Map<IEnumerable<GetAllCoursesViewModel>>(Courses);

        }



        [HttpGet]
        public async Task<GetByIdCourseViewModel> GetByID(int id)
        {
           
            var res = await _courseService.GetByID(id);
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
        }



        [HttpGet]
        public IEnumerable<GetAllCoursesViewModel> Get(int? courseID, string? courseName, int? courseHours) // IQueryable and Serilization // IList For best practice
        {
            var res = _courseService.Get(courseID, courseName, courseHours);
            List<GetAllCoursesViewModel> getAllCoursesViewModel = new List<GetAllCoursesViewModel>();
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
        }



        [HttpPost]
        public async Task Create(CreateCourseViewModel course)
        {
            /*
            var dto = new CreateCourseDTO
            {
               Name=course.Name,
               Description=course.Description,
               Hours=course.Hours,
               InstructorID=course.InstructorID
            };
            await _courseService.Create(dto);*/
            await _courseService.Create(_mapper.Map<CreateCourseDTO>(course));
           
        }


        [HttpPut]
        public async Task Update(int courseid,UpdateCourseViewModel viewModel)
        {
            var currentCourse = _courseService.GetByID(courseid).Result;
            viewModel = new UpdateCourseViewModel
            {
                
                Name = viewModel.Name =="string"? currentCourse.Name: viewModel.Name,
                Description = viewModel.Description == "string" ? currentCourse.Description :viewModel.Description,
                Hours = viewModel.Hours != 0 ? viewModel.Hours : currentCourse.Hours,
                InstructorId = viewModel.InstructorId != 0 ? viewModel.InstructorId : currentCourse.Instructor.ID
            };
            // await _courseService.Update(dto);
            /*
            var dto = new UpdateCourseDTO
            {
                
                Name = viewModel.Name,
                Description = viewModel.Description,
                Hours = viewModel.Hours,
                InstructorId = viewModel.InstructorId
            };
            */
            await _courseService.Update(_mapper.Map<UpdateCourseDTO>(viewModel));
            
        }
        



        [HttpDelete]
        public async Task<bool> SoftDelete(int id)
        {
            await _courseService.SoftDelete(id);

            return true;
        }


        [HttpDelete]
        public async Task DeleteCourse(int id)
        {             //real delete
            var res =_courseService.HardDelete(id);
            
        }

        [HttpPost]
        public async Task<bool> AssignExamToCourse(int courseID, int examID)
        {
            var res =_courseService.AssignExamToCourse(courseID, examID);
            return res.Result;
        }

   

    }
}
