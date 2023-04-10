using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Entites;
using Models.Models.Request;

namespace PracticeAPI.Controllers
{
    public class AuthController : BaseController<AuthController>
    {
        public AuthController(PracticeDbContext context, ILogger<AuthController> logger, 
            IConfiguration config) : base(context, logger, config)
        { 
            
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] AuthenticationModel model)
        {
            var data = _context.Customers.FirstOrDefault(m => m.Username == model.Username);
            if (data == null) return BadRequest("Username/Password incorrect");

            var isValid = model.Username.ValidPassword(data.Salt, model.Password, data.Password);
            if (!isValid) return BadRequest("Username/password incorrect");

            var accessToken = GenerateToken(model.Username);
            return Ok(accessToken);
        }
    }
}
