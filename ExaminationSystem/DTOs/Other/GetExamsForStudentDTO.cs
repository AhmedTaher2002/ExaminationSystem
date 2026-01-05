using ExaminationSystem.DTOs.Exam;

namespace ExaminationSystem.DTOs.Other
{
    public class GetExamsForStudentDTO
    {
        public string Name {  get; set; }
        public string StudentId { get; set; }

        public IEnumerable<GetAllExamsDTO> getAllExamsDTO { get; set; }
    }
}
