using AutoMapper;
using ExaminationSystem.DTOs.Exam;
using ExaminationSystem.DTOs.Other;
using ExaminationSystem.DTOs.Student;
using ExaminationSystem.Models;
using ExaminationSystem.Models.Enums;
using ExaminationSystem.Repositories;
using ExaminationSystem.ViewModels.Student;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Services
{
    public class StudentService
    {
        private readonly StudentRepository _studentRepository;
        private readonly StudentCourseRepository _studentCourseRepository ;
        private readonly StudentExamRepository _studentExamRepository;
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
            _choiceRepository = new ChoiceRepository();
            _courseRepository = new CourseRepository();
            _examRepository = new ExamRepository();
            _mapper = mapper;
        }

        public List<GetAllStudentsDTO> GetAll()
        {
            var students = _studentRepository.GetAll().AsNoTracking().ToList();
            return _mapper.Map<List<GetAllStudentsDTO>>(students);
            /*
            return _studentRepository.GetAll()
                .Select(s => new GetAllStudentsDTO
                {
                    ID = s.ID,
                    Name = s.FullName,
                    Email = s.Email
                }).AsNoTracking().ToList();
            */
        }
        public async Task<GetStudentByIdViewModel> GetByID(int id)
        {
            if (!_studentRepository.IsExist(id))
                throw new Exception("Student Not Found");

            var student = await _studentRepository.GetByID(id);
            return _mapper.Map<GetStudentByIdViewModel>(student);
            /*
            return new GetStudentByIdViewModel
            {
                Name = student.FullName,
                Email = student.Email
            };*/
        }
        public async Task<GetAllStudentsDTO> Get(int? id, string? name, string? email)
        {
            var query = _studentRepository.GetAll().AsQueryable();
            if (id.HasValue)
            {
                query = query.Where(s => s.ID == id.Value);
            }
            else if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s => s.FullName.Contains(name));
            }
            else if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(s => s.Email.Contains(email));
            }
            else {
                throw new Exception("ALL Is Null");
            }
                var student = await query.AsNoTracking().FirstOrDefaultAsync()?? throw new Exception("Student Not Found"); 
         
            /*
            return new GetAllStudentsDTO
            {
                ID = student.ID,
                Name = student.FullName,
                Email = student.Email
            };
            */
            return _mapper.Map<GetAllStudentsDTO>(student);
        }
        public async Task Create(CreateStudentDTO dto)
        {
            /*
            Student student = new Student
            {
                FullName = dto.Name,
                Email = dto.Email
            };*/
            var student = _mapper.Map<Student>(dto);
            await _studentRepository.Add(student);
        }

        public async Task Update(UpdateStudentDTO dto)
        {

            /*
            var student = new UpdateStudentDTO
            {
                ID = id,
                FullName = StudentName
            };
            */
            var student = _mapper.Map<Student>(dto);

            await _studentRepository.Update(student);
        }

        public async Task SoftDelete(int studentId)
        {
            if (!_studentRepository.IsExist(studentId))
                throw new Exception("Student Not Found");

            await _studentRepository.SoftDelete(studentId);
        }

        public async Task HardDelete(int studentId)
        {
            if (!_studentRepository.IsExist(studentId))
                throw new Exception("Student Not Found");

            await _studentRepository.HardDelete(studentId);
        }
        //Assign student in course
        public async Task<bool> EnrollInCourse(DTOs.Other.StudentCourseDTO studentCourseDTO)
        {
            if (!_studentRepository.IsExist(studentCourseDTO.StudentId) || !_courseRepository.IsExist(studentCourseDTO.CourseId))
                throw new Exception("Invalid student or course");

            if (_studentCourseRepository.IsAssigned(studentCourseDTO.StudentId,studentCourseDTO.CourseId))
                throw new Exception("Student already enrolled");

            await _studentCourseRepository.Add(new StudentCourse
            {
                StudentId = studentCourseDTO.StudentId,
                CourseId = studentCourseDTO.CourseId
            });

            return true;
        }
        public async Task SubmitAnswers(int studentId, int examId, List<StudentAnswerDTO> answers)
        {
           
            foreach (var answer in answers)

            {
                await _studentAnswerRepository.Add(new StudentAnswer
                {
                    StudentId = studentId,
                    ExamId = examId,
                    QuestionId = answer.QuestionId,
                    SelectedChoiceId = answer.SelectedChoiceId
                });
            }
        }

        public ExamResultDTO ViewResult(int studentId,int examId)
        {
            var correctCount = _studentAnswerRepository.CountCorrectAnswers(studentId,examId);
            var total = _studentAnswerRepository.GetAnswersByStudentExam(studentId,examId).Count();

            return new ExamResultDTO
            {
                Score = (decimal)correctCount / total * 100
            };
        }
        public async Task<List<Course>> GetCoursesForStudent(int studentId)
        {
            if (!_studentRepository.IsExist(studentId))
                throw new Exception("Student Not Found");

            var courses = await _studentCourseRepository.GetByStudentCourses(studentId)
                .Include(sc => sc.Course)
                .Select(sc => sc.Course)
                .AsNoTracking()
                .ToListAsync();

            return courses;
        }
        public async Task SoftDeleteStudentFromCourse(StudentCourseDTO studentCourseDTO)
        {
            if (!_studentCourseRepository.IsAssigned(studentCourseDTO.StudentId,studentCourseDTO.CourseId))
                throw new Exception("Enrollment Not Found");

            await _studentCourseRepository.SoftDelete(_mapper.Map<StudentCourse>(studentCourseDTO));
        }
        public async Task HardDeleteStudentFromCourse(StudentCourseDTO studentCourseDTO)
        {
            if (!_studentCourseRepository.IsAssigned(studentCourseDTO.StudentId,studentCourseDTO.CourseId))
                throw new Exception("Enrollment Not Found");

            await _studentCourseRepository.HardDelete(_mapper.Map<StudentCourse>(studentCourseDTO));
        }
        public IEnumerable<GetExamsForStudentDTO> GetExamsForStudent(int studentId)
        {
            if (!_studentRepository.IsExist(studentId))
                throw new Exception("Student Not Found");
            
            var res= _studentExamRepository.GetByStudent(studentId)
                .Include(se => se.Exam)
                .AsNoTracking()
                .ToList();
            

            return _mapper.Map<IEnumerable<GetExamsForStudentDTO>>(res);
        }

        }
    }
