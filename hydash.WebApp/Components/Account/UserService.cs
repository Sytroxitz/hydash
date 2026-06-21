using hydash.WebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace hydash.WebApp.Components.Account
{
    public class UserService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMemoryCache _cache;

        public UserService(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserManager<ApplicationUser> userManager, IMemoryCache memoryCache)
        {
            _dbContextFactory = dbContextFactory;
            _userManager = userManager;
            _cache = memoryCache;
        }

        public async Task<ApplicationUser> GetUserAsync(string userId)
        {
            if (_cache.TryGetValue(userId, out ApplicationUser user))
            {
                return user;
            }

            // Use a lock to prevent concurrent access to this block.
            // This ensures only one thread enters this block at a time.
            //lock (_dbContextFactory)
            {
                using (var context = _dbContextFactory.CreateDbContext())
                {
                    user = _userManager.FindByIdAsync(userId).Result; // Use Result to block, ensuring no concurrent DbContext usage
                }
            }

            if (user != null)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                };

                _cache.Set(userId, user, cacheEntryOptions);
            }

            return user;
        }


        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                InvalidateUserCache(user.Id);
            }

            return result;
        }

        public void InvalidateUserCache(string userId)
        {
            _cache.Remove(userId);
        }
    }
}
