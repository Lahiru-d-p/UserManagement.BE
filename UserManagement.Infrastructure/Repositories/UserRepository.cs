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
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.FindAsync( id);
        }

        public async Task<User> GetUserByNames(string firstname, string lastName)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.FirstName == firstname && u.LastName == lastName);
        }
        public async Task<User> UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<IEnumerable<User>> SearchUsers(string? firstName, string? lastName, DateTime? startDate, DateTime? endDate, string? gender)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(firstName))
                query = query.Where(u => u.FirstName.Contains(firstName));

            if (!string.IsNullOrEmpty(lastName))
                query = query.Where(u => u.LastName.Contains(lastName));

            if (startDate.HasValue)
                query = query.Where(u => u.DateOfBirth >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(u => u.DateOfBirth <= endDate.Value);

            if (!string.IsNullOrEmpty(gender))
                query = query.Where(u => u.Gender == gender);

            return await query.ToListAsync();
        }
    }
}
