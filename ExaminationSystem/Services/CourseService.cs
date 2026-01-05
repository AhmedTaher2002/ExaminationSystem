using AutoMapper;
using ExaminationSystem.DTOs.Course;
using ExaminationSystem.DTOs.Student;
using ExaminationSystem.Models;
using ExaminationSystem.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace ExaminationSystem.Services
{
    public class CourseService
    {
        private readonly CourseRepository _courseRepository;
        private readonly ExamRepository _examRepository;
        private readonly InstructorRepository _instructorRepository;
        private readonly StudentCourseRepository _studentCourseRepository;
        private readonly IMapper _mapper;

        public CourseService(IMapper mapper)
        {
            _courseRepository = new CourseRepository();
            _examRepository = new ExamRepository();
            _instructorRepository = new InstructorRepository();
            _studentCourseRepository = new StudentCourseRepository();
            _mapper = mapper;
        }

        public IEnumerable<GetAllCoursesDTO> GetAll()
        {
            /*var res= _courseRepository.GetAll()
                .Select(course => new GetAllCoursesDTO
                {
                    ID = course.ID,
                    Name = course.Name,
                    Description = course.Description,
                    Hours = course.Hours,
                    Instructor = new GetInstructorInfoDTO
                    {
                        ID = course.Instructor.ID,
                        Name = course.Instructor.FullName
                    }
                }).AsNoTracking().ToList();
            */
            var courses = _courseRepository.GetAll().Include(i => i.Instructor).AsNoTracking().ToList();
            var res = _mapper.Map<IEnumerable<GetAllCoursesDTO>>(courses);
            return res;
        }

        public async Task<GetByIdCourseDTO> GetByID(int id)
        {
            if (!_courseRepository.IsExist(id))
                throw new Exception("Course Not Found");

            var course = await _courseRepository.Get(c=>c.ID == id).Include(i=>i.Instructor).AsNoTracking().FirstOrDefaultAsync();
            return _mapper.Map<GetByIdCourseDTO>(course);
            /*
            return new GetByIdCourseDTO
            {
                ID = course.ID,
                Name = course.Name,
                Description = course.Description,
                Hours = course.Hours,
                Instructor = new GetInstructorInfoDTO
                {
                    ID = course.Instructor.ID,
                    Name = course.Instructor.FullName
                }
            };*/
        }

        public IEnumerable<GetAllCoursesDTO> Get(int? courseID, string? courseName, int? courseHours)
        {
            var query = _courseRepository.GetAll();

            if (courseID.HasValue)
                query = query.Where(c => c.ID == courseID);

            if (!string.IsNullOrWhiteSpace(courseName))
                query = query.Where(c => c.Name.Contains(courseName));

            if (courseHours.HasValue)
                query = query.Where(c => c.Hours == courseHours);
            /*
            return query
                .Select(course => new GetAllCoursesDTO
                {
                    ID = course.ID,
                    Name = course.Name,
                    Description = course.Description,
                    Hours = course.Hours,
                    Instructor = new GetInstructorInfoDTO
                    {
                        ID = course.Instructor.ID,
                        Name = course.Instructor.FullName
                    }
                }).ToList();*/
            var courses = query.Include(i => i.Instructor).AsNoTracking().ToList();
            var res = _mapper.Map<IEnumerable<GetAllCoursesDTO>>(courses);
            return res;
        }

        public async Task Create(CreateCourseDTO courseDTO)
        {
            if (_courseRepository.IsExist(courseDTO.Name).Result)
                throw new Exception("Course is Exist");

            if (!_instructorRepository.IsExist(courseDTO.InstructorID))
                throw new Exception("Instructor Not Found");
            /*
            var course = new Course
            {
                Name = courseDTO.Name,
                Description = courseDTO.Description,
                Hours = courseDTO.Hours,
                InstructorId = courseDTO.InstructorID
            };
            */
            var course = _mapper.Map<Course>(courseDTO);
            await _courseRepository.Add(course);
        }

        public async Task Update(int courseid, UpdateCourseDTO courseDTO)
        {

            if (!_instructorRepository.IsExist(courseDTO.InstructorId))
                throw new Exception("Instructor Not Found");
            var currentCourse =await _courseRepository.GetByIDWithTracking(courseid);
            courseDTO = new()
            {
                Name = courseDTO.Name == "string" ? currentCourse.Name : courseDTO.Name,
                Description = courseDTO.Description == "string" ? currentCourse.Description : courseDTO.Description,
                Hours = courseDTO.Hours != 0 ? currentCourse.Hours : courseDTO.Hours,
                InstructorId = courseDTO.InstructorId != 0 ? courseDTO.InstructorId : currentCourse.InstructorId
            };
            /*
            var course = new Course
            {
                ID = id,
                Name = courseDTO.Name,
                Description = courseDTO.Description,
                Hours = courseDTO.Hours,
                InstructorId = courseDTO.InstructorId
            };*/
            var course = _mapper.Map<Course>(courseDTO);

            await _courseRepository.Update(course);
        }

        public async Task SoftDelete(int courseId)
        {
            if (!_courseRepository.IsExist(courseId))
                throw new Exception("Course Not Found");
            //Soft Delete all exams related to this course
            await SoftDeleteAllExamsFromCourse(courseId);
            await _courseRepository.SoftDelete(courseId);
        }

        public async Task HardDelete(int CourseId)
        {
            if (!_courseRepository.IsExist(CourseId))
                throw new Exception("Course Not Found");
            //Hard Delete all exams related to this course
            await _courseRepository.HardDelete(CourseId);
        }

        public IEnumerable<GetAllStudentsDTO> GetStudents(int courseId)
        {
            var students = _studentCourseRepository.GetStudentsByCourse(courseId);
            return _mapper.Map<List<GetAllStudentsDTO>>(students);
        }

        public async Task<bool> AssignExamToCourse(int courseID, int examID)
        {
            if (!_courseRepository.IsExist(courseID))
                throw new Exception("Course Not Found");

            if (!_examRepository.IsExist(examID))
                throw new Exception("Exam Not Found");

            var exam = await _examRepository.GetByID(examID);
            exam.CourseId = courseID;

            await _examRepository.Update(exam);
            return true;
        }

        public async Task SoftDeleteAllExamsFromCourse(int courseID)
        {
            if (!_courseRepository.IsExist(courseID))
                throw new Exception("Course Not Found");

            var course = _courseRepository.GetCourseWithExams(courseID).Result;

            foreach (var exam in course.Exams)
            {
                await _examRepository.SoftDelete(exam.ID);
            }
        }

        public async Task HardDeleteAllExamsFromCourse(int courseID)
        {
            if (!_courseRepository.IsExist(courseID))
                throw new Exception("Course Not Found");

            var course = _courseRepository.GetCourseWithExams(courseID).Result;

            foreach (var exam in course.Exams)
            {
                await _examRepository.HardDelete(exam.ID);
            }
        }
        

    }
}
