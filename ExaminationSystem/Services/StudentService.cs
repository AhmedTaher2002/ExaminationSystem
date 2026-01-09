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
using ExaminationSystem.ViewModels.Student;
using Microsoft.EntityFrameworkCore;
using PredicateExtensions;
using System.Linq.Expressions;

namespace ExaminationSystem.Services
{
    public class StudentService
    {
        private readonly StudentRepository _studentRepository;
        private readonly StudentCourseRepository _studentCourseRepository;
        private readonly StudentExamRepository _studentExamRepository;
        private readonly ExamQuestionRepository _examQuestionRepository;
        private readonly StudentAnswerRepository _studentAnswerRepository;
        private readonly ChoiceRepository _choiceRepository;
        private readonly CourseRepository _courseRepository;
        private readonly ExamRepository _examRepository;
        private readonly IMapper _mapper;

        public StudentService(IMapper mapper)
        {
            _studentRepository = new StudentRepository();
            _studentCourseRepository = new StudentCourseRepository();
            _studentExamRepository = new StudentExamRepository();
            _studentAnswerRepository = new StudentAnswerRepository();
            _examQuestionRepository = new ExamQuestionRepository();
            _choiceRepository = new ChoiceRepository();
            _courseRepository = new CourseRepository();
            _examRepository = new ExamRepository();
            _mapper = mapper;
        }

        #region Student CRUD

        // Get all students
        public async Task<ResponseViewModel<IEnumerable<GetAllStudentsDTO>>> GetAll()
        {
            var students = _studentRepository.GetAll().AsNoTracking();
            var result = await students.ProjectTo<GetAllStudentsDTO>(_mapper.ConfigurationProvider).ToListAsync();

            return (result != null)
                ? new SuccessResponseViewModel<IEnumerable<GetAllStudentsDTO>>(result)
                : new FailResponseViewModel<IEnumerable<GetAllStudentsDTO>>("No Students Found", ErrorCode.StudentNotFound);
        }

        // Get student by ID
        public async Task<ResponseViewModel<GetStudentByIdDTO>> GetByID(int id)
        {
            if (!_studentRepository.IsExists(id))
                return new FailResponseViewModel<GetStudentByIdDTO>("Student not Exist", ErrorCode.InvalidStudentId);

            var student = _studentRepository.GetByID(id);
            var result = await student.ProjectTo<GetStudentByIdDTO>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

            return (result != null)
                ? new SuccessResponseViewModel<GetStudentByIdDTO>(result)
                : new FailResponseViewModel<GetStudentByIdDTO>("Student not Exist", ErrorCode.StudentNotFound);
        }

        // Filter students dynamically
        public async Task<ResponseViewModel<IEnumerable<GetAllStudentsDTO>>> Get(int? id, string? name, string? email)
        {
            var predicate = StudentPredicateBuilder(id, name, email);
            var students = _studentRepository.Get(predicate).AsNoTracking();
            var result = await students.ProjectTo<GetAllStudentsDTO>(_mapper.ConfigurationProvider).ToListAsync();

            return (result != null && result.Count > 0)
                ? new SuccessResponseViewModel<IEnumerable<GetAllStudentsDTO>>(result)
                : new FailResponseViewModel<IEnumerable<GetAllStudentsDTO>>("No Students Found", ErrorCode.StudentNotFound);
        }

        // Create a new student
        public async Task<ResponseViewModel<bool>> Create(CreateStudentDTO dto)
        {
            if (_studentRepository.IsExists(dto.Email))
                return new FailResponseViewModel<bool>("Email already exists", ErrorCode.InvalidStudentEmail);

            var student = _mapper.Map<Student>(dto);
            await _studentRepository.AddAsync(student);

            return (student != null)
                ? new SuccessResponseViewModel<bool>(true)
                : new FailResponseViewModel<bool>("Student Not Created", ErrorCode.StudentNotCreated);
        }

