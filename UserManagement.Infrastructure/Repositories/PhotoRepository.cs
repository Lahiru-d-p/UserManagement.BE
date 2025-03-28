using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Infrastructure.Data;

namespace UserManagement.Infrastructure.Repositories
{
    public class PhotoRepository:IPhotoRepository
    {
        private readonly ApplicationDbContext _context;
        public PhotoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserPhoto> InsertPhotoPath(UserPhoto userPhoto)
        {
            _context.UserPhotos.Add(userPhoto);
            await _context.SaveChangesAsync();
            return userPhoto;
        }

        public async Task<UserPhoto> GetPhotoById(int id)
        {
            return await _context.UserPhotos.FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<IEnumerable<UserPhoto>> GetPhotosPathByUserId(int id)
        {
            return await _context.UserPhotos.Where(u => u.FK_UserID == id).ToListAsync();
        }

        public async Task<bool> DeletePhoto(UserPhoto userPhoto)
        {
            _context.UserPhotos.Remove(userPhoto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
