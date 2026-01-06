using AutoMapper;
using ExaminationSystem.DTOs.Course;
using ExaminationSystem.DTOs.Instructor;
using ExaminationSystem.DTOs.Other;
using ExaminationSystem.DTOs.Question;
using ExaminationSystem.DTOs.Student;
using ExaminationSystem.Models;
using ExaminationSystem.Repositories;
using Microsoft.EntityFrameworkCore;

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

        public List<GetAllInstructorsDTO> GetAll()
        {
            var instructors = _instructorRepository.GetAll().AsNoTracking().ToList();
            return _mapper.Map<List<GetAllInstructorsDTO>>(instructors);
            /*
            return _instructorRepository.GetAll()
                .Select(i => new GetAllInstructorsDTO
                {
                    ID = i.ID,
                    Name = i.FullName,
                    Email = i.Email
                })
                .AsNoTracking().ToList();
            */
        }

        public async Task<GetInstructorByIdDTO> GetByID(int id)
        {
            if (!_instructorRepository.IsExist(id))
                throw new Exception("Instructor Not Found");

            var instructor = await _instructorRepository.GetByID(id);
            return _mapper.Map<GetInstructorByIdDTO>(instructor);
            /*
            return new GetInstructorByIdDTO
            {
                ID = instructor.ID,
                Name = instructor.FullName,
                Email = instructor.Email
            };
            */

        }
        public IEnumerable<GetAllInstructorsDTO> Get(int? id, string? name, string? email)
        {
            var query = _instructorRepository.GetAll().AsQueryable();
            if (id.HasValue)
            {
                query = query.Where(i => i.ID == id.Value);
            }
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(i => i.FullName.Contains(name));
            }
            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(i => i.Email.Contains(email));
            }
            var instructor =  query.AsNoTracking().FirstOrDefaultAsync()?? throw new Exception("StudentCourse Not Found"); 
            
            /*
            return new GetAllInstructorsDTO
            {
                ID = instructor.ID,
                Name = instructor.FullName,
                Email = instructor.Email
            };
            */
            return _mapper.Map<IEnumerable<GetAllInstructorsDTO>>(instructor);
        }

        public async Task Create(CreateInstructorDTO dto)
        {
            /*
            Instructor instructor = new Instructor
            {
                FullName = dto.Name,
                Email = dto.Email
            };
            */
            var instructor = _mapper.Map<Instructor>(dto);
            await _instructorRepository.Add(instructor);
        }

        public async Task Update(int id, UpdateInstructorDTO dto)
        {
            if (!_instructorRepository.IsExist(id))
                throw new Exception("Instructor Not Found");
            var instruc =await _instructorRepository.GetByIDWithTracking(id);
            dto = new()
            {
                Email=dto.Name=="string"?instruc.Email:dto.Email,
                Name=dto.Name=="string"?instruc.FullName:dto.Name,                
            };
            /*
            Instructor instructor = new Instructor
            {
                ID = id,
                FullName = dto.Name,
                Email = dto.Email
            };
            */
            var instructor = _mapper.Map<Instructor>(dto);
            await _instructorRepository.Update(instructor);
        }

        public async Task SoftDelete(int instructorId)
        {
            if (!_instructorRepository.IsExist(instructorId))
                throw new Exception("Instructor Not Found");

            await _instructorRepository.SoftDelete(instructorId);
        }
        public async Task HardDelete(int instructorId)
        {
            if (!_instructorRepository.IsExist(instructorId))
                throw new Exception("Instructor Not Found");
            
            await _instructorRepository.HardDelete(instructorId);
        }
        public IEnumerable<GetAllCoursesDTO> GeTCoursesForInstructor(int instructorId)
        { 
            var cources= _courseRepository.Get(c => c.InstructorId == instructorId).ToList();
            return _mapper.Map<IEnumerable<GetAllCoursesDTO>>(cources);
        }
        public IEnumerable<GetAllStudentsDTO> GetStudentsInCourse(int courseId)
        {
            if (!_courseRepository.IsExist(courseId))
                throw new Exception("Course Not Found");

            var res= _studentCourseRepository.GetByStudentCourses(courseId)
                .Include(sc => sc.Student)
                .Select(sc => sc.Student)
                .AsNoTracking()
                .ToList();
            return _mapper.Map<IEnumerable<GetAllStudentsDTO>>(res);
        }
        public IEnumerable<GetAllQuestionsDTO> GetQuestionsForInstructor(int instructorId)
        {
            var Questions=_questionRepository.Get(i=>i.InstructorId==instructorId&&!i.IsDeleted).ToList();
            return _mapper.Map< IEnumerable<GetAllQuestionsDTO>>(Questions);
        }
        public IEnumerable<GetAllQuestionsDTO> GetQuestionsByExam(int examId)
        {
            if (!_examRepository.IsExist(examId))
                throw new Exception("Exam Not Found");

            var res = _examQuestionRepository.GetQuestionsByExam(examId);
            return _mapper.Map<IEnumerable<GetAllQuestionsDTO>>(res);
        }
        public IEnumerable<GetStudentAnswersDTO> GetStudentAnswers(StudentExamDTO studentExamDTO)
        {
            var res = _studentAnswerRepository.Get(studentExamDTO.StudentId, studentExamDTO.ExamId)
                .Include(a => a.Question).Include(a => a.SelectedChoice)
                .AsNoTracking();
            var studentanswer= _mapper.Map<IEnumerable<GetStudentAnswersDTO>>(res).ToList();
            return studentanswer;

        }
        public async Task AssignStudentToCourse(StudentCourseDTO studentCourseDTO)
        {
            if (!_studentRepository.IsExist(studentCourseDTO.StudentId))
                throw new Exception("Student Not Found");

            if (!_courseRepository.IsExist(studentCourseDTO.CourseId))
                throw new Exception("Course Not Found");

            if (_studentCourseRepository.IsAssigned(studentCourseDTO.StudentId,studentCourseDTO.CourseId))
                throw new Exception("Student Already Assigned To This Course");

            /*
            StudentCourse studentCourse = new StudentCourse
            {
                StudentId = studentCourseDTO.StudentId,
                CourseId = studentCourseDTO.CourseId
            };
            await _studentCourseRepository.Add(studentCourse);
            */
            await _studentCourseRepository.Add(_mapper.Map<StudentCourse>(studentCourseDTO));
        }
        public async Task<bool> AssignExamToCourse(CourseExamDTO courseExamDTO)
        {
            if (!_courseRepository.IsExist(courseExamDTO.CourseId))
                throw new Exception("Course Not Found");

            if (!_examRepository.IsExist(courseExamDTO.ExamId))
                throw new Exception("Exam Not Found");

            var exam = await _examRepository.GetByID(courseExamDTO.ExamId);
            exam.CourseId = courseExamDTO.CourseId;
            await _examRepository.Update(exam);
            return true;
        }
        public async Task AddQuestionToExam(ExamQuestionDTO examQuestionDTO)
        {
            if (!_examRepository.IsExist(examQuestionDTO.ExamId))
                throw new Exception("Exam Not Found");

            if (!_questionRepository.IsExist(examQuestionDTO.QuestionId))
                throw new Exception("Question Not Found");

            if (_examQuestionRepository.IsAssigned(examQuestionDTO.ExamId, examQuestionDTO.QuestionId))
                throw new Exception("Question Already Added To Exam");

            ExamQuestion examQuestion = new ()
            {
                ExamId = examQuestionDTO.ExamId,
                QuestionId = examQuestionDTO.QuestionId
            };

            await _examQuestionRepository.Add(examQuestion);
        }
        public async Task RemoveQuestionFromExam(ExamQuestionDTO examQuestionDTO)
        {
            if (!_examQuestionRepository.IsAssigned(examQuestionDTO.ExamId, examQuestionDTO.QuestionId))
                throw new Exception("Question Not Assigned To Exam");

            await _examQuestionRepository.SoftDelete(examQuestionDTO.ExamId, examQuestionDTO.QuestionId);
        }

    }
}
