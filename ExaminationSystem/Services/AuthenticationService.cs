using ExaminationSystem.DTOs.Auth;
using ExaminationSystem.Helper;
using ExaminationSystem.Models.Enums;
using ExaminationSystem.Repositories;
using ExaminationSystem.ViewModels.Response;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Services
{
    public class AuthenticationService
    {
        #region Repositories

        private readonly StudentRepository _studentRepository;
        private readonly InstructorRepository _instructorRepository;

        public AuthenticationService()
        {
            _studentRepository = new StudentRepository();
            _instructorRepository = new InstructorRepository();
        }

        #endregion

        #region Authentication

        // Login using Email or Username
        public async Task<ResponseViewModel<string>> Login(LoginDTO dto)
        {
            // Try to find student
            var student = await _studentRepository
                .Get(s => (s.Email == dto.EmailOrUsername || s.Username == dto.EmailOrUsername) && !s.IsDeleted)
                .FirstOrDefaultAsync();

            if (student != null)
            {
                if (student.PasswordHash != dto.Password)
                    return new FailResponseViewModel<string>("Invalid email/username or password for student.",ErrorCode.StudentInvalidCredentials);

                var token = GenerateToken.Generate(student.ID.ToString(), student.FullName, student.Role.ToString());
                return new SuccessResponseViewModel<string>(token);
            }

            // Try to find instructor
            var instructor = await _instructorRepository
                .Get(i => (i.Email == dto.EmailOrUsername || i.Username == dto.EmailOrUsername) && !i.IsDeleted)
                .FirstOrDefaultAsync();

            if (instructor != null)
            {
                if (instructor.PasswordHash != dto.Password)
                    return new FailResponseViewModel<string>(
                        "Invalid email/username or password for instructor.",
                        ErrorCode.InstructorInvalidCredentials
                    );

                var token = GenerateToken.Generate(instructor.ID.ToString(), instructor.FullName, "Instructor");
                return new SuccessResponseViewModel<string>(token);
            }

            return new FailResponseViewModel<string>(
                "No user found with the provided email or username.",
                ErrorCode.UserNotFound
            );
        }

        #endregion
    }
}
