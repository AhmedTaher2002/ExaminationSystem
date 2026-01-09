using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExaminationSystem.DTOs.Course;
using ExaminationSystem.DTOs.Exam;
using ExaminationSystem.DTOs.Instructor;
using ExaminationSystem.DTOs.Other;
using ExaminationSystem.DTOs.Question;
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
    public class InstructorService
    {
        private readonly InstructorRepository _instructorRepository;
        private readonly ExamRepository _examRepository;
        private readonly QuestionRepository _questionRepository;
        private readonly ExamQuestionRepository _examQuestionRepository;
        private readonly StudentAnswerRepository _studentAnswerRepository;
        private readonly StudentRepository _studentRepository;
        private readonly CourseRepository _courseRepository;
        private readonly StudentCourseRepository _studentCourseRepository;
        private readonly IMapper _mapper;

        public InstructorService(IMapper mapper)
        {
            _instructorRepository = new InstructorRepository();
            _examRepository = new ExamRepository();
            _questionRepository = new QuestionRepository();
            _examQuestionRepository = new ExamQuestionRepository();
            _studentRepository = new StudentRepository();
            _courseRepository = new CourseRepository();
            _studentCourseRepository = new StudentCourseRepository();
            _studentAnswerRepository = new StudentAnswerRepository();
            _mapper = mapper;
        }

        // Get all instructors
        public async Task<ResponseViewModel<IEnumerable<GetAllInstructorsDTO>>> GetAll()
        {
            var instructors = _instructorRepository.GetAll();
            var result = await instructors.ProjectTo<GetAllInstructorsDTO>(_mapper.ConfigurationProvider)
                                          .AsNoTracking().ToListAsync();
            return (result != null) ? new SuccessResponseViewModel<IEnumerable<GetAllInstructorsDTO>>(result)
                                    : new FailResponseViewModel<IEnumerable<GetAllInstructorsDTO>>("No Courses Found", ErrorCode.CourseNotFound);
        }

        // Get instructor by ID
        public async Task<ResponseViewModel<GetInstructorByIdDTO>> GetByID(int id)
        {
            if (!_instructorRepository.IsExists(id))
                return new FailResponseViewModel<GetInstructorByIdDTO>("InstructorId Not Exist", ErrorCode.InvalidInstrutorId);

            var instructor = await _instructorRepository.GetByID(id)
                                                        .ProjectTo<GetInstructorByIdDTO>(_mapper.ConfigurationProvider)
                                                        .AsNoTracking()
                                                        .FirstOrDefaultAsync();

            return (instructor != null) ? new SuccessResponseViewModel<GetInstructorByIdDTO>(instructor)
                                       : new FailResponseViewModel<GetInstructorByIdDTO>("Instructor not Returned", ErrorCode.InstrutorNotFound);
        }

        // Filter instructors dynamically
        public Task<ResponseViewModel<IEnumerable<GetAllInstructorsDTO>>> Get(int? id, string? name, string? Username, string? email)
        {
            var predicate = InstructorPredicateBuilder(id, name, Username, email);
            var instructors = _instructorRepository.Get(predicate);
            var result = instructors.ProjectTo<GetAllInstructorsDTO>(_mapper.ConfigurationProvider).AsNoTracking();

            return (result != null) ? Task.FromResult<ResponseViewModel<IEnumerable<GetAllInstructorsDTO>>>(new SuccessResponseViewModel<IEnumerable<GetAllInstructorsDTO>>(result))
                                    : Task.FromResult<ResponseViewModel<IEnumerable<GetAllInstructorsDTO>>>(new FailResponseViewModel<IEnumerable<GetAllInstructorsDTO>>("No Instructors Found", ErrorCode.InstrutorNotFound));
        }

        // Create a new instructor
        public async Task<ResponseViewModel<bool>> Create(CreateInstructorDTO dto)
        {
            var instructor = _mapper.Map<Instructor>(dto);
            await _instructorRepository.AddAsync(instructor);
            return new SuccessResponseViewModel<bool>(true);
        }

        // Update existing instructor
        public async Task<ResponseViewModel<bool>> Update(int id, UpdateInstructorDTO dto)
        {
            if (!_instructorRepository.IsExists(id))
                return new FailResponseViewModel<bool>("InstructorId Not Exist", ErrorCode.InvalidInstrutorId);

            var instruc = await _instructorRepository.GetByIDWithTracking(id);

            // Keep existing values if defaults are passed
            dto = new()
            {
                Name = dto.Name == "string" ? instruc.FullName : dto.Name,
                Username = dto.Username == "string" ? instruc.Username : dto.Username,
            };

            var instructor = _mapper.Map<Instructor>(dto);
            await _instructorRepository.UpdateAsync(instructor);
            return new SuccessResponseViewModel<bool>(true);
        }

        // Soft delete instructor
        public async Task<ResponseViewModel<bool>> SoftDelete(int instructorId)
        {
            if (!_instructorRepository.IsExists(instructorId))
                return new FailResponseViewModel<bool>("InstructorId Not Exist", ErrorCode.InvalidInstrutorId);

            await _instructorRepository.SoftDeleteAsync(instructorId);
            return new SuccessResponseViewModel<bool>(true);
        }

        // Get all courses assigned to an instructor
        public async Task<ResponseViewModel<IEnumerable<GetAllCoursesDTO>>> GeTCoursesForInstructor(int instructorId)
        {
            var courses = _courseRepository.Get(c => c.InstructorId == instructorId);
            var result = courses.ProjectTo<GetAllCoursesDTO>(_mapper.ConfigurationProvider).AsNoTracking().ToList();
            return (result != null) ? new SuccessResponseViewModel<IEnumerable<GetAllCoursesDTO>>(result)
                                    : new FailResponseViewModel<IEnumerable<GetAllCoursesDTO>>("Instructor Not Found", ErrorCode.InstrutorNotFound);
        }

        // Get students enrolled in a specific course
        public async Task<ResponseViewModel<IEnumerable<GetAllStudentsDTO>>> GetStudentsInCourse(int courseId)
        {
            if (!_courseRepository.IsExists(courseId))
                return new FailResponseViewModel<IEnumerable<GetAllStudentsDTO>>("InstructorId Not Exist", ErrorCode.InvalidInstrutorId);

            var student = _studentCourseRepository.GetByStudentCourses(courseId)
                                                  .Include(sc => sc.Student)
                                                  .Select(sc => sc.Student)
                                                  .AsNoTracking();
            var result = student.ProjectTo<GetAllStudentsDTO>(_mapper.ConfigurationProvider).AsNoTracking().ToList();
            return new SuccessResponseViewModel<IEnumerable<GetAllStudentsDTO>>(result);
        }

        // Get questions created by an instructor
        public async Task<ResponseViewModel<IEnumerable<GetAllQuestionsDTO>>> GetQuestionsForInstructor(int instructorId)
        {
            if (!_instructorRepository.IsExists(instructorId))
                return new FailResponseViewModel<IEnumerable<GetAllQuestionsDTO>>("InstructorId Not Exist", ErrorCode.InvalidInstrutorId);

            var questions = _questionRepository.Get(i => i.InstructorId == instructorId && !i.IsDeleted);
            var result = questions.ProjectTo<GetAllQuestionsDTO>(_mapper.ConfigurationProvider).AsNoTracking().ToList();
            return new SuccessResponseViewModel<IEnumerable<GetAllQuestionsDTO>>(result);
        }

        // Get questions assigned to an exam
        public async Task<ResponseViewModel<IEnumerable<GetAllQuestionsDTO>>> GetQuestionsByExam(int examId)
        {
            if (!_examRepository.IsExists(examId))
                new FailResponseViewModel<IEnumerable<GetAllExamsDTO>>("ExamId Not Exists", ErrorCode.InvalidExamId);

            var examQuestions = _examQuestionRepository.GetQuestionsByExam(examId);
            var result = _mapper.Map<IEnumerable<GetAllQuestionsDTO>>(examQuestions);
            return (result != null) ? new SuccessResponseViewModel<IEnumerable<GetAllQuestionsDTO>>(result)
                                    : new FailResponseViewModel<IEnumerable<GetAllQuestionsDTO>>("Exam question Not Found", ErrorCode.QuestionNotFound);
        }

        // Get a student's answers for an exam
        public async Task<ResponseViewModel<IEnumerable<GetStudentAnswersDTO>>> GetStudentAnswers(StudentExamDTO studentExamDTO)
        {
            var result = _studentAnswerRepository.Get(studentExamDTO.StudentId, studentExamDTO.ExamId)
                                                 .Include(a => a.Question)
                                                 .Include(a => a.SelectedChoice)
                                                 .AsNoTracking();

            var studentAnswer = await result.ProjectTo<IEnumerable<GetStudentAnswersDTO>>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
            return (studentAnswer != null) ? new SuccessResponseViewModel<IEnumerable<GetStudentAnswersDTO>>(studentAnswer)
                                           : new FailResponseViewModel<IEnumerable<GetStudentAnswersDTO>>("No Answers Found", ErrorCode.StudentNotSubmittedExam);
        }

        // Assign a student to a course
        public async Task<ResponseViewModel<bool>> AssignStudentToCourse(StudentCourseDTO studentCourseDTO)
        {
            if (!_studentRepository.IsExists(studentCourseDTO.StudentId))
                return new FailResponseViewModel<bool>("Student Not Found", ErrorCode.StudentNotFound);
            if (!_courseRepository.IsExists(studentCourseDTO.CourseId))
                return new FailResponseViewModel<bool>("Course Not Found", ErrorCode.InvalidCourseId);
            if (_studentCourseRepository.IsAssigned(studentCourseDTO.StudentId, studentCourseDTO.CourseId))
                return new FailResponseViewModel<bool>("Student Already Assigned To Course", ErrorCode.StudentNotEnrolledInCourse);

            await _studentCourseRepository.Add(_mapper.Map<StudentCourse>(studentCourseDTO));
            return new SuccessResponseViewModel<bool>(true);
        }

        // Assign an exam to a course
        public async Task<ResponseViewModel<bool>> AssignExamToCourse(CourseExamDTO courseExamDTO)
        {
            if (!_courseRepository.IsExists(courseExamDTO.CourseId))
                return new FailResponseViewModel<bool>("Course Not Found", ErrorCode.InvalidCourseId);
            if (!_examRepository.IsExists(courseExamDTO.ExamId))
                return new FailResponseViewModel<bool>("ExamId Not Exists", ErrorCode.InvalidExamId);

            var exam = await _examRepository.GetByIDWithTracking(courseExamDTO.ExamId);
            exam.CourseId = courseExamDTO.CourseId;
            await _examRepository.UpdateAsync(exam);
            return new SuccessResponseViewModel<bool>(true);
        }

        // Add a question to an exam
        public async Task<ResponseViewModel<bool>> AddQuestionToExam(ExamQuestionDTO examQuestionDTO)
        {
            if (!_examRepository.IsExists(examQuestionDTO.ExamId))
                return new FailResponseViewModel<bool>("ExamId Not Exists", ErrorCode.InvalidExamId);
            if (!_questionRepository.IsExists(examQuestionDTO.QuestionId))
                return new FailResponseViewModel<bool>("Question Not Exists", ErrorCode.InvalidQuestionId);
            if (_examQuestionRepository.IsAssigned(examQuestionDTO.ExamId, examQuestionDTO.QuestionId))
                return new FailResponseViewModel<bool>("Question Not Assigned to this Exam", ErrorCode.QuestionNotAssignedToExam);

            var examQuestion = _mapper.Map<ExamQuestion>(examQuestionDTO);
            await _examQuestionRepository.Add(examQuestion);
            return (examQuestion != null) ? new SuccessResponseViewModel<bool>(true)
                                          : new FailResponseViewModel<bool>("Question Not Assigned to this Exam", ErrorCode.QuestionNotAssignedToExam);
        }

        // Remove a question from an exam
        public async Task<ResponseViewModel<bool>> RemoveQuestionFromExam(ExamQuestionDTO examQuestionDTO)
        {
            if (!_examQuestionRepository.IsAssigned(examQuestionDTO.ExamId, examQuestionDTO.QuestionId))
                return new FailResponseViewModel<bool>("Question Not Assigned to this Exam", ErrorCode.QuestionNotAssignedToExam);

            await _examQuestionRepository.SoftDelete(examQuestionDTO.ExamId, examQuestionDTO.QuestionId);
            return new SuccessResponseViewModel<bool>(true);
        }

        // Build dynamic predicate for filtering instructors
        private Expression<Func<Instructor, bool>> InstructorPredicateBuilder(int? id, string? name, string? Username, string? email)
        {
            var predicate = PredicateExtensions.PredicateExtensions.Begin<Instructor>(true);
            if (id.HasValue)
                predicate = predicate.And(i => i.ID == id.Value);
            if (!string.IsNullOrEmpty(name))
                predicate = predicate.And(i => i.FullName.Contains(name));
            if (!string.IsNullOrEmpty(email))
                predicate = predicate.And(i => i.Email.Contains(email));
            if (!string.IsNullOrEmpty(Username))
                predicate = predicate.And(i => i.Username.Contains(Username));
            return predicate;
        }
    }
}
