using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Model;
using UserManagementAPI.Application.Services;

namespace UserManagementAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly PhotoService _photoService;

        public UsersController(UserService userService, PhotoService photoService)
        {
            _userService = userService;
            _photoService = photoService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserInsertModel model)
        {
            var existingUser = await _userService.GetUserByNames(model.FirstName, model.LastName);
            if (existingUser != null)
                return BadRequest(new
                {
                    status = false,
                    statusText = "User Already Exists"
                });
            var registeredUser = await _userService.RegisterUser(model);
            return Ok(new
            {
                status = true,
                statusText = "User Registered Successfully"
            });
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginModel model)
        {
            var token = await _userService.LoginAsync(model);
            if (token == "Invalid credentials") return Unauthorized(new
            {
                status = false,
                statusText = token
            });
            return Ok(new
            {
                status = true,
                statusText = "Login Success",
                data = token
            });
        }
        [Authorize]
        [HttpPost("upload-photo/{userId}")]
        public async Task<IActionResult> UploadPhoto(int userId, [FromForm] IFormFile[] files)
        {
            foreach (var file in files)
            {
                var filePath = await _photoService.SavePhoto(file,userId);
            }
            return Ok(new
            {
                status = true,
                statusText = "Photos uploaded successfully."
            });
        }
        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateModel model)
        {
            var existingUser = await _userService.GetUserById(id);
            if (existingUser == null)
                return NotFound(new
                {
                    status = false,
                    statusText = "User Not Found"
                });
            var result =  await _userService.UpdateUser(existingUser, model);
            return Ok(new
            {
                status = true,
                statusText = "User Updated"
            });
        }
        [Authorize]
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserData(int id)
        {
            var users = await _userService.GetUserById(id);
            return Ok(new
            {
                status = true,
                data = users
            });
        }
        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers(
            [FromQuery] string? firstName,
            [FromQuery] string? lastName,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? gender)
        {
            var users = await _userService.SearchUsers(firstName, lastName, startDate, endDate, gender);
            return Ok(new
            {
                status = true,
                data = users
            });
        }

        [Authorize]
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<IActionResult> UploadPhoto(int photoId)
        {
            var result = await _photoService.DeletePhoto(photoId);
            if (!result) return NotFound(new
            {
                status = false,
                statusText = "Photo deletion failed"
            });
            return Ok(new
            {
                status = true,
                statusText = "Photo deleted successfully"
            });
        }
    }
}