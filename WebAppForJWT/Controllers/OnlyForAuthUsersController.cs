using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net.Security;
using WebAppForJWT.Models;
using WebAppForJWT.Repository;

namespace WebAppForJWT.Controllers
{
    [ApiController]
    [Route("Api/[controller]")]
    [Authorize]
    public class OnlyForAuthUsersController(IUserRepo userRepo) : Controller
    {
        IUserRepo _userRepo => userRepo;
        
        [HttpGet("ForAuthorizedOnlyPage")]
        public async Task<IActionResult> PrivatePage()
        {
            IEnumerable<User> users = await _userRepo.GetAllUsers();

            return Ok(users);
            //return Ok("PrivatePage");
        }

        [HttpGet("ForModeratorsOnlyPage")]
        [Authorize(Policy = "RolePolicy")]
        public async Task<IActionResult> ForModeratorsOnly()
        {
            return Ok("Hello moderator");
        }
    }
}
