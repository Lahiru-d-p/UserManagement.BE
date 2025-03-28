

using Microsoft.AspNetCore.Http;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Repositories;

namespace UserManagementAPI.Application.Services
{
    public class PhotoService
    {
        private readonly string _uploadDirectory = "wwwroot/uploads";
        private readonly IPhotoRepository _photoRepository;

        public PhotoService(IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }

        public async Task<bool> SavePhoto(IFormFile file, int userId)
        {
            if (file.ContentType != "image/jpeg")
                throw new Exception("Only JPEG images are allowed.");

            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
            string extension = Path.GetExtension(file.FileName);
            string filePath = Path.Combine(_uploadDirectory, file.FileName);
            int counter = 1;

            while (File.Exists(filePath))
            {
                string newFileName = $"{fileName}_{counter}{extension}";
                filePath = Path.Combine(_uploadDirectory, newFileName);
                counter++;
            }

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var newPhoto = new UserPhoto { FK_UserID = userId, PhotoPath = filePath };
            await _photoRepository.InsertPhotoPath(newPhoto);

            return true;
        }


        public async Task<bool> DeletePhoto(int photoId)
        {
            var photo = await _photoRepository.GetPhotoById(photoId);
            if (photo == null)
                throw new Exception("Photo not found.");

            if (File.Exists(photo.PhotoPath))
            {
                File.Delete(photo.PhotoPath);
            }

            await _photoRepository.DeletePhoto(photo);

            return true;
        }
    }
}