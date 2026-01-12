using ExaminationSystem.Models.Enums;
using ExaminationSystem.Repositories;
using ExaminationSystem.ViewModels.Response;
using ExaminationSystem.ViewModels.User;

namespace ExaminationSystem.Services
{
    public class AuthService
    {
        private readonly StudentRepository _studentRepository;
        private readonly InstructorRepository _instructorRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(IJwtTokenService jwtTokenService)
        {
            _studentRepository = new StudentRepository();
            _instructorRepository = new InstructorRepository();
            _jwtTokenService = jwtTokenService;
        }

        public async Task<ResponseViewModel<LoginResponseViewModel>> Login(LoginRequestViewModel model)
        {
            // 🔹 Check Student
            var student = await _studentRepository.GetByEmailAsync(model.Email);

            if (student != null && student.Password == model.Password)
            {
                var token = _jwtTokenService.GenerateToken(
                    student.ID,
                    student.Email,
                    "Student"
                );

                return new SuccessResponseViewModel<LoginResponseDTO>(token);
            }

            // 🔹 Check Instructor
            var instructor = await _instructorRepository.GetByEmailAsync(model.Email);

            if (instructor != null && instructor.Password == model.Password)
            {
                var token = _jwtTokenService.GenerateToken(
                    instructor.ID,
                    instructor.Email,
                    "Instructor"
                );

                return new SuccessResponseViewModel<LoginResponseViewModel>(token);
            }

            return new FailResponseViewModel<LoginResponseViewModel>(
                "Invalid email or password",
                ErrorCode.InvalidCredentials
            );
        }
    }
}
