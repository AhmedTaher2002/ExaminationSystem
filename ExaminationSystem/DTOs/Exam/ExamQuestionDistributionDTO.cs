namespace ExaminationSystem.DTOs.Exam
{
    public class ExamQuestionDistributionDTO
    {
        public int ExamId { get; set; }
        public int TotalQuestions { get; set; }

        public int SimplePercentage { get; set; }
        public int MediumPercentage { get; set; }
        public int HardPercentage { get; set; }
        

    }
}