        // Update student info
        public async Task<ResponseViewModel<bool>> Update(int studentID, UpdateStudentDTO dto)
        {
            if (!_studentRepository.IsExists(studentID))
                return new FailResponseViewModel<bool>("Student Not Found", ErrorCode.InvalidStudentId);

            var existingStudent = await _studentRepository.GetByID(studentID).FirstOrDefaultAsync();

            dto = new UpdateStudentDTO
            {
                StudentId = studentID,
                Name = dto.Name == "string" ? existingStudent.FullName : dto.Name,
                Username = dto.Username == "string" ? existingStudent.Username : dto.Username,
            };

            var student = _mapper.Map<Student>(dto);
            await _studentRepository.UpdateAsync(student);

            return (student != null)
                ? new SuccessResponseViewModel<bool>(true)
                : new FailResponseViewModel<bool>("Student Not Updated", ErrorCode.StudentNotUpdated);
        }

        // Soft delete student
        public async Task<ResponseViewModel<bool>> SoftDelete(int studentId)
        {
            if (!_studentRepository.IsExists(studentId))
                return new FailResponseViewModel<bool>("Student Not Found", ErrorCode.InvalidStudentId);

            await _studentRepository.SoftDeleteAsync(studentId);
            return new SuccessResponseViewModel<bool>(true);
        }

        #endregion

        #region Course Enrollment

        // Enroll student in a course
        public async Task<ResponseViewModel<bool>> EnrollInCourse(StudentCourseDTO studentCourseDTO)
        {
            if (!_studentRepository.IsExists(studentCourseDTO.StudentId))
                return new FailResponseViewModel<bool>("Student Not Found", ErrorCode.InvalidStudentId);
            if (!_courseRepository.IsExists(studentCourseDTO.CourseId))
                return new FailResponseViewModel<bool>("Course Not Found", ErrorCode.InvalidCourseId);
            if (_studentCourseRepository.IsAssigned(studentCourseDTO.StudentId, studentCourseDTO.CourseId))
                return new FailResponseViewModel<bool>("Student Not Assigned to Course", ErrorCode.StudentNotAssignedToCourse);

            await _studentCourseRepository.Add(_mapper.Map<StudentCourse>(studentCourseDTO));
            return new SuccessResponseViewModel<bool>(true);
        }

        // Remove student from course
        public async Task<ResponseViewModel<bool>> SoftDeleteStudentFromCourse(StudentCourseDTO studentCourseDTO)
        {
            if (!_studentCourseRepository.IsAssigned(studentCourseDTO.StudentId, studentCourseDTO.CourseId))
                return new FailResponseViewModel<bool>("Student Not Assigned to Course", ErrorCode.StudentNotAssignedToCourse);

            await _studentCourseRepository.SoftDelete(_mapper.Map<StudentCourse>(studentCourseDTO));
            return new SuccessResponseViewModel<bool>(true);
        }

        // Get courses for a student
        public async Task<ResponseViewModel<IEnumerable<GetAllCoursesDTO>>> GetCoursesForStudent(int studentId)
        {
            if (!_studentRepository.IsExists(studentId))
                return new FailResponseViewModel<IEnumerable<GetAllCoursesDTO>>("Student Not Found", ErrorCode.InvalidStudentId);

            var courses = _studentCourseRepository.GetByStudentCourses(studentId)
                                                  .Include(sc => sc.Course)
                                                  .Select(sc => sc.Course);

            var result = await courses.ProjectTo<GetAllCoursesDTO>(_mapper.ConfigurationProvider).ToListAsync();

            return (result != null)
                ? new SuccessResponseViewModel<IEnumerable<GetAllCoursesDTO>>(result)
                : new FailResponseViewModel<IEnumerable<GetAllCoursesDTO>>("No Courses Found with the provided filters", ErrorCode.CourseNotFound);
        }

        #endregion

        #region Exam Management

        // Start an exam for a student
        public async Task<ResponseViewModel<bool>> StartExam(StudentExamDTO dto)
        {
            var studentExam = await _studentExamRepository.GetWithTracking(dto.StudentId, dto.ExamId);

            if (studentExam == null)
                return new FailResponseViewModel<bool>("Student Not Assigned to Course", ErrorCode.StudentNotAssignedToCourse);
            if (studentExam.StartedTime != null)
                return new FailResponseViewModel<bool>("Exam already started", ErrorCode.StudentStartedExam);
            if (studentExam.Exam.Type == ExamType.Final && _studentExamRepository.HasFinalExam(dto.StudentId))
                return new FailResponseViewModel<bool>("Student already took final exam", ErrorCode.StudentAreadyTakeFinalExam);

            studentExam.StartedTime = DateTime.UtcNow;
            studentExam.IsSubmitted = false;
            await _studentExamRepository.Update(studentExam);

            return new SuccessResponseViewModel<bool>(true);
        }

