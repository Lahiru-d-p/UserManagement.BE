
using System.IdentityModel.Tokens.Jwt;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Model;
using UserManagement.Infrastructure.Repositories;
using static System.Net.Mime.MediaTypeNames;

namespace UserManagementAPI.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPhotoRepository _photoRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository,IConfiguration configuration,IPhotoRepository photoRepository)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _photoRepository = photoRepository;
        }

        public async Task<User> RegisterUser(UserInsertModel model)
        {
            
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Remark = model.Remark,
                Title = model.Title,
            };
            var hashedPassword = new PasswordHasher<User>().HashPassword(user, model.Password);
            user.PasswordHash = hashedPassword;
            return await _userRepository.RegisterUser(user);
        }

        public async Task<User> GetUserByNames(string firstName, string lastName)
        {
            var existingUser = await _userRepository.GetUserByNames(firstName, lastName);
                return existingUser;
        }

        public async Task<string> LoginAsync(UserLoginModel model)
        {
            var user = await _userRepository.GetUserByNames(model.FirstName,model.LastName);
            if (user == null || (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, model.Password)
                == PasswordVerificationResult.Failed))
                return "Invalid credentials";

            return CreateToken(user);
        }
        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Title)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
        public async Task<User> GetUserById(int id)
        {
            return await _userRepository.GetUserById(id);
        }

        public async Task<bool> UpdateUser(User existingUser, UserUpdateModel model)
        {
            existingUser.FirstName = model.FirstName;
            existingUser.LastName = model.LastName;
            existingUser.Gender = model.Gender;
            existingUser.DateOfBirth = model.DateOfBirth;
            existingUser.Remark = model.Remark;
            existingUser.Title = model.Title;

            var result = await _userRepository.UpdateUser(existingUser);
            return true;
        }

        public async Task<IEnumerable<UserViewModel>> SearchUsers(string? firstName, string? lastName, DateTime? startDate, DateTime? endDate, string? gender)
        {
            var users = await _userRepository.SearchUsers(firstName, lastName, startDate, endDate, gender);

            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var userPhotos = await GetUserPhotosAsync(user.Id);

                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Title = user.Title,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender,
                    Remark = user.Remark,
                    UserPhotos = userPhotos
                });
            }

            return userViewModels;
        }

        private async Task<List<PhotoViewModel>> GetUserPhotosAsync(int userId)
        {
            var userPhotos = await _photoRepository.GetPhotosPathByUserId(userId); 

            var photoViewModels = new List<PhotoViewModel>();

            foreach (var photo in userPhotos)
            {
                string fileBase64 = await GetFileAsBase64Async(photo.PhotoPath);

                photoViewModels.Add(new PhotoViewModel
                {
                    id = photo.Id,
                    File = $"data:image/png;base64,{fileBase64}"
                });
            }

                return photoViewModels;
        }

        private async Task<string> GetFileAsBase64Async(string filePath)
        {
            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return Convert.ToBase64String(fileBytes);
        }



    }
}