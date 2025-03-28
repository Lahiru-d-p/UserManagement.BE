using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Repositories
{
    public interface IPhotoRepository
    {
        Task<UserPhoto> InsertPhotoPath(UserPhoto userPhoto);
        Task<IEnumerable<UserPhoto>> GetPhotosPathByUserId(int id);
        Task<bool> DeletePhoto(UserPhoto userPhoto);
        Task<UserPhoto> GetPhotoById(int id);
    }
}
