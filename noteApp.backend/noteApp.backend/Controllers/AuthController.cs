using Microsoft.AspNetCore.Mvc;
using noteApp.backend.Data;
using noteApp.backend.Dtos;
using noteApp.backend.Helpers;
using noteApp.backend.Models;

namespace noteApp.backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtServices _jwtServices;

        public AuthController(IUserRepository userRepository, JwtServices jwtServices)
        {
            _userRepository = userRepository;
            _jwtServices = jwtServices;
        }

        [HttpPost("register")]
        public IActionResult Register(UserDto dto)
        {
            if (dto.Email == null) return BadRequest();
            User user = new() { Email = dto.Email, Username = dto.Username, Password = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.Password) };
            _userRepository.Create(user);
                
            Login(dto);

            return Ok("registered");
        }

        [HttpPost("login")]
        public IActionResult Login(UserDto dto)
        {
            var user = _userRepository.GetByUsername(dto.Username);

            if (user == null) return BadRequest();

            if (!BCrypt.Net.BCrypt.EnhancedVerify(dto.Password, user.Password)) return Unauthorized();

            var jwt = _jwtServices.Create(user);

            Response.Cookies.Append("jwt", jwt, new CookieOptions
            {
                HttpOnly = true
            });

            return Ok("logged correctly");
        }

        [HttpGet("user")]
        public IActionResult GetUser()
        {
            try
            {
                var jwt = Request.Cookies["jwt"];
                if (jwt == null) return Unauthorized();
                var token = _jwtServices.Verify(jwt);
                string? username = token?.Identity?.Name;
                Guid id = Guid.Parse(token.FindFirst("Id").Value);

                User? user = _userRepository.GetById(id);

                if (User == null) return NotFound();

                return Ok(user);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex);
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            if (Request.Cookies["jwt"] != null)
            {
                Response.Cookies.Delete("jwt", new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true
                });
            }
            
            return Ok("logout");
        }
    }
}
