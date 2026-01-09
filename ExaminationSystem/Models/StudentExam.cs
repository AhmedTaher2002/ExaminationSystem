using System.ComponentModel.DataAnnotations;

namespace ExaminationSystem.Models
{
    public class StudentExam : BaseModel
    {
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int ExamId { get; set; }
        public Exam Exam { get; set; } = null!;

        public decimal Score { get; set; }

        public DateTime SubmissionTime { get; set; }

        public bool IsSubmitted { get; set; }
        public ICollection<StudentAnswer> Answers { get; set; } = new List<StudentAnswer>();
        public DateTime StartedTime { get; internal set; }

    }
} 