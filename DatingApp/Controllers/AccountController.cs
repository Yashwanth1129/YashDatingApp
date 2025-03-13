using DatingApp.Data;
using DatingApp.DTO;
using DatingApp.Entity;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DatingApp.Controllers
{
    
    public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> register(RegisterDto registerDto)
        {
            if (await UserExist(registerDto.Username)) return BadRequest("User is taken");

            using var hmac = new HMACSHA3_512();

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            context.User.Add(user);
            await context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> login(LoginDto loginDto)
        {
            var res = await context.User.FirstOrDefaultAsync(tmp=> tmp.UserName == loginDto.UserName.ToLower());
            
            if (res == null) { return Unauthorized("Invalid Username"); }

            using var hmac = new HMACSHA3_512(res.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i=0;i<computedHash.Length;i++)
            {
                if (computedHash[i] != res.PasswordHash[i]) {
                    return Unauthorized("Invalid Password");
                }
            }

            return new UserDto
            {
                Username = res.UserName,
                Token = tokenService.CreateToken(res)
            };

        }

        private async Task<bool> UserExist(string username)
        {
            return await context.User.AnyAsync(tmp=>tmp.UserName.ToLower() == username.ToLower());
        }

    }
}
