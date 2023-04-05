using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Configurations.Entities
{
	public class OrderConfiguration : IEntityTypeConfiguration<Order>
	{
		public void Configure(EntityTypeBuilder<Order> builder)
		{
			builder.Property(e => e.CardName).IsRequired(false);
			builder.Property(e => e.State).IsRequired(false);
			builder.Property(e => e.ZipCode).IsRequired(false);

			builder.Property(e => e.CardName).IsRequired(false);
			builder.Property(e => e.CardNumber).IsRequired(false);
			builder.Property(e => e.CVV).IsRequired(false);
			builder.Property(e => e.Expiration).IsRequired(false);

			builder.Property(e => e.LastModifiedBy).IsRequired(false);
			builder.Property(e => e.CreatedBy).IsRequired(false);
		}
	}
}