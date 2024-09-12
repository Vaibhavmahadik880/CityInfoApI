using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CityInfo.API.DbContexts;
using CityInfo.API.Entities;

namespace CityInfo.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize] // Require authentication to access this controller
    public class UserController : ControllerBase
    {
        private readonly CityInfoContext _context;

        public UserController(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<UserDetailDto>> GetUserDetails(string username)
        {
            // Retrieve the user from the database
            var user = await _context.UserDetails
                .Where(u => u.Username == username)
                .Select(u => new UserDetailDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }
    }
}
