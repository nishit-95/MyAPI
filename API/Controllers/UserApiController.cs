using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Repositories;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly IUserInterface _userRepo;
        private readonly IConfiguration myConfig;

        public UserApiController(IUserInterface userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            myConfig = config;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromForm] t_User user)
        {
            if (user.ProfilePicture != null && user.ProfilePicture.Length > 0)
            {

                var fileName = user.c_Email + Path.GetExtension(

                user.ProfilePicture.FileName);

                var filePath = Path.Combine("../MVC/wwwroot/profile_images", fileName);
                user.c_Image = fileName;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    user.ProfilePicture.CopyTo(stream);
                }
            }
            Console.WriteLine("user.c_fname: " + user.c_UserName);
            var status = await _userRepo.Register(user);
            if (status == 1)
            {
                return Ok(new { success = true, message = "User Registered" });
            }
            else if (status == 0)
            {
                return Ok(new { success = false, message = "User Already Exist" });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    message = "There was some error while Registration"
                });

            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromForm] vm_Login user)
        {
            t_User UserData = await _userRepo.Login(user);
            if (UserData.c_UserId != 0)
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("Userid", UserData.c_UserId.ToString()),
                    new Claim("UserName", UserData.c_UserName)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(myConfig["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                issuer: myConfig["Jwt:Issuer"],
                audience: myConfig["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: signIn
                );
                return Ok(new
                {
                    success = true,
                    message = "Login Success",
                    UserData = UserData,
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            else
            {
                return Ok(new
                {
                    success = false,
                    message = "Invalid email or password",
                    UserData = UserData
                });
            }
        }
    }
}