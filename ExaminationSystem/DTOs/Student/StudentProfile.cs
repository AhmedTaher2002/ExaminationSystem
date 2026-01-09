using AutoMapper;
using ExaminationSystem.ViewModels.Student;

namespace ExaminationSystem.DTOs.Student
{
    public class StudentProfile : Profile
    {
        public StudentProfile()
        {
            //Service
            CreateMap<Models.Student, GetAllStudentsDTO>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.FullName));

            CreateMap<Models.Student, GetStudentByIdViewModel>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.FullName));
            
            CreateMap<CreateStudentDTO, Models.Student>()
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Username, o => o.MapFrom(s => s.Username));

            CreateMap<UpdateStudentDTO, Models.Student>()
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Username, o => o.MapFrom(s => s.Username));


            //Controller
            CreateMap<GetAllStudentsDTO, GetAllStudentsViewModel>();
            CreateMap<GetStudentByIdDTO, GetStudentByIdViewModel>();

            CreateMap<CreateStudentViewModel, CreateStudentDTO>();
            CreateMap<UpdateStudentViewModel, UpdateStudentDTO>();
        }
    }
}
