using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CityInfo.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase

    {
        private IConfiguration _configuration;
        private readonly CityInfoContext _context;

        public AuthenticationController(IConfiguration configuration, CityInfoContext context)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public class RegistrationRequestBody
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
        }


        private class CityInfoUser
        {
            public CityInfoUser(int userId, string userName, string firstName, string lastName, string city)
            {
                UserId = userId;
                UserName = userName;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }

            public int UserId { get; set; }

            public string UserName { get; set; }

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }
          //  public ICollection<UserDetails> Details { get; set; }
        }
        public class AuthenticationRequestBody
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }
        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(AuthenticationRequestBody authenticationRequestBody)

        {
            var user = ValidateUserCredentials(authenticationRequestBody.UserName, authenticationRequestBody.Password);
            if (authenticationRequestBody.UserName == null || authenticationRequestBody.Password == null)
            {
                return BadRequest("Username and password are required.");
            }
            if (user == null)
            {
                return Unauthorized();
            }
            // creating a token
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);
            

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
            claimsForToken.Add(new Claim("given_name", user.FirstName));
            claimsForToken.Add(new Claim("family_name", user.LastName));
            claimsForToken.Add(new Claim("city", user.City));


            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials
               );
            var tokenToReturn= new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return Ok(tokenToReturn);
        }

        //private CityInfoUser ValidateUserCredentials(string? userName, string? password)
        //    {
        //        return new CityInfoUser(1,userName??"","vaibhav","mahadik","Mumbai");
        //    }
        private CityInfoUser? ValidateUserCredentials(string? userName, string? password)
        {
            // Replace with your DbContext and logic to retrieve the user
            var user = _context.UserDetails.SingleOrDefault(u => u.Username == userName);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null; // Invalid credentials
            }


            //if (user == null || !string.Equals(user.Password, password, StringComparison.Ordinal))
            //{
            //    return null; // Invalid credentials
            //}


            return new CityInfoUser(user.Id, user.Username, user.FirstName ?? "", user.LastName ?? "", "CityName");

        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegistrationRequestBody registrationRequestBody)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(registrationRequestBody.Username) ||
                string.IsNullOrWhiteSpace(registrationRequestBody.Password))
            {
                return BadRequest("Username and password are required.");
            }

            // Check if username already exists
            if (await _context.UserDetails.AnyAsync(u => u.Username == registrationRequestBody.Username))
            {
                return Conflict("Username is already taken.");
            }

            // Hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registrationRequestBody.Password);

            // Create a new user
            var newUser = new UserDetails
            {
                Username = registrationRequestBody.Username,
                Password = hashedPassword,
                FirstName = registrationRequestBody.FirstName,
                LastName = registrationRequestBody.LastName
            };

            // Add the new user to the database
            _context.UserDetails.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }
    }
}
