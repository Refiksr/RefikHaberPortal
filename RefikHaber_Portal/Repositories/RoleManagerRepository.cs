using RefikHaber.Models;
using RefikHaber.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RefikHaber_Portal.Repositories
{
    public class RoleManagerRepository
    {
        private readonly UygulamaDbContext _context;

        public RoleManagerRepository(UygulamaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AppRole>> GetAllRolesAsync()
        {
            return await _context.Roles
                .Select(role => new AppRole
                {
                    Id = role.Id,
                    Name = role.Name
                })
                .ToListAsync();
        }

        public async Task<IdentityResult> AddRoleAsync(AppRole role)
        {
            var result = await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteRoleAsync(string roleId)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Rol bulunamadı." });
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Kullanıcı bulunamadı." });
            }

            var userRoles = await _context.UserRoles.Where(ur => ur.UserId == userId).ToListAsync();
            _context.UserRoles.RemoveRange(userRoles);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return IdentityResult.Success;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(user => new ApplicationUser
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersAsync()
        {
            return await _context.Users
                .Select(user => new ApplicationUser
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                })
                .ToListAsync();
        }

        public async Task<Dictionary<string, List<string>>> GetUserRolesGroupedAsync()
        {
            var userRoles = await _context.UserRoles.ToListAsync();
            var roles = await _context.Roles.ToListAsync();

            return userRoles
                .GroupBy(ur => ur.UserId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(ur => roles.FirstOrDefault(r => r.Id == ur.RoleId)?.Id).Where(roleId => roleId != null).ToList()
                );
        }

        public async Task<bool> HasUsersInRoleAsync(string roleId)
        {
            return await _context.UserRoles.AnyAsync(ur => ur.RoleId == roleId);
        }

        public async Task UpdateUserRolesAsync(string userId, IEnumerable<string> newRoles)
        {
            var currentRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            _context.UserRoles.RemoveRange(currentRoles);

            var newUserRoles = newRoles.Select(roleId => new IdentityUserRole<string>
            {
                UserId = userId,
                RoleId = roleId
            });

            await _context.UserRoles.AddRangeAsync(newUserRoles);
            await _context.SaveChangesAsync();
        }

    }

}
