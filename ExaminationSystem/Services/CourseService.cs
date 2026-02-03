using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExaminationSystem.DTOs.Course;
using ExaminationSystem.DTOs.Exam;
using ExaminationSystem.DTOs.Other;
using ExaminationSystem.DTOs.Student;
using ExaminationSystem.Models;
using ExaminationSystem.Models.Enums;
using ExaminationSystem.Repositories;
using ExaminationSystem.ViewModels.Response;
using Microsoft.EntityFrameworkCore;
using PredicateExtensions;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ExaminationSystem.Services
{
    public class CourseService
    {
        #region Repositories & Mapper
        private readonly CourseRepository _courseRepository;
        private readonly ExamRepository _examRepository;
        private readonly InstructorRepository _instructorRepository;
        private readonly StudentCourseRepository _studentCourseRepository;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public CourseService(IMapper mapper)
        {
            _courseRepository = new CourseRepository();
            _examRepository = new ExamRepository();
            _instructorRepository = new InstructorRepository();
            _studentCourseRepository = new StudentCourseRepository();
            _mapper = mapper;
        }
        #endregion

        #region Course CRUD

        public async Task<ResponseViewModel<IEnumerable<GetAllCoursesDTO>>> GetAll()
        {
            var courses = _courseRepository.GetAll().Include(i => i.Instructor);
            var result = await courses.ProjectTo<GetAllCoursesDTO>(_mapper.ConfigurationProvider).ToListAsync();

            if (courses == null || !courses.Any())
                return new FailResponseViewModel<IEnumerable<GetAllCoursesDTO>>("No courses found", ErrorCode.CourseNotFound);

            return new SuccessResponseViewModel<IEnumerable<GetAllCoursesDTO>>(result);
        }

        public async Task<ResponseViewModel<GetByIdCourseDTO>> GetByID(int id)
        {
            if (!_courseRepository.IsExists(id))
                return new FailResponseViewModel<GetByIdCourseDTO>("Course ID does not exist", ErrorCode.InvalidCourseId);

            var course = _courseRepository.GetByID(id);
            var result = await course.ProjectTo<GetByIdCourseDTO>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

            if (result == null)
                return new FailResponseViewModel<GetByIdCourseDTO>("Course not found", ErrorCode.CourseNotFound);

            return new SuccessResponseViewModel<GetByIdCourseDTO>(result);
        }

        public async Task<ResponseViewModel<IEnumerable<GetAllCoursesDTO>>> Get(int? courseID, string? courseName, int? courseHours)
        {
            if (courseID == null && courseName == null && courseHours == null)
                return new FailResponseViewModel<IEnumerable<GetAllCoursesDTO>>("At least one filter must be provided", ErrorCode.InvalidCourseFilter);

            var predicate = CoursePredicateBuilder(courseID, courseName, courseHours);
            var courses = _courseRepository.Get(predicate);
            var result = await courses.ProjectTo<GetAllCoursesDTO>(_mapper.ConfigurationProvider).ToListAsync();

            return (result != null && result.Any())
                ? new SuccessResponseViewModel<IEnumerable<GetAllCoursesDTO>>(result)
                : new FailResponseViewModel<IEnumerable<GetAllCoursesDTO>>("No courses found with the provided filters", ErrorCode.CourseNotFound);
        }

        public async Task<ResponseViewModel<bool>> Create(CreateCourseDTO courseDTO)
        {
            if (await _courseRepository.IsExist(courseDTO.Name))
                return new FailResponseViewModel<bool>("Course already exists", ErrorCode.CourseAlreadyExists);

            if (!_instructorRepository.IsExists(courseDTO.InstructorID))
                return new FailResponseViewModel<bool>("Instructor ID does not exist", ErrorCode.InvalidInstructorId);

            if (courseDTO == null)
                return new FailResponseViewModel<bool>("Course data is invalid", ErrorCode.CourseNotCreated);

            var course = _mapper.Map<Course>(courseDTO);
            await _courseRepository.AddAsync(course);

            return new SuccessResponseViewModel<bool>(true);
        }

        public async Task<ResponseViewModel<bool>> Update(int courseId, UpdateCourseDTO courseDTO)
        {
            if (!_instructorRepository.IsExists(courseDTO.InstructorId))
                return new FailResponseViewModel<bool>("Instructor ID does not exist", ErrorCode.InvalidInstructorId);

            var currentCourse = await _courseRepository.GetByIDWithTracking(courseId);

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

        public async Task<ResponseViewModel<bool>> SoftDelete(int courseId)
        {
            if (!_courseRepository.IsExists(courseId))
                return new FailResponseViewModel<bool>("Course does not exist", ErrorCode.InvalidCourseId);

            await SoftDeleteAllExamsFromCourse(courseId);
            await _courseRepository.SoftDeleteAsync(courseId);

            return new SuccessResponseViewModel<bool>(true);
        }

        #endregion

        #region Course Enrollment

        public async Task<ResponseViewModel<IEnumerable<StudentCourseDTO>>> GetStudentsInCourse(int courseId)
        {
            var students = _studentCourseRepository.GetStudentsByCourse(courseId);
            var result = await students.ProjectTo<StudentCourseDTO>(_mapper.ConfigurationProvider).ToListAsync();

            return (result != null && result.Any())
                ? new SuccessResponseViewModel<IEnumerable<StudentCourseDTO>>(result)
                : new FailResponseViewModel<IEnumerable<StudentCourseDTO>>("No students enrolled in this course", ErrorCode.CourseHasNoStudents);
        }

        #endregion

        #region Assignments & Exams

        public async Task<ResponseViewModel<bool>> AssignInstructorToCourse(int courseId, int instructorId)
        {
            if (!_courseRepository.IsExists(courseId))
                return new FailResponseViewModel<bool>("Course does not exist", ErrorCode.InvalidCourseId);
            if (!_instructorRepository.IsExists(instructorId))
                return new FailResponseViewModel<bool>("Instructor does not exist", ErrorCode.InvalidInstructorId);

            var course = await _courseRepository.GetByIDWithTracking(courseId);
            course.InstructorId = instructorId;
            await _courseRepository.UpdateAsync(course);

            return new SuccessResponseViewModel<bool>(true);
        }

        public async Task<ResponseViewModel<bool>> AssignExamToCourse(int courseID, int examID)
        {
            if (!_courseRepository.IsExists(courseID))
                return new FailResponseViewModel<bool>("Course does not exist", ErrorCode.InvalidCourseId);
            if (!_examRepository.IsExists(examID))
                return new FailResponseViewModel<bool>("Exam does not exist", ErrorCode.InvalidExamId);

            var exam = await _examRepository.GetByID(examID).FirstOrDefaultAsync();
            if (exam == null)
                return new FailResponseViewModel<bool>("Exam not found", ErrorCode.ExamNotFound);

            exam.CourseId = courseID;
            await _examRepository.UpdateAsync(exam);

            return new SuccessResponseViewModel<bool>(true);
        }

        public async Task<ResponseViewModel<IEnumerable<GetAllExamsDTO>>> GetExamsForCourse(int courseId)
        {
            if (!_courseRepository.IsExists(courseId))
                return new FailResponseViewModel<IEnumerable<GetAllExamsDTO>>("Course does not exist", ErrorCode.InvalidCourseId);

            var exams = _examRepository.GetExamsByCourse(courseId);
            var result = await exams.ProjectTo<GetAllExamsDTO>(_mapper.ConfigurationProvider).ToListAsync();

            return (result != null && result.Any())
                ? new SuccessResponseViewModel<IEnumerable<GetAllExamsDTO>>(result)
                : new FailResponseViewModel<IEnumerable<GetAllExamsDTO>>("No exams found for this course", ErrorCode.ExamNotFound);
        }

        public async Task<ResponseViewModel<bool>> SoftDeleteAllExamsFromCourse(int courseID)
        {
            if (!_courseRepository.IsExists(courseID))
                return new FailResponseViewModel<bool>("Course does not exist", ErrorCode.InvalidCourseId);

            var course = await _courseRepository.GetCourseWithExams(courseID);

            foreach (var exam in course.Exams)
                await _examRepository.SoftDeleteAsync(exam.ID);

            return new SuccessResponseViewModel<bool>(true);
        }

        #endregion

        #region Private Helpers

        private Expression<Func<Course, bool>> CoursePredicateBuilder(int? courseID, string? courseName, int? courseHours)
        {
            var predicate = PredicateExtensions.PredicateExtensions.Begin<Course>(true);

            if (courseID.HasValue)
                predicate = predicate.And(c => c.ID == courseID);
            if (courseHours.HasValue)
                predicate = predicate.And(c => c.Hours >= courseHours);
            if (!string.IsNullOrEmpty(courseName))
                predicate = predicate.And(c => c.Name == courseName);

            return predicate;
        }

        #endregion
    }
}
