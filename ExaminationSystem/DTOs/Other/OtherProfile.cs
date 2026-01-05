using AutoMapper;
using ExaminationSystem.DTOs.Choice;
using ExaminationSystem.ViewModels.Other;

namespace ExaminationSystem.DTOs.Other
{
    public class OtherProfile:Profile
    {
        public OtherProfile() { 
            CreateMap<StudentCourseViewModel,StudentCourseDTO>();
            CreateMap<ExamQuestionViewModel,ExamQuestionDTO>();
            CreateMap<ExamResultViewModel,ExamResultDTO>();
            CreateMap<StudentAnswerViewModel,StudentAnswerDTO>();
            CreateMap<GetExamsForStudentDTO, GetExamsForStudentViewModel>();

            
        }
    }
}
