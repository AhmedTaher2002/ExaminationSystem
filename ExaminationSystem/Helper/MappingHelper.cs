using AutoMapper;
using ExaminationSystem.DTOs.Auth;
using ExaminationSystem.DTOs.Choice;
using ExaminationSystem.DTOs.Course;
using ExaminationSystem.DTOs.Exam;
using ExaminationSystem.DTOs.Instructor;
using ExaminationSystem.DTOs.Other;
using ExaminationSystem.DTOs.Question;
using ExaminationSystem.DTOs.Student;
using ExaminationSystem.Models;
using ExaminationSystem.ViewModels.Choice;
using ExaminationSystem.ViewModels.Course;
using ExaminationSystem.ViewModels.Exam;
using ExaminationSystem.ViewModels.Instructor;
using ExaminationSystem.ViewModels.Other;
using ExaminationSystem.ViewModels.Question;
using ExaminationSystem.ViewModels.Student;
using ExaminationSystem.ViewModels.User;

namespace ExaminationSystem.Helper
{
    public class MappingHelper:Profile
    {
        public MappingHelper()
        {
            
            #region Auth Profile

            CreateMap<LoginViewModel, LoginDTO>().ReverseMap();
            CreateMap<Models.Instructor, LoginDTO>()
                .ForMember(opt=>opt.EmailOrUsername,opt=>opt.MapFrom(opt=>opt.Email))
                .ForMember(opt => opt.EmailOrUsername, opt => opt.MapFrom(opt => opt.Username));
            CreateMap<Models.Student, LoginDTO>()
                .ForMember(opt => opt.EmailOrUsername, opt => opt.MapFrom(opt => opt.Email))
                .ForMember(opt => opt.EmailOrUsername, opt => opt.MapFrom(opt => opt.Username));

            #endregion

            #region Choice Profile
            //Service
            CreateMap<Models.Choice, GetAllChoicesDTO>().ReverseMap();
            CreateMap<Models.Choice, GetChoiceByIdDTO>().ReverseMap();

            CreateMap<CreateChoiceDTO, Models.Choice>().ReverseMap();
            CreateMap<UpdateChoiceDTO, Models.Choice>().ReverseMap();

            //Controller
            CreateMap<GetAllChoicesDTO, GetAllChoicesViewModel>().ReverseMap();
            CreateMap<GetChoiceByIdDTO, GetChoiceByIdViewModel>().ReverseMap();

            CreateMap<CreateChoiceViewModel, CreateChoiceDTO>().ReverseMap();
            CreateMap<UpdateChoiceViewModel, UpdateChoiceDTO>().ReverseMap();

            #endregion

            #region Course Profile
            //service
            CreateMap<Models.Course, GetAllCoursesDTO>()
                .ForMember(dest => dest.Instructor,
                    opt => opt.MapFrom(src => src.Instructor == null
                        ? null
                        : new GetInstructorInfoDTO
                        {
                            ID = src.Instructor.ID,
                            Name = src.Instructor.FullName
                        }));

            CreateMap<Models.Course, GetByIdCourseDTO>()
                .ForMember(dest => dest.Instructor,
                    opt => opt.MapFrom(src => src.Instructor == null
                        ? null
                        : new GetInstructorInfoDTO
                        {
                            ID = src.Instructor.ID,
                            Name = src.Instructor.FullName
                        }));

            CreateMap<CreateCourseDTO, Models.Course>();
            CreateMap<UpdateCourseDTO, Models.Course>();

            // =========================
            // DTO -> ViewModel
            // =========================
            CreateMap<GetAllCoursesDTO, GetAllCoursesViewModel>();
            CreateMap<GetByIdCourseDTO, GetByIdCourseViewModel>();

            // 🔥 REQUIRED FIX
            CreateMap<GetInstructorInfoDTO, GetInstuctorInfoViewModel>();

            // =========================
            // ViewModel -> DTO
            // =========================
            CreateMap<CreateCourseViewModel, CreateCourseDTO>();
            CreateMap<UpdateCourseViewModel, UpdateCourseDTO>();
            #endregion

            #region Exam Profile

            //Service
            CreateMap<Models.Exam, GetAllExamsDTO>();
            CreateMap<Models.Exam, GetExamByIdDTO>();

            CreateMap<CreateExamDTO, Models.Exam>();
            CreateMap<UpdateExamDTO, Models.Exam>();

            //Controller
            CreateMap<GetAllExamsDTO, GetAllExamsViewModel>();
            CreateMap<GetExamByIdDTO, GetExamByIdViewModel>();

            CreateMap<CreateExamViewModel, CreateExamDTO>();
            CreateMap<UpdateExamViewModel, UpdateExamDTO>();

            #endregion

            #region Instructor Profile
            // Model -> DTO
            CreateMap<Models.Instructor, GetAllInstructorsDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName));
            CreateMap<Models.Instructor, GetInstructorByIdDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName));
            CreateMap<Models.Instructor, GetInstructorInfoDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName));

            // DTO -> Model
            CreateMap<CreateInstructorDTO, Models.Instructor>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name));
            CreateMap<UpdateInstructorDTO, Models.Instructor>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name));


            // DTO <-> ViewModel
            CreateMap<GetAllInstructorsDTO, GetAllInstructorsViewModel>();
            CreateMap<GetInstructorByIdDTO, GetInstructorByIdViewModel>();

            CreateMap<CreateInstructorViewModel, CreateInstructorDTO>();
            CreateMap<UpdateInstructorViewModel, UpdateInstructorDTO>();

            #endregion


            #region Question Profile

            //Service
            CreateMap<Models.Question, GetAllQuestionsDTO>();
            CreateMap<Models.Question, GetQuestionByIdDTO>();

            CreateMap<CreateQuestionDTO, Models.Question>();
            CreateMap<UpdateQuestionDTO, Models.Question>();

            //Controller
            CreateMap<GetAllQuestionsDTO, GetAllQuestionsViewModel>();
            CreateMap<GetQuestionByIdDTO, GetQuestionByIdViewModel>();

            CreateMap<CreateQuestionViewModel, CreateQuestionDTO>();
            CreateMap<UpdateQuestionViewModel, UpdateQuestionDTO>();

            #endregion

            #region Student Profile

            // Model -> DTO (read)
            CreateMap<Models.Student, GetAllStudentsDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName));
            CreateMap<Models.Student, GetStudentByIdDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName));

            // DTO -> Model (create / update)
            CreateMap<CreateStudentDTO, Models.Student>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name));
            CreateMap<UpdateStudentDTO, Models.Student>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                // avoid overwriting navigation collections by default
                .ForMember(dest => dest.StudentCourses, opt => opt.Ignore())
                .ForMember(dest => dest.StudentExams, opt => opt.Ignore());

            // DTO <-> ViewModel (controller boundary)
            CreateMap<GetAllStudentsDTO, GetAllStudentsViewModel>();
            CreateMap<GetStudentByIdDTO, GetStudentByIdViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID));

            CreateMap<CreateStudentViewModel, CreateStudentDTO>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash));
            CreateMap<UpdateStudentViewModel, UpdateStudentDTO>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash));
            CreateMap<StudentCourseViewModel, StudentCourseDTO>();
            CreateMap<StudentCourseDTO, StudentCourse>();
            #endregion
            

        }
    }
}