        // Submit a batch of answers
        public async Task<ResponseViewModel<bool>> SubmitAnswers(List<StudentAnswerDTO> answers)
        {
            foreach (var answer in answers)
            {
                var result = await SubmitAnswer(answer);
                if (!result.IsSuccess) return result;
            }

            return new SuccessResponseViewModel<bool>(true);
        }

        // Get exams assigned to a student
        public IEnumerable<GetExamsForStudentDTO> GetExamsForStudent(int studentId)
        {
            if (!_studentRepository.IsExists(studentId))
                throw new Exception("Student Not Found");

            var exams = _studentExamRepository.Get(se => se.StudentId == studentId)
                                             .Include(se => se.Exam)
                                             .Where(se => !se.Exam.IsDeleted)
                                             .AsNoTracking()
                                             .ToList();

            return _mapper.Map<IEnumerable<GetExamsForStudentDTO>>(exams);
        }

        #endregion

        #region Private Helpers

        // Dynamic predicate for filtering students
        private Expression<Func<Student, bool>> StudentPredicateBuilder(int? id, string? name, string? email)
        {
            var predicate = PredicateExtensions.PredicateExtensions.Begin<Student>(true);
            if (id.HasValue) predicate = predicate.And(s => s.ID == id.Value);
            if (!string.IsNullOrEmpty(name)) predicate = predicate.And(s => s.FullName.Contains(name));
            if (!string.IsNullOrEmpty(email)) predicate = predicate.And(s => s.Email.Contains(email));
            return predicate;
        }

        // Check if exam time has expired
        private async Task<ResponseViewModel<bool>> CheckExamTime(StudentExam studentExam)
        {
            var endTime = studentExam.StartedTime.AddMinutes(studentExam.Exam.DurationMinutes);

            if (DateTime.UtcNow <= endTime)
                return new SuccessResponseViewModel<bool>(true);

            await AutoSubmitExam(studentExam);
            return new FailResponseViewModel<bool>("Exam time expired — exam auto submitted", ErrorCode.ExamTimeExpired);
        }

        // Automatically submit exam and calculate score
        private async Task AutoSubmitExam(StudentExam studentExam)
        {
            if (studentExam.IsSubmitted) return;

            int correct = _studentAnswerRepository.CountCorrectAnswers(studentExam.StudentId, studentExam.ExamId);
            int total = _studentAnswerRepository.GetAnswersByStudentExam(studentExam.StudentId, studentExam.ExamId).Count();

            studentExam.Score = total == 0 ? 0 : (decimal)correct / total * 100;
            studentExam.IsSubmitted = true;

            await _studentExamRepository.Update(studentExam);
        }

        // Submit a single answer
        private async Task<ResponseViewModel<bool>> SubmitAnswer(StudentAnswerDTO dto)
        {
            var studentExam = await _studentExamRepository.GetWithTracking(dto.StudentId, dto.ExamId);

            if (studentExam == null)
                return new FailResponseViewModel<bool>("Student not assigned to exam", ErrorCode.StudentNotAssignedToCourse);
            if (studentExam.StartedTime == null)
                return new FailResponseViewModel<bool>("Exam not started", ErrorCode.ExamNotStarted);
            if (studentExam.IsSubmitted)
                return new FailResponseViewModel<bool>("Exam already submitted", ErrorCode.ExamAlreadySubmitted);

            var timeCheck = await CheckExamTime(studentExam);
            if (!timeCheck.IsSuccess) return timeCheck;

            if (!_examQuestionRepository.IsAssigned(dto.ExamId, dto.QuestionId))
                return new FailResponseViewModel<bool>("Question not part of exam", ErrorCode.InvalidQuestion);
            if (!await _choiceRepository.IsChoiceBelongsToQuestion(dto.SelectedChoiceId, dto.QuestionId))
                return new FailResponseViewModel<bool>("Invalid choice", ErrorCode.InvalidChoice);

            await _studentAnswerRepository.AddOrUpdate(_mapper.Map<StudentAnswer>(dto));
            return new SuccessResponseViewModel<bool>(true);
        }

        #endregion
    }
}
