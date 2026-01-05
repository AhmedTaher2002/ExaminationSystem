using AutoMapper;
using ExaminationSystem.DTOs.Exam;

using ExaminationSystem.DTOs.Other;
using ExaminationSystem.DTOs.Question;

using ExaminationSystem.Models;
using ExaminationSystem.Models.Enums;
using ExaminationSystem.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Services
{
    public class ExamService
    {
        private readonly ExamRepository _examRepository;
        private readonly CourseRepository _courseRepository;
        private readonly StudentRepository _studentRepository;
        private readonly QuestionRepository _questionRepository;
        private readonly ExamQuestionRepository _examQuestionRepository;
        private readonly StudentExamRepository _studentExamRepository;
        private readonly StudentAnswerRepository _studentAnswerRepository;
        private readonly IMapper _mapper;
        public ExamService(IMapper mapper)
        {
            _examRepository = new ExamRepository();
            _courseRepository = new CourseRepository();
            _studentRepository = new StudentRepository();
            _studentExamRepository =new StudentExamRepository();
            _studentAnswerRepository = new StudentAnswerRepository();
            _examQuestionRepository=new ExamQuestionRepository();
            _questionRepository=new QuestionRepository();
            _mapper = mapper;
        }

        public List<GetAllExamsDTO> GetAll()
        {
            /*
              var dto =_examRepository.GetAll()
                .Select(e => new GetAllExamsDTO
                {
                    ID = e.ID,
                    Title = e.Title,
                    Type = e.Type,
                    CourseId = e.CourseId
                })
                .AsNoTracking().ToList();
            */
            var dto = _mapper.Map<List<GetAllExamsDTO>>(_examRepository.GetAll().AsNoTracking().ToList());
            return dto;
        }

        public async Task<GetExamByIdDTO> GetByID(int id)
        {
            if (!_examRepository.IsExist(id))
                throw new Exception("Exam Not Found");

            var exam = await _examRepository.GetByID(id);
            return _mapper.Map<GetExamByIdDTO>(exam);

            /*
            var dto= new GetExamByIdDTO
            {
                ID = exam.ID,
                Title = exam.Title,
                Type = exam.Type,
                CourseId = exam.CourseId
            };
            return dto;
            */
        }
        public async Task<IEnumerable<GetAllExamsDTO>>Get(int? id, string? title, ExamType? type)
        {
            var query = _examRepository.GetAll().AsQueryable();
            if (id.HasValue)
            {
                query = query.Where(e => e.ID == id.Value);
            }
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(e => e.Title.Contains(title));
            }
            if (type.HasValue)
            {
                query = query.Where(e => e.Type == type.Value);
            }
            var exam = await query.AsNoTracking().FirstOrDefaultAsync()?? throw new Exception("Exam Not Found"); 
 
            /*
            var dto = new GetAllExamsDTO
            {
                ID = exam.ID,
                Title = exam.Title,
                Type = exam.Type,
                CourseId = exam.CourseId
            };
            return dto;
            */
            return _mapper.Map<IEnumerable<GetAllExamsDTO>>(exam);

        }

        public async Task Create(CreateExamDTO dto)
        {
            if (!_courseRepository.IsExist(dto.CourseId))
                throw new Exception("Course Not Found");
            /*
            Exam exam = new Exam
            {
                Title = dto.Title,
                Type = dto.Type,
                CourseId = dto.CourseId,
                NumberOfQuestions = dto.NumberOfQuestions
            };
            */
            var exam = _mapper.Map<Exam>(dto);

            await _examRepository.Add(exam);
        }

        public async Task Update(int id, UpdateExamDTO dto)
        {
            if (!_examRepository.IsExist(id))
                throw new Exception("Exam Not Found");
            /*
            Exam exam = new Exam
            {
                ID = id,
                Title = dto.Title,
                Type = dto.Type,
                NumberOfQuestions = dto.NumberOfQuestions
            };
            */
            var exam = _mapper.Map<Exam>(dto);
            await _examRepository.Update(exam);
        }

        public async Task SoftDelete(int examId)
        {
            if (!_examRepository.IsExist(examId))
                throw new Exception("Exam Not Found");

            await _examRepository.SoftDelete(examId);
        }
        public async Task HardDelete(int examId)
        {
            if (!_examRepository.IsExist(examId))
                throw new Exception("Exam Not Found");

            await _examRepository.HardDelete(examId);
        }
        public IEnumerable<GetAllQuestionsDTO> GetExamQuestions(int examId)
        {
            var questions = _examQuestionRepository.GetQuestionsByExam(examId);
            return _mapper.Map<List<GetAllQuestionsDTO>>(questions);
        }
        public async Task AssignQuestion(ExamQuestionDTO examQuestionDTO)
        {
            if (!_examRepository.IsExist(examQuestionDTO.ExamId)) throw new Exception("Exam Not Exsist");
            if (!_examRepository.IsExist(examQuestionDTO.QuestionId)) throw new Exception("Question Not Exsist");
            if (_examQuestionRepository.IsAssigned(examQuestionDTO))
                throw new Exception("Question is Assign to Exam");
            //await _examQuestionRepository.Add(new ExamQuestion { ExamId = examQuestionDTO.ExamId, QuestionId = examQuestionDTO.QuestionId });
            await _examQuestionRepository.Add(_mapper.Map<ExamQuestion>(examQuestionDTO));
        }
        public async Task<ExamEvaluationResultDTO> EvaluateExamforStudent(StudentExamDTO studentExamDTO)
        {
            if (!_examRepository.IsExist(studentExamDTO.ExamId))
                { throw new Exception("Exam Not Found"); }
            if (!_studentRepository.IsExist(studentExamDTO.StudentId))
                { throw new Exception("Student not found"); }

            int correct = _studentAnswerRepository.CountCorrectAnswers(studentExamDTO.StudentId, studentExamDTO.ExamId);
            int total = _studentAnswerRepository.GetAnswersByStudentExam(studentExamDTO.StudentId, studentExamDTO.ExamId).Count();
            if (_studentExamRepository.IsAssigned(studentExamDTO.StudentId, studentExamDTO.ExamId))
                throw new Exception("Student not assigned");

            var student =await _studentRepository.GetByID(studentExamDTO.StudentId);
            var res = new ExamEvaluationResultDTO
            {
                StudentId = studentExamDTO.StudentId,
                FullName = student.FullName,
                Score = (decimal)correct / total * 100
            };
            var studentExam = new StudentExam
            {
                StudentId = studentExamDTO.StudentId,
                ExamId = studentExamDTO.ExamId,
                Score = res.Score

            };

            await _studentExamRepository.Update(studentExam);
            return res;
        }
        public async Task<IEnumerable<ExamEvaluationResultDTO>> EvaluateAllExams(int examId)
        {
            if (!_examRepository.IsExist(examId))
                throw new Exception("Exam Not Found");

            var studentExams = _studentExamRepository
                .Get(se => se.ExamId == examId)
                .AsNoTracking()
                .ToList();

            if (!studentExams.Any(a=>a.IsDeleted))
                throw new Exception("No students assigned to this exam");

            var results = new List<ExamEvaluationResultDTO>();

            foreach (var se in studentExams)
            {
                int correct = _studentAnswerRepository
                    .CountCorrectAnswers(se.StudentId, examId);

                int total = _studentAnswerRepository
                    .GetAnswersByStudentExam(se.StudentId, examId)
                    .Count();

                if (total == 0)
                    continue;

                decimal score = (decimal)correct / total * 100;

                se.Score = score;
                se.IsSubmitted = true;

                await _studentExamRepository.Update(se);

                results.Add(new ExamEvaluationResultDTO
                {
                    StudentId = se.StudentId,
                    Score = score
                });
            }

            return results;
        }
        public async Task AutoGenerateQuestions(int examId, int totalQuestions)
        {
            _examQuestionRepository.RemoveAllQuestionsFromExam(examId);

            int perLevel = totalQuestions / 3;

            var simple = _questionRepository.Get(q => q.Level == QuestionLevel.Simple).Take(perLevel);
            var medium = _questionRepository.Get(q => q.Level == QuestionLevel.Medium).Take(perLevel);
            var hard = _questionRepository.Get(q => q.Level == QuestionLevel.Hard).Take(perLevel);

            foreach (var q in simple.Concat(medium).Concat(hard))
                await AssignQuestion(new ExamQuestionDTO { ExamId = examId ,QuestionId=q.ID});
        }
    }
}
