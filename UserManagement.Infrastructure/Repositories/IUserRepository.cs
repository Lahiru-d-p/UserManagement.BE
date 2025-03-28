using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.Repositories
{
   public interface IUserRepository
    {
        Task<User> RegisterUser(User user);
        Task<User> GetUserById(int id);
        Task<User> GetUserByNames(string firstname,string lastName);
        Task<User> UpdateUser(User user);
        Task<IEnumerable<User>> SearchUsers(string firstName, string lastName, DateTime? startDate, DateTime? endDate, string gender);
    }
}
