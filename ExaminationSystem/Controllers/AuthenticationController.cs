using ExaminationSystem.Data;
using ExaminationSystem.ViewModels.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly Context _context;
        public LoginController()
        {
            _context = new Context();
        }
        [HttpPost]
        public IActionResult Login([FromBody] LoginRequestViewModel request)
        {
            _context.

            // Placeholder logic for user authentication
            if (_context.Students.AnyAsync(a=>a.Username== request.Username) && request.Password == "password")
            {
                return Ok(new { Token = "fake-jwt-token" });
            }


            return Unauthorized();
        }
    }
}
