using System.Data;

namespace ExaminationSystem.Filters
{
    public class GlobalErrorHandlerMiddleware : IMiddleware
    {
        Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            throw new NotImplementedException();
        }
    }
}
