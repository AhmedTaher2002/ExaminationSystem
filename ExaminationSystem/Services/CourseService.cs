using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExaminationSystem.DTOs.Course;
using ExaminationSystem.DTOs.Exam;
using ExaminationSystem.DTOs.Student;
using ExaminationSystem.Models;
using ExaminationSystem.Models.Enums;
using ExaminationSystem.Repositories;
using ExaminationSystem.ViewModels.Response;
using Microsoft.EntityFrameworkCore;
using PredicateExtensions;
using System.Linq.Expressions;

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

        // Get all courses with instructor info
        public async Task<ResponseViewModel<IEnumerable<GetAllCoursesDTO>>> GetAll()
        {
            var courses = _courseRepository.GetAll().Include(i => i.Instructor);
            var result = await courses.ProjectTo<GetAllCoursesDTO>(_mapper.ConfigurationProvider).ToListAsync();

            // Check if no courses exist
            if (courses == null)
                return new FailResponseViewModel<IEnumerable<GetAllCoursesDTO>>("No Courses Found", ErrorCode.CourseNotFound);

            return new SuccessResponseViewModel<IEnumerable<GetAllCoursesDTO>>(result);
        }

        // Get course details by ID
        public async Task<ResponseViewModel<GetByIdCourseDTO>> GetByID(int id)
        {
            var course = _courseRepository.GetByID(id);
            var result = await course.ProjectTo<GetByIdCourseDTO>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

            // Validate course existence
            if (!_courseRepository.IsExists(id))
                return new FailResponseViewModel<GetByIdCourseDTO>("Course Id is not Exist", ErrorCode.InvalidCourseId);

            // Handle not found case
            if (result == null)
                return new FailResponseViewModel<GetByIdCourseDTO>("Course Not Found", ErrorCode.CourseNotFound);

            return new SuccessResponseViewModel<GetByIdCourseDTO>(result);
        }

        // Get courses using filters
        public async Task<ResponseViewModel<IEnumerable<GetAllCoursesDTO>>> Get(int? courseID, string? courseName, int? courseHours)
        {
            // At least one filter must be provided
            if (courseID == null && courseName == null && courseHours == null)
                return new FailResponseViewModel<IEnumerable<GetAllCoursesDTO>>("At least one filter must be provided", ErrorCode.InvalidCourseFilter);

            // Build dynamic predicate
            var predicate = CoursePredicateBuilder(courseID, courseName, courseHours);

            // Execute filtered query
            var courses = _courseRepository.Get(predicate);
            var result = await courses.ProjectTo<GetAllCoursesDTO>(_mapper.ConfigurationProvider).ToListAsync();

            return (result != null)
                ? new SuccessResponseViewModel<IEnumerable<GetAllCoursesDTO>>(result)
                : new FailResponseViewModel<IEnumerable<GetAllCoursesDTO>>("No Courses Found with the provided filters", ErrorCode.CourseNotFound);
        }

        // Create new course
        public async Task<ResponseViewModel<bool>> Create(CreateCourseDTO courseDTO)
        {
            // Prevent duplicate course name
            if (await _courseRepository.IsExist(courseDTO.Name))
                return new FailResponseViewModel<bool>("Course Not Created", ErrorCode.CourseAreadyExists);

            // Validate instructor
            if (!_instructorRepository.IsExists(courseDTO.InstructorID))
                return new FailResponseViewModel<bool>("InstructorID Not Exist", ErrorCode.InvalidInstrutorId);

            // Validate input
            if (courseDTO == null)
                return new FailResponseViewModel<bool>("Course Not Created", ErrorCode.CourseNotCreated);

            var course = _mapper.Map<Course>(courseDTO);
            await _courseRepository.AddAsync(course);

            return new SuccessResponseViewModel<bool>(true);
        }

        // Update existing course
        public async Task<ResponseViewModel<bool>> Update(int courseid, UpdateCourseDTO courseDTO)
        {
            // Validate instructor
            if (!_instructorRepository.IsExists(courseDTO.InstructorId))
                return new FailResponseViewModel<bool>("InstructorID Not Exist", ErrorCode.InvalidInstrutorId);

            // Get current course for fallback values
            var currentCourse = await _courseRepository.GetByIDWithTracking(courseid);

            // Replace default values with existing ones
            courseDTO = new()
            {
                Name = courseDTO.Name == "string" ? currentCourse.Name : courseDTO.Name,
                Description = courseDTO.Description == "string" ? currentCourse.Description : courseDTO.Description,
                Hours = courseDTO.Hours != 0 ? currentCourse.Hours : courseDTO.Hours,
                InstructorId = courseDTO.InstructorId != 0 ? courseDTO.InstructorId : currentCourse.InstructorId
            };

            var course = _mapper.Map<Course>(courseDTO);
            await _courseRepository.UpdateAsync(course);

            return new SuccessResponseViewModel<bool>(true);
        }

        // Soft delete course and related exams
        public async Task<ResponseViewModel<bool>> SoftDelete(int courseId)
        {
            // Validate course
            if (!_courseRepository.IsExists(courseId))
                return new FailResponseViewModel<bool>("Course Not Created", ErrorCode.CourseAreadyExists);

            // Remove all exams under this course
            await SoftDeleteAllExamsFromCourse(courseId);

            // Soft delete course
            await _courseRepository.SoftDeleteAsync(courseId);

            return new SuccessResponseViewModel<bool>(true);
        }

        // Get all students enrolled in a course
        public IEnumerable<GetAllStudentsDTO> GetStudentsInCourse(int courseId)
        {
            var students = _studentCourseRepository.GetStudentsByCourse(courseId);
            return _mapper.Map<List<GetAllStudentsDTO>>(students);
        }

        // Assign instructor to course
        public async Task<ResponseViewModel<bool>> AssignInstructorToCourse(int courseId, int instructorId)
        {
            // Validate course and instructor
            if (!_courseRepository.IsExists(courseId))
                return new FailResponseViewModel<bool>("Course Not Exist", ErrorCode.InvalidCourseId);
            if (!_instructorRepository.IsExists(instructorId))
                return new FailResponseViewModel<bool>("Instructor Not Exist", ErrorCode.InvalidInstrutorId);

            var course = await _courseRepository.GetByIDWithTracking(courseId);

            // Assign instructor
            course.InstructorId = instructorId;
            await _courseRepository.UpdateAsync(course);
            return new SuccessResponseViewModel<bool>(true);

        }

        // Assign exam to course
        public async Task<ResponseViewModel<bool>> AssignExamToCourse(int courseID, int examID)
        {
            // Validate course and exam
            if (!_courseRepository.IsExists(courseID))
                return new FailResponseViewModel<bool>("Course Not Exist", ErrorCode.InvalidCourseId);

            if (!_examRepository.IsExists(examID))
                return new FailResponseViewModel<bool>("Exam Not Exist", ErrorCode.InvalidExamId);

            var exam = await _examRepository.GetByID(examID).FirstOrDefaultAsync();

            // Ensure exam exists
            if (exam == null)
                return new FailResponseViewModel<bool>("Exam Not Found", ErrorCode.ExamNotFound);

            exam.CourseId = courseID;
            await _examRepository.UpdateAsync(exam);

            return new SuccessResponseViewModel<bool>(true);
        }

        public async Task<ResponseViewModel<IEnumerable<GetAllExamsDTO>>> GetExamsForCourse(int courseId)
        {
            // Validate course
            if (!_courseRepository.IsExists(courseId))
                return new FailResponseViewModel<IEnumerable<GetAllExamsDTO>>("Course Not Exist", ErrorCode.InvalidCourseId);
            
            var exams = _examRepository.GetExamsByCourse(courseId);
            var result = await exams.ProjectTo<GetAllExamsDTO>(_mapper.ConfigurationProvider).ToListAsync();
            
            return (result != null)
                ? new SuccessResponseViewModel<IEnumerable<GetAllExamsDTO>>(result)
                : new FailResponseViewModel<IEnumerable<GetAllExamsDTO>>("No Exams Found for the provided course", ErrorCode.ExamNotFound);

        }


        // Soft delete all exams under a course
        public async Task<ResponseViewModel<bool>> SoftDeleteAllExamsFromCourse(int courseID)
        {
            // Validate course
            if (!_courseRepository.IsExists(courseID))
                return new FailResponseViewModel<bool>("Course Not Exist", ErrorCode.InvalidCourseId);

            var course = await _courseRepository.GetCourseWithExams(courseID);

            // Soft delete each exam
            foreach (var exam in course.Exams)
                await _examRepository.SoftDeleteAsync(exam.ID);

            return new SuccessResponseViewModel<bool>(true);
        }

        // Build dynamic filtering predicate
        private Expression<Func<Course, bool>> CoursePredicateBuilder(int? courseID, string? courseName, int? courseHours)
        {
            var Predicate = PredicateExtensions.PredicateExtensions.Begin<Course>(true);

            if (courseID.HasValue)
                Predicate = Predicate.And(c => c.ID == courseID);

            if (courseHours.HasValue)
                Predicate = Predicate.And(c => c.Hours >= courseHours);

            if (!string.IsNullOrEmpty(courseName))
                Predicate = Predicate.And(c => c.Name == courseName);

            return Predicate;
        }

    }
}
