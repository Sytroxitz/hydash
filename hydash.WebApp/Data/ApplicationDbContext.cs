using hydash.WebApp.Data.EF;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace hydash.WebApp.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
	{
		// Constructor to pass options to the base IdentityDbContext
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}

		public virtual DbSet<Permission> Permissions { get; set; }
		public virtual DbSet<RolePermission> RolePermissions { get; set; }

		public override int SaveChanges()
		{
			StampUpdatedUsers();
			return base.SaveChanges();
		}

		public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
		{
			StampUpdatedUsers();
			return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
		}

		private void StampUpdatedUsers()
		{
			foreach (var entry in ChangeTracker.Entries<ApplicationUser>())
			{
				if (entry.State == EntityState.Modified)
				{
					entry.Entity.UpdatedAt = DateTime.UtcNow;
				}
			}
		}
	}
}
