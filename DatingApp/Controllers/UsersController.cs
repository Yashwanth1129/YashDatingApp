using DatingApp.Data;
using DatingApp.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseApiController
    {
        private readonly DataContext context;

        public UsersController(DataContext _context)
        {
            context = _context;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUser()
        {
            var res = await context.User.ToListAsync();
            return Ok(res);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUserById(int id)
        {
            var res = await context.User.Where(tmp=> tmp.Id == id).ToListAsync();

            if(res==null) return NotFound();

            return Ok(res);
        }
    }
}
