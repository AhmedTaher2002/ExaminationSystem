
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
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseMiddleware<ExaminationSystem.Filters.GlobalErrorHandlerMiddleware>();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
