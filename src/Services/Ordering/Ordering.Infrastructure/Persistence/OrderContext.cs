using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Common;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence
{
	public class OrderContext : DbContext
	{

		public OrderContext(DbContextOptions<OrderContext> options) : base(options)
		{
		}

		public DbSet<Order> Orders { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderContext).Assembly);
			base.OnModelCreating(modelBuilder);
		}


		public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			foreach (var entry in ChangeTracker.Entries<EntityBase>())
			{
				switch (entry.State)
				{
					case EntityState.Added:
						entry.Entity.CreatedDate = DateTime.Now;
						entry.Entity.CreatedBy = "Pablo";
						break;
					case EntityState.Modified:
						entry.Entity.LastModifiedDate = DateTime.Now;
						entry.Entity.LastModifiedBy = "Pablo";
						break;
				}
			}

			return base.SaveChangesAsync(cancellationToken);
		}

	}
}
