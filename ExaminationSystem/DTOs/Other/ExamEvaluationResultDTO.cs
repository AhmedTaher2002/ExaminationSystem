namespace ExaminationSystem.DTOs.Other
{
    public class ExamEvaluationResultDTO
    {
        public decimal Score { get; set; }
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
    }
}
