
using System.Security;

namespace ExaminationSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(typeof(ExaminationSystem.DTOs.Course.CourseProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(ExaminationSystem.DTOs.Choice.ChoiceProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(ExaminationSystem.DTOs.Exam.ExamProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(ExaminationSystem.DTOs.Instructor.InstructorProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(ExaminationSystem.DTOs.Question.QuestionProfile).Assembly);
            builder.Services.AddAutoMapper(typeof(ExaminationSystem.DTOs.Student.StudentProfile).Assembly);
            builder.Services.AddScoped<ExaminationSystem.Filters.GlobalErrorHandlerMiddleware>();
            builder.Services.AddScoped<ExaminationSystem.Filters.TransactionMiddleware>();
            
            builder.Services.AddAuthentication(opt=>opt.DefaultAuthenticateScheme =
                Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme    )
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<ExaminationSystem.Filters.GlobalErrorHandlerMiddleware>();
            app.UseMiddleware<ExaminationSystem.Filters.TransactionMiddleware>();
            
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
