namespace ExaminationSystem.DTOs.Choice
{
    public class GetChoiceByIdDTO
    {
        public int ID { get; set; }
        public string Text { get; set; } = null!;
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }
    }
}
